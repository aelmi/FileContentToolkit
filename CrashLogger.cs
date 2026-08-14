using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CodeShuttle.Diagnostics
{
    /// <summary>
    /// Hooks every "I'm about to die" event for a WinForms app and writes a stack-trace report
    /// to %APPDATA%\CodeShuttle\logs\. Best-effort: any failure writing the log is swallowed so
    /// we don't recurse on a logging exception.
    ///
    /// The log is a plaintext file a user may well email to support, so paths under the folder
    /// being scanned — which reveal a customer's project structure — are redacted, as is the
    /// user's profile directory. No password is ever persisted or logged.
    /// </summary>
    public static class CrashLogger
    {
        private static readonly object _gate = new();
        private static bool _installed;
        private static string? _scanRoot;

        /// <summary>Maximum crash logs kept on disk; the oldest are pruned beyond this.</summary>
        public const int MaxRetainedLogs = 20;

        public static string LogsDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CodeShuttle", "logs");

        /// <summary>
        /// The folder currently being scanned. Set by the shell so any path underneath it can be
        /// redacted out of crash reports.
        /// </summary>
        public static string? ScanRoot
        {
            get { lock (_gate) return _scanRoot; }
            set { lock (_gate) _scanRoot = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        /// <summary>Raised after a log is written so the shell can tell the user where it went.</summary>
        public static event Action<string, Exception?>? CrashLogged;

        public static void Install()
        {
            lock (_gate)
            {
                if (_installed) return;
                _installed = true;
            }

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                Write("AppDomain.UnhandledException", e.ExceptionObject as Exception, terminating: e.IsTerminating);

            Application.ThreadException += (s, e) =>
                Write("Application.ThreadException", e.Exception, terminating: false);

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Write("TaskScheduler.UnobservedTaskException", e.Exception, terminating: false);
                e.SetObserved();
            };
        }

        public static string? Write(string source, Exception? ex, bool terminating)
        {
            string? path = null;
            try
            {
                lock (_gate)
                {
                    Directory.CreateDirectory(LogsDirectory);
                    path = NextLogPath();

                    var sb = new StringBuilder();
                    sb.AppendLine(CultureInfo.InvariantCulture, $"# Crash report — {DateTime.Now:yyyy-MM-dd HH:mm:ss zzz}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"Source     : {source}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"Terminating: {terminating}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"App        : {Assembly.GetEntryAssembly()?.GetName().Name}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"Version    : {AppVersion.Full}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"OS         : {Environment.OSVersion}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"CLR        : {Environment.Version}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"PID        : {Environment.ProcessId}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"Cwd        : {Redact(Environment.CurrentDirectory)}");
                    sb.AppendLine();
                    sb.AppendLine("## Exception");
                    sb.AppendLine(Redact(ex?.ToString() ?? "(no exception object)"));

                    File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
                    PruneOldLogs();
                }
            }
            catch
            {
                return null;
            }

            try { CrashLogged?.Invoke(path!, ex); } catch { /* never recurse out of the logger */ }
            return path;
        }

        /// <summary>
        /// Second-granularity file names collided when several handlers fired at once, so each
        /// report gets milliseconds and, failing that, a counter.
        /// </summary>
        private static string NextLogPath()
        {
            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture);
            var candidate = Path.Combine(LogsDirectory, $"crash-{stamp}.log");
            int n = 1;
            while (File.Exists(candidate))
                candidate = Path.Combine(LogsDirectory, $"crash-{stamp}-{n++}.log");
            return candidate;
        }

        private static void PruneOldLogs()
        {
            try
            {
                var files = new DirectoryInfo(LogsDirectory)
                    .GetFiles("crash-*.log")
                    .OrderByDescending(f => f.LastWriteTimeUtc)
                    .Skip(MaxRetainedLogs);
                foreach (var f in files)
                {
                    try { f.Delete(); } catch { }
                }
            }
            catch { /* pruning is opportunistic */ }
        }

        /// <summary>
        /// Removes the scan root and the user's profile directory from text bound for the log.
        /// Exception messages routinely embed full paths.
        /// </summary>
        public static string Redact(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var result = text;

            var root = ScanRoot;
            if (!string.IsNullOrEmpty(root))
            {
                result = result.Replace(root.TrimEnd(Path.DirectorySeparatorChar),
                                        "<scan-root>", StringComparison.OrdinalIgnoreCase);
            }

            var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrEmpty(profile))
            {
                result = result.Replace(profile.TrimEnd(Path.DirectorySeparatorChar),
                                        "<user>", StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }
    }
}

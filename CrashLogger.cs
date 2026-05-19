using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FileContentToolkit.Diagnostics
{
    /// <summary>
    /// Hooks every "I'm about to die" event for a WinForms app and writes a stack-trace report
    /// to %APPDATA%\FileContentToolkit\logs\crash-yyyyMMdd-HHmmss.log. Best-effort: any failure
    /// writing the log is swallowed so we don't recurse on a logging exception.
    /// </summary>
    public static class CrashLogger
    {
        private static readonly object _gate = new();
        private static bool _installed;

        public static string LogsDirectory
        {
            get
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FileContentToolkit", "logs");
                return dir;
            }
        }

        public static void Install()
        {
            if (_installed) return;
            _installed = true;

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
            try
            {
                lock (_gate)
                {
                    Directory.CreateDirectory(LogsDirectory);
                    var ts = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                    var path = Path.Combine(LogsDirectory, $"crash-{ts}.log");

                    var sb = new StringBuilder();
                    sb.AppendLine($"# Crash report — {DateTime.Now:yyyy-MM-dd HH:mm:ss zzz}");
                    sb.AppendLine($"Source     : {source}");
                    sb.AppendLine($"Terminating: {terminating}");
                    sb.AppendLine($"App        : {Assembly.GetEntryAssembly()?.GetName().Name}");
                    sb.AppendLine($"Version    : {Assembly.GetEntryAssembly()?.GetName().Version}");
                    sb.AppendLine($"OS         : {Environment.OSVersion}");
                    sb.AppendLine($"CLR        : {Environment.Version}");
                    sb.AppendLine($"PID        : {Environment.ProcessId}");
                    sb.AppendLine($"Cwd        : {Environment.CurrentDirectory}");
                    sb.AppendLine();
                    sb.AppendLine("## Exception");
                    sb.AppendLine(ex?.ToString() ?? "(no exception object)");

                    File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
                    return path;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

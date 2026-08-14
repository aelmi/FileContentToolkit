using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeShuttle.Settings;

namespace CodeShuttle.Diagnostics
{
    /// <summary>
    /// Builds the support blob behind About's "Copy diagnostics".
    /// </summary>
    /// <remarks>
    /// This is a support tool, not an exfiltration channel. It reports the environment and the
    /// application's own well-known folders and nothing else — in particular it must never carry
    /// a path the user scanned, a file name from a scan, or any scanned content, because the
    /// natural next step after copying it is pasting it into an email or a public issue tracker.
    ///
    /// The rule is enforced structurally rather than by review: nothing is passed in. There is no
    /// parameter through which a scanned path could arrive, and a unit test feeds a scan root to
    /// <see cref="CrashLogger.ScanRoot"/> and asserts it does not appear in the output.
    /// </remarks>
    public static class DiagnosticsReport
    {
        /// <summary>Assembles the report. Every value is derived from the environment or from a constant.</summary>
        public static string Build()
        {
            var sb = new StringBuilder();

            sb.AppendLine("CodeShuttle diagnostics");
            sb.AppendLine("-----------------------");
            Add(sb, "Version", AppVersion.Full);
            Add(sb, "Product", "CodeShuttle");
            Add(sb, "Edition", AboutInfo.Edition);
            sb.AppendLine();

            Add(sb, "OS", RuntimeInformation.OSDescription);
            Add(sb, "OS architecture", RuntimeInformation.OSArchitecture.ToString());
            Add(sb, "Process architecture", RuntimeInformation.ProcessArchitecture.ToString());
            Add(sb, ".NET", RuntimeInformation.FrameworkDescription);
            Add(sb, "64-bit process", Environment.Is64BitProcess.ToString());
            Add(sb, "Processors", Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine();

            Add(sb, "DPI mode", SafeDpiMode());
            Add(sb, "Screens", SafeScreenSummary());
            Add(sb, "Culture", CultureInfo.CurrentCulture.Name);
            Add(sb, "UI culture", CultureInfo.CurrentUICulture.Name);
            sb.AppendLine();

            // The application's own folders only. These are fixed locations under %APPDATA%,
            // never anywhere the user pointed a scan at.
            Add(sb, "Settings folder", SafeDirectoryOf(AppSettings.SettingsPath));
            Add(sb, "Log folder", CrashLogger.LogsDirectory);

            return sb.ToString();
        }

        private static void Add(StringBuilder sb, string label, string value) =>
            sb.Append(label).Append(": ").AppendLine(value);

        private static string SafeDirectoryOf(string path)
        {
            try { return Path.GetDirectoryName(path) ?? "(unknown)"; }
            catch (ArgumentException) { return "(unknown)"; }
        }

        private static string SafeDpiMode()
        {
            // Reading the mode requires the WinForms application context to be initialised, which
            // it is not under a plain unit-test host.
            try { return Application.HighDpiMode.ToString(); }
            catch (InvalidOperationException) { return "(unavailable)"; }
        }

        private static string SafeScreenSummary()
        {
            try
            {
                var screens = Screen.AllScreens;
                if (screens.Length == 0) return "(none)";

                var sb = new StringBuilder();
                for (int i = 0; i < screens.Length; i++)
                {
                    if (i > 0) sb.Append("; ");
                    var b = screens[i].Bounds;
                    sb.Append(CultureInfo.InvariantCulture, $"{b.Width}x{b.Height}");
                    if (screens[i].Primary) sb.Append(" (primary)");
                }
                return sb.ToString();
            }
            catch (Exception ex) when (ex is InvalidOperationException or ExternalException)
            {
                return "(unavailable)";
            }
        }
    }

    /// <summary>Product identity strings shown in About. One place, so they cannot drift.</summary>
    public static class AboutInfo
    {
        public const string ProductName = "CodeShuttle";
        public const string Tagline = "Send your code to AI. Bring the answers back.";
        public const string Edition = "Standard";

        /// <summary>
        /// The copyright holder. About previously rendered a bare year with no holder at all,
        /// which is not a copyright notice.
        /// </summary>
        public const string CopyrightHolder = "MyCompany";

        public const int CopyrightYear = 2026;

        public static string Copyright => $"© {CopyrightYear} {CopyrightHolder}";

        public const string WebsiteUrl = "https://github.com/aelmi/CodeShuttle";
        public const string DocsUrl = "https://github.com/aelmi/CodeShuttle#readme";
        public const string ReleaseNotesUrl = "https://github.com/aelmi/CodeShuttle/releases";
        public const string ReportBugUrl = "https://github.com/aelmi/CodeShuttle/issues";

        /// <summary>
        /// Third-party attributions. Legally required and previously absent entirely.
        /// </summary>
        /// <remarks>
        /// This renders the real THIRD-PARTY-NOTICES.txt, embedded at build time, rather than a
        /// parallel hardcoded copy of it — a hardcoded copy is a string that drifts out of date
        /// silently, and the drift is only ever discovered by a lawyer. The file is embedded
        /// rather than read from disk because the product ships as a single self-contained exe
        /// and must render its own notices even if someone moves that exe on its own.
        /// The literal below is a last-resort fallback for a malformed build.
        /// </remarks>
        public static string ThirdPartyNotices => _notices.Value;

        private static readonly Lazy<string> _notices = new(LoadNotices);

        private static string LoadNotices()
        {
            try
            {
                var asm = typeof(AboutInfo).Assembly;
                var name = Array.Find(asm.GetManifestResourceNames(),
                    n => n.EndsWith("THIRD-PARTY-NOTICES.txt", StringComparison.OrdinalIgnoreCase));
                if (name != null)
                {
                    using var stream = asm.GetManifestResourceStream(name);
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream, Encoding.UTF8);
                        var text = reader.ReadToEnd().Trim();
                        if (text.Length > 0) return text;
                    }
                }
            }
            catch (IOException) { /* fall through to the literal */ }
            catch (BadImageFormatException) { /* fall through to the literal */ }

            return "CodeShuttle includes no third-party NuGet packages." + Environment.NewLine +
                   Environment.NewLine +
                   ".NET and Windows Forms" + Environment.NewLine +
                   "Copyright (c) .NET Foundation and Contributors." + Environment.NewLine +
                   "Licensed under the MIT License." + Environment.NewLine +
                   "https://github.com/dotnet/runtime/blob/main/LICENSE.TXT";
        }
    }
}

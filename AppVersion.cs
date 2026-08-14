using System;
using System.Reflection;

namespace CodeShuttle
{
    /// <summary>
    /// Single source of truth for the application version.
    ///
    /// Reads <see cref="AssemblyInformationalVersionAttribute"/> rather than
    /// <c>AssemblyVersion</c>. AssemblyVersion is deliberately pinned at 1.0.0.0 in
    /// Directory.Build.props (so assembly binding stays stable across patch releases), which
    /// means anything reading it reports "1.0.0.0" forever — that broke the About box, made the
    /// update checker claim an update was available on every launch, and made every crash report
    /// useless for triage.
    /// </summary>
    public static class AppVersion
    {
        private static readonly string _full = ReadInformationalVersion();
        private static readonly string _display = StripMetadata(_full);

        /// <summary>
        /// Full informational version, including any "+build-metadata" suffix
        /// (e.g. "1.2.3+a1b2c3d"). Use for crash logs and diagnostics.
        /// </summary>
        public static string Full => _full;

        /// <summary>
        /// Human-facing version with build metadata stripped (e.g. "1.2.3" or "1.2.3-beta.1").
        /// Use for the About dialog and update comparisons.
        /// </summary>
        public static string Display => _display;

        private static string ReadInformationalVersion()
        {
            var asm = Assembly.GetEntryAssembly() ?? typeof(AppVersion).Assembly;

            var informational = asm
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            if (!string.IsNullOrWhiteSpace(informational))
                return informational!;

            // Fall back to the file version, then the assembly version, so we always
            // return something rather than an empty string.
            var fileVersion = asm
                .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                .Version;

            if (!string.IsNullOrWhiteSpace(fileVersion))
                return fileVersion!;

            return asm.GetName().Version?.ToString() ?? "0.0.0";
        }

        private static string StripMetadata(string version)
        {
            int plus = version.IndexOf('+');
            return plus >= 0 ? version.Substring(0, plus) : version;
        }
    }
}

namespace CodeShuttle
{
    /// <summary>Why a candidate file did not make it into the output.</summary>
    public enum SkipReason
    {
        /// <summary>The file looked like binary content (null bytes in the sample).</summary>
        Binary,

        /// <summary>The file exceeded the configured maximum size.</summary>
        TooLarge,

        /// <summary>The file (or its folder) could not be opened because of permissions.</summary>
        AccessDenied,

        /// <summary>The file could not be read for some other IO reason (locked, device error…).</summary>
        IoError,

        /// <summary>A .gitignore / .dockerignore / user ignore pattern excluded it.</summary>
        IgnoredByRule,

        /// <summary>The path failed containment validation (see <see cref="PathSafety"/>).</summary>
        UnsafePath
    }

    /// <summary>A single file that was left out, with the reason and any extra detail.</summary>
    public sealed class SkippedFile
    {
        public string Path { get; init; } = "";
        public SkipReason Reason { get; init; }
        public string Detail { get; init; } = "";

        public override string ToString() => $"{Reason}: {Path}{(Detail.Length > 0 ? " — " + Detail : "")}";
    }
}

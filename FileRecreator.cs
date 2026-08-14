using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeShuttle
{
    public enum FilePlanStatus
    {
        New,
        Unchanged,
        Modified,

        /// <summary>The entry's path failed containment validation and will never be written.</summary>
        Rejected
    }

    public class FilePlan
    {
        public string OriginalHeader { get; init; } = "";
        public string RelativePath { get; init; } = "";
        public string TargetPath { get; init; } = "";

        /// <summary>Incoming content, normalised to LF with no trailing newline.</summary>
        public string NewContent { get; init; } = "";

        /// <summary>On-disk content, normalised the same way, or null if the file does not exist.</summary>
        public string? ExistingContent { get; init; }

        public FilePlanStatus Status { get; init; }
        public bool Include { get; set; } = true;

        /// <summary>Populated when <see cref="Status"/> is <see cref="FilePlanStatus.Rejected"/>.</summary>
        public string RejectionReason { get; init; } = "";

        // How the file is written back: preserved from the bundle metadata when present,
        // otherwise from the file already on disk, otherwise sensible defaults.
        public string EncodingToken { get; init; } = BundleFormat.Utf8NoBom;
        public EolStyle Eol { get; init; } = EolStyle.Crlf;
        public string? EolMap { get; init; }
        public bool EndsWithNewline { get; init; } = true;

        public bool IsWritable => Status is FilePlanStatus.New or FilePlanStatus.Modified or FilePlanStatus.Unchanged;
    }

    /// <summary>The outcome of planning: the entries, plus any problem that blocks applying at all.</summary>
    public sealed class RecreatePlan
    {
        public List<FilePlan> Plans { get; } = new();

        /// <summary>Problems that make the whole plan unsafe (e.g. two entries targeting one file).</summary>
        public List<string> Errors { get; } = new();

        public bool CanProceed => Errors.Count == 0;

        public int Count => Plans.Count;
    }

    public enum ApplyOutcome { Written, Skipped, Failed }

    public sealed class FileApplyResult
    {
        public string TargetPath { get; init; } = "";
        public ApplyOutcome Outcome { get; init; }
        public string Error { get; init; } = "";
    }

    public sealed class ApplyReport
    {
        public List<FileApplyResult> Results { get; } = new();
        public string? BackupDirectory { get; set; }
        public bool Cancelled { get; set; }

        /// <summary>
        /// The backup set could not be created or its manifest could not be written, so nothing
        /// this apply overwrites is recoverable.
        /// </summary>
        /// <remarks>
        /// This used to be signalled only by <see cref="BackupDirectory"/> being null, and the
        /// write loop ran anyway — so a denied roaming profile, a full %APPDATA% or an AV lock
        /// produced an irreversible overwrite of the user's source tree reported as a clean
        /// success. A caller that sees this flag set with an empty <see cref="Results"/> has an
        /// apply that was *refused*, not one that ran unprotected.
        /// </remarks>
        public bool BackupFailed { get; set; }

        /// <summary>Why the backup set could not be created. Named to the user, not swallowed.</summary>
        public string? BackupError { get; set; }

        /// <summary>
        /// Individual files that could not be copied aside. Their writes are skipped: a file we
        /// cannot back up is a file we must not destroy.
        /// </summary>
        public List<FileApplyResult> BackupFailures { get; } = new();

        /// <summary>True when the apply completed with no backup protection in place.</summary>
        public bool WroteWithoutBackup => BackupFailed && Results.Count > 0;

        public int Written => Results.Count(r => r.Outcome == ApplyOutcome.Written);
        public int Skipped => Results.Count(r => r.Outcome == ApplyOutcome.Skipped);
        public int Failed => Results.Count(r => r.Outcome == ApplyOutcome.Failed);
    }

    public static class FileRecreator
    {
        /// <summary>Both Windows path separators. Hoisted so Split does not allocate per call.</summary>
        private static readonly char[] PathSeparators = { '\\', '/' };

        /// <summary>
        /// Parses a bundle into a plan: every entry is a file the bundle would write, paired with
        /// the existing on-disk content so callers can diff before anything is written.
        ///
        /// Every path is validated for containment (<see cref="PathSafety"/>) and rejected entries
        /// are kept in the plan with <see cref="FilePlanStatus.Rejected"/> — never dropped silently,
        /// because a silently-dropped hostile entry is indistinguishable from a bug.
        /// </summary>
        public static RecreatePlan Plan(string output, string baseFolder)
        {
            var plan = new RecreatePlan();
            List<BundleEntry> entries;
            try
            {
                entries = BundleFormat.Parse(output);
            }
            catch (FormatException ex)
            {
                plan.Errors.Add("The bundle could not be parsed: " + ex.Message);
                return plan;
            }

            if (entries.Count == 0) return plan;

            var relatives = ComputeRelativePaths(entries.Select(e => e.Path).ToList());

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                var relative = relatives[i];

                if (!PathSafety.TryResolveContained(baseFolder, relative, out var targetPath, out var reason))
                {
                    plan.Plans.Add(new FilePlan
                    {
                        OriginalHeader = entry.Path,
                        RelativePath = relative,
                        TargetPath = "",
                        NewContent = entry.Content,
                        ExistingContent = null,
                        Status = FilePlanStatus.Rejected,
                        RejectionReason = reason,
                        Include = false
                    });
                    continue;
                }

                BundleEntry? existingFile = null;
                if (File.Exists(targetPath))
                {
                    try { existingFile = BundleFormat.FromFile(targetPath, relative); }
                    catch { existingFile = null; }
                }

                var status = existingFile == null
                    ? FilePlanStatus.New
                    : (BundleFormat.ContentEquals(existingFile.Content, entry.Content)
                        ? FilePlanStatus.Unchanged
                        : FilePlanStatus.Modified);

                // Metadata precedence: what the bundle stated, else what the file already is,
                // else platform defaults. This is what stops recreation flattening UTF-16 to
                // UTF-8 and rewriting every LF in the repository as CRLF.
                var source = entry.HasMetadata ? entry : existingFile;

                plan.Plans.Add(new FilePlan
                {
                    OriginalHeader = entry.Path,
                    RelativePath = relative,
                    TargetPath = targetPath,
                    NewContent = entry.Content,
                    ExistingContent = existingFile?.Content,
                    Status = status,
                    Include = status != FilePlanStatus.Unchanged,
                    EncodingToken = source?.EncodingToken ?? BundleFormat.Utf8NoBom,
                    Eol = source?.Eol ?? (Environment.NewLine == "\n" ? EolStyle.Lf : EolStyle.Crlf),
                    EolMap = source?.EolMap,
                    EndsWithNewline = source?.EndsWithNewline ?? true
                });
            }

            DetectDuplicateTargets(plan);
            return plan;
        }

        /// <summary>
        /// Two entries resolving to the same file means one silently destroys the other. This is
        /// the backstop for path-flattening bugs of the kind UNC headers used to trigger, and it
        /// blocks the whole apply rather than picking a winner.
        /// </summary>
        private static void DetectDuplicateTargets(RecreatePlan plan)
        {
            var duplicates = plan.Plans
                .Where(p => p.Status != FilePlanStatus.Rejected && p.TargetPath.Length > 0)
                .GroupBy(p => Path.GetFullPath(p.TargetPath), StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var group in duplicates)
            {
                var headers = string.Join(", ", group.Select(p => p.OriginalHeader));
                plan.Errors.Add(
                    $"{group.Count()} entries resolve to the same file '{group.Key}' ({headers}). " +
                    "Applying would silently destroy all but one, so nothing has been written.");
            }
        }

        /// <summary>
        /// Turns bundle header paths into paths relative to a shared root.
        ///
        /// Rooted paths are canonicalised through <see cref="Path.GetFullPath"/> first, and the
        /// path root (drive or the whole <c>\\server\share\</c> for UNC) is treated as one
        /// indivisible unit — splitting it on separators is what used to collapse every UNC
        /// bundle down to bare file names.
        /// </summary>
        internal static List<string> ComputeRelativePaths(List<string> headers)
        {
            var tails = new List<string[]>(headers.Count);
            var roots = new List<string>(headers.Count);

            foreach (var header in headers)
            {
                var normalised = NormaliseHeader(header);
                string root;
                try { root = Path.GetPathRoot(normalised) ?? string.Empty; }
                catch { root = string.Empty; }

                var tail = normalised.Substring(root.Length);
                roots.Add(root);
                tails.Add(tail.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries));
            }

            bool sameRoot = roots.Distinct(StringComparer.OrdinalIgnoreCase).Count() == 1;

            int common = 0;
            if (sameRoot && tails.Count > 0)
            {
                // Never consume the file name itself: leave at least one segment.
                int limit = tails.Min(t => t.Length) - 1;
                while (common < limit)
                {
                    var candidate = tails[0][common];

                    // Never absorb a traversal segment into the "common root". Doing so would
                    // quietly sanitise a hostile path instead of surfacing it, and PathSafety
                    // would then never see the '..' it exists to reject.
                    if (candidate == ".." || candidate == ".") break;

                    if (tails.All(t => string.Equals(t[common], candidate, StringComparison.OrdinalIgnoreCase)))
                        common++;
                    else
                        break;
                }
            }

            return tails
                .Select(t => string.Join(Path.DirectorySeparatorChar.ToString(), t.Skip(common)))
                .ToList();
        }

        private static string NormaliseHeader(string header)
        {
            var value = (header ?? string.Empty).Trim();
            if (value.Length == 0) return value;

            if (Path.IsPathRooted(value))
            {
                // GetFullPath canonicalises "." and ".." here, but the resulting *relative*
                // path is still validated by PathSafety before anything is written.
                try { return Path.GetFullPath(value); } catch { return value; }
            }

            // Strip a leading "./" or ".\" without collapsing anything else — a ".." that
            // survives here is exactly what PathSafety must see and reject.
            while (value.StartsWith("./", StringComparison.Ordinal) || value.StartsWith(".\\", StringComparison.Ordinal))
                value = value.Substring(2);

            return value;
        }

        // -------------------- applying --------------------

        /// <summary>
        /// Writes every included plan. Before the first write, each target that already exists is
        /// copied into a timestamped backup set; every write is staged through a temp file in the
        /// destination directory so a failure can never leave a half-written file.
        /// </summary>
        /// <param name="allowUnbackedWrite">
        /// Whether to proceed when no backup set could be created at all. Defaults to false, which
        /// means the apply is <em>refused</em> rather than run unprotected: the caller gets a
        /// report with <see cref="ApplyReport.BackupFailed"/> set, no results, and nothing written.
        /// The only correct way to pass true is after asking the user, naming
        /// <see cref="ApplyReport.BackupError"/>, and having them say yes.
        /// </param>
        /// <remarks>
        /// The product's flagship action is sold on "backup and undo before apply". Falling through
        /// to the write loop after the backup set failed is the one branch where that guarantee
        /// silently did not hold, and the caller could not tell the difference — a routine denied
        /// roaming profile on a corporate SOE overwrote forty source files irreversibly and
        /// reported success.
        /// </remarks>
        public static async Task<ApplyReport> ExecuteAsync(
            IEnumerable<FilePlan> plans,
            string targetRoot,
            IProgress<int>? progress = null,
            string? backupsRoot = null,
            bool allowUnbackedWrite = false,
            CancellationToken ct = default)
        {
            var list = plans.Where(p => p.Include && p.IsWritable && p.TargetPath.Length > 0).ToList();
            var report = new ApplyReport();
            if (list.Count == 0) return report;

            await Task.Run(() =>
            {
                // Targets whose pre-write copy failed. A file we could not copy aside is a file we
                // must not overwrite: its lock may release between backup and write, and then the
                // only copy is gone. Previously this was swallowed with a comment claiming it was
                // "still reported per-file below", which was false — ApplyReport.Results carries
                // write outcomes only, so a backup failure had no downstream reader at all.
                var unbacked = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                BackupSet? backup = null;
                try
                {
                    backup = backupsRoot == null
                        ? BackupSet.Create(targetRoot)
                        : BackupSet.Create(targetRoot, backupsRoot);
                    foreach (var p in list)
                    {
                        ct.ThrowIfCancellationRequested();
                        try { backup.Backup(p.TargetPath); }
                        catch (Exception ex)
                        {
                            unbacked.Add(p.TargetPath);
                            report.BackupFailures.Add(new FileApplyResult
                            {
                                TargetPath = p.TargetPath,
                                Outcome = ApplyOutcome.Skipped,
                                Error = ex.Message
                            });
                        }
                    }
                    backup.WriteManifest();
                    report.BackupDirectory = backup.Directory;
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    report.BackupDirectory = null;
                    report.BackupFailed = true;
                    report.BackupError = ex.Message;
                }

                // No backup set at all, and nobody has said it is acceptable to write anyway.
                // Refuse: an unprotected overwrite the user did not agree to is worse than an
                // apply that did not happen.
                if (report.BackupFailed && !allowUnbackedWrite) return;

                for (int i = 0; i < list.Count; i++)
                {
                    if (ct.IsCancellationRequested) { report.Cancelled = true; break; }

                    var p = list[i];

                    if (unbacked.Contains(p.TargetPath))
                    {
                        report.Results.Add(new FileApplyResult
                        {
                            TargetPath = p.TargetPath,
                            Outcome = ApplyOutcome.Skipped,
                            Error = "Not written: this file could not be backed up first."
                        });
                        progress?.Report((int)((long)(i + 1) * 100 / list.Count));
                        continue;
                    }

                    try
                    {
                        var bytes = BundleFormat.Render(p.NewContent, p.EncodingToken, p.Eol, p.EolMap, p.EndsWithNewline);
                        AtomicFile.WriteAllBytes(p.TargetPath, bytes);
                        report.Results.Add(new FileApplyResult { TargetPath = p.TargetPath, Outcome = ApplyOutcome.Written });
                    }
                    catch (Exception ex)
                    {
                        report.Results.Add(new FileApplyResult
                        {
                            TargetPath = p.TargetPath,
                            Outcome = ApplyOutcome.Failed,
                            Error = ex.Message
                        });
                    }

                    progress?.Report((int)((long)(i + 1) * 100 / list.Count));
                }
            }, ct).ConfigureAwait(false);

            return report;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeShuttle.Theming;
using CodeShuttle.Diff;
using CodeShuttle.UI;

namespace CodeShuttle.Dialogs
{
    public partial class DiffViewerForm : ThemedForm
    {
        private readonly List<FilePlan> _plans;

        /// <summary>The active palette. Diff colours are tokens now, not literals.</summary>
        private static ThemeTokens T => ThemeManager.Tokens;

        // One Font per rendered line used to leak a GDI handle per line; 12k lines exhausted the
        // 10,000-handle per-process limit and the whole application stopped rendering. These four
        // cover every style the renderer uses and are disposed with the form.
        //
        // They are rebuilt (and the previous set disposed) whenever the theme changes, because the
        // applier assigns rtbDiff its monospace font after the constructor has run — deriving them
        // once in the constructor would pin them to the pre-theme font.
        private Font _fontRegular = null!;
        private Font _fontBold = null!;
        private Font _fontItalic = null!;
        private Font _fontBoldItalic = null!;

        public DiffViewerForm(IEnumerable<FilePlan> plans)
        {
            _plans = plans?.ToList() ?? throw new ArgumentNullException(nameof(plans));
            InitializeComponent();

            RebuildStyledFonts();


            int newCount = _plans.Count(p => p.Status == FilePlanStatus.New);
            int modCount = _plans.Count(p => p.Status == FilePlanStatus.Modified);
            int sameCount = _plans.Count(p => p.Status == FilePlanStatus.Unchanged);
            int rejectedCount = _plans.Count(p => p.Status == FilePlanStatus.Rejected);
            var subtitle =
                $"{_plans.Count} file(s):  {newCount} new · {modCount} modified · {sameCount} unchanged";
            if (rejectedCount > 0) subtitle += $" · {rejectedCount} rejected";
            lblHeaderSubtitle.Text = subtitle + ".";

            PopulateList();
            UpdateWriteHint();

            if (lstFilePlans.Items.Count > 0) lstFilePlans.SelectedIndex = 0;
        }

        private void RebuildStyledFonts()
        {
            DisposeStyledFonts();
            _fontRegular = new Font(rtbDiff.Font, FontStyle.Regular);
            _fontBold = new Font(rtbDiff.Font, FontStyle.Bold);
            _fontItalic = new Font(rtbDiff.Font, FontStyle.Italic);
            _fontBoldItalic = new Font(rtbDiff.Font, FontStyle.Bold | FontStyle.Italic);
        }

        /// <summary>
        /// The control-tree walk can set <c>rtbDiff</c>'s own colours but cannot reach the
        /// per-run colours inside the rich text, so the diff has to be re-rendered from the new
        /// palette by hand.
        /// </summary>
        protected override void ApplyTheme()
        {
            base.ApplyTheme();
            if (rtbDiff == null || IsDisposed) return;
            RebuildStyledFonts();
            LstFilePlans_SelectedIndexChanged(this, EventArgs.Empty);
        }

        /// <summary>Called from the designer's Dispose so the styled fonts are released with the form.</summary>
        private void DisposeStyledFonts()
        {
            _fontRegular?.Dispose();
            _fontBold?.Dispose();
            _fontItalic?.Dispose();
            _fontBoldItalic?.Dispose();
        }

        public IReadOnlyList<FilePlan> ApprovedPlans =>
            _plans.Where(p => p.Include && p.Status != FilePlanStatus.Rejected).ToList();

        private void PopulateList()
        {
            lstFilePlans.BeginUpdate();
            try
            {
                lstFilePlans.Items.Clear();
                foreach (var p in _plans)
                {
                    string tag = p.Status switch
                    {
                        FilePlanStatus.New => "[NEW]      ",
                        FilePlanStatus.Modified => "[MODIFIED] ",
                        FilePlanStatus.Unchanged => "[UNCHANGED]",
                        FilePlanStatus.Rejected => "[REJECTED] ",
                        _ => "[?]        "
                    };
                    lstFilePlans.Items.Add($"{tag}  {p.RelativePath}", p.Include && p.Status != FilePlanStatus.Rejected);
                }
            }
            finally { lstFilePlans.EndUpdate(); }
        }

        private void LstFilePlans_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Rendering must never take the dialog down: this is a synchronous UI event and the
            // content is untrusted.
            try
            {
                int i = lstFilePlans.SelectedIndex;
                if (i < 0 || i >= _plans.Count) { rtbDiff.Clear(); return; }
                RenderDiff(_plans[i]);
            }
            catch (Exception ex)
            {
                rtbDiff.Clear();
                rtbDiff.Text = "This file could not be displayed: " + ex.Message;
            }
        }

        private void LstFilePlans_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (e.Index >= 0 && e.Index < _plans.Count)
            {
                var plan = _plans[e.Index];
                if (plan.Status == FilePlanStatus.Rejected)
                {
                    // An unsafe path can never be written, however the checkbox is clicked.
                    e.NewValue = CheckState.Unchecked;
                    plan.Include = false;
                }
                else
                {
                    plan.Include = e.NewValue == CheckState.Checked;
                }
            }
            UpdateWriteHint();
        }

        private void UpdateWriteHint()
        {
            int n = _plans.Count(p => p.Include && p.Status != FilePlanStatus.Rejected);
            lblWriteHint.Text = n == 1 ? "1 file will be written." : $"{n} files will be written.";
            btnWrite.Enabled = n > 0;
        }

        private void RenderDiff(FilePlan plan)
        {
            rtbDiff.SuspendLayout();
            try
            {
                rtbDiff.Clear();

                if (plan.Status == FilePlanStatus.Rejected)
                {
                    AppendStyled($"--- {plan.RelativePath}\n", T.TextSecondary, bold: true);
                    AppendStyled("REJECTED — unsafe path. This entry will not be written.\n\n",
                        T.DiffRemove, bold: true);
                    AppendStyled(plan.RejectionReason + "\n\n", T.DiffRemove);
                    AppendStyled($"Path as it appeared in the bundle:\n{plan.OriginalHeader}\n",
                        T.TextSecondary, italic: true);
                    return;
                }

                AppendStyled($"--- {plan.TargetPath}\n", T.TextSecondary, bold: true);

                if (plan.Status == FilePlanStatus.New)
                {
                    AppendStyled("(new file)\n\n", T.DiffAdd, bold: true);
                    AppendLines((plan.NewContent ?? "").Split('\n'), "+ ", T.DiffAdd);
                    return;
                }

                if (plan.Status == FilePlanStatus.Unchanged)
                {
                    AppendStyled("(no changes — identical to the file on disk)\n", T.TextSecondary, italic: true);
                    return;
                }

                AppendStyled("+++ (incoming)\n", T.TextSecondary, bold: true);

                var diff = LineDiff.Compute(plan.ExistingContent ?? "", plan.NewContent ?? "");

                // Consecutive lines of the same kind are appended in one operation: the styling
                // calls are the expensive part, and doing them per line was quadratic.
                int i = 0;
                while (i < diff.Count)
                {
                    var kind = diff[i].Kind;
                    int runEnd = i;
                    while (runEnd < diff.Count && diff[runEnd].Kind == kind) runEnd++;

                    var sb = new StringBuilder();
                    var prefix = kind switch
                    {
                        DiffLineKind.Add => "+ ",
                        DiffLineKind.Remove => "- ",
                        DiffLineKind.Summary => "",
                        _ => "  "
                    };
                    for (int k = i; k < runEnd; k++)
                        sb.Append(prefix).Append(diff[k].Text).Append('\n');

                    var (color, bold, italic) = kind switch
                    {
                        DiffLineKind.Add => (T.DiffAdd, false, false),
                        DiffLineKind.Remove => (T.DiffRemove, false, false),
                        DiffLineKind.Summary => (T.TextSecondary, true, true),
                        _ => (T.DiffContext, false, false)
                    };

                    AppendStyled(sb.ToString(), color, bold, italic);
                    i = runEnd;
                }
            }
            finally
            {
                rtbDiff.ResumeLayout();
                rtbDiff.Select(0, 0);
                rtbDiff.ScrollToCaret();
            }
        }

        private void AppendLines(IEnumerable<string> lines, string prefix, Color color)
        {
            var sb = new StringBuilder();
            foreach (var line in lines) sb.Append(prefix).Append(line).Append('\n');
            AppendStyled(sb.ToString(), color);
        }

        private void AppendStyled(string text, Color color, bool bold = false, bool italic = false)
        {
            rtbDiff.SelectionStart = rtbDiff.TextLength;
            rtbDiff.SelectionLength = 0;
            rtbDiff.SelectionColor = color;
            rtbDiff.SelectionFont = (bold, italic) switch
            {
                (true, true) => _fontBoldItalic,
                (true, false) => _fontBold,
                (false, true) => _fontItalic,
                _ => _fontRegular
            };
            rtbDiff.AppendText(text);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FileContentToolkit.Diff;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public partial class DiffViewerForm : Form
    {
        private readonly List<FilePlan> _plans;

        public DiffViewerForm(IEnumerable<FilePlan> plans)
        {
            _plans = plans?.ToList() ?? throw new ArgumentNullException(nameof(plans));
            InitializeComponent();

            if (Theme.AppIcon != null) Icon = Theme.AppIcon;
            Theme.AttachHover(btnWrite, btnWrite.BackColor);
            Theme.AttachHover(btnCancel, btnCancel.BackColor);

            int newCount = _plans.Count(p => p.Status == FilePlanStatus.New);
            int modCount = _plans.Count(p => p.Status == FilePlanStatus.Modified);
            int sameCount = _plans.Count(p => p.Status == FilePlanStatus.Unchanged);
            lblHeaderSubtitle.Text =
                $"{_plans.Count} file(s):  {newCount} new · {modCount} modified · {sameCount} unchanged.";

            PopulateList();
            UpdateWriteHint();

            if (lstFilePlans.Items.Count > 0) lstFilePlans.SelectedIndex = 0;
        }

        public IReadOnlyList<FilePlan> ApprovedPlans =>
            _plans.Where(p => p.Include).ToList();

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
                        _ => "[?]        "
                    };
                    lstFilePlans.Items.Add($"{tag}  {p.RelativePath}", p.Include);
                }
            }
            finally { lstFilePlans.EndUpdate(); }
        }

        private void LstFilePlans_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int i = lstFilePlans.SelectedIndex;
            if (i < 0 || i >= _plans.Count) { rtbDiff.Clear(); return; }
            RenderDiff(_plans[i]);
        }

        private void LstFilePlans_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (e.Index >= 0 && e.Index < _plans.Count)
            {
                _plans[e.Index].Include = e.NewValue == CheckState.Checked;
            }
            // BeginInvoke so the count reflects the new state after the check actually applies.
            BeginInvoke(new Action(UpdateWriteHint));
        }

        private void UpdateWriteHint()
        {
            int n = _plans.Count(p => p.Include);
            lblWriteHint.Text = n == 1 ? "1 file will be written." : $"{n} files will be written.";
            btnWrite.Enabled = n > 0;
        }

        private void RenderDiff(FilePlan plan)
        {
            rtbDiff.SuspendLayout();
            try
            {
                rtbDiff.Clear();
                AppendStyled($"--- {plan.TargetPath}\n", Color.FromArgb(108, 117, 125), bold: true);

                if (plan.Status == FilePlanStatus.New)
                {
                    AppendStyled("(new file)\n\n", Color.FromArgb(40, 167, 69), bold: true);
                    foreach (var line in (plan.NewContent ?? "").Replace("\r\n", "\n").Split('\n'))
                        AppendStyled("+ " + line + "\n", Color.FromArgb(40, 167, 69));
                    return;
                }

                if (plan.Status == FilePlanStatus.Unchanged)
                {
                    AppendStyled("(no changes — identical to the file on disk)\n", Color.FromArgb(108, 117, 125), italic: true);
                    return;
                }

                AppendStyled($"+++ (incoming)\n", Color.FromArgb(108, 117, 125), bold: true);

                var diff = LineDiff.Compute(plan.ExistingContent ?? "", plan.NewContent ?? "");
                foreach (var d in diff)
                {
                    switch (d.Kind)
                    {
                        case DiffLineKind.Add:
                            AppendStyled("+ " + d.Text + "\n", Color.FromArgb(40, 167, 69));
                            break;
                        case DiffLineKind.Remove:
                            AppendStyled("- " + d.Text + "\n", Color.FromArgb(220, 53, 69));
                            break;
                        default:
                            AppendStyled("  " + d.Text + "\n", Color.FromArgb(73, 80, 87));
                            break;
                    }
                }
            }
            finally
            {
                rtbDiff.ResumeLayout();
                rtbDiff.Select(0, 0);
                rtbDiff.ScrollToCaret();
            }
        }

        private void AppendStyled(string text, Color color, bool bold = false, bool italic = false)
        {
            rtbDiff.SelectionStart = rtbDiff.TextLength;
            rtbDiff.SelectionLength = 0;
            rtbDiff.SelectionColor = color;
            var style = FontStyle.Regular;
            if (bold) style |= FontStyle.Bold;
            if (italic) style |= FontStyle.Italic;
            rtbDiff.SelectionFont = new Font(rtbDiff.Font, style);
            rtbDiff.AppendText(text);
        }
    }
}

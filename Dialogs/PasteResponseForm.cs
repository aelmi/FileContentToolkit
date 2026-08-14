using System;
using System.IO;
using System.Windows.Forms;
using CodeShuttle.Help;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    /// <summary>
    /// The inbound half of the round trip: paste an AI's reply, choose the folder it applies to,
    /// and hand a validated plan to the diff viewer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Before this existed, the only way to bring an answer back was to paste it into the output
    /// pane — a control that is read-only and is being used as an <em>output</em>. The response
    /// now has its own surface, and reaching the diff viewer no longer requires having generated
    /// anything first.
    /// </para>
    /// <para>
    /// <b>This form deliberately contains no parsing of its own.</b> It calls
    /// <see cref="BundleFormat.Parse"/> to decide whether the text is a pack at all, and
    /// <see cref="FileRecreator.Plan"/> to turn it into file plans. Every containment check —
    /// rooted paths, <c>..</c> segments, alternate data streams, reserved device names,
    /// duplicate targets — lives behind <c>FileRecreator.Plan</c> and <c>PathSafety</c>, and a
    /// second parse path here would bypass all of it. That is not a hypothetical: a bundle header
    /// escaping the target folder into the Windows Startup directory is an arbitrary file write,
    /// which is arbitrary code execution. The traversal payload is fed through
    /// <see cref="BuildPlan"/> by a unit test for exactly this reason.
    /// </para>
    /// </remarks>
    public partial class PasteResponseForm : ThemedForm
    {
        /// <summary>The validated plan, available once the dialog returns OK.</summary>
        public RecreatePlan? Plan { get; private set; }

        /// <summary>The folder the plan was resolved against.</summary>
        public string TargetRoot => txtTarget.Text.Trim();

        public PasteResponseForm(string? initialFolder = null)
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(initialFolder))
                txtTarget.Text = initialFolder;

            UpdateReviewEnabled();
        }

        /// <summary>The pasted text. Settable so the flow can be exercised without a message loop.</summary>
        internal string ResponseText
        {
            get => txtResponse.Text;
            set => txtResponse.Text = value;
        }

        /// <summary>The chosen target folder. Settable for the same reason.</summary>
        internal string TargetFolder
        {
            get => txtTarget.Text;
            set => txtTarget.Text = value;
        }

        private void TxtResponse_TextChanged(object? sender, EventArgs e) => UpdateReviewEnabled();

        private void TxtTarget_TextChanged(object? sender, EventArgs e) => UpdateReviewEnabled();

        private void UpdateReviewEnabled()
        {
            btnReview.Enabled =
                !string.IsNullOrWhiteSpace(txtResponse.Text) &&
                !string.IsNullOrWhiteSpace(txtTarget.Text);
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Select the folder these files belong to",
                UseDescriptionForTitle = true,
            };

            if (!string.IsNullOrWhiteSpace(txtTarget.Text) && Directory.Exists(txtTarget.Text))
                dlg.SelectedPath = txtTarget.Text;

            if (dlg.ShowDialog(this) == DialogResult.OK)
                txtTarget.Text = dlg.SelectedPath;
        }

        private void BtnHelp_Click(object? sender, EventArgs e)
        {
            using var help = new HelpForm(HelpTopics.ApplyingAnswersBack);
            help.ShowDialog(this);
        }

        /// <summary>
        /// Turns the pasted text into a plan, or explains why it cannot.
        /// </summary>
        /// <remarks>
        /// The single entry point for the paste path. Both the Review button and the unit tests
        /// go through here, so a test that feeds a traversal payload is exercising the same code
        /// the button does rather than a parallel copy of it.
        /// </remarks>
        internal RecreatePlan? BuildPlan(out string problem)
        {
            problem = "";

            var text = txtResponse.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                problem = "Paste the AI's response first.";
                return null;
            }

            var root = txtTarget.Text.Trim();
            if (string.IsNullOrWhiteSpace(root))
            {
                problem = "Choose the folder these files belong to.";
                return null;
            }

            if (!Directory.Exists(root))
            {
                problem = "That folder does not exist:" + Environment.NewLine + root;
                return null;
            }

            // FileRecreator.Plan is the only parser this form is permitted to reach, and it is
            // where every containment check lives.
            RecreatePlan plan;
            try
            {
                plan = FileRecreator.Plan(text, root);
            }
            catch (FormatException ex)
            {
                problem = "The response could not be read as a CodeShuttle pack." +
                          Environment.NewLine + Environment.NewLine + ex.Message;
                return null;
            }

            if (plan.Count == 0)
            {
                problem =
                    "No file entries were found in that text." + Environment.NewLine +
                    Environment.NewLine +
                    "The reply needs to contain the pack's file headers. If the AI rewrote only a " +
                    "fragment, ask it to return complete files in the format you sent.";
                return null;
            }

            return plan;
        }

        private void BtnReview_Click(object? sender, EventArgs e)
        {
            var plan = BuildPlan(out var problem);

            if (plan == null)
            {
                lblStatus.Text = FirstLine(problem);
                MessageBox.Show(this, problem, "Paste AI response",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // CanProceed is where the duplicate-target refusal lives. Applying a plan that has not
            // been checked would let two entries resolve to one file and destroy the first.
            if (!plan.CanProceed)
            {
                lblStatus.Text = "This pack cannot be applied safely.";
                MessageBox.Show(this,
                    "This pack cannot be applied safely:" + Environment.NewLine + Environment.NewLine +
                    string.Join(Environment.NewLine + Environment.NewLine, plan.Errors),
                    "Paste AI response", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Plan = plan;
            DialogResult = DialogResult.OK;
            Close();
        }

        private static string FirstLine(string text)
        {
            int at = text.IndexOf('\n');
            return at < 0 ? text : text.Substring(0, at).TrimEnd('\r');
        }
    }
}

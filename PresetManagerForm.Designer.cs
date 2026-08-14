using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Dialogs
{
    partial class PresetManagerForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private Panel pnlBody;
        private ListBox lstPresets;
        /// <summary>
        /// A read-only multiline TextBox, not a Label. It is rebuilt wholesale whenever the
        /// selection changes, and a Label raises no UI Automation event when its text is
        /// replaced — so the details were never announced to a screen reader at all.
        /// </summary>
        private TextBox lblDetails;
        private Button btnLoad;
        private Button btnRename;
        private Button btnDelete;

        private Panel pnlBottom;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            lblHeaderSubtitle = new Label();
            pnlBody = new Panel();
            lstPresets = new ListBox();
            lblDetails = new TextBox();
            btnLoad = new Button();
            btnRename = new Button();
            btnDelete = new Button();
            pnlBottom = new Panel();
            btnClose = new Button();

            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlBottom.SuspendLayout();
            SuspendLayout();

            //
            // pnlHeader
            //
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 14, 20, 11);
            pnlHeader.Size = new Size(720, 79);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Location = new Point(20, 14);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Manage Presets";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Location = new Point(20, 48);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "Saved folder + extension configurations.";
            //
            // pnlBody
            //
            pnlBody.Controls.Add(lstPresets);
            pnlBody.Controls.Add(lblDetails);
            pnlBody.Controls.Add(btnLoad);
            pnlBody.Controls.Add(btnRename);
            pnlBody.Controls.Add(btnDelete);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 79);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(20, 20, 20, 20);
            pnlBody.Size = new Size(720, 499);
            pnlBody.TabIndex = 1;
            //
            // lstPresets
            //
            lstPresets.BorderStyle = BorderStyle.FixedSingle;
            lstPresets.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            lstPresets.FormattingEnabled = true;
            lstPresets.Location = new Point(20, 23);
            lstPresets.Name = "lstPresets";
            lstPresets.Size = new Size(240, 374);
            lstPresets.TabIndex = 0;
            // The list has no visible caption, so without this it announces as an unnamed list.
            lstPresets.AccessibleName = "Saved presets";
            lstPresets.AccessibleDescription = "Choose a preset, then Load, Rename or Delete it.";
            lstPresets.SelectedIndexChanged += LstPresets_SelectedIndexChanged;
            lstPresets.DoubleClick += BtnLoad_Click;
            //
            // lblDetails
            //
            lblDetails.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            lblDetails.BorderStyle = BorderStyle.FixedSingle;
            lblDetails.Location = new Point(276, 23);
            lblDetails.Name = "lblDetails";
            lblDetails.Size = new Size(404, 319);
            lblDetails.TabIndex = 1;
            lblDetails.Text = "(no preset selected)";
            lblDetails.Multiline = true;
            lblDetails.ReadOnly = true;
            lblDetails.ScrollBars = ScrollBars.Vertical;
            lblDetails.WordWrap = true;
            lblDetails.TabStop = true;
            lblDetails.AccessibleName = "Preset details";
            //
            // btnLoad
            //
            btnLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLoad.Cursor = Cursors.Hand;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Location = new Point(276, 356);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(90, 41);
            btnLoad.TabIndex = 2;
            btnLoad.Text = "&Load";
            btnLoad.UseVisualStyleBackColor = false;
            btnLoad.Click += BtnLoad_Click;
            //
            // btnRename
            //
            btnRename.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRename.Cursor = Cursors.Hand;
            btnRename.FlatAppearance.BorderSize = 0;
            btnRename.FlatStyle = FlatStyle.Flat;
            btnRename.Location = new Point(372, 356);
            btnRename.Name = "btnRename";
            btnRename.Size = new Size(90, 41);
            btnRename.TabIndex = 3;
            btnRename.Text = "Rename";
            btnRename.UseVisualStyleBackColor = false;
            btnRename.Click += BtnRename_Click;
            //
            // btnDelete
            //
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(468, 356);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(90, 41);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += BtnDelete_Click;
            //
            // pnlBottom
            //
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 578);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 14, 20, 14);
            pnlBottom.Size = new Size(720, 68);
            pnlBottom.TabIndex = 2;
            //
            // btnClose
            //
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Cursor = Cursors.Hand;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(610, 14);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(90, 40);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            //
            // PresetManagerForm
            //
            // Enter on the list now loads the highlighted preset. Double-click was the only way
            // to do it, which is no way at all without a mouse.
            AcceptButton = btnLoad;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new Size(720, 646);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(620, 499);
            Name = "PresetManagerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Manage Presets";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);

            // Theme roles. Colours and fonts are resolved from ThemeTokens /
            // ThemeFonts at runtime; anything not listed here takes the default
            // for its control type.
            ThemeRoles.Set(btnClose, ThemeRole.ButtonSecondary, FontRole.BodyBold);
            ThemeRoles.Set(btnDelete, ThemeRole.ButtonDanger, FontRole.BodyBold);
            ThemeRoles.Set(btnLoad, ThemeRole.ButtonSuccess, FontRole.BodyBold);
            ThemeRoles.Set(btnRename, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(lblDetails, ThemeRole.Surface, FontRole.Body);
            ThemeRoles.Set(lblHeaderSubtitle, FontRole.BodyItalic);
            ThemeRoles.Set(lblHeaderTitle, FontRole.Title);
            ThemeRoles.Set(lstPresets, FontRole.Body);
            ThemeRoles.Set(pnlBody, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlBottom, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ResumeLayout(false);
        }

        #endregion
    }
}

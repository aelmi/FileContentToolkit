using System.Drawing;
using System.Windows.Forms;

namespace FileContentToolkit.Dialogs
{
    partial class PresetManagerForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private Panel pnlBody;
        private ListBox lstPresets;
        private Label lblDetails;
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
            lblDetails = new Label();
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
            pnlHeader.BackColor = Color.FromArgb(0, 102, 204);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 12, 20, 10);
            pnlHeader.Size = new Size(720, 70);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(20, 12);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Text = "Manage Presets";
            //
            // lblHeaderSubtitle
            //
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblHeaderSubtitle.ForeColor = Color.WhiteSmoke;
            lblHeaderSubtitle.Location = new Point(20, 42);
            lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            lblHeaderSubtitle.Text = "Saved folder + extension configurations.";
            //
            // pnlBody
            //
            pnlBody.BackColor = Color.White;
            pnlBody.Controls.Add(lstPresets);
            pnlBody.Controls.Add(lblDetails);
            pnlBody.Controls.Add(btnLoad);
            pnlBody.Controls.Add(btnRename);
            pnlBody.Controls.Add(btnDelete);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 70);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(20, 18, 20, 18);
            pnlBody.Size = new Size(720, 440);
            pnlBody.TabIndex = 1;
            //
            // lstPresets
            //
            lstPresets.BorderStyle = BorderStyle.FixedSingle;
            lstPresets.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            lstPresets.Font = new Font("Segoe UI", 9.5F);
            lstPresets.FormattingEnabled = true;
            lstPresets.ItemHeight = 22;
            lstPresets.Location = new Point(20, 20);
            lstPresets.Name = "lstPresets";
            lstPresets.Size = new Size(240, 330);
            lstPresets.TabIndex = 0;
            lstPresets.SelectedIndexChanged += LstPresets_SelectedIndexChanged;
            lstPresets.DoubleClick += BtnLoad_Click;
            //
            // lblDetails
            //
            lblDetails.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            lblDetails.BorderStyle = BorderStyle.FixedSingle;
            lblDetails.BackColor = Color.FromArgb(245, 247, 250);
            lblDetails.Font = new Font("Segoe UI", 9.5F);
            lblDetails.ForeColor = Color.FromArgb(33, 37, 41);
            lblDetails.Location = new Point(276, 20);
            lblDetails.Name = "lblDetails";
            lblDetails.Padding = new Padding(10);
            lblDetails.Size = new Size(404, 282);
            lblDetails.TabIndex = 1;
            lblDetails.Text = "(no preset selected)";
            lblDetails.TextAlign = ContentAlignment.TopLeft;
            lblDetails.AutoEllipsis = true;
            //
            // btnLoad
            //
            btnLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLoad.BackColor = Color.FromArgb(40, 167, 69);
            btnLoad.Cursor = Cursors.Hand;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnLoad.ForeColor = Color.White;
            btnLoad.Location = new Point(276, 314);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(90, 36);
            btnLoad.TabIndex = 2;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = false;
            btnLoad.Click += BtnLoad_Click;
            //
            // btnRename
            //
            btnRename.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRename.BackColor = Color.FromArgb(51, 122, 183);
            btnRename.Cursor = Cursors.Hand;
            btnRename.FlatAppearance.BorderSize = 0;
            btnRename.FlatStyle = FlatStyle.Flat;
            btnRename.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRename.ForeColor = Color.White;
            btnRename.Location = new Point(372, 314);
            btnRename.Name = "btnRename";
            btnRename.Size = new Size(90, 36);
            btnRename.TabIndex = 3;
            btnRename.Text = "Rename";
            btnRename.UseVisualStyleBackColor = false;
            btnRename.Click += BtnRename_Click;
            //
            // btnDelete
            //
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(468, 314);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(90, 36);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += BtnDelete_Click;
            //
            // pnlBottom
            //
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 510);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 12, 20, 12);
            pnlBottom.Size = new Size(720, 60);
            pnlBottom.TabIndex = 2;
            //
            // btnClose
            //
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(108, 117, 125);
            btnClose.Cursor = Cursors.Hand;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(610, 12);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(90, 36);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            //
            // PresetManagerForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            CancelButton = btnClose;
            ClientSize = new Size(720, 570);
            Controls.Add(pnlBody);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(620, 440);
            Name = "PresetManagerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Manage Presets";

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}

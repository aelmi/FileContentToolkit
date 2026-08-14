using CodeShuttle.Theming;

namespace CodeShuttle
{
    partial class ExtensionCountsForm
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblPath;
        private Label lblSubfolders;
        private Button btnRefresh;

        // Grid
        private DataGridView gridCounts;

        // Bottom
        private Panel pnlBottom;
        private Label lblTotal;
        private Button btnAddExtension;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            btnRefresh = new Button();
            lblPath = new Label();
            lblSubfolders = new Label();
            gridCounts = new DataGridView();
            pnlBottom = new Panel();
            btnClose = new Button();
            btnAddExtension = new Button();
            lblTotal = new Label();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridCounts).BeginInit();
            pnlBottom.SuspendLayout();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(btnRefresh);
            pnlHeader.Controls.Add(lblPath);
            pnlHeader.Controls.Add(lblSubfolders);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(13, 9, 13, 6);
            pnlHeader.Size = new Size(458, 79);
            pnlHeader.TabIndex = 0;
            //
            // lblHeaderTitle
            //
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Location = new Point(13, 9);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(153, 20);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "Extension Summary";
            //
            // btnRefresh
            //
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Location = new Point(382, 9);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(63, 24);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            //
            // lblPath
            //
            lblPath.AutoSize = true;
            lblPath.Location = new Point(13, 33);
            lblPath.Name = "lblPath";
            lblPath.Size = new Size(54, 17);
            lblPath.TabIndex = 2;
            lblPath.Text = "Folder: -";
            //
            // lblSubfolders
            //
            lblSubfolders.AutoSize = true;
            lblSubfolders.Location = new Point(13, 52);
            lblSubfolders.Name = "lblSubfolders";
            lblSubfolders.Size = new Size(112, 15);
            lblSubfolders.TabIndex = 3;
            lblSubfolders.Text = "Include subfolders: -";
            //
            // gridCounts
            //
            gridCounts.AllowUserToAddRows = false;
            gridCounts.AllowUserToDeleteRows = false;
            gridCounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridCounts.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            gridCounts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            gridCounts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            gridCounts.DefaultCellStyle = dataGridViewCellStyle2;
            gridCounts.Dock = DockStyle.Fill;
            gridCounts.EnableHeadersVisualStyles = false;
            gridCounts.Location = new Point(0, 79);
            gridCounts.Name = "gridCounts";
            gridCounts.ReadOnly = true;
            gridCounts.RowHeadersVisible = false;
            gridCounts.RowHeadersWidth = 39;
            gridCounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCounts.Size = new Size(458, 237);
            gridCounts.TabIndex = 1;
            gridCounts.AccessibleName = "Extension counts";
            gridCounts.AccessibleDescription = "File extensions found under the chosen folder, with a count for each.";
            //
            // pnlBottom
            //
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Controls.Add(btnAddExtension);
            pnlBottom.Controls.Add(lblTotal);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 316);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(13, 6, 13, 9);
            pnlBottom.Size = new Size(458, 42);
            pnlBottom.TabIndex = 2;
            //
            // btnClose
            //
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(391, 8);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(54, 27);
            btnClose.TabIndex = 2;
            btnClose.Text = "&Close";
            // The dialog previously had no CancelButton at all, so Escape did not close it.
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.UseVisualStyleBackColor = false;
            //
            // btnAddExtension
            //
            btnAddExtension.FlatAppearance.BorderSize = 0;
            btnAddExtension.FlatStyle = FlatStyle.Flat;
            btnAddExtension.Location = new Point(329, 8);
            btnAddExtension.Margin = new Padding(0, 0, 6, 0);
            btnAddExtension.Name = "btnAddExtension";
            btnAddExtension.Size = new Size(54, 27);
            btnAddExtension.TabIndex = 1;
            btnAddExtension.Text = "&Add Selected";
            btnAddExtension.UseVisualStyleBackColor = false;
            //
            // lblTotal
            //
            lblTotal.AutoSize = true;
            lblTotal.Dock = DockStyle.Left;
            lblTotal.Location = new Point(13, 6);
            lblTotal.Name = "lblTotal";
            lblTotal.Padding = new Padding(0, 5, 0, 0);
            lblTotal.Size = new Size(81, 22);
            lblTotal.TabIndex = 0;
            lblTotal.AccessibleName = "Total files";
            lblTotal.Text = "Total files: 0";
            //
            // ExtensionCountsForm
            //
            AcceptButton = btnAddExtension;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new Size(458, 358);
            Controls.Add(gridCounts);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimumSize = new Size(369, 352);
            Name = "ExtensionCountsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Extension Counts";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridCounts).EndInit();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();

            // Theme roles. Colours and fonts are resolved from ThemeTokens /
            // ThemeFonts at runtime; anything not listed here takes the default
            // for its control type.
            ThemeRoles.Set(btnAddExtension, ThemeRole.ButtonSuccess, FontRole.MediumBold);
            ThemeRoles.Set(btnClose, ThemeRole.ButtonSecondary, FontRole.MediumBold);
            ThemeRoles.Set(btnRefresh, ThemeRole.ButtonAccent, FontRole.BodyBold);
            ThemeRoles.Set(lblHeaderTitle, FontRole.Title);
            ThemeRoles.Set(lblPath, FontRole.Medium);
            ThemeRoles.Set(lblSubfolders, FontRole.BodyItalic);
            ThemeRoles.Set(lblTotal, FontRole.MediumBold);
            ThemeRoles.Set(pnlBottom, ThemeRole.SurfaceAlt);
            ThemeRoles.Set(pnlHeader, ThemeRole.Header);
            ResumeLayout(false);
        }

        #endregion
    }
}
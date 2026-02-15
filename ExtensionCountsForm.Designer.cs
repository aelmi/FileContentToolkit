namespace FileContentToolkit
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
            pnlHeader.BackColor = Color.FromArgb(0, 102, 204);
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(btnRefresh);
            pnlHeader.Controls.Add(lblPath);
            pnlHeader.Controls.Add(lblSubfolders);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(20, 15, 20, 10);
            pnlHeader.Size = new Size(720, 130);
            pnlHeader.TabIndex = 0;
            // 
            // lblHeaderTitle
            // 
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.Location = new Point(20, 15);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(241, 32);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "Extension Summary";
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.BackColor = Color.FromArgb(51, 122, 183);
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(600, 15);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 40);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            // 
            // lblPath
            // 
            lblPath.AutoSize = true;
            lblPath.Font = new Font("Segoe UI", 10F);
            lblPath.ForeColor = Color.WhiteSmoke;
            lblPath.Location = new Point(20, 55);
            lblPath.Name = "lblPath";
            lblPath.Size = new Size(85, 28);
            lblPath.TabIndex = 2;
            lblPath.Text = "Folder: -";
            // 
            // lblSubfolders
            // 
            lblSubfolders.AutoSize = true;
            lblSubfolders.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblSubfolders.ForeColor = Color.WhiteSmoke;
            lblSubfolders.Location = new Point(20, 85);
            lblSubfolders.Name = "lblSubfolders";
            lblSubfolders.Size = new Size(177, 25);
            lblSubfolders.TabIndex = 3;
            lblSubfolders.Text = "Include subfolders: -";
            // 
            // gridCounts
            // 
            gridCounts.AllowUserToAddRows = false;
            gridCounts.AllowUserToDeleteRows = false;
            gridCounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridCounts.BackgroundColor = Color.White;
            gridCounts.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(0, 102, 204);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(51, 122, 183);
            dataGridViewCellStyle1.SelectionForeColor = Color.White;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            gridCounts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            gridCounts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.5F);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(51, 122, 183);
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            gridCounts.DefaultCellStyle = dataGridViewCellStyle2;
            gridCounts.Dock = DockStyle.Fill;
            gridCounts.EnableHeadersVisualStyles = false;
            gridCounts.Location = new Point(0, 130);
            gridCounts.Name = "gridCounts";
            gridCounts.ReadOnly = true;
            gridCounts.RowHeadersVisible = false;
            gridCounts.RowHeadersWidth = 62;
            gridCounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCounts.Size = new Size(720, 390);
            gridCounts.TabIndex = 1;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Controls.Add(btnAddExtension);
            pnlBottom.Controls.Add(lblTotal);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 520);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(20, 10, 20, 15);
            pnlBottom.Size = new Size(720, 70);
            pnlBottom.TabIndex = 2;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.FromArgb(108, 117, 125);
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(615, 13);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(85, 45);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            // 
            // btnAddExtension
            // 
            btnAddExtension.BackColor = Color.FromArgb(40, 167, 69);
            btnAddExtension.FlatAppearance.BorderSize = 0;
            btnAddExtension.FlatStyle = FlatStyle.Flat;
            btnAddExtension.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddExtension.ForeColor = Color.White;
            btnAddExtension.Location = new Point(517, 13);
            btnAddExtension.Margin = new Padding(0, 0, 10, 0);
            btnAddExtension.Name = "btnAddExtension";
            btnAddExtension.Size = new Size(85, 45);
            btnAddExtension.TabIndex = 0;
            btnAddExtension.Text = "Add Selected";
            btnAddExtension.UseVisualStyleBackColor = false;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Dock = DockStyle.Left;
            lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(33, 37, 41);
            lblTotal.Location = new Point(20, 10);
            lblTotal.Name = "lblTotal";
            lblTotal.Padding = new Padding(0, 8, 0, 0);
            lblTotal.Size = new Size(128, 36);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total files: 0";
            // 
            // ExtensionCountsForm
            // 
            AutoScaleDimensions = new SizeF(11F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(720, 590);
            Controls.Add(gridCounts);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimumSize = new Size(580, 580);
            Name = "ExtensionCountsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Extension Counts";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridCounts).EndInit();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
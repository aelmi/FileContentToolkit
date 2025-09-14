using System.Drawing;
using System.Windows.Forms;

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
        private FlowLayoutPanel pnlBottomButtons;
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
            lblTotal = new Label();
            pnlBottomButtons = new FlowLayoutPanel();
            btnAddExtension = new Button();
            btnClose = new Button();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridCounts).BeginInit();
            pnlBottom.SuspendLayout();
            pnlBottomButtons.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.White;
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(btnRefresh);
            pnlHeader.Controls.Add(lblPath);
            pnlHeader.Controls.Add(lblSubfolders);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(16, 12, 16, 8);
            pnlHeader.Size = new Size(680, 116);
            pnlHeader.TabIndex = 0;
            // 
            // lblHeaderTitle
            // 
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.FromArgb(0, 102, 204);
            lblHeaderTitle.Location = new Point(8, 8);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(218, 30);
            lblHeaderTitle.TabIndex = 0;
            lblHeaderTitle.Text = "Extension Summary";
            // 
            // btnRefresh
            // 
            btnRefresh.AutoSize = true;
            btnRefresh.BackColor = Color.FromArgb(51, 122, 183);
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Segoe UI", 9F);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(289, 12);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(80, 35);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            // 
            // lblPath
            // 
            lblPath.Font = new Font("Segoe UI", 9F);
            lblPath.ForeColor = Color.FromArgb(64, 64, 64);
            lblPath.Location = new Point(10, 55);
            lblPath.Name = "lblPath";
            lblPath.Size = new Size(520, 24);
            lblPath.TabIndex = 1;
            lblPath.Text = "Folder: -";
            // 
            // lblSubfolders
            // 
            lblSubfolders.AutoSize = true;
            lblSubfolders.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblSubfolders.ForeColor = Color.FromArgb(64, 64, 64);
            lblSubfolders.Location = new Point(10, 83);
            lblSubfolders.Name = "lblSubfolders";
            lblSubfolders.Size = new Size(179, 25);
            lblSubfolders.TabIndex = 2;
            lblSubfolders.Text = "Include subfolder(s): -";
            // 
            // gridCounts
            // 
            gridCounts.AllowUserToAddRows = false;
            gridCounts.AllowUserToDeleteRows = false;
            gridCounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridCounts.BackgroundColor = Color.White;
            gridCounts.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(245, 247, 250);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            gridCounts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            gridCounts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = Color.Gainsboro;
            dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            gridCounts.DefaultCellStyle = dataGridViewCellStyle2;
            gridCounts.Dock = DockStyle.Fill;
            gridCounts.EnableHeadersVisualStyles = false;
            gridCounts.Location = new Point(0, 116);
            gridCounts.Name = "gridCounts";
            gridCounts.ReadOnly = true;
            gridCounts.RowHeadersVisible = false;
            gridCounts.RowHeadersWidth = 62;
            gridCounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCounts.Size = new Size(680, 380);
            gridCounts.TabIndex = 1;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(lblTotal);
            pnlBottom.Controls.Add(pnlBottomButtons);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 496);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(16, 8, 16, 12);
            pnlBottom.Size = new Size(680, 64);
            pnlBottom.TabIndex = 2;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Dock = DockStyle.Left;
            lblTotal.Font = new Font("Segoe UI", 9.5F);
            lblTotal.ForeColor = Color.FromArgb(64, 64, 64);
            lblTotal.Location = new Point(16, 8);
            lblTotal.Name = "lblTotal";
            lblTotal.Padding = new Padding(0, 8, 0, 0);
            lblTotal.Size = new Size(110, 33);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total files: 0";
            // 
            // pnlBottomButtons
            // 
            pnlBottomButtons.Controls.Add(btnAddExtension);
            pnlBottomButtons.Controls.Add(btnClose);
            pnlBottomButtons.Dock = DockStyle.Right;
            pnlBottomButtons.Location = new Point(482, 8);
            pnlBottomButtons.Name = "pnlBottomButtons";
            pnlBottomButtons.Size = new Size(182, 44);
            pnlBottomButtons.TabIndex = 1;
            pnlBottomButtons.WrapContents = false;
            // 
            // btnAddExtension
            // 
            btnAddExtension.AutoSize = true;
            btnAddExtension.BackColor = Color.FromArgb(40, 167, 69);
            btnAddExtension.FlatAppearance.BorderSize = 0;
            btnAddExtension.FlatStyle = FlatStyle.Flat;
            btnAddExtension.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnAddExtension.ForeColor = Color.White;
            btnAddExtension.Location = new Point(3, 3);
            btnAddExtension.Margin = new Padding(3, 3, 8, 3);
            btnAddExtension.Name = "btnAddExtension";
            btnAddExtension.Size = new Size(75, 35);
            btnAddExtension.TabIndex = 0;
            btnAddExtension.Text = "Add";
            btnAddExtension.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            btnClose.AutoSize = true;
            btnClose.BackColor = Color.FromArgb(51, 122, 183);
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(89, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 35);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            // 
            // ExtensionCountsForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(680, 560);
            Controls.Add(gridCounts);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
            MinimumSize = new Size(520, 520);
            Name = "ExtensionCountsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Extension Counts";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridCounts).EndInit();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            pnlBottomButtons.ResumeLayout(false);
            pnlBottomButtons.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
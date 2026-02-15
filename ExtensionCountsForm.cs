using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FileContentToolkit
{
    public partial class ExtensionCountsForm : Form
    {
        private readonly FileContentService _service;

        public List<string> AddedExtensions { get; private set; } = new List<string>();

        public ExtensionCountsForm(FileContentService service)
        {
            _service = service;
            InitializeComponent();

            if (IsDesignMode()) return;

            // Header labels
            lblPath.Text = string.IsNullOrEmpty(_service.FolderPath)
                ? "Folder: (not set)"
                : $"Folder: {_service.FolderPath}";
            lblSubfolders.Text = _service.IncludeSubfolders
                ? "Include subfolders: Yes"
                : "Include subfolders: No";

            // Events
            btnRefresh.Click += (s, e) => LoadData();
            btnClose.Click += (s, e) => Close();
            btnAddExtension.Click += BtnAddExtension_Click;

            // Double-click to add
            gridCounts.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) AddSelectedRowsExtensions();
            };

            // Multi-select support
            gridCounts.MultiSelect = true;
            gridCounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Modern styling tweaks (beyond designer)
            gridCounts.EnableHeadersVisualStyles = false;
            gridCounts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
            gridCounts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridCounts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            gridCounts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 122, 183);
            gridCounts.DefaultCellStyle.SelectionForeColor = Color.White;

            Shown += (s, e) => LoadData();
        }

        private bool IsDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime || (Site?.DesignMode ?? false);
        }

        private void LoadData()
        {
            var list = _service.GetAvailableExtensionCounts(false);

            var dt = new DataTable();
            dt.Columns.Add("Extension", typeof(string));
            dt.Columns.Add("Count", typeof(int));

            foreach (var x in list)
            {
                dt.Rows.Add(x.Extension, x.Count);
            }

            gridCounts.AutoGenerateColumns = true;
            gridCounts.DataSource = dt;

            foreach (DataGridViewColumn c in gridCounts.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            gridCounts.Columns["Extension"].HeaderText = "Extension";
            gridCounts.Columns["Count"].HeaderText = "Count";
            gridCounts.Columns["Count"].Width = 120;
            gridCounts.Columns["Extension"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            lblTotal.Text = $"Total files: {list.Sum(r => r.Count):N0}";
        }

        private void BtnAddExtension_Click(object? sender, EventArgs e)
        {
            AddSelectedRowsExtensions();
        }

        private void AddSelectedRowsExtensions()
        {
            var rows = gridCounts.SelectedRows.Cast<DataGridViewRow>().ToList();

            if (rows.Count == 0 && gridCounts.CurrentRow != null)
            {
                rows.Add(gridCounts.CurrentRow);
            }

            if (rows.Count == 0)
                return;

            bool anyAdded = false;

            foreach (var row in rows)
            {
                var extObj = row.Cells["Extension"].Value;
                if (extObj == null) continue;

                var ext = extObj.ToString()!.Trim();
                if (string.Equals(ext, "(no ext)", StringComparison.OrdinalIgnoreCase)) continue;
                if (!ext.StartsWith(".")) ext = "." + ext;

                if (!_service.Extensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                {
                    _service.AddExtension(ext);
                    AddedExtensions.Add(ext);
                    anyAdded = true;
                }
            }

            DialogResult = anyAdded ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }
    }
}
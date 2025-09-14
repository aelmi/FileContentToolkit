using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace FileContentToolkit
{
    public partial class ExtensionCountsForm : Form
    {
        private readonly FileContentService _service;

        // Track all extensions added during this dialog (for MainForm to reselect, etc.)
        public System.Collections.Generic.List<string> AddedExtensions { get; private set; } = new System.Collections.Generic.List<string>();

        public ExtensionCountsForm(FileContentService service)
        {
            _service = service;
            InitializeComponent();

            if (IsDesignMode()) return; // allow designer to render safely

            // Header text from service
            lblPath.Text = string.IsNullOrEmpty(_service.FolderPath)
                ? "Folder: (not set)"
                : $"Folder: {_service.FolderPath}";
            lblSubfolders.Text = _service.IncludeSubfolders
                ? "Include subfolder(s): Yes"
                : "Include subfolder(s): No";

            // Events
            btnRefresh.Click += (s, e) => LoadData();
            btnClose.Click += (s, e) => Close();
            btnAddExtension.Click += BtnAddExtension_Click;

            // Optional: double-click to add (uses multi-select handler too)
            gridCounts.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) AddSelectedRowsExtensions();
            };

            // Ensure multi-select is enabled (also set in Designer)
            gridCounts.MultiSelect = true;
            gridCounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Shown += (s, e) => LoadData();
        }

        private bool IsDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime || (Site?.DesignMode ?? false);
        }

        private void LoadData()
        {
            var list = _service.GetAvailableExtensionCounts(false);

            // Bind a DataTable so columns are sortable
            var dt = new DataTable();
            dt.Columns.Add("Extension", typeof(string));
            dt.Columns.Add("Count", typeof(int));
            foreach (var x in list)
                dt.Rows.Add(x.Extension, x.Count);

            gridCounts.AutoGenerateColumns = true;
            gridCounts.DataSource = dt;

            // Make columns clickable/sortable
            foreach (DataGridViewColumn c in gridCounts.Columns)
                c.SortMode = DataGridViewColumnSortMode.Automatic;

            // Friendly headers and sizing
            gridCounts.Columns["Extension"].HeaderText = "Extension";
            gridCounts.Columns["Count"].HeaderText = "Count";
            gridCounts.Columns["Count"].Width = 120;

            // Footer total
            lblTotal.Text = $"Total files: {list.Sum(r => r.Count)}";
        }


        private void BtnAddExtension_Click(object? sender, EventArgs e)
        {
            AddSelectedRowsExtensions();
        }

        private void AddSelectedRowsExtensions()
        {
            // Prefer SelectedRows; if none selected, fall back to CurrentRow
            var rows = gridCounts.SelectedRows.Cast<DataGridViewRow>().ToList();
            if (rows.Count == 0 && gridCounts.CurrentRow != null)
                rows.Add(gridCounts.CurrentRow);

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

            // Close dialog; return OK only if at least one new extension was added
            DialogResult = anyAdded ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }
    }
}
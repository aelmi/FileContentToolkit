using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle
{
    public partial class ExtensionCountsForm : ThemedForm
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
            btnRefresh.Click += async (s, e) => await LoadDataAsync();
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

            // Grid colours and the header font come from the theme; the applier owns every cell
            // style on this control so that dark mode reaches the header row too.
            gridCounts.EnableHeadersVisualStyles = false;
            gridCounts.ColumnHeadersDefaultCellStyle.Font = ThemeFonts.Get(FontRole.MediumBold);

            Shown += async (s, e) => await LoadDataAsync();
        }

        private bool IsDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime || (Site?.DesignMode ?? false);
        }

        /// <summary>
        /// Enumerating on Shown ran a full-tree walk synchronously on the UI thread — a
        /// multi-minute freeze on a large or network folder, with no window painted.
        /// </summary>
        private async Task LoadDataAsync()
        {
            List<(string Extension, int Count)> list;
            try
            {
                UseWaitCursor = true;
                list = await _service.GetAvailableExtensionCountsAsync(false, CancellationToken.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "The folder could not be scanned: " + ex.Message,
                    "Extension counts", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            finally { UseWaitCursor = false; }

            if (IsDisposed) return;

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

            gridCounts.Sort(gridCounts.Columns["Extension"], ListSortDirection.Ascending);

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
                if (!ext.StartsWith('.')) ext = "." + ext;

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
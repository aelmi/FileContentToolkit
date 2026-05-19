using System;
using System.Linq;
using System.Windows.Forms;
using FileContentToolkit.Settings;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public partial class PresetManagerForm : Form
    {
        private readonly AppSettings _settings;

        public Preset? SelectedPreset { get; private set; }
        public bool LoadRequested { get; private set; }

        public PresetManagerForm(AppSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            InitializeComponent();

            if (Theme.AppIcon != null) Icon = Theme.AppIcon;
            Theme.AttachHover(btnLoad, btnLoad.BackColor);
            Theme.AttachHover(btnRename, btnRename.BackColor);
            Theme.AttachHover(btnDelete, btnDelete.BackColor);
            Theme.AttachHover(btnClose, btnClose.BackColor);

            ReloadList();
        }

        private void ReloadList()
        {
            lstPresets.BeginUpdate();
            try
            {
                lstPresets.Items.Clear();
                foreach (var p in _settings.Presets.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
                    lstPresets.Items.Add(p.Name);
            }
            finally { lstPresets.EndUpdate(); }
            RefreshDetails();
        }

        private Preset? Current() =>
            lstPresets.SelectedItem is string name
                ? _settings.Presets.FirstOrDefault(p => p.Name == name)
                : null;

        private void LstPresets_SelectedIndexChanged(object? sender, EventArgs e) => RefreshDetails();

        private void RefreshDetails()
        {
            var p = Current();
            if (p == null)
            {
                lblDetails.Text = "(no preset selected)";
                btnLoad.Enabled = btnRename.Enabled = btnDelete.Enabled = false;
                return;
            }
            btnLoad.Enabled = btnRename.Enabled = btnDelete.Enabled = true;
            lblDetails.Text =
                $"Name: {p.Name}\r\n" +
                $"Folder: {p.FolderPath}\r\n" +
                $"Include subfolders: {p.IncludeSubfolders}\r\n\r\n" +
                $"Extensions:\r\n  {string.Join(", ", p.Extensions)}\r\n\r\n" +
                $"Ignore patterns:\r\n  {string.Join(", ", p.IgnorePatterns)}";
        }

        private void BtnLoad_Click(object? sender, EventArgs e)
        {
            var p = Current();
            if (p == null) return;
            SelectedPreset = p;
            LoadRequested = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnRename_Click(object? sender, EventArgs e)
        {
            var p = Current();
            if (p == null) return;
            var newName = ThemedPrompt.Show(this, "Rename preset", "New name:", p.Name);
            if (string.IsNullOrWhiteSpace(newName) || newName == p.Name) return;
            if (_settings.Presets.Any(x => x.Name.Equals(newName, StringComparison.OrdinalIgnoreCase) && !ReferenceEquals(x, p)))
            {
                MessageBox.Show(this, "A preset with that name already exists.", "Rename",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            p.Name = newName;
            ReloadList();
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var p = Current();
            if (p == null) return;
            if (MessageBox.Show(this, $"Delete preset \"{p.Name}\"?", "Delete preset",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            _settings.Presets.Remove(p);
            ReloadList();
        }
    }
}

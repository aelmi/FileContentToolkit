using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FileContentToolkit.Settings;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public class PresetManagerForm : Form
    {
        private readonly AppSettings _settings;

        private readonly ListBox _list;
        private readonly Label _details;
        private readonly Button _btnLoad;
        private readonly Button _btnRename;
        private readonly Button _btnDelete;
        private readonly Button _btnClose;

        public Preset? SelectedPreset { get; private set; }
        public bool LoadRequested { get; private set; }

        public PresetManagerForm(AppSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            Text = "Manage Presets";
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(620, 440);
            ClientSize = new Size(720, 500);
            Theme.ApplyForm(this);

            Controls.Add(Theme.BuildHeader("Manage Presets", "Saved folder + extension configurations."));

            var bottom = Theme.BuildBottomBar();
            _btnClose = Theme.SecondaryButton("Close");
            _btnClose.Size = new Size(90, 36);
            _btnClose.DialogResult = DialogResult.Cancel;
            bottom.Resize += (s, e) =>
            {
                _btnClose.Left = bottom.ClientSize.Width - 20 - _btnClose.Width;
                _btnClose.Top = (bottom.ClientSize.Height - _btnClose.Height) / 2;
            };
            bottom.Controls.Add(_btnClose);
            Controls.Add(bottom);
            CancelButton = _btnClose;

            var body = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.White,
                Padding = new Padding(20, 18, 20, 18)
            };

            _list = new ListBox
            {
                Left = 0,
                Top = 0,
                Width = 240,
                Height = 320,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                Font = Theme.BodyFont,
                BorderStyle = BorderStyle.FixedSingle
            };
            _list.SelectedIndexChanged += (s, e) => RefreshDetails();

            _details = new Label
            {
                Left = 256,
                Top = 0,
                Width = 380,
                Height = 280,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Theme.FormBg,
                ForeColor = Theme.BodyText,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(10),
                AutoEllipsis = true,
                Font = Theme.BodyFont
            };

            _btnLoad = Theme.SuccessButton("Load");
            _btnLoad.Size = new Size(80, 36);
            _btnLoad.Left = 256; _btnLoad.Top = 290;
            _btnLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            _btnRename = Theme.PrimaryButton("Rename");
            _btnRename.Size = new Size(85, 36);
            _btnRename.Left = _btnLoad.Right + 6; _btnRename.Top = 290;
            _btnRename.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            _btnDelete = Theme.DangerButton("Delete");
            _btnDelete.Size = new Size(85, 36);
            _btnDelete.Left = _btnRename.Right + 6; _btnDelete.Top = 290;
            _btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            _btnLoad.Click += (s, e) => LoadSelected();
            _btnRename.Click += (s, e) => RenameSelected();
            _btnDelete.Click += (s, e) => DeleteSelected();

            body.Controls.AddRange(new Control[] { _list, _details, _btnLoad, _btnRename, _btnDelete });
            Controls.Add(body);
            body.BringToFront();

            ReloadList();
        }

        private void ReloadList()
        {
            _list.BeginUpdate();
            try
            {
                _list.Items.Clear();
                foreach (var p in _settings.Presets.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
                    _list.Items.Add(p.Name);
            }
            finally { _list.EndUpdate(); }
            RefreshDetails();
        }

        private Preset? Current() =>
            _list.SelectedItem is string name
                ? _settings.Presets.FirstOrDefault(p => p.Name == name)
                : null;

        private void RefreshDetails()
        {
            var p = Current();
            if (p == null) { _details.Text = "(no preset selected)"; return; }
            _details.Text =
                $"Name: {p.Name}\r\n" +
                $"Folder: {p.FolderPath}\r\n" +
                $"Include subfolders: {p.IncludeSubfolders}\r\n\r\n" +
                $"Extensions:\r\n  {string.Join(", ", p.Extensions)}\r\n\r\n" +
                $"Ignore patterns:\r\n  {string.Join(", ", p.IgnorePatterns)}";
        }

        private void LoadSelected()
        {
            var p = Current();
            if (p == null) return;
            SelectedPreset = p;
            LoadRequested = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void RenameSelected()
        {
            var p = Current();
            if (p == null) return;
            var newName = ThemedPrompt.Show(this, "Rename preset", "New name:", p.Name);
            if (string.IsNullOrWhiteSpace(newName) || newName == p.Name) return;
            if (_settings.Presets.Any(x => x.Name.Equals(newName, StringComparison.OrdinalIgnoreCase) && !ReferenceEquals(x, p)))
            {
                MessageBox.Show(this, "A preset with that name already exists.", "Rename", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            p.Name = newName;
            ReloadList();
        }

        private void DeleteSelected()
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

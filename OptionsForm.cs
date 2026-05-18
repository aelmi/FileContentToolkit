using System;
using System.Windows.Forms;
using FileContentToolkit.Settings;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public partial class OptionsForm : Form
    {
        private readonly AppSettings _settings;

        public OptionsForm(AppSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            InitializeComponent();

            // Pick up the shared app icon (the Designer-baked colors/fonts already match the theme)
            if (Theme.AppIcon != null) Icon = Theme.AppIcon;

            // Hover effects on themed buttons (matches the rest of the app)
            Theme.AttachHover(btnOk, btnOk.BackColor);
            Theme.AttachHover(btnCancel, btnCancel.BackColor);

            LoadFromSettings();
        }

        private void LoadFromSettings()
        {
            numMaxKb.Value = Math.Min(int.MaxValue, Math.Max(0, _settings.MaxFileSizeBytes / 1024));
            chkSkipBinary.Checked = _settings.SkipBinaryFiles;
            chkAutoEncoding.Checked = _settings.AutoDetectEncoding;
            chkGitIgnore.Checked = _settings.UseGitIgnoreFiles;
            chkWatch.Checked = _settings.WatchFolderForChanges;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            _settings.MaxFileSizeBytes = (long)numMaxKb.Value * 1024L;
            _settings.SkipBinaryFiles = chkSkipBinary.Checked;
            _settings.AutoDetectEncoding = chkAutoEncoding.Checked;
            _settings.UseGitIgnoreFiles = chkGitIgnore.Checked;
            _settings.WatchFolderForChanges = chkWatch.Checked;
        }
    }
}

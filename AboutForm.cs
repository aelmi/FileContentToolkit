using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using FileContentToolkit.Settings;
using FileContentToolkit.UI;

namespace FileContentToolkit.Dialogs
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            if (Theme.AppIcon != null)
            {
                Icon = Theme.AppIcon;
                picIcon.Image = Theme.AppIcon.ToBitmap();
            }
            Theme.AttachHover(btnClose, btnClose.BackColor);

            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            lblVersion.Text = ver != null ? $"Version {ver.ToString(3)}" : "Version 1.0.0";
            lblCopyright.Text = $"© {DateTime.Now.Year}";
        }

        private void LnkProject_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var folder = Path.GetDirectoryName(AppSettings.SettingsPath);
                if (string.IsNullOrEmpty(folder)) return;
                Directory.CreateDirectory(folder);
                Process.Start(new ProcessStartInfo
                {
                    FileName = folder,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not open settings folder: " + ex.Message,
                    "Open folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

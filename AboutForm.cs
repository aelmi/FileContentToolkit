using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CodeShuttle.Controls;
using CodeShuttle.Diagnostics;
using CodeShuttle.Settings;
using CodeShuttle.Theming;
using CodeShuttle.UI;

namespace CodeShuttle.Dialogs
{
    public partial class AboutForm : ThemedForm
    {
        public AboutForm()
        {
            InitializeComponent();

            if (Theme.AppIcon != null)
            {
                Icon = Theme.AppIcon;
                picIcon.Image = Theme.AppIcon.ToBitmap();
            }

            lblAppName.Text = AboutInfo.ProductName;
            lblEdition.Text = AboutInfo.Edition + " edition";
            lblVersion.Text = "Version " + AppVersion.Full;

            // A bare year is not a copyright notice. The holder is required.
            lblCopyright.Text = AboutInfo.Copyright;

            txtNotices.Text = AboutInfo.ThirdPartyNotices.Replace("\n", Environment.NewLine);
        }

        // ------------------------------------------------------------------ links

        private void LnkWebsite_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
            => OpenUrl(AboutInfo.WebsiteUrl);

        private void LnkDocs_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
            => OpenUrl(AboutInfo.DocsUrl);

        private void LnkReleaseNotes_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
            => OpenUrl(AboutInfo.ReleaseNotesUrl);

        private void LnkReportBug_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
            => OpenUrl(AboutInfo.ReportBugUrl);

        /// <summary>Shows the licence text that ships beside the application.</summary>
        private void LnkProject_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "LICENSE.txt");
                if (!File.Exists(path))
                {
                    MessageBox.Show(this,
                        "The licence file could not be found beside the application.",
                        "Licence", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or IOException or UnauthorizedAccessException)
            {
                MessageBox.Show(this, "Could not open the licence: " + ex.Message,
                    "Licence", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Opens an external link, refusing anything that is not https.
        /// </summary>
        /// <remarks>
        /// The URLs here are compile-time constants, so this is defence in depth rather than a
        /// live threat — but <c>UseShellExecute</c> will happily launch a local executable given
        /// the right string, and the equivalent check already guards the update notice.
        /// </remarks>
        private void OpenUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                return;

            try
            {
                Process.Start(new ProcessStartInfo { FileName = uri.AbsoluteUri, UseShellExecute = true });
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or IOException)
            {
                MessageBox.Show(this, "Could not open the link: " + ex.Message,
                    "Open link", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ------------------------------------------------------------------ support actions

        private void BtnCopyDiagnostics_Click(object? sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(DiagnosticsReport.Build());
                Toast.Show(this, "Diagnostics copied to the clipboard.");
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                MessageBox.Show(this, "Could not copy the diagnostics: " + ex.Message,
                    "Copy diagnostics", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnOpenSettings_Click(object? sender, EventArgs e)
            => OpenFolder(Path.GetDirectoryName(AppSettings.SettingsPath), "settings");

        private void BtnOpenLogs_Click(object? sender, EventArgs e)
            => OpenFolder(CrashLogger.LogsDirectory, "log");

        private void OpenFolder(string? folder, string what)
        {
            if (string.IsNullOrEmpty(folder)) return;

            try
            {
                Directory.CreateDirectory(folder);
                Process.Start(new ProcessStartInfo { FileName = folder, UseShellExecute = true });
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or IOException or UnauthorizedAccessException)
            {
                MessageBox.Show(this, $"Could not open the {what} folder: " + ex.Message,
                    "Open folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Automation;
using CodeShuttle.Controls;
using CodeShuttle.Settings;
using CodeShuttle.Theming;
using CodeShuttle.UI;

namespace CodeShuttle
{
    /// <summary>
    /// Shell concerns: keyboard routing, window and splitter persistence, the progress and
    /// cancellation affordance, skipped-file reporting, and the empty and first-run states.
    /// </summary>
    public partial class MainForm
    {
        // ------------------------------------------------------------------ busy / progress

        /// <summary>What, if anything, is currently running. Only one at a time by construction.</summary>
        private enum BusyOperation
        {
            None,
            Scan,
            Generate,
            Apply,

            /// <summary>
            /// Running, but with no cancellation source behind it — the content search loop has
            /// no token. Kept distinct from <see cref="None"/> so the status text still appears
            /// while the Cancel button correctly does not.
            /// </summary>
            Search,
        }

        private BusyOperation _busy = BusyOperation.None;

        /// <summary>
        /// Staged reveal timer. The convention is: past 200 ms show a wait cursor, past 1 s put
        /// text in the status bar, past 3 s show the progress bar and the cancel button. Showing
        /// all of it immediately makes a fast scan flicker; showing none of it makes a slow one
        /// look like a hang.
        /// </summary>
        private System.Windows.Forms.Timer? _busyReveal;

        private int _busyElapsedMs;
        private string _busyLabel = "";

        private const int BusyCursorAtMs = 200;
        private const int BusyStatusAtMs = 1000;
        private const int BusyProgressAtMs = 3000;

        private void BeginBusy(BusyOperation operation, string label)
        {
            _busy = operation;
            _busyLabel = label;
            _busyElapsedMs = 0;

            _busyReveal ??= CreateBusyRevealTimer();
            _busyReveal.Start();
        }

        private System.Windows.Forms.Timer CreateBusyRevealTimer()
        {
            var timer = new System.Windows.Forms.Timer { Interval = 100 };
            timer.Tick += (s, e) =>
            {
                _busyElapsedMs += timer.Interval;

                if (_busyElapsedMs >= BusyCursorAtMs && Cursor != Cursors.AppStarting)
                    Cursor = Cursors.AppStarting;

                if (_busyElapsedMs >= BusyStatusAtMs && sbScanStatus.Text != _busyLabel)
                    sbScanStatus.Text = _busyLabel;

                if (_busyElapsedMs >= BusyProgressAtMs && !sbProgress.Visible)
                {
                    sbProgress.Visible = true;
                    // Only offered where there is a token behind it. A Cancel button that does
                    // nothing is worse than no Cancel button.
                    sbCancel.Visible = _busy is BusyOperation.Scan or BusyOperation.Generate or BusyOperation.Apply;
                }

                if (_busyElapsedMs >= BusyProgressAtMs) timer.Stop();
            };
            return timer;
        }

        private void ReportBusyProgress(int percent)
        {
            int clamped = Math.Min(100, Math.Max(0, percent));
            sbProgress.Value = clamped;

            // Only meaningful once the bar is actually on screen.
            if (sbProgress.Visible) sbScanStatus.Text = $"{_busyLabel} {clamped}%";
        }

        private void EndBusy(string? finalStatus)
        {
            _busy = BusyOperation.None;
            _busyReveal?.Stop();
            _busyElapsedMs = 0;

            Cursor = Cursors.Default;
            sbProgress.Visible = false;
            sbProgress.Value = 0;
            sbCancel.Visible = false;

            if (finalStatus != null)
            {
                sbScanStatus.Text = finalStatus;
                AnnounceStatus(finalStatus);
            }
        }

        /// <summary>
        /// Pushes a status change through UI Automation. A status bar that changes silently is
        /// invisible to a screen reader, which is where most of this product's feedback lives.
        /// </summary>
        private void AnnounceStatus(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            try
            {
                statusBar.AccessibilityObject?.RaiseAutomationNotification(
                    AutomationNotificationKind.Other,
                    AutomationNotificationProcessing.MostRecent,
                    message);
            }
            catch (NotSupportedException)
            {
                // Not implemented on older Windows builds.
            }
        }

        /// <summary>Cancels whichever operation the progress bar is currently reporting.</summary>
        private void SbCancel_Click(object? sender, EventArgs e) => CancelCurrentOperation();

        private void CancelCurrentOperation()
        {
            switch (_busy)
            {
                case BusyOperation.Scan: _scanCts?.Cancel(); break;
                case BusyOperation.Generate: _generateCts?.Cancel(); break;
                case BusyOperation.Apply: _applyCts?.Cancel(); break;
                case BusyOperation.None:
                case BusyOperation.Search:
                    break;
            }
        }

        // ------------------------------------------------------------------ skipped files

        private IReadOnlyList<SkippedFile> _lastSkipped = Array.Empty<SkippedFile>();

        /// <summary>
        /// Surfaces the skip list the scan already produced. Without this the user exports a
        /// "complete" bundle that is quietly missing files — the failure mode most corrosive to
        /// trust in a tool sold on packing up a whole codebase.
        /// </summary>
        private void UpdateSkippedIndicator(IReadOnlyList<SkippedFile>? skipped)
        {
            _lastSkipped = skipped ?? Array.Empty<SkippedFile>();

            if (_lastSkipped.Count == 0)
            {
                sbSkipped.Visible = false;
                sbSkipped.Text = "";
                return;
            }

            sbSkipped.Text = _lastSkipped.Count == 1
                ? "1 file skipped"
                : $"{_lastSkipped.Count} files skipped";
            sbSkipped.Visible = true;
        }

        private void SbSkipped_Click(object? sender, EventArgs e)
        {
            if (_lastSkipped.Count == 0) return;

            var sb = new StringBuilder();
            foreach (var group in _lastSkipped.GroupBy(f => f.Reason).OrderBy(g => g.Key.ToString(), StringComparer.Ordinal))
            {
                sb.AppendLine(CultureInfo.CurrentCulture, $"{DescribeReason(group.Key)} ({group.Count()})");
                foreach (var file in group.Take(20)) sb.AppendLine("    " + file.Path);
                if (group.Count() > 20) sb.AppendLine(CultureInfo.CurrentCulture, $"    … and {group.Count() - 20} more");
                sb.AppendLine();
            }

            MessageBox.Show(this, sb.ToString().TrimEnd(), "Skipped files",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static string DescribeReason(SkipReason reason) => reason switch
        {
            SkipReason.Binary => "Looked like binary content",
            SkipReason.TooLarge => "Larger than the configured maximum size",
            SkipReason.AccessDenied => "Permission denied",
            SkipReason.IoError => "Could not be read (locked, or a device error)",
            SkipReason.IgnoredByRule => "Excluded by an ignore rule",
            SkipReason.UnsafePath => "Path failed containment validation",
            _ => reason.ToString(),
        };

        // ------------------------------------------------------------------ window placement

        /// <summary>
        /// Restores size, position, state and splitter. Suppressed while restoring so the
        /// splitter's own move event does not immediately write back what it just read.
        /// </summary>
        private bool _restoringPlacement;

        private void RestoreWindowPlacement()
        {
            _restoringPlacement = true;
            try
            {
                var saved = _settings.WindowBounds;
                if (saved != null && saved.Width > 0 && saved.Height > 0)
                {
                    var bounds = new Rectangle(saved.X, saved.Y, saved.Width, saved.Height);

                    // A window remembered on a monitor that has since been unplugged restores
                    // off-screen and cannot be reached with the mouse.
                    if (WindowPlacement.IsVisibleOnAnyScreen(bounds))
                    {
                        StartPosition = FormStartPosition.Manual;
                        Bounds = bounds;
                    }
                }

                // Minimised is never persisted, but an older or hand-edited file could hold it.
                WindowState = _settings.WindowState == FormWindowState.Minimized
                    ? FormWindowState.Normal
                    : _settings.WindowState;

                ApplySplitterDistance(_settings.SplitterDistance);
            }
            finally
            {
                _restoringPlacement = false;
            }
        }

        /// <summary>
        /// Clamps the saved distance into what the splitter will currently accept. Assigning a
        /// value outside Panel1MinSize..(Width - Panel2MinSize) throws.
        /// </summary>
        private void ApplySplitterDistance(int distance)
        {
            if (distance <= 0) return;

            int max = splitMain.Width - splitMain.Panel2MinSize - splitMain.SplitterWidth;
            if (max <= splitMain.Panel1MinSize) return;

            splitMain.SplitterDistance = Math.Min(Math.Max(distance, splitMain.Panel1MinSize), max);
        }

        private void SaveWindowPlacement()
        {
            // RestoreBounds rather than Bounds: while maximised or minimised, Bounds describes the
            // maximised frame, so saving it loses the size to restore to.
            var bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;

            _settings.WindowBounds = new WindowBoundsSetting
            {
                X = bounds.X,
                Y = bounds.Y,
                Width = bounds.Width,
                Height = bounds.Height,
            };

            // Restoring to a minimised window looks like the app failed to launch.
            _settings.WindowState = WindowState == FormWindowState.Minimized
                ? FormWindowState.Normal
                : WindowState;

            _settings.SplitterDistance = splitMain.SplitterDistance;
        }

        private void SplitMain_SplitterMoved(object? sender, SplitterEventArgs e)
        {
            if (_restoringPlacement) return;
            _settings.SplitterDistance = splitMain.SplitterDistance;
            _settings.SaveDebounced();
        }

        // ------------------------------------------------------------------ empty / first run

        private EmptyStateView? _emptyFiles;

        /// <summary>
        /// Shows or hides the guidance panel over the file list. The rule that an extension must
        /// be added before Add Folder does anything was previously enforced only by an error
        /// dialog raised after the user had already tried it.
        /// </summary>
        private void UpdateEmptyStates()
        {
            if (lstFiles.Items.Count > 0)
            {
                if (_emptyFiles != null) _emptyFiles.Visible = false;
                return;
            }

            _emptyFiles ??= CreateFilesEmptyState();

            bool needsExtension = fileService.Extensions.Count == 0;
            bool firstRun = !_settings.HasCompletedFirstRun;

            if (needsExtension)
            {
                _emptyFiles.Title = firstRun ? "Welcome to CodeShuttle" : "Add a file extension first";
                _emptyFiles.Body = firstRun
                    ? "Send your code to AI, then bring the answers back. Start by choosing which " +
                      "file types to collect — add an extension such as .cs or .py, then pick a folder."
                    : "Nothing is collected until at least one extension is listed. Add one such as " +
                      ".cs or .py, then choose a folder.";
                _emptyFiles.ActionText = "Add an extension";
            }
            else
            {
                _emptyFiles.Title = "No files yet";
                _emptyFiles.Body = "The extensions are set. Choose a folder to scan, or drag files " +
                                   "straight onto this list.";
                _emptyFiles.ActionText = "Browse for a folder";
            }

            _emptyFiles.Visible = true;
            _emptyFiles.BringToFront();
        }

        private EmptyStateView CreateFilesEmptyState()
        {
            var view = new EmptyStateView { Name = "emptyFiles" };
            view.ActionClicked += (s, e) =>
            {
                MarkFirstRunComplete();
                if (fileService.Extensions.Count == 0)
                {
                    cmbExtension.Focus();
                    cmbExtension.DroppedDown = true;
                }
                else
                {
                    BtnBrowse_Click(this, EventArgs.Empty);
                }
            };

            grpFiles.Controls.Add(view);
            view.BringToFront();

            // Created after the form was themed, so it has to be painted explicitly.
            ThemeApplier.Apply(view, ThemeManager.Tokens, ThemeManager.IsDark);
            return view;
        }

        private void MarkFirstRunComplete()
        {
            if (_settings.HasCompletedFirstRun) return;
            _settings.HasCompletedFirstRun = true;
            _settings.SaveDebounced();
        }

        // ------------------------------------------------------------------ recreate strip

        /// <summary>
        /// Kept because the designer still wires the hide button's Click, but the button is no
        /// longer shown: the round-trip strip is permanent. Hiding it is a no-op rather than a
        /// removed handler so that the wiring cannot dangle.
        /// </summary>
        private static void BtnHideRecreateInfo_Click(object? sender, EventArgs e)
        {
        }

        /// <summary>
        /// Keeps the round-trip strip's actions in step with whether there is a pack to act on.
        /// </summary>
        /// <remarks>
        /// The strip itself is permanent. It was a dismissible banner shown only once output
        /// existed, which meant the second half of the product — paste the reply back, review it
        /// as a diff, apply it — was invisible to anyone who had not already generated something,
        /// and permanently invisible to anyone who had once clicked "hide". Pasting a reply does
        /// not require a pack to be open, so hiding the entry point behind one was backwards.
        /// </remarks>
        private void UpdateRecreateStrip()
        {
            pnlRecreateInfo.Visible = true;
            // Applying needs a pack in the pane; pasting a reply brings its own. Guarded because
            // reading the length re-creates a torn-down RichTextBox handle — see OutputReadable.
            btnApplyAiChanges.Enabled = OutputReadable && rtbOutput.TextLength > 0;
        }

        // ------------------------------------------------------------------ keyboard

        /// <summary>
        /// Central keyboard routing.
        /// </summary>
        /// <remarks>
        /// Handled here rather than on individual controls because the product's only two chords
        /// used to live on the output box's KeyDown, so Ctrl+F worked exclusively while that box
        /// had focus. <c>ProcessCmdKey</c> sees the key before any control does, so a binding is
        /// global to the window by construction.
        /// </remarks>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Shortcuts.SearchInFiles.Keys) { searchBox.FocusQuery(); return true; }
            if (keyData == Shortcuts.BrowseFolder.Keys) { BtnBrowse_Click(this, EventArgs.Empty); return true; }
            if (keyData == Shortcuts.AddFolder.Keys) { BtnAddFolder_Click(this, EventArgs.Empty); return true; }

            if (keyData == Shortcuts.Refresh.Keys || keyData == Shortcuts.RefreshAlt.Keys)
            {
                _ = RefreshFilesInBackground();
                return true;
            }

            if (keyData == Shortcuts.Generate.Keys || keyData == Shortcuts.GenerateAlt.Keys)
            {
                if (btnGenerate.Enabled) BtnGenerate_Click(this, EventArgs.Empty);
                return true;
            }

            if (keyData == Shortcuts.CopyOutput.Keys)
            {
                // Inside a text box Ctrl+C must keep copying the selection; intercepting it there
                // would break the most ordinary keystroke in the application.
                if (ActiveControl is TextBoxBase) return base.ProcessCmdKey(ref msg, keyData);
                BtnCopyOutput_Click(this, EventArgs.Empty);
                return true;
            }

            if (keyData == Shortcuts.CopyOutputAs.Keys)
            {
                cmsCopyAs.Show(btnCopyOutput, new Point(0, btnCopyOutput.Height));
                return true;
            }

            if (keyData == Shortcuts.ExportOutput.Keys) { BtnExportOutput_Click(this, EventArgs.Empty); return true; }

            if (keyData == Shortcuts.PasteResponse.Keys) { BtnPasteResponse_Click(this, EventArgs.Empty); return true; }

            // Before base, which is where the menu's own shortcut keys are dispatched: F1 must
            // resolve the topic for the focused pane rather than always opening the same page.
            if (keyData == Shortcuts.Help.Keys) { ShowContextualHelp(); return true; }

            if (keyData == Shortcuts.Find.Keys || keyData == Shortcuts.Replace.Keys)
            {
                BtnFindReplace_Click(this, EventArgs.Empty);
                return true;
            }

            if (keyData == Shortcuts.Options.Keys) { BtnOptions_Click(this, EventArgs.Empty); return true; }
            if (keyData == Shortcuts.Presets.Keys) { BtnLoadPreset_Click(this, EventArgs.Empty); return true; }

            if (keyData == Shortcuts.CancelOperation.Keys &&
                _busy is BusyOperation.Scan or BusyOperation.Generate or BusyOperation.Apply)
            {
                CancelCurrentOperation();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}

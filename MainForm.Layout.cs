using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Controls;
using CodeShuttle.Theming;
using CodeShuttle.UI;

namespace CodeShuttle
{
    /// <summary>
    /// The main window's structure.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The window is a pipeline — pick a source, filter it, pack it, ship it and apply the reply —
    /// and the previous arrangement expressed none of that. The stages were spread across an
    /// accent-filled header band, two <see cref="GroupBox"/> frames whose titles named controls
    /// rather than steps, a Generate button floating alone at the bottom centre, and three menus.
    /// Here the pipeline is the layout: the rail reads Source → Filters → Files top to bottom, and
    /// the right pane reads Pack → budget → round-trip.
    /// </para>
    /// <para>
    /// Built in code rather than in the designer file, and run after <c>InitializeComponent</c>.
    /// Every control keeps the field name, event wiring, accessible name and tooltip the designer
    /// gave it — this method only re-parents and re-docks them. That is what lets a restructure of
    /// this size leave the seventeen hundred lines of handler logic in the other partials, and the
    /// test suite, completely untouched.
    /// </para>
    /// <para>
    /// Docked children are laid out from the end of the Controls collection backwards, so the last
    /// control added claims its edge first and the control added first receives whatever is left.
    /// Every <c>Controls.Add</c> sequence below therefore starts with the filling child and ends
    /// with the outermost edge.
    /// </para>
    /// </remarks>
    public partial class MainForm
    {
        private ChipList chipExtensions = null!;
        private SectionHeader hdrSource = null!;
        private SectionHeader hdrFilters = null!;
        private SectionHeader hdrFiles = null!;
        private EmptyStateView emptyOutput = null!;

        private Panel pnlCommandBar = null!;
        private Panel pnlFolderChip = null!;
        private Panel railHost = null!;
        private Panel paneHost = null!;
        private Panel secSource = null!;
        private Panel secFilters = null!;
        private Panel secFiles = null!;
        private Panel railFooter = null!;
        private Panel outputHost = null!;
        private Panel ignoreRow = null!;
        private Label lblFolderGlyph = null!;
        private Label lblProtect = null!;

        /// <summary>
        /// Extension the "+ add" pill prompts for, so a custom extension can still be typed now
        /// that the combo box and its Add button are gone.
        /// </summary>
        private ToolStripMenuItem mnuAddCustom = null!;

        private ToolStripMenuItem mnuPsSaveCurrent = null!;

        /// <summary>The project catalogue, at the head of the Presets menu.</summary>
        private ToolStripMenuItem mnuPsProjectType = null!;

        private void BuildLayout()
        {
            SuspendLayout();

            DetachOldContainers();
            BuildCommandBar();
            BuildRail();
            BuildPane();

            // The splitter survives: it is the one piece of the old structure that was already
            // right, and it carries the user's saved pane width.
            splitMain.Panel1.Controls.Add(railHost);
            splitMain.Panel2.Controls.Add(paneHost);
            splitMain.Panel1MinSize = 300;
            splitMain.Panel2MinSize = 380;

            Controls.Clear();
            Controls.Add(splitMain);
            Controls.Add(pnlCommandBar);
            Controls.Add(menuMain);
            Controls.Add(statusBar);
            MainMenuStrip = menuMain;

            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Empties the containers the restructure dissolves, without disposing their children —
        /// the children are about to be re-parented and must keep their handles and wiring.
        /// </summary>
        private void DetachOldContainers()
        {
            foreach (var container in new Control[]
                     { pnlTop, pnlLeft, pnlRight, pnlBottom, grpExtensions, grpFiles, pnlFileButtons, pnlOutput })
            {
                container.Controls.Clear();
            }
            splitMain.Panel1.Controls.Clear();
            splitMain.Panel2.Controls.Clear();
        }

        // ------------------------------------------------------------------ command bar

        private void BuildCommandBar()
        {
            pnlFolderChip = new Panel { Dock = DockStyle.Fill, Name = "pnlFolderChip", Padding = new Padding(1) };
            pnlFolderChip.Paint += (s, e) =>
            {
                var t = ThemeManager.Tokens;
                using var pen = new Pen(txtFolderPath.Focused ? t.BorderFocus : t.Border,
                                        txtFolderPath.Focused ? 1.6f : 1f);
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, pnlFolderChip.Width - 1, pnlFolderChip.Height - 1));
            };
            ThemeRoles.Set(pnlFolderChip, ThemeRole.Surface);

            lblFolderGlyph = new Label
            {
                Dock = DockStyle.Left,
                Text = "▤",
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                TabStop = false,
                AccessibleRole = AccessibleRole.Graphic,
            };
            ThemeRoles.Set(lblFolderGlyph, ThemeRole.Heading, FontRole.Medium);

            txtFolderPath.Dock = DockStyle.Fill;
            txtFolderPath.BorderStyle = BorderStyle.None;
            // A text box would otherwise take the sunken fill by type and read as a grey slab
            // inside the chip that already provides the well.
            ThemeRoles.Set(txtFolderPath, ThemeRole.Surface, FontRole.Body);
            txtFolderPath.GotFocus += (s, e) => pnlFolderChip.Invalidate();
            txtFolderPath.LostFocus += (s, e) => pnlFolderChip.Invalidate();

            // The chip's own caret, rather than a separate labelled button out on a toolbar: the
            // recent list is a property of the folder field, so it belongs on it.
            btnRecentFolders.Dock = DockStyle.Right;
            btnRecentFolders.Text = "▾";
            btnRecentFolders.FlatStyle = FlatStyle.Flat;
            btnRecentFolders.FlatAppearance.BorderSize = 0;
            btnRecentFolders.AutoSize = false;
            ThemeRoles.Set(btnRecentFolders, ThemeRole.ButtonSubtle);

            pnlFolderChip.Controls.Add(txtFolderPath);
            pnlFolderChip.Controls.Add(lblFolderGlyph);
            pnlFolderChip.Controls.Add(btnRecentFolders);

            // Refresh loses its word and becomes the glyph it always was; it sits with Browse and
            // Tree because all three act on the folder, not on the extension list it used to live in.
            btnRefreshExtensions.Text = "↻";
            btnOptions.Text = "⚙";
            btnBrowse.Text = "Browse";
            btnTree.Text = "Tree";

            foreach (var b in new[] { btnBrowse, btnTree, btnRefreshExtensions, btnOptions })
            {
                b.Dock = DockStyle.Right;
                b.AutoSize = false;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.Margin = new Padding(0);
                ThemeRoles.Set(b, ThemeRole.ButtonSubtle, FontRole.Body);
            }
            cmbEncoding.Dock = DockStyle.Right;

            pnlCommandBar = new Panel { Dock = DockStyle.Top, Name = "pnlCommandBar", Padding = new Padding(12, 8, 12, 8) };
            ThemeRoles.Set(pnlCommandBar, ThemeRole.Surface);

            pnlCommandBar.Controls.Add(pnlFolderChip);
            pnlCommandBar.Controls.Add(btnBrowse);
            pnlCommandBar.Controls.Add(btnTree);
            pnlCommandBar.Controls.Add(btnRefreshExtensions);
            pnlCommandBar.Controls.Add(cmbEncoding);
            pnlCommandBar.Controls.Add(btnOptions);
        }

        // ------------------------------------------------------------------ rail

        private void BuildRail()
        {
            // ---- source ----------------------------------------------------------------
            hdrSource = NewHeader("Source");

            foreach (var c in new[] { chkIncludeSubfolders, chkWatch })
            {
                c.Dock = DockStyle.Top;
                c.AutoSize = true;
                c.Margin = new Padding(0);
                c.Padding = new Padding(0, 3, 0, 3);
                // Watch folder was styled as a filled button on the old accent header band. On the
                // rail that fill spans the full column and reads as a selected row.
                ThemeRoles.Set(c, ThemeRole.Default, FontRole.Body);
                c.Appearance = Appearance.Normal;
                c.BackColor = Color.Transparent;
            }

            secSource = NewSection();
            secSource.Controls.Add(chkWatch);
            secSource.Controls.Add(chkIncludeSubfolders);
            secSource.Controls.Add(hdrSource);

            // ---- filters ---------------------------------------------------------------
            hdrFilters = NewHeader("Filters");
            hdrFilters.ActionText = "Presets ▾";
            hdrFilters.ActionClicked += (s, e) =>
                cmsPresets.Show(hdrFilters, new Point(hdrFilters.ActionBounds.Left, hdrFilters.ActionBounds.Bottom));

            chipExtensions = new ChipList
            {
                Name = "chipExtensions",
                Dock = DockStyle.Top,
                Padding = new Padding(0, 2, 0, 8),
                AddMenu = cmsAddDropdown,
            };
            chipExtensions.ChipRemoved += ChipExtensions_ChipRemoved;

            BuildIgnoreRow();

            secFilters = NewSection();
            secFilters.Controls.Add(ignoreRow);
            secFilters.Controls.Add(chipExtensions);
            secFilters.Controls.Add(hdrFilters);

            // ---- files -----------------------------------------------------------------
            hdrFiles = NewHeader("Files");
            hdrFiles.ActionText = "Sort ▾";
            hdrFiles.ActionClicked += (s, e) =>
                ctxFiles.Show(hdrFiles, new Point(hdrFiles.ActionBounds.Left, hdrFiles.ActionBounds.Bottom));

            searchBox.Dock = DockStyle.Top;
            searchBox.Padding = new Padding(0, 2, 0, 8);

            lstFiles.Dock = DockStyle.Fill;
            lstFiles.BorderStyle = BorderStyle.FixedSingle;
            lstFiles.IntegralHeight = false;

            BuildRailFooter();

            secFiles = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 14, 15, 15) };
            secFiles.Controls.Add(lstFiles);
            secFiles.Controls.Add(railFooter);
            secFiles.Controls.Add(searchBox);
            secFiles.Controls.Add(hdrFiles);

            railHost = new Panel { Dock = DockStyle.Fill, Name = "railHost" };
            ThemeRoles.Set(railHost, ThemeRole.SurfaceSunken);
            railHost.Controls.Add(secFiles);
            railHost.Controls.Add(secFilters);
            railHost.Controls.Add(secSource);
        }

        private void BuildIgnoreRow()
        {
            ignoreRow = new Panel { Dock = DockStyle.Top, Padding = new Padding(1) };
            ignoreRow.Paint += (s, e) =>
            {
                var t = ThemeManager.Tokens;
                using var pen = new Pen(txtIgnorePatterns.Focused ? t.BorderFocus : t.Border,
                                        txtIgnorePatterns.Focused ? 1.6f : 1f);
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, ignoreRow.Width - 1, ignoreRow.Height - 1));
            };
            ThemeRoles.Set(ignoreRow, ThemeRole.Surface);

            // The caption becomes a prefix inside the field. It was a separate label on its own
            // line above a full-width box, which cost a row of height to say one word.
            lblIgnorePatterns.Dock = DockStyle.Left;
            lblIgnorePatterns.Text = "Ignore";
            lblIgnorePatterns.AutoSize = false;
            lblIgnorePatterns.TextAlign = ContentAlignment.MiddleLeft;
            lblIgnorePatterns.Padding = new Padding(9, 0, 6, 0);
            ThemeRoles.Set(lblIgnorePatterns, ThemeRole.TextDisabled, FontRole.Small);

            txtIgnorePatterns.Dock = DockStyle.Fill;
            txtIgnorePatterns.BorderStyle = BorderStyle.None;
            ThemeRoles.Set(txtIgnorePatterns, ThemeRole.Surface, FontRole.Body);
            txtIgnorePatterns.GotFocus += (s, e) => ignoreRow.Invalidate();
            txtIgnorePatterns.LostFocus += (s, e) => ignoreRow.Invalidate();

            btnEditRules.Dock = DockStyle.Right;
            btnEditRules.Text = "edit";
            btnEditRules.AutoSize = false;
            btnEditRules.FlatStyle = FlatStyle.Flat;
            btnEditRules.FlatAppearance.BorderSize = 0;
            ThemeRoles.Set(btnEditRules, ThemeRole.ButtonSubtle, FontRole.Small);

            ignoreRow.Controls.Add(txtIgnorePatterns);
            ignoreRow.Controls.Add(lblIgnorePatterns);
            ignoreRow.Controls.Add(btnEditRules);
        }

        private void BuildRailFooter()
        {
            railFooter = new Panel { Dock = DockStyle.Bottom, Padding = new Padding(0, 10, 0, 0) };

            btnAddMultipleFiles.Text = "+ Files";
            btnAddFolder.Text = "+ Folder";
            btnRemoveFile.Text = "Remove";
            btnMoveUp.Text = "▲";
            btnMoveDown.Text = "▼";

            foreach (var b in new[] { btnAddMultipleFiles, btnAddFolder, btnRemoveFile, btnMoveUp, btnMoveDown })
            {
                b.Dock = DockStyle.Left;
                b.AutoSize = false;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.Margin = new Padding(0);
            }

            // Add is neutral; Remove is the only coloured control in the rail, and it is ghosted
            // rather than filled so that a destructive action is legible without being loud.
            ThemeRoles.Set(btnAddMultipleFiles, ThemeRole.ButtonSubtle, FontRole.Body);
            ThemeRoles.Set(btnAddFolder, ThemeRole.ButtonSubtle, FontRole.Body);
            ThemeRoles.Set(btnMoveUp, ThemeRole.ButtonSubtle, FontRole.Body);
            ThemeRoles.Set(btnMoveDown, ThemeRole.ButtonSubtle, FontRole.Body);
            ThemeRoles.Set(btnRemoveFile, ThemeRole.ButtonSubtle, FontRole.Body);
            btnRemoveFile.ForeColor = ThemeManager.Tokens.Danger;

            btnRemoveFile.Dock = DockStyle.Right;
            btnMoveDown.Dock = DockStyle.Right;
            btnMoveUp.Dock = DockStyle.Right;

            railFooter.Controls.Add(btnAddFolder);
            railFooter.Controls.Add(btnAddMultipleFiles);
            railFooter.Controls.Add(btnMoveUp);
            railFooter.Controls.Add(btnMoveDown);
            railFooter.Controls.Add(btnRemoveFile);
        }

        // ------------------------------------------------------------------ pane

        private void BuildPane()
        {
            // ---- header ----------------------------------------------------------------
            lblOutput.Dock = DockStyle.Left;
            lblOutput.Text = "PACK";
            lblOutput.AutoSize = false;
            lblOutput.TextAlign = ContentAlignment.MiddleLeft;
            ThemeRoles.Set(lblOutput, ThemeRole.TextDisabled, FontRole.SmallBold);

            btnGenerate.Text = "Generate";
            foreach (var b in new Control[] { btnFindReplace, btnExportOutput, btnCopyOutput, btnGenerate })
            {
                b.Dock = DockStyle.Right;
                b.AutoSize = false;
                b.Margin = new Padding(0);
                if (b is Button btn) { btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderSize = 0; }
            }

            // Exactly one filled button in the window. Find, Edit and Export are ghosts because
            // they act on something that may not exist yet; Copy is outlined because it is the
            // common next step once it does, and a second accent fill would make neither read as
            // the primary one.
            ThemeRoles.Set(btnFindReplace, ThemeRole.ButtonSubtle, FontRole.Body);
            ThemeRoles.Set(btnExportOutput, ThemeRole.ButtonSubtle, FontRole.Body);
            ThemeRoles.Set(btnCopyOutput, ThemeRole.ButtonSubtle, FontRole.Body);
            ThemeRoles.Set(btnGenerate, ThemeRole.ButtonAccent, FontRole.BodyBold);
            btnFindReplace.Text = "Find";

            pnlOutputHeader.Dock = DockStyle.Top;
            pnlOutputHeader.Padding = new Padding(14, 8, 14, 8);
            ThemeRoles.Set(pnlOutputHeader, ThemeRole.Surface);
            pnlOutputHeader.Controls.Add(lblOutput);
            pnlOutputHeader.Controls.Add(btnFindReplace);
            pnlOutputHeader.Controls.Add(btnExportOutput);
            pnlOutputHeader.Controls.Add(btnCopyOutput);
            pnlOutputHeader.Controls.Add(btnGenerate);

            BuildProtectStrip();

            // ---- output + empty state --------------------------------------------------
            rtbOutput.Dock = DockStyle.Fill;
            rtbOutput.BorderStyle = BorderStyle.None;

            emptyOutput = new EmptyStateView
            {
                Name = "emptyOutput",
                Title = "No pack yet",
                Body = "A pack is your selected files flattened into one block of text, ready to paste "
                       + "into any AI chat. Filter the files on the left, then generate.",
                ActionText = "Generate pack",
            };
            emptyOutput.ActionClicked += (s, e) => BtnGenerate_Click(this, EventArgs.Empty);

            outputHost = new Panel { Dock = DockStyle.Fill, Name = "outputHost", Padding = new Padding(14, 10, 14, 10) };
            ThemeRoles.Set(outputHost, ThemeRole.SurfaceSunken);
            outputHost.Controls.Add(emptyOutput);
            outputHost.Controls.Add(rtbOutput);
            emptyOutput.BringToFront();

            // Synchronous, not on the statistics debounce: a third of a second between generating
            // a pack and seeing it reads as the button having done nothing.
            rtbOutput.TextChanged += (s, e) => UpdateOutputPresence();

            // ---- budget strip ----------------------------------------------------------
            pnlBudget.Dock = DockStyle.Bottom;
            pnlBudget.Padding = new Padding(14, 7, 14, 7);
            ThemeRoles.Set(pnlBudget, ThemeRole.Surface);
            pnlBudget.Controls.Clear();

            lblBudgetModel.Dock = DockStyle.Left;
            lblBudgetModel.Text = "Fits in";
            lblBudgetModel.AutoSize = false;
            lblBudgetModel.TextAlign = ContentAlignment.MiddleLeft;
            ThemeRoles.Set(lblBudgetModel, ThemeRole.TextSecondary, FontRole.Small);

            cmbBudgetModel.Dock = DockStyle.Left;
            barBudget.Dock = DockStyle.Fill;
            lblBudgetText.Dock = DockStyle.Right;
            lblBudgetText.AutoSize = false;
            lblBudgetText.TextAlign = ContentAlignment.MiddleRight;
            ThemeRoles.Set(lblBudgetText, ThemeRole.TextSecondary, FontRole.Small);

            btnBudgetBreakdown.Dock = DockStyle.Right;
            btnBudgetBreakdown.AutoSize = false;
            btnBudgetBreakdown.FlatStyle = FlatStyle.Flat;
            btnBudgetBreakdown.FlatAppearance.BorderSize = 0;
            btnBudgetBreakdown.Text = "Breakdown";
            ThemeRoles.Set(btnBudgetBreakdown, ThemeRole.ButtonSubtle, FontRole.Small);

            pnlBudget.Controls.Add(barBudget);
            pnlBudget.Controls.Add(cmbBudgetModel);
            pnlBudget.Controls.Add(lblBudgetModel);
            pnlBudget.Controls.Add(lblBudgetText);
            pnlBudget.Controls.Add(btnBudgetBreakdown);

            // ---- stat line -------------------------------------------------------------
            lblOutputStats.Dock = DockStyle.Bottom;
            lblOutputStats.AutoSize = false;
            lblOutputStats.TextAlign = ContentAlignment.MiddleLeft;
            lblOutputStats.Padding = new Padding(14, 0, 14, 0);
            ThemeRoles.Set(lblOutputStats, ThemeRole.TextDisabled, FontRole.Small);

            // ---- round-trip strip ------------------------------------------------------
            pnlRecreateInfo.Dock = DockStyle.Bottom;
            pnlRecreateInfo.Padding = new Padding(14, 9, 14, 9);
            // Shown from construction, not from the first statistics refresh. The designer starts
            // it hidden, and on a fresh window nothing had updated the output statistics yet — so
            // the strip stayed invisible until the user generated something, which is precisely
            // the state in which they most need to be told the round trip exists.
            pnlRecreateInfo.Visible = true;
            ThemeRoles.Set(pnlRecreateInfo, ThemeRole.SurfaceAlt);

            lblRecreateInfo.Text =
                "Round-trip — paste the AI's reply back and review it as a diff before anything is written.";
            lblRecreateInfo.AutoSize = false;
            lblRecreateInfo.TextAlign = ContentAlignment.MiddleLeft;
            ThemeRoles.Set(lblRecreateInfo, ThemeRole.TextSecondary, FontRole.Small);

            // The table was sized for the old banner and clipped its buttons at the strip's height.
            tblRecreateInfo.Dock = DockStyle.Fill;
            tblRecreateInfo.AutoSize = false;
            tblRecreateInfo.RowStyles.Clear();
            tblRecreateInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            foreach (var b in new[] { btnPasteResponse, btnApplyAiChanges })
            {
                b.Dock = DockStyle.Fill;
                b.AutoSize = false;
                b.Margin = new Padding(6, 0, 0, 0);
                b.FlatStyle = FlatStyle.Flat;
            }

            // The strip is permanent now, so its dismiss button has nothing to dismiss. Round-trip
            // is half the product; it was living behind a "hide this" affordance and a Tools menu.
            btnHideRecreateInfo.Visible = false;
            ThemeRoles.Set(btnPasteResponse, ThemeRole.ButtonSecondary, FontRole.Body);
            ThemeRoles.Set(btnApplyAiChanges, ThemeRole.ButtonSecondary, FontRole.Body);

            paneHost = new Panel { Dock = DockStyle.Fill, Name = "paneHost" };
            ThemeRoles.Set(paneHost, ThemeRole.Surface);
            paneHost.Controls.Add(outputHost);
            paneHost.Controls.Add(pnlBudget);
            paneHost.Controls.Add(lblOutputStats);
            paneHost.Controls.Add(pnlRecreateInfo);
            // Added before the header, so the header docks outermost and this strip lands directly
            // above the output box — between the pack's identity and the pack itself.
            paneHost.Controls.Add(pnlProtectTools);
            paneHost.Controls.Add(pnlOutputHeader);
        }

        /// <summary>
        /// The row of text actions directly above the output box: Edit, and the four compression
        /// and encryption commands.
        /// </summary>
        /// <remarks>
        /// These act on the text in the pane, which is why they sit against the pane rather than in
        /// the pack header with Export, Copy and Generate — those act on the pack as an artifact.
        /// Docked left in reading order rather than right, because unlike the header buttons they
        /// are a sequence you read, not a set you reach for.
        /// </remarks>
        private void BuildProtectStrip()
        {
            lblProtect = new Label { Name = "lblProtect" };
            lblProtect.Text = "PROTECT";
            lblProtect.Dock = DockStyle.Left;
            lblProtect.AutoSize = false;
            lblProtect.TextAlign = ContentAlignment.MiddleLeft;
            ThemeRoles.Set(lblProtect, ThemeRole.TextDisabled, FontRole.SmallBold);

            foreach (var b in new[] { btnDecompressEnc, btnCompressEnc, btnDecompress, btnCompress, btnEditOutput })
            {
                b.Dock = DockStyle.Left;
                b.AutoSize = false;
                b.Margin = new Padding(0);
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                ThemeRoles.Set(b, ThemeRole.ButtonSubtle, FontRole.Body);
            }

            btnEditOutput.Text = "Edit";

            pnlProtectTools.Dock = DockStyle.Top;
            pnlProtectTools.Padding = new Padding(14, 6, 14, 8);
            ThemeRoles.Set(pnlProtectTools, ThemeRole.Surface);

            // Left-docked children lay out from the end of the collection backwards, so this list is
            // reversed on screen: Edit, Compress, Decompress, Encrypt, Decrypt, after the label.
            pnlProtectTools.Controls.Add(btnDecompressEnc);
            pnlProtectTools.Controls.Add(btnCompressEnc);
            pnlProtectTools.Controls.Add(btnDecompress);
            pnlProtectTools.Controls.Add(btnCompress);
            pnlProtectTools.Controls.Add(btnEditOutput);
            pnlProtectTools.Controls.Add(lblProtect);
        }

        // ------------------------------------------------------------------ helpers

        private static Panel NewSection() => new()
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(15, 14, 15, 16),
        };

        private static SectionHeader NewHeader(string title)
        {
            var header = new SectionHeader { Title = title, Padding = new Padding(0, 0, 0, 6) };
            ThemeRoles.Set(header, FontRole.SmallBold);
            return header;
        }

        /// <summary>
        /// Gives the second-tier actions a hairline border.
        /// </summary>
        /// <remarks>
        /// The palette has one filled accent button and one destructive fill, which leaves every
        /// other action as a borderless ghost — and a row of borderless ghosts reads as text, not
        /// as controls. An outline restores the affordance without adding a second colour.
        /// Applied after the theme, which sets <c>BorderSize</c> to zero for every role.
        /// </remarks>
        private void ApplyOutlineButtons()
        {
            var t = ThemeManager.Tokens;
            foreach (var b in new[]
                     { btnCopyOutput, btnEditOutput, btnCompress, btnDecompress, btnCompressEnc, btnDecompressEnc,
                       btnAddMultipleFiles, btnAddFolder, btnPasteResponse, btnApplyAiChanges })
            {
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.BorderColor = t.Border;
                b.BackColor = t.Surface;
                b.ForeColor = t.TextPrimary;
                b.FlatAppearance.MouseOverBackColor = t.SurfaceAlt;
            }

            // Edit stays lit while the pane is unlocked. Two channels, not one: the accent wash
            // and a matching border, so the state does not rest on colour alone.
            if (_editingOutput)
            {
                btnEditOutput.BackColor = t.Selection;
                btnEditOutput.ForeColor = t.AccentOnSurface;
                btnEditOutput.FlatAppearance.BorderColor = t.AccentOnSurface;
                btnEditOutput.FlatAppearance.MouseOverBackColor = t.Selection;
            }
        }

        /// <summary>
        /// Sizes everything that is measured from the font. Re-run on every theme change, because
        /// the theme is what sets the fonts.
        /// </summary>
        private void ApplyLayoutMetrics()
        {
            int unit = Font.Height;
            int control = unit + 14;
            int square = control;

            pnlFolderChip.Height = control;
            lblFolderGlyph.Width = square;
            btnRecentFolders.Width = square;
            btnBrowse.Width = unit * 5;
            btnTree.Width = unit * 4;
            btnRefreshExtensions.Width = square;
            btnOptions.Width = square;
            cmbEncoding.Width = unit * 6;
            pnlCommandBar.Height = control + pnlCommandBar.Padding.Vertical;

            ignoreRow.Height = control;
            lblIgnorePatterns.Width = unit * 4;
            btnEditRules.Width = unit * 3;

            int footerButton = unit + 12;
            railFooter.Height = footerButton + railFooter.Padding.Vertical;
            btnAddMultipleFiles.Width = unit * 5;
            btnAddFolder.Width = unit * 6;
            btnRemoveFile.Width = unit * 5;
            btnMoveUp.Width = btnMoveDown.Width = unit * 2;
            foreach (var b in new[] { btnAddMultipleFiles, btnAddFolder, btnRemoveFile, btnMoveUp, btnMoveDown })
                b.Height = footerButton;

            int headerButton = unit + 14;
            pnlOutputHeader.Height = headerButton + pnlOutputHeader.Padding.Vertical;
            lblOutput.Width = unit * 4;
            btnFindReplace.Width = unit * 4;
            btnExportOutput.Width = unit * 5;
            btnCopyOutput.Width = unit * 6;
            btnGenerate.Width = unit * 6;
            foreach (var b in new Control[] { btnFindReplace, btnExportOutput, btnCopyOutput, btnGenerate })
                b.Height = headerButton;

            // Measured, not guessed at a multiple of the font height: "Decom&press" and
            // "🔓 Decrypt..." are different lengths, and a fixed multiple clips the longer one at
            // any font size the user picks.
            pnlProtectTools.Height = headerButton + pnlProtectTools.Padding.Vertical;
            lblProtect.Width = unit * 5;
            foreach (var b in new[] { btnEditOutput, btnCompress, btnDecompress, btnCompressEnc, btnDecompressEnc })
            {
                int text = TextRenderer.MeasureText(b.Text, b.Font).Width;
                b.Width = Math.Max(unit * 4, text + unit * 2);
                b.Height = headerButton;
            }

            pnlBudget.Height = unit + 10 + pnlBudget.Padding.Vertical;
            lblBudgetModel.Width = unit * 4;
            cmbBudgetModel.Width = unit * 9;
            // Carries "~48,200 of 200,000 tokens (estimate)" in full; truncating the one number
            // the strip exists to show would defeat it.
            lblBudgetText.Width = unit * 16;
            btnBudgetBreakdown.Width = unit * 6;
            barBudget.Height = 6;
            // A Fill child cannot be vertically centred by docking, so the strip's padding does it.
            int barInset = Math.Max(0, (pnlBudget.Height - pnlBudget.Padding.Vertical - barBudget.Height) / 2);
            barBudget.Margin = new Padding(10, barInset, 10, barInset);

            lblOutputStats.Height = unit + 8;
            pnlRecreateInfo.Height = unit + 14 + pnlRecreateInfo.Padding.Vertical;
        }

        /// <summary>
        /// Repaints from the active palette and re-measures, because the theme is what assigns the
        /// fonts every size in <see cref="ApplyLayoutMetrics"/> is derived from.
        /// </summary>
        protected override void ApplyTheme()
        {
            if (IsDisposed || Disposing) return;

            base.ApplyTheme();

            searchBox.RefreshTheme();
            btnRemoveFile.ForeColor = ThemeManager.Tokens.Danger;
            ApplyOutlineButtons();
            ApplyLayoutMetrics();

            // The owner-drawn controls read their colours at paint time, so they only need telling
            // that the colours changed.
            chipExtensions.Invalidate();
            foreach (var h in new[] { hdrSource, hdrFilters, hdrFiles }) h.Invalidate();
            pnlFolderChip.Invalidate();
            ignoreRow.Invalidate();
        }

        // ------------------------------------------------------------------ behaviour

        /// <summary>
        /// Adds the two menu entries the restructure needs: a way to type a custom extension now
        /// that the combo box is gone, and a way to save a preset now that its button has folded
        /// into the Filters header's drop-down.
        /// </summary>
        private void WireLayoutMenus()
        {
            mnuAddCustom = new ToolStripMenuItem("Custom extension…");
            mnuAddCustom.Click += (s, e) =>
            {
                var value = ThemedPrompt.Show(this, "Add extension",
                    "File extension (with or without the leading dot):");
                if (string.IsNullOrWhiteSpace(value)) return;

                // Routed through the existing handler rather than reimplemented, so the leading-dot
                // normalisation and the duplicate check keep living in exactly one place.
                cmbExtension.Text = value;
                BtnAdd_Click(this, EventArgs.Empty);
            };
            cmsAddDropdown.Items.Insert(0, mnuAddCustom);
            cmsAddDropdown.Items.Insert(1, new ToolStripSeparator());

            // "Presets" is where someone looks for "set this up for a WPF project", so the project
            // catalogue leads the menu and the user's own saved presets follow it.
            mnuPsProjectType = new ToolStripMenuItem("Project type");
            foreach (var item in BuildProjectPresetItems())
                mnuPsProjectType.DropDownItems.Add(item);

            mnuPsSaveCurrent = new ToolStripMenuItem("Save current as preset…");
            mnuPsSaveCurrent.Click += BtnSavePreset_Click;

            cmsPresets.Items.Insert(0, mnuPsProjectType);
            cmsPresets.Items.Insert(1, new ToolStripSeparator());
            cmsPresets.Items.Insert(2, mnuPsSaveCurrent);
            cmsPresets.Items.Insert(3, new ToolStripSeparator());
        }

        private void ChipExtensions_ChipRemoved(object? sender, ChipEventArgs e)
        {
            fileService.RemoveExtension(e.Value);
            SyncUIWithService();
            _ = RefreshFilesInBackground();
        }

        /// <summary>
        /// Pushes model state into the parts of the rail and pane that the designer-era code does
        /// not know about: the chips, the section counts, and the output pane's empty state.
        /// </summary>
        private void RefreshLayoutState()
        {
            // A background scan completing during teardown lands here, and rebuilding the chips
            // re-runs layout over controls that are already being disposed.
            if (IsDisposed || Disposing) return;

            chipExtensions.SetItems(fileService.Extensions);

            hdrFilters.Count = fileService.Extensions.Count > 0
                ? fileService.Extensions.Count.ToString(System.Globalization.CultureInfo.CurrentCulture)
                : "";
            hdrFiles.Count = fileService.SelectedFiles.Count > 0
                ? fileService.SelectedFiles.Count.ToString("N0", System.Globalization.CultureInfo.CurrentCulture)
                : "";

            UpdateOutputPresence();
        }

        /// <summary>
        /// Whether the output pane can be read without side effects.
        /// </summary>
        /// <remarks>
        /// Reading <c>Text</c> or <c>TextLength</c> from a <see cref="RichTextBox"/> whose handle
        /// does not exist <em>creates</em> the handle, because the control keeps its document in
        /// the native window. That is ordinarily invisible, but tearing the window down destroys
        /// every handle and then raises <c>TextChanged</c> as the document is unloaded — so a
        /// handler that reads the length re-creates the handle of a control that is at that moment
        /// being disposed, and <c>Control.Dispose</c> throws "Dispose() cannot be called while
        /// doing CreateHandle()". The result was a .NET crash dialog on closing the window any
        /// time a pack had been generated.
        ///
        /// Checking the form's own state is not enough: the form is disposed last, so it is still
        /// alive and handle-backed while its children are going away.
        /// </remarks>
        private bool OutputReadable =>
            !_tearingDown && !IsDisposed && !Disposing
            && rtbOutput is { IsDisposed: false, Disposing: false, IsHandleCreated: true };

        /// <summary>
        /// Set once the window is definitely closing, before any control is disposed.
        /// </summary>
        /// <remarks>
        /// Guarding each reader of the output pane individually turned into whack-a-mole: the
        /// pane is read by the statistics, the token gauge, the round-trip strip and the empty
        /// state, and teardown reaches them through several events that fire as controls come
        /// apart — a combo box collapsing its selection, a text box raising TextChanged as its
        /// document unloads. One flag, set at the last moment the window is still whole, closes
        /// all of those paths at once and cannot be missed by a future reader that routes through
        /// <see cref="OutputReadable"/>.
        /// </remarks>
        private bool _tearingDown;

        /// <summary>
        /// True while the output pane is unlocked for typing, from Edit.
        /// </summary>
        /// <remarks>
        /// Tracked explicitly rather than read from <c>rtbOutput.ReadOnly</c>, because the
        /// compression handlers unlock the pane for the length of one assignment and would
        /// otherwise register as an editing session.
        /// </remarks>
        private bool _editingOutput;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.Cancel) return;

            _tearingDown = true;

            // Emptied while the window is still whole, and this is not tidiness.
            //
            // Closing the form destroys its handle, which cascades to every child. A RichTextBox
            // keeps its document inside the native window, so when its handle goes and the control
            // is not itself being disposed yet, it immediately re-creates the handle to preserve
            // the text. The dispose walk then reaches a control in the middle of CreateHandle and
            // Control.Dispose throws "Dispose() cannot be called while doing CreateHandle()" —
            // a .NET crash dialog on exit, every time, for anyone who had generated a pack.
            //
            // With no document there is nothing to preserve and no handle is re-created. Guarding
            // the readers is not sufficient on its own: this re-creation is inside the framework,
            // not in any call of ours.
            if (rtbOutput is { IsDisposed: false, IsHandleCreated: true })
                rtbOutput.Clear();
        }

        /// <summary>
        /// Shows the pack or the empty state, and enables the actions that need a pack.
        /// </summary>
        /// <remarks>
        /// Driven by the output pane's own <c>TextChanged</c>, not by the model refresh. It began
        /// life inside <see cref="RefreshLayoutState"/>, which reacts to the file and extension
        /// lists and which Generate does not call — so a generated pack landed in a pane that was
        /// still hidden behind "No pack yet". The pack existed, was counted in the statistics and
        /// the token gauge, and could be copied; the window simply never showed it. Anything that
        /// writes to the pane is covered by hanging this off the pane itself: Generate, the four
        /// Tools ▸ Compression actions, Find and Replace, manual editing and Paste AI response.
        /// </remarks>
        private void UpdateOutputPresence()
        {
            if (!OutputReadable) return;

            bool hasOutput = rtbOutput.TextLength > 0;

            // Except while editing: the empty state is painted *over* the output box, so leaving it
            // up during an edit would hide the very box the user just unlocked to type into.
            // Only the empty state is toggled. The output box stays visible underneath it and is
            // simply covered, because hiding a RichTextBox stops Windows creating its handle: the
            // control then buffers whatever is assigned to it and creates the handle later, at a
            // moment of its own choosing. With a megabyte of generated pack in that buffer, the
            // handle creation landed inside the dispose walk and threw "Dispose() cannot be called
            // while doing CreateHandle()" — a crash dialog every time the window was closed after
            // generating anything.
            emptyOutput.Visible = !hasOutput && !_editingOutput;

            // Nothing to find, export or copy until there is a pack.
            btnFindReplace.Enabled = hasOutput;
            btnExportOutput.Enabled = hasOutput;
            btnCopyOutput.Enabled = hasOutput;

            // Edit is the exception, and is never disabled. An empty pane is a legitimate thing to
            // want to type into: pasting a bundle a colleague sent, or writing one by hand to
            // decrypt or decompress it. Gating Edit on there already being a pack made the one
            // control that can *create* pane content depend on pane content already existing.
            btnEditOutput.Enabled = true;

            // The four protect buttons are enabled against what the pane actually holds, sniffed
            // from the blob's magic prefix, so the strip cannot offer to decrypt plain text or to
            // encrypt something that is already sealed. Each disabled button keeps a tooltip saying
            // why, because a greyed control with no explanation is its own bug report.
            var pane = rtbOutput.Text ?? string.Empty;
            bool encrypted = CompressionUtils.LooksLikeEncryptedBase64(pane);
            bool compressed = CompressionUtils.LooksLikeCompressedBase64(pane);
            bool plain = hasOutput && !encrypted && !compressed;

            btnCompress.Enabled = plain;
            btnCompressEnc.Enabled = plain;
            btnDecompress.Enabled = compressed;
            btnDecompressEnc.Enabled = encrypted;
        }
    }
}

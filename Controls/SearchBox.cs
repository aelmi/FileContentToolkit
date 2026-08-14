using System;
using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Controls
{
    /// <summary>
    /// The file-content search row: query, recent terms, and the three match toggles, in one field.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These six controls were six independent designer fields at absolute coordinates, three of
    /// which announced themselves to a screen reader as their glyph — "Aa", "dot asterisk
    /// checkbox", "black down-pointing triangle". Grouping them means the accessible names, the
    /// access keys and the tab order are declared once, next to each other, where a gap is
    /// visible.
    /// </para>
    /// <para>
    /// The toggles now sit inside the field rather than on a row beneath it. Loose below the box
    /// they read as three unrelated options that happened to be nearby; inside it they read as
    /// modifiers of the query, which is what they are. The caption above the field went with them
    /// — the section header names this area, and the access key it used to carry is replaced by a
    /// real shortcut (<c>Shortcuts.SearchInFiles</c>) that works from anywhere in the window
    /// rather than only when the caption is reachable.
    /// </para>
    /// </remarks>
    public sealed class SearchBox : Panel
    {
        private readonly Panel _frame = new();
        private readonly Label _glyph = new();
        private readonly TextBox _query = new();
        private readonly FlowLayoutPanel _tools = new();
        private readonly CheckBox _matchCase = new();
        private readonly CheckBox _wholeWord = new();
        private readonly CheckBox _useRegex = new();
        private readonly Button _recents = new();
        private readonly Label _matches = new();

        /// <summary>Raised by Enter in the query box.</summary>
        public event EventHandler? SearchRequested;

        /// <summary>Raised by the recent-terms drop-down button.</summary>
        public event EventHandler? RecentsRequested;

        public SearchBox()
        {
            Dock = DockStyle.Top;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TabStop = false;

            // --- the field ---------------------------------------------------------------
            _frame.Dock = DockStyle.Top;
            _frame.Padding = new Padding(1);
            _frame.Paint += FrameOnPaint;
            ThemeRoles.Set(_frame, ThemeRole.Surface);

            _glyph.Dock = DockStyle.Left;
            _glyph.AutoSize = false;
            _glyph.Text = "⌕";
            _glyph.TextAlign = ContentAlignment.MiddleCenter;
            _glyph.TabStop = false;
            // Decorative: the field is already named by the query box inside it.
            _glyph.AccessibleRole = AccessibleRole.Graphic;
            ThemeRoles.Set(_glyph, ThemeRole.TextDisabled, FontRole.Medium);

            _query.Dock = DockStyle.Fill;
            _query.BorderStyle = BorderStyle.None;
            // Without an explicit role a text box takes the sunken fill by type, which inside the
            // field's own well reads as a grey block rather than as an input.
            ThemeRoles.Set(_query, ThemeRole.Surface, FontRole.Body);
            _query.TabIndex = 0;
            _query.PlaceholderText = "Search in files…";
            _query.AccessibleName = "Search in files";
            _query.AccessibleDescription = "Text to look for inside the selected files.";
            _query.GotFocus += (s, e) => _frame.Invalidate();
            _query.LostFocus += (s, e) => _frame.Invalidate();
            _query.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                // Without SuppressKeyPress the form's AcceptButton also fires and the text box
                // beeps at the unhandled Enter.
                e.Handled = true;
                e.SuppressKeyPress = true;
                SearchRequested?.Invoke(this, EventArgs.Empty);
            };

            _tools.Dock = DockStyle.Right;
            _tools.AutoSize = true;
            _tools.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tools.WrapContents = false;
            _tools.Margin = new Padding(0);

            ConfigureToggle(_matchCase, "Aa", "Match case", "Only match text with the same capitalisation.", 1);
            ConfigureToggle(_wholeWord, "ab|", "Whole word", "Only match complete words.", 2);
            ConfigureToggle(_useRegex, ".*", "Regular expression", "Treat the search term as a .NET regular expression.", 3);

            _recents.AutoSize = false;
            _recents.FlatStyle = FlatStyle.Flat;
            _recents.FlatAppearance.BorderSize = 0;
            _recents.Cursor = Cursors.Hand;
            _recents.Margin = new Padding(2, 0, 0, 0);
            _recents.Text = "▾";
            _recents.TabIndex = 4;
            // Announced as "black down-pointing small triangle" without this.
            _recents.AccessibleName = "Recent searches";
            _recents.AccessibleDescription = "Choose from search terms you have used before.";
            _recents.AccessibleRole = AccessibleRole.ButtonDropDown;
            _recents.Click += (s, e) => RecentsRequested?.Invoke(this, EventArgs.Empty);
            ThemeRoles.Set(_recents, ThemeRole.ButtonSubtle);

            _tools.Controls.Add(_matchCase);
            _tools.Controls.Add(_wholeWord);
            _tools.Controls.Add(_useRegex);
            _tools.Controls.Add(_recents);

            // A Fill child must be added before its docked siblings: docked children are laid out
            // from the end of the collection backwards, so the lowest index is positioned last and
            // receives whatever space the edges did not claim.
            _frame.Controls.Add(_query);
            _frame.Controls.Add(_glyph);
            _frame.Controls.Add(_tools);

            // --- match count -------------------------------------------------------------
            _matches.AutoSize = true;
            _matches.Dock = DockStyle.Top;
            _matches.Text = "";
            _matches.Padding = new Padding(2, 5, 0, 0);
            _matches.TabIndex = 5;
            _matches.AccessibleName = "Search results";
            ThemeRoles.Set(_matches, ThemeRole.TextSecondary, FontRole.Small);

            Controls.Add(_matches);
            Controls.Add(_frame);

            ApplyMetrics();
        }

        private static void ConfigureToggle(CheckBox box, string text, string name, string description, int tabIndex)
        {
            box.Appearance = Appearance.Button;
            box.AutoSize = false;
            box.FlatStyle = FlatStyle.Flat;
            box.FlatAppearance.BorderSize = 0;
            box.TextAlign = ContentAlignment.MiddleCenter;
            box.Cursor = Cursors.Hand;
            box.Text = text;
            box.TabIndex = tabIndex;
            box.Margin = new Padding(1, 0, 1, 0);
            box.UseVisualStyleBackColor = false;
            box.AccessibleName = name;
            box.AccessibleDescription = description;
            box.CheckedChanged += (s, e) => PaintToggle(box);
        }

        /// <summary>
        /// A checked toggle takes the accent wash rather than a pressed bevel: at this size a bevel
        /// is invisible, and the wash matches the extension chips, so "on" looks the same
        /// everywhere in the rail.
        /// </summary>
        private static void PaintToggle(CheckBox box)
        {
            var t = ThemeManager.Tokens;
            box.BackColor = box.Checked ? t.Selection : Color.Transparent;
            box.ForeColor = box.Checked ? t.AccentOnSurface : t.TextDisabled;
            box.FlatAppearance.MouseOverBackColor = box.Checked ? t.Selection : t.SurfaceAlt;
            box.FlatAppearance.CheckedBackColor = t.Selection;
        }

        private void FrameOnPaint(object? sender, PaintEventArgs e)
        {
            var t = ThemeManager.Tokens;
            bool focused = _query.Focused;
            using var pen = new Pen(focused ? t.BorderFocus : t.Border, focused ? 1.6f : 1f);
            e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, _frame.Width - 1, _frame.Height - 1));
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            ApplyMetrics();
        }

        /// <summary>
        /// Derives every size from the current font, so the field scales with the theme's type
        /// scale and with per-monitor DPI without carrying fixed pixel geometry.
        /// </summary>
        private void ApplyMetrics()
        {
            int unit = Font.Height;
            int cell = unit + 6;

            _frame.Height = unit + 14;
            _glyph.Width = cell;

            foreach (var box in new[] { _matchCase, _wholeWord, _useRegex })
            {
                box.Size = new Size(cell + 4, cell);
                PaintToggle(box);
            }
            _recents.Size = new Size(cell, cell);

            // Vertically centre the toggle strip inside the field.
            int inset = Math.Max(0, (_frame.Height - 2 - cell) / 2);
            _tools.Padding = new Padding(0, inset, 4, inset);
        }

        /// <summary>Repaints the toggles from the active palette after a theme change.</summary>
        public void RefreshTheme()
        {
            PaintToggle(_matchCase);
            PaintToggle(_wholeWord);
            PaintToggle(_useRegex);
            _frame.Invalidate();
        }

        /// <summary>Applies tooltips through the form's shared provider.</summary>
        public void SetToolTips(ToolTip tips)
        {
            if (tips == null) return;
            tips.SetToolTip(_query, "Text to find inside the selected files (Enter to search)");
            tips.SetToolTip(_recents, "Recent searches");
            tips.SetToolTip(_matchCase, "Match case");
            tips.SetToolTip(_wholeWord, "Whole word");
            tips.SetToolTip(_useRegex, "Regular expression");
        }

        public string Query
        {
            get => _query.Text;
            set => _query.Text = value ?? "";
        }

        public bool MatchCase
        {
            get => _matchCase.Checked;
            set => _matchCase.Checked = value;
        }

        public bool WholeWord
        {
            get => _wholeWord.Checked;
            set => _wholeWord.Checked = value;
        }

        public bool UseRegex
        {
            get => _useRegex.Checked;
            set => _useRegex.Checked = value;
        }

        /// <summary>Result summary shown under the field, e.g. "12 matches in 3 files".</summary>
        public string MatchesText
        {
            get => _matches.Text;
            set => _matches.Text = value ?? "";
        }

        /// <summary>
        /// Disabled while a search is running. There is no longer a Search button — Enter runs the
        /// search — so this gates the query box itself.
        /// </summary>
        public bool SearchEnabled
        {
            get => _query.Enabled;
            set => _query.Enabled = value;
        }

        /// <summary>The control a recent-searches menu should drop from.</summary>
        public Control RecentsAnchor => _recents;

        public void FocusQuery()
        {
            _query.Focus();
            _query.SelectAll();
        }
    }
}

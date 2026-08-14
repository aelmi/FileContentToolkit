using System;
using System.Drawing;
using System.Windows.Forms;
using CodeShuttle.Theming;

namespace CodeShuttle.Controls
{
    /// <summary>
    /// The panel shown over a list or pane that has nothing in it yet: a heading, a sentence of
    /// explanation, and the single action that gets the user out of the empty state.
    /// </summary>
    /// <remarks>
    /// Every list in the product started life as a blank white box. Nothing said that an extension
    /// has to be added before "Add Folder" does anything — that rule was enforced only by an error
    /// dialog raised after the user had already tried. Stating the requirement before the failure
    /// is the entire point of this control.
    ///
    /// Laid out with docked, auto-sizing rows rather than coordinates so it needs no DPI-specific
    /// geometry and no <c>AutoScaleDimensions</c> of its own.
    /// </remarks>
    public sealed class EmptyStateView : Panel
    {
        private readonly Label _title = new();
        private readonly Label _body = new();
        private readonly Button _action = new();
        private readonly Panel _actionRow = new();
        private readonly Panel _stack = new();

        /// <summary>Raised when the call-to-action button is pressed.</summary>
        public event EventHandler? ActionClicked;

        public EmptyStateView()
        {
            Dock = DockStyle.Fill;
            // Not in the tab order: it is an explanation, and it disappears the moment the pane
            // it covers has content.
            TabStop = false;
            AccessibleRole = AccessibleRole.Pane;

            // Stays docked. Undocking it to centre the block collapses it to nothing: its children
            // are docked, so they take their width from the parent, and an AutoSize parent that is
            // not itself docked has no width to give them. The vertical offset in OnLayout does
            // the same job — a block with air above it rather than one jammed against the top edge.
            _stack.Dock = DockStyle.Top;
            _stack.AutoSize = true;
            _stack.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _stack.Padding = new Padding(24, 24, 24, 24);

            _title.Dock = DockStyle.Top;
            _title.AutoSize = true;
            _title.Margin = new Padding(0);
            ThemeRoles.Set(_title, FontRole.Heading);

            _body.Dock = DockStyle.Top;
            _body.AutoSize = true;
            _body.MaximumSize = new Size(420, 0);
            _body.Padding = new Padding(0, 8, 0, 0);
            ThemeRoles.Set(_body, ThemeRole.TextSecondary, FontRole.Body);

            _actionRow.Dock = DockStyle.Top;
            _actionRow.AutoSize = true;
            _actionRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _actionRow.Padding = new Padding(0, 16, 0, 0);

            _action.AutoSize = true;
            _action.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _action.FlatStyle = FlatStyle.Flat;
            _action.FlatAppearance.BorderSize = 0;
            _action.Cursor = Cursors.Hand;
            _action.Padding = new Padding(14, 6, 14, 6);
            _action.Click += (s, e) => ActionClicked?.Invoke(this, EventArgs.Empty);
            ThemeRoles.Set(_action, ThemeRole.ButtonAccent, FontRole.BodyBold);

            _actionRow.Controls.Add(_action);

            // Added last-to-first: a docked control claims its edge in reverse z-order, so this
            // sequence renders as title, body, action from the top down.
            _stack.Controls.Add(_actionRow);
            _stack.Controls.Add(_body);
            _stack.Controls.Add(_title);
            Controls.Add(_stack);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            Recentre();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Recentre();
        }

        /// <summary>
        /// Sits the block a little above the vertical middle, scaled to the pane. Pinned to the
        /// top of a tall empty rectangle it reads as content that failed to load rather than as a
        /// deliberate state.
        /// </summary>
        private void Recentre()
        {
            if (Width <= 0 || Height <= 0) return;
            int top = Math.Clamp((int)(Height * 0.18), 24, 160);
            if (_stack.Padding.Top == top) return;
            _stack.Padding = new Padding(28, top, 28, 24);
        }

        public string Title
        {
            get => _title.Text;
            set { _title.Text = value; AccessibleName = value; Recentre(); }
        }

        public string Body
        {
            get => _body.Text;
            set => _body.Text = value;
        }

        /// <summary>
        /// Text of the call-to-action. Setting it to null or empty hides the button, for empty
        /// states whose resolution is not a single click.
        /// </summary>
        public string ActionText
        {
            get => _action.Text;
            set
            {
                _action.Text = value ?? "";
                _action.AccessibleName = StripAccessKey(_action.Text);
                _actionRow.Visible = !string.IsNullOrEmpty(_action.Text);
            }
        }

        /// <summary>Strips the ampersand so a screen reader announces "Add Folder", not "Add &amp;Folder".</summary>
        private static string StripAccessKey(string text) => text.Replace("&", "");
    }
}

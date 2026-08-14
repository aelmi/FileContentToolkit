using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CodeShuttle.UI
{
    /// <summary>
    /// Decides whether a remembered window rectangle is still usable.
    /// </summary>
    /// <remarks>
    /// Restoring saved bounds blindly is how an application ends up invisible: a window saved on
    /// a second monitor that has since been unplugged, or on a display whose resolution shrank,
    /// restores off-screen with no way to reach it but the keyboard. The geometry is kept free of
    /// any <see cref="Screen"/> dependency so it can be unit-tested without a display attached.
    /// </remarks>
    public static class WindowPlacement
    {
        /// <summary>
        /// The fraction of the window that must land on some screen for the position to be kept.
        /// A window may legitimately hang off an edge; it may not be essentially gone.
        /// </summary>
        private const double MinimumVisibleFraction = 0.25;

        /// <summary>The title bar needs to be reachable, not just some corner of the window.</summary>
        private const int CaptionHeight = 32;

        /// <summary>
        /// True when <paramref name="bounds"/> is sufficiently visible on at least one of
        /// <paramref name="screens"/>, and its caption strip is reachable by the mouse.
        /// </summary>
        public static bool IsVisibleOn(Rectangle bounds, IEnumerable<Rectangle> screens)
        {
            if (screens == null) return false;
            if (bounds.Width <= 0 || bounds.Height <= 0) return false;

            long area = (long)bounds.Width * bounds.Height;
            long covered = 0;
            bool captionReachable = false;

            var caption = new Rectangle(bounds.X, bounds.Y, bounds.Width, System.Math.Min(CaptionHeight, bounds.Height));

            foreach (var screen in screens)
            {
                var hit = Rectangle.Intersect(bounds, screen);
                if (!hit.IsEmpty) covered += (long)hit.Width * hit.Height;

                if (!Rectangle.Intersect(caption, screen).IsEmpty) captionReachable = true;
            }

            // Overlapping screens would double-count, so the total is clamped rather than trusted.
            if (covered > area) covered = area;

            return captionReachable && covered >= (long)(area * MinimumVisibleFraction);
        }

        /// <summary>Overload against the monitors actually attached right now.</summary>
        public static bool IsVisibleOnAnyScreen(Rectangle bounds)
        {
            var screens = new List<Rectangle>();
            foreach (var s in Screen.AllScreens) screens.Add(s.WorkingArea);
            return IsVisibleOn(bounds, screens);
        }
    }
}

using System.Collections.Generic;
using System.Drawing;
using CodeShuttle.UI;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// The multi-monitor visibility check behind window-position restore. Deliberately expressed
    /// against injected screen rectangles rather than <c>Screen.AllScreens</c>, so it runs on a
    /// build agent with no display attached and can describe monitors that are not there.
    /// </summary>
    public class WindowPlacementTests
    {
        private static readonly Rectangle Primary = new(0, 0, 1920, 1040);
        private static readonly Rectangle Secondary = new(1920, 0, 1920, 1040);

        [Fact]
        public void Window_fully_on_the_primary_screen_is_visible()
        {
            var bounds = new Rectangle(200, 150, 1100, 700);
            Assert.True(WindowPlacement.IsVisibleOn(bounds, new[] { Primary }));
        }

        [Fact]
        public void Window_saved_on_a_second_monitor_is_visible_while_it_is_attached()
        {
            var bounds = new Rectangle(2100, 100, 1100, 700);
            Assert.True(WindowPlacement.IsVisibleOn(bounds, new[] { Primary, Secondary }));
        }

        /// <summary>The case the whole check exists for: that monitor has been unplugged.</summary>
        [Fact]
        public void Window_saved_on_a_detached_monitor_is_not_visible()
        {
            var bounds = new Rectangle(2100, 100, 1100, 700);
            Assert.False(WindowPlacement.IsVisibleOn(bounds, new[] { Primary }));
        }

        [Fact]
        public void Window_hanging_slightly_off_an_edge_is_still_visible()
        {
            // Three quarters on screen. Legitimate, and should not be thrown away.
            var bounds = new Rectangle(1620, 100, 400, 400);
            Assert.True(WindowPlacement.IsVisibleOn(bounds, new[] { Primary }));
        }

        [Fact]
        public void Window_almost_entirely_off_screen_is_not_visible()
        {
            var bounds = new Rectangle(1900, 100, 1000, 700);
            Assert.False(WindowPlacement.IsVisibleOn(bounds, new[] { Primary }));
        }

        /// <summary>
        /// A window whose body overlaps but whose caption is above the top of every screen cannot
        /// be dragged back, so it counts as not visible.
        /// </summary>
        [Fact]
        public void Window_with_its_caption_above_every_screen_is_not_visible()
        {
            var bounds = new Rectangle(300, -200, 900, 700);
            Assert.False(WindowPlacement.IsVisibleOn(bounds, new[] { Primary }));
        }

        [Fact]
        public void No_screens_means_not_visible()
        {
            var bounds = new Rectangle(0, 0, 900, 700);
            Assert.False(WindowPlacement.IsVisibleOn(bounds, new List<Rectangle>()));
        }

        [Theory]
        [InlineData(0, 100)]
        [InlineData(100, 0)]
        [InlineData(-10, -10)]
        public void Degenerate_sizes_are_not_visible(int width, int height)
        {
            var bounds = new Rectangle(10, 10, width, height);
            Assert.False(WindowPlacement.IsVisibleOn(bounds, new[] { Primary }));
        }

        /// <summary>
        /// Overlapping screens (a mirrored pair) must not let double-counted area push a mostly
        /// off-screen window over the visibility threshold.
        /// </summary>
        [Fact]
        public void Overlapping_screens_do_not_double_count_covered_area()
        {
            var mirrored = new[] { Primary, Primary };
            var bounds = new Rectangle(1900, 100, 1000, 700);
            Assert.False(WindowPlacement.IsVisibleOn(bounds, mirrored));
        }
    }
}

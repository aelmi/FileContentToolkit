using System.Drawing;

namespace CodeShuttle.Theming
{
    /// <summary>
    /// The complete colour surface of the application, expressed as semantic roles rather than
    /// literal colours. Every palette is one instance of this record; adding a third palette
    /// (high contrast, a custom accent) is purely additive and requires no code change anywhere
    /// else — which is the whole point of replacing the previous RGB-guessing mapper.
    /// </summary>
    /// <remarks>
    /// The nineteen members named in the plan are all present. A small number of additional
    /// members were required by controls that actually exist in this product and had no honest
    /// home among the nineteen (a pale-yellow information banner, hover states for the success
    /// and danger buttons, the unchanged-context colour in the diff view, and accent-coloured
    /// text drawn on a surface rather than on the accent itself).
    /// </remarks>
    public sealed record ThemeTokens
    {
        /// <summary>Window and default panel background.</summary>
        public required Color Surface { get; init; }

        /// <summary>Raised or secondary panels: toolbars, button bars, header strips of a section.</summary>
        public required Color SurfaceAlt { get; init; }

        /// <summary>Recessed content: text boxes, list boxes, grids, trees, the output pane.</summary>
        public required Color SurfaceSunken { get; init; }

        /// <summary>Body text.</summary>
        public required Color TextPrimary { get; init; }

        /// <summary>Supporting text. Guaranteed >= 4.5:1 against <see cref="Surface"/>.</summary>
        public required Color TextSecondary { get; init; }

        /// <summary>Disabled text and inert chrome. Guaranteed >= 3:1 against <see cref="Surface"/>.</summary>
        public required Color TextDisabled { get; init; }

        /// <summary>Primary accent fill. Guaranteed >= 4.5:1 against <see cref="AccentText"/>.</summary>
        public required Color Accent { get; init; }

        /// <summary>Text drawn on top of <see cref="Accent"/>, <see cref="Success"/> or <see cref="Danger"/>.</summary>
        public required Color AccentText { get; init; }

        /// <summary>Hover state for accent fills.</summary>
        public required Color AccentHover { get; init; }

        /// <summary>Accent used as text or as a link on a surface, rather than as a fill.</summary>
        public required Color AccentOnSurface { get; init; }

        /// <summary>Hairlines, separators and control borders.</summary>
        public required Color Border { get; init; }

        /// <summary>Focus ring. Consumed by WS4's focus visuals as well as input borders.</summary>
        public required Color BorderFocus { get; init; }

        /// <summary>Destructive fill. Guaranteed >= 4.5:1 against <see cref="AccentText"/>.</summary>
        public required Color Danger { get; init; }

        public required Color DangerHover { get; init; }

        /// <summary>Affirmative fill. Guaranteed >= 4.5:1 against <see cref="AccentText"/>.</summary>
        public required Color Success { get; init; }

        public required Color SuccessHover { get; init; }

        /// <summary>
        /// Neutral fill for a secondary button. Distinct from <see cref="TextSecondary"/>, which
        /// inverts between palettes and would put white text on pale grey in dark mode.
        /// Guaranteed >= 4.5:1 against <see cref="AccentText"/>.
        /// </summary>
        public required Color Neutral { get; init; }

        public required Color NeutralHover { get; init; }

        /// <summary>Caution fill, used for the information banner.</summary>
        public required Color Warning { get; init; }

        /// <summary>Text drawn on <see cref="Warning"/>.</summary>
        public required Color WarningText { get; init; }

        /// <summary>Selected-item background.</summary>
        public required Color Selection { get; init; }

        /// <summary>Added lines in the diff view. Guaranteed >= 4.5:1 against <see cref="SurfaceSunken"/>.</summary>
        public required Color DiffAdd { get; init; }

        public required Color DiffAddBg { get; init; }

        /// <summary>Removed lines in the diff view. Guaranteed >= 4.5:1 against <see cref="SurfaceSunken"/>.</summary>
        public required Color DiffRemove { get; init; }

        public required Color DiffRemoveBg { get; init; }

        /// <summary>Unchanged context lines in the diff view.</summary>
        public required Color DiffContext { get; init; }
    }
}

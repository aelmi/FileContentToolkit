using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace CodeShuttle.Help
{
    /// <summary>One help section: a stable id, a title, and the embedded Markdown behind it.</summary>
    public sealed class HelpTopic
    {
        /// <summary>Stable identifier, also the tag value attached to containers.</summary>
        public string Id { get; init; } = "";

        public string Title { get; init; } = "";

        /// <summary>Embedded resource file name, relative to the Help folder.</summary>
        public string Resource { get; init; } = "";

        public override string ToString() => Title;
    }

    /// <summary>
    /// The help table of contents, the embedded Markdown loader, and the F1 resolver.
    /// </summary>
    /// <remarks>
    /// Deliberately not the WebView2 system the reviews sketched: no topic tree, no inverted
    /// index, no <c>help://</c> links, and no new runtime dependency. A topic list beside
    /// rendered Markdown gives the same contextual behaviour — F1 lands on the section for
    /// whatever you were looking at — at a fraction of the surface.
    /// </remarks>
    public static class HelpTopics
    {
        public const string GettingStarted = "getting-started";
        public const string SelectingFiles = "selecting-files";
        public const string BuildingThePack = "building-the-pack";
        public const string ApplyingAnswersBack = "applying-answers-back";
        public const string Searching = "searching";
        public const string Presets = "presets";
        public const string Settings = "settings";
        public const string Troubleshooting = "troubleshooting";
        public const string Reference = "reference";

        public static IReadOnlyList<HelpTopic> All { get; } = new[]
        {
            new HelpTopic { Id = GettingStarted,      Title = "Getting Started",       Resource = "getting-started.md" },
            new HelpTopic { Id = SelectingFiles,      Title = "Selecting Files",       Resource = "selecting-files.md" },
            new HelpTopic { Id = BuildingThePack,     Title = "Building the Pack",     Resource = "building-the-pack.md" },
            new HelpTopic { Id = ApplyingAnswersBack, Title = "Applying Answers Back", Resource = "applying-answers-back.md" },
            new HelpTopic { Id = Searching,           Title = "Searching",             Resource = "searching.md" },
            new HelpTopic { Id = Presets,             Title = "Presets",               Resource = "presets.md" },
            new HelpTopic { Id = Settings,            Title = "Settings",              Resource = "settings.md" },
            new HelpTopic { Id = Troubleshooting,     Title = "Troubleshooting",       Resource = "troubleshooting.md" },
            new HelpTopic { Id = Reference,           Title = "Reference",             Resource = "reference.md" },
        };

        public static HelpTopic? Find(string? id) =>
            id == null ? null : All.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));

        /// <summary>The topic F1 lands on when nothing more specific applies.</summary>
        public static HelpTopic Default => All[0];

        // ------------------------------------------------------------------ topic tagging

        /// <summary>
        /// Which topic a control belongs to.
        /// </summary>
        /// <remarks>
        /// Held in a weak side table rather than in <see cref="Control.Tag"/>, which the language
        /// preset menu items and the tree picker already use for their own payloads. Same reason
        /// the theme system keeps its roles out of Tag.
        /// </remarks>
        private static readonly ConditionalWeakTable<Control, string> Tags = new();

        public static void Set(Control control, string topicId)
        {
            ArgumentNullException.ThrowIfNull(control);
            Tags.Remove(control);
            Tags.Add(control, topicId);
        }

        public static string? Get(Control? control) =>
            control != null && Tags.TryGetValue(control, out var id) ? id : null;

        /// <summary>
        /// Resolves the topic for a focused control by walking up its parent chain.
        /// </summary>
        /// <remarks>
        /// Walking up rather than requiring a tag on every control is the point: a group box or a
        /// panel is tagged once and every control inside it inherits, so adding a control does
        /// not mean remembering to tag it. Returns null when nothing in the chain is tagged, so
        /// the caller decides what the fallback is rather than having one baked in here.
        /// </remarks>
        public static HelpTopic? ResolveFor(Control? focused)
        {
            for (var c = focused; c != null; c = c.Parent)
            {
                var topic = Find(Get(c));
                if (topic != null) return topic;
            }
            return null;
        }

        /// <summary>How many distinct containers currently carry a tag. Used by tests.</summary>
        internal static int CountTagged(Control root)
        {
            int count = Get(root) != null ? 1 : 0;
            foreach (Control child in root.Controls) count += CountTagged(child);
            return count;
        }

        // ------------------------------------------------------------------ content

        /// <summary>
        /// Reads a topic's Markdown out of the assembly.
        /// </summary>
        /// <remarks>
        /// Returns a readable placeholder rather than throwing if a resource is missing: a help
        /// window that fails to open is a worse outcome than one section reading "not available".
        /// </remarks>
        public static string Read(HelpTopic topic)
        {
            ArgumentNullException.ThrowIfNull(topic);

            var assembly = typeof(HelpTopics).Assembly;
            var name = ResourceName(assembly, topic.Resource);
            if (name == null) return $"# {topic.Title}\n\nThis topic is not available in this build.";

            using var stream = assembly.GetManifestResourceStream(name);
            if (stream == null) return $"# {topic.Title}\n\nThis topic is not available in this build.";

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static string? ResourceName(Assembly assembly, string fileName)
        {
            // Matched by suffix rather than composed from the root namespace, because the
            // embedded name depends on the folder layout and would break silently if the Help
            // folder ever moved.
            var suffix = "." + fileName;
            return assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>True when every declared topic has its resource embedded. Used by tests.</summary>
        internal static bool AllResourcesPresent() =>
            All.All(t => ResourceName(typeof(HelpTopics).Assembly, t.Resource) != null);
    }
}

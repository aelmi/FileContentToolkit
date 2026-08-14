using System.Collections.Generic;
using System.Linq;

namespace CodeShuttle
{
    /// <summary>One project type: what to collect, and what to leave out.</summary>
    /// <param name="Group">Ecosystem the preset is filed under in the menu.</param>
    /// <param name="Name">What the user recognises the project as, e.g. "C# WinForms".</param>
    /// <param name="Extensions">Extensions the pack should contain.</param>
    /// <param name="Ignore">
    /// Directories and globs worth excluding for this stack. Merged into whatever the user already
    /// has rather than replacing it, because an ignore list is usually hand-tuned.
    /// </param>
    public sealed record ProjectPreset(string Group, string Name, string[] Extensions, string[] Ignore);

    /// <summary>
    /// The catalogue of project types offered by "Presets ▸ Project type" and by the extension
    /// chips' "+ add" menu.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This replaces a flat list of sixteen <em>languages</em> — "C# project", "Python", "Java".
    /// A language is not what anyone has: they have a WinForms app, or a Django site, or a Next.js
    /// front end, and the extensions those need differ sharply even within one language. "C#
    /// project" offered <c>.razor</c> and <c>.cshtml</c> to someone working on a desktop app, and
    /// no <c>.xaml</c> at all to someone working on WPF.
    /// </para>
    /// <para>
    /// Each entry also carries the build output worth ignoring for that stack, which is the other
    /// half of "set this up for me": a .NET pack that sweeps in <c>bin/</c> and <c>obj/</c> is
    /// mostly compiler output, and a Node one that sweeps in <c>node_modules/</c> will not fit in
    /// any context window ever built.
    /// </para>
    /// </remarks>
    public static class ProjectPresets
    {
        private static readonly string[] DotNetIgnore = { "bin/", "obj/", "packages/", ".vs/" };
        private static readonly string[] PythonIgnore = { "__pycache__/", ".venv/", "venv/", ".pytest_cache/", "*.pyc" };
        private static readonly string[] NodeIgnore = { "node_modules/", "dist/", "build/", ".next/", "coverage/" };
        private static readonly string[] JvmIgnore = { "build/", "target/", ".gradle/", "out/" };

        public static readonly ProjectPreset[] All =
        {
            // ---- .NET ----------------------------------------------------------------
            new(".NET", "C# WinForms",
                new[] { ".cs", ".resx", ".csproj", ".sln", ".config", ".settings", ".json" }, DotNetIgnore),
            new(".NET", "C# WPF",
                new[] { ".cs", ".xaml", ".resx", ".csproj", ".sln", ".config", ".json" }, DotNetIgnore),
            new(".NET", "ASP.NET Core",
                new[] { ".cs", ".cshtml", ".razor", ".csproj", ".sln", ".json", ".js", ".css", ".html" }, DotNetIgnore),
            new(".NET", "Blazor",
                new[] { ".razor", ".cs", ".csproj", ".sln", ".json", ".css", ".html" }, DotNetIgnore),
            new(".NET", "MAUI / Xamarin",
                new[] { ".cs", ".xaml", ".csproj", ".sln", ".json", ".plist", ".xml" }, DotNetIgnore),
            new(".NET", "Class library / console",
                new[] { ".cs", ".csproj", ".sln", ".json", ".config" }, DotNetIgnore),

            // ---- Python --------------------------------------------------------------
            new("Python", "Python",
                new[] { ".py", ".pyi", ".toml", ".cfg", ".ini", ".txt" }, PythonIgnore),
            new("Python", "Django",
                new[] { ".py", ".html", ".css", ".js", ".toml", ".cfg", ".txt" }, PythonIgnore),
            new("Python", "Flask / FastAPI",
                new[] { ".py", ".html", ".json", ".toml", ".cfg", ".txt" }, PythonIgnore),
            new("Python", "Notebooks / data science",
                new[] { ".py", ".ipynb", ".toml", ".cfg", ".txt" }, PythonIgnore),

            // ---- Web and JavaScript --------------------------------------------------
            new("Web", "TypeScript / React",
                new[] { ".ts", ".tsx", ".js", ".jsx", ".json", ".css", ".scss", ".html" }, NodeIgnore),
            new("Web", "Next.js",
                new[] { ".ts", ".tsx", ".js", ".jsx", ".json", ".css", ".scss", ".mdx" }, NodeIgnore),
            new("Web", "Vue",
                new[] { ".vue", ".ts", ".js", ".json", ".css", ".scss", ".html" }, NodeIgnore),
            new("Web", "Angular",
                new[] { ".ts", ".html", ".scss", ".css", ".json" }, NodeIgnore),
            new("Web", "Node.js API",
                new[] { ".js", ".ts", ".mjs", ".cjs", ".json" }, NodeIgnore),
            new("Web", "Plain HTML / CSS / JS",
                new[] { ".html", ".htm", ".css", ".js", ".json" }, NodeIgnore),

            // ---- Mobile --------------------------------------------------------------
            new("Mobile", "Android (Kotlin)",
                new[] { ".kt", ".kts", ".java", ".xml", ".gradle", ".properties" }, JvmIgnore),
            new("Mobile", "iOS (Swift)",
                new[] { ".swift", ".plist", ".storyboard", ".xib", ".strings" },
                new[] { "Pods/", "DerivedData/", "*.xcworkspace/" }),
            new("Mobile", "Flutter",
                new[] { ".dart", ".yaml", ".json" }, new[] { "build/", ".dart_tool/" }),
            new("Mobile", "React Native",
                new[] { ".ts", ".tsx", ".js", ".jsx", ".json" }, NodeIgnore),

            // ---- JVM and systems -----------------------------------------------------
            new("JVM", "Java (Maven / Gradle)",
                new[] { ".java", ".xml", ".gradle", ".properties" }, JvmIgnore),
            new("JVM", "Kotlin",
                new[] { ".kt", ".kts", ".gradle", ".properties" }, JvmIgnore),
            new("Systems", "C / C++",
                new[] { ".c", ".cpp", ".cc", ".cxx", ".h", ".hpp", ".hh", ".cmake" },
                new[] { "build/", "cmake-build-*/", "*.o", "*.obj" }),
            new("Systems", "Rust",
                new[] { ".rs", ".toml" }, new[] { "target/" }),
            new("Systems", "Go",
                new[] { ".go", ".mod", ".sum" }, new[] { "bin/", "vendor/" }),

            // ---- Other stacks --------------------------------------------------------
            new("Other", "PHP / Laravel",
                new[] { ".php", ".phtml", ".json", ".css", ".js" },
                new[] { "vendor/", "node_modules/", "storage/" }),
            new("Other", "Ruby / Rails",
                new[] { ".rb", ".erb", ".rake", ".gemspec", ".yml" },
                new[] { "vendor/", "tmp/", "log/" }),
            new("Other", "SQL / database",
                new[] { ".sql", ".ddl", ".dml", ".pks", ".pkb" }, System.Array.Empty<string>()),
            new("Other", "Shell / scripts",
                new[] { ".sh", ".bash", ".zsh", ".ps1", ".psm1", ".bat", ".cmd" }, System.Array.Empty<string>()),
            new("Other", "Infrastructure as code",
                new[] { ".tf", ".tfvars", ".yaml", ".yml", ".json" },
                new[] { ".terraform/", "*.tfstate" }),

            // ---- Cross-cutting -------------------------------------------------------
            new("Content", "Docs / markup",
                new[] { ".md", ".txt", ".rst", ".adoc" }, System.Array.Empty<string>()),
            new("Content", "Config files",
                new[] { ".json", ".yaml", ".yml", ".toml", ".ini", ".xml" }, System.Array.Empty<string>()),
        };

        /// <summary>The catalogue in menu order, grouped by ecosystem.</summary>
        public static IEnumerable<IGrouping<string, ProjectPreset>> ByGroup =>
            All.GroupBy(p => p.Group);
    }
}

namespace FileContentToolkit
{
    /// <summary>
    /// Static catalogue of common project / language extension bundles.
    /// Used by the "Add language preset" submenu in the Add split-button dropdown
    /// and the lstExtensions right-click context menu.
    /// </summary>
    public static class LanguagePresets
    {
        public static readonly (string Name, string[] Extensions)[] All =
        {
            ("C# project",            new[] { ".cs", ".csproj", ".sln", ".razor", ".cshtml", ".config" }),
            ("C / C++",               new[] { ".c", ".cpp", ".cc", ".cxx", ".h", ".hpp" }),
            ("Web (HTML/CSS/JS)",     new[] { ".html", ".htm", ".css", ".scss", ".js", ".json" }),
            ("TypeScript / React",    new[] { ".ts", ".tsx", ".js", ".jsx", ".json", ".css", ".scss" }),
            ("Node.js",               new[] { ".js", ".ts", ".mjs", ".json", ".md" }),
            ("Python",                new[] { ".py", ".pyx", ".pyi", ".toml", ".cfg", ".txt" }),
            ("Java",                  new[] { ".java", ".gradle", ".xml", ".properties" }),
            ("Kotlin",                new[] { ".kt", ".kts", ".gradle" }),
            ("Go",                    new[] { ".go", ".mod", ".sum" }),
            ("Rust",                  new[] { ".rs", ".toml" }),
            ("Ruby",                  new[] { ".rb", ".erb", ".rake", ".gemspec" }),
            ("PHP",                   new[] { ".php", ".phtml" }),
            ("Swift",                 new[] { ".swift", ".plist" }),
            ("Shell / Scripts",       new[] { ".sh", ".bash", ".zsh", ".ps1", ".bat", ".cmd" }),
            ("Docs / Markup",         new[] { ".md", ".txt", ".rst", ".adoc" }),
            ("Config files",          new[] { ".json", ".yaml", ".yml", ".toml", ".ini", ".xml", ".env" }),
        };
    }
}

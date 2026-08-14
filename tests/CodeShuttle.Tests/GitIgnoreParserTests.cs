using System;
using CodeShuttle.Filters;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// The parser decides what leaves the machine. A bug here either sweeps a secret into an
    /// export or silently drops files the user expected to see.
    /// </summary>
    public class GitIgnoreParserTests
    {
        private static GitIgnoreParser Parser(params string[] rules)
        {
            var p = new GitIgnoreParser();
            p.AddRules(rules);
            return p;
        }

        [Theory]
        [InlineData("*.log", "app.log", true)]
        [InlineData("*.log", "logs/app.log", true)]
        [InlineData("*.log", "app.txt", false)]
        [InlineData("build", "build/out.js", true)]
        [InlineData("build", "src/build/out.js", true)]
        [InlineData("/build", "build/out.js", true)]
        [InlineData("/build", "src/build/out.js", false)]
        [InlineData("node_modules/", "node_modules/pkg/index.js", true)]
        [InlineData("temp?.txt", "temp1.txt", true)]
        [InlineData("temp?.txt", "temp12.txt", false)]
        [InlineData("**/generated", "a/b/generated/x.cs", true)]
        public void PatternForms(string rule, string path, bool expected)
        {
            Assert.Equal(expected, Parser(rule).IsIgnored(path));
        }

        /// <summary>
        /// P1-4: git anchors a pattern containing a '/' anywhere other than trailing. Treating
        /// only a LEADING slash as anchoring made "doc/frotz" wrongly match "a/b/doc/frotz".
        /// </summary>
        [Fact]
        public void PatternContainingASlashIsAnchoredToTheRoot()
        {
            var parser = Parser("doc/frotz");

            Assert.True(parser.IsIgnored("doc/frotz"));
            Assert.False(parser.IsIgnored("a/b/doc/frotz"));
        }

        /// <summary>
        /// P1-4 regression: git is case-SENSITIVE by default. Hardcoding IgnoreCase meant a
        /// stray "*.MD" excluded every .md file in the repository.
        /// </summary>
        [Fact]
        public void MatchingIsCaseSensitiveByDefault()
        {
            var parser = Parser("*.MD");

            Assert.False(parser.IsIgnored("README.md"));
            Assert.True(parser.IsIgnored("README.MD"));
        }

        [Fact]
        public void CaseInsensitiveMatchingIsAvailableWhenAskedFor()
        {
            var parser = new GitIgnoreParser(ignoreCase: true);
            parser.AddRules(new[] { "*.MD" });

            Assert.True(parser.IsIgnored("README.md"));
        }

        [Fact]
        public void NegationReIncludes()
        {
            var parser = Parser("*.log", "!keep.log");

            Assert.True(parser.IsIgnored("app.log"));
            Assert.False(parser.IsIgnored("keep.log"));
        }

        [Fact]
        public void CommentsAndBlankLinesAreIgnored()
        {
            var parser = Parser("# a comment", "", "   ", "*.tmp");

            Assert.Equal(1, parser.RuleCount);
            Assert.True(parser.IsIgnored("x.tmp"));
        }

        [Fact]
        public void NoRulesMeansNothingIsIgnored()
        {
            var parser = new GitIgnoreParser();

            Assert.False(parser.HasRules);
            Assert.False(parser.IsIgnored("anything/at/all.cs"));
        }

        [Fact]
        public void BackslashSeparatorsAreNormalised()
        {
            Assert.True(Parser("bin/").IsIgnored(@"bin\Debug\app.dll"));
        }

        /// <summary>
        /// P1-3: .dockerignore must be a SEPARATE, opt-in rule set. A typical one starts with
        /// "*" and re-includes with "!", which merged into the gitignore list matched everything
        /// and scanned an ordinary repository down to zero files.
        /// </summary>
        [Fact]
        public void DockerIgnoreIsLoadedSeparatelyAndIsNotMergedIntoGitIgnore()
        {
            using var temp = new TempDir();
            temp.WriteText(".gitignore", "*.log\n");
            temp.WriteText(".dockerignore", "*\n!Dockerfile\n");

            var git = GitIgnoreParser.FromFolder(temp.Path);
            var docker = GitIgnoreParser.FromDockerIgnore(temp.Path);

            // The gitignore parser knows nothing about the dockerignore's catch-all.
            Assert.False(git.IsIgnored("src/Program.cs"));
            Assert.True(git.IsIgnored("app.log"));

            // The dockerignore rules still work, on their own rule set.
            Assert.True(docker.IsIgnored("src/Program.cs"));
            Assert.False(docker.IsIgnored("Dockerfile"));
        }

        /// <summary>
        /// P1-16: re-parsing and recompiling every rule on each scan meant a full reparse per
        /// keystroke under the typing debounce.
        /// </summary>
        [Fact]
        public void CachedParserIsReusedUntilTheIgnoreFileChanges()
        {
            using var temp = new TempDir();
            temp.WriteText(".gitignore", "*.log\n");
            GitIgnoreParser.ClearCache();

            var first = GitIgnoreParser.FromFolderCached(temp.Path);
            var second = GitIgnoreParser.FromFolderCached(temp.Path);
            Assert.Same(first, second);

            // A different file length invalidates the entry.
            temp.WriteText(".gitignore", "*.log\n*.tmp\n*.bak\n");
            var third = GitIgnoreParser.FromFolderCached(temp.Path);
            Assert.NotSame(first, third);
            Assert.True(third.IsIgnored("x.tmp"));
        }
    }
}

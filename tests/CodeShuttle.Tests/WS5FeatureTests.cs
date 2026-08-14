using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeShuttle.Diagnostics;
using CodeShuttle.Dialogs;
using CodeShuttle.Filters;
using CodeShuttle.Help;
using CodeShuttle.Settings;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// The WS5 features that can be verified without a message loop: prompt templates, the token
    /// budget, the secret gate, settings transfer, the help resolver, and the About diagnostics.
    /// </summary>
    [Collection(AppSettingsCollection.Name)]
    public class WS5FeatureTests
    {
        private static string Framed(params (string Path, string Content)[] entries) =>
            BundleFormat.Write(entries.Select(e => new BundleEntry
            {
                Path = e.Path,
                Content = e.Content,
                EndsWithNewline = true,
                HasMetadata = true,
            }));

        // ---------------------------------------------------------------- prompt templates

        [Fact]
        public void Settings_seed_the_two_built_in_templates_on_first_use()
        {
            var settings = new AppSettings();
            Assert.Empty(settings.PromptTemplates);

            var templates = PromptTemplateStore.Load(settings);

            Assert.Equal(2, templates.Count);
            Assert.Contains(templates, t => t.BuiltIn == PromptBuiltIn.Claude);
            Assert.Contains(templates, t => t.BuiltIn == PromptBuiltIn.ChatGpt);
        }

        /// <summary>
        /// The point of the whole exercise: the two prompt builders that were written, correct,
        /// and reachable from nowhere are now actually invoked, and receive the question argument
        /// their signature always accepted.
        /// </summary>
        [Theory]
        [InlineData(PromptBuiltIn.Claude)]
        [InlineData(PromptBuiltIn.ChatGpt)]
        public void An_unedited_built_in_renders_through_its_formatter_method(PromptBuiltIn which)
        {
            var settings = new AppSettings();
            var template = PromptTemplateStore.Load(settings).First(t => t.BuiltIn == which);
            var pack = Framed((@"C:\src\a.cs", "class A { }"));

            Assert.True(template.UsesBuiltInRenderer);

            var rendered = PromptTemplateStore.Render(template, pack, "Why is this slow?");
            var expected = which == PromptBuiltIn.Claude
                ? OutputFormatter.ForClaudePrompt(pack, "Why is this slow?")
                : OutputFormatter.ForChatGptPrompt(pack, "Why is this slow?");

            Assert.Equal(expected, rendered);
            Assert.Contains("Why is this slow?", rendered);
            Assert.Contains("class A { }", rendered);
        }

        [Fact]
        public void A_blank_question_falls_back_to_the_generic_instruction()
        {
            var settings = new AppSettings();
            var template = PromptTemplateStore.Load(settings).First(t => t.BuiltIn == PromptBuiltIn.Claude);

            var rendered = PromptTemplateStore.Render(template, Framed((@"C:\a.cs", "x")), "   ");

            Assert.Contains(PromptTemplate.DefaultQuestion, rendered);
        }

        /// <summary>Editing a built-in's body must take effect, not be silently discarded.</summary>
        [Fact]
        public void Editing_a_built_in_body_switches_it_to_generic_rendering()
        {
            var settings = new AppSettings();
            var template = PromptTemplateStore.Load(settings).First(t => t.BuiltIn == PromptBuiltIn.Claude);

            template.Body = "MY WRAPPER\n{files}\n{question}";
            Assert.False(template.UsesBuiltInRenderer);

            var rendered = PromptTemplateStore.Render(template, Framed((@"C:\a.cs", "body")), "go");

            Assert.StartsWith("MY WRAPPER", rendered);
            Assert.Contains("go", rendered);
        }

        /// <summary>
        /// A template with no <c>{files}</c> placeholder still gets the pack appended. Producing a
        /// prompt with no code in it is the one outcome that wastes an entire round trip.
        /// </summary>
        [Fact]
        public void A_template_without_the_files_placeholder_still_carries_the_pack()
        {
            var template = new PromptTemplate { Name = "bare", Body = "Just a question: {question}" };

            var rendered = template.Render("THE-FILES", "what?");

            Assert.Contains("what?", rendered);
            Assert.Contains("THE-FILES", rendered);
        }

        [Fact]
        public void Restoring_built_ins_leaves_user_templates_alone()
        {
            var settings = new AppSettings();
            PromptTemplateStore.Load(settings);
            settings.PromptTemplates.Add(new PromptTemplate { Name = "mine", Body = "{files}" });

            PromptTemplateStore.ResetBuiltIns(settings);

            Assert.Contains(settings.PromptTemplates, t => t.Name == "mine");
            Assert.Equal(2, settings.PromptTemplates.Count(t => t.BuiltIn != PromptBuiltIn.None));
        }

        // ---------------------------------------------------------------- token budget

        [Theory]
        [InlineData(1_000, 200_000, BudgetLevel.Ok)]
        [InlineData(159_999, 200_000, BudgetLevel.Ok)]
        [InlineData(160_000, 200_000, BudgetLevel.Near)]
        [InlineData(200_000, 200_000, BudgetLevel.Near)]
        [InlineData(200_001, 200_000, BudgetLevel.Over)]
        public void The_gauge_classifies_against_the_selected_window(int tokens, int window, BudgetLevel expected)
        {
            Assert.Equal(expected, TokenBudget.Classify(tokens, window));
        }

        /// <summary>A window of zero means "not measuring", not "everything is over".</summary>
        [Fact]
        public void With_no_window_configured_nothing_is_over_budget()
        {
            Assert.Equal(BudgetLevel.Ok, TokenBudget.Classify(50_000_000, 0));
            Assert.Equal(0, TokenBudget.PercentOf(50_000_000, 0));
        }

        [Fact]
        public void An_unknown_persisted_model_falls_back_rather_than_throwing()
        {
            Assert.Equal(TokenBudget.Claude.Id, TokenBudget.Resolve("gpt-9-turbo-ultra").Id);
            Assert.Equal(TokenBudget.Claude.Id, TokenBudget.Resolve(null).Id);
        }

        [Fact]
        public void The_custom_entry_takes_its_window_from_the_users_figure()
        {
            Assert.Equal(64_000, TokenBudget.WindowFor(TokenBudget.Custom, 64_000));
            Assert.Equal(200_000, TokenBudget.WindowFor(TokenBudget.Claude, 64_000));
        }

        [Fact]
        public void The_breakdown_ranks_files_largest_first()
        {
            var pack = Framed(
                (@"C:\small.cs", "a"),
                (@"C:\big.cs", new string('x', 5000)),
                (@"C:\medium.cs", new string('y', 500)));

            var breakdown = TokenBudget.Breakdown(pack);

            Assert.Equal(3, breakdown.Count);
            Assert.EndsWith("big.cs", breakdown[0].Path);
            Assert.EndsWith("small.cs", breakdown[2].Path);
        }

        /// <summary>
        /// The suggestion has to be actionable: removing what it names must actually bring the
        /// pack under the window, and it must name as few files as possible.
        /// </summary>
        [Fact]
        public void The_trim_suggestion_names_the_fewest_files_that_would_fit()
        {
            var breakdown = new List<TokenBudget.FileTokens>
            {
                new(@"C:\a.cs", 900),
                new(@"C:\b.cs", 400),
                new(@"C:\c.cs", 100),
            };

            // 1400 against a 400 window: dropping the largest alone leaves 500, still over, so
            // the suggestion has to reach the second file — and must stop there.
            const int total = 1400;
            const int window = 400;

            var trim = TokenBudget.SuggestTrim(breakdown, total, window);

            Assert.Equal(2, trim.Count);
            Assert.True(total - trim.Sum(f => f.Tokens) <= window,
                "removing what the suggestion names must actually bring the pack under the window");
            Assert.True(total - trim.Take(trim.Count - 1).Sum(f => f.Tokens) > window,
                "the suggestion names one file more than necessary");
        }

        /// <summary>The suggestion is taken from the largest end, not an arbitrary subset.</summary>
        [Fact]
        public void The_trim_suggestion_starts_with_the_largest_file()
        {
            var breakdown = new List<TokenBudget.FileTokens>
            {
                new(@"C:\huge.cs", 5000),
                new(@"C:\tiny.cs", 10),
            };

            var trim = TokenBudget.SuggestTrim(breakdown, totalTokens: 5010, windowTokens: 100);

            Assert.EndsWith("huge.cs", trim[0].Path);
        }

        [Fact]
        public void A_pack_that_already_fits_gets_no_trim_suggestion()
        {
            var breakdown = new List<TokenBudget.FileTokens> { new(@"C:\a.cs", 100) };
            Assert.Empty(TokenBudget.SuggestTrim(breakdown, totalTokens: 100, windowTokens: 500));
        }

        /// <summary>The number must be labelled as an estimate wherever it appears.</summary>
        [Fact]
        public void The_estimate_caveat_says_it_is_not_a_tokenizer()
        {
            Assert.Contains("estimate", TokenBudget.EstimateCaveat, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("tokenizer", TokenBudget.EstimateCaveat, StringComparison.OrdinalIgnoreCase);
        }

        // ---------------------------------------------------------------- secret gate

        /// <summary>
        /// The scenario from the plan: generating over a folder containing a live-looking AWS key
        /// must produce something for the warning dialog to show, attributed to the file it came
        /// from rather than to an offset in the concatenated pack.
        /// </summary>
        [Fact]
        public void A_pack_containing_an_aws_key_is_flagged_and_attributed_to_its_file()
        {
            var pack = Framed(
                (@"C:\src\ok.cs", "class Ok { }"),
                (@"C:\src\.env", "AWS_ACCESS_KEY_ID=AKIAIOSFODNN7EXAMPLE"));

            var matches = SecretGuard.Scan(pack);

            var aws = Assert.Single(matches, m => m.Kind == SecretKind.AwsAccessKeyId);
            Assert.EndsWith(".env", aws.Path);
            Assert.Equal(1, aws.Line);
        }

        [Fact]
        public void Redaction_removes_the_value_and_leaves_a_marker()
        {
            var pack = Framed((@"C:\src\.env", "AWS_ACCESS_KEY_ID=AKIAIOSFODNN7EXAMPLE"));
            var matches = SecretGuard.Scan(pack);

            var redacted = SecretGuard.Redact(pack, matches);

            Assert.DoesNotContain("AKIAIOSFODNN7EXAMPLE", redacted, StringComparison.Ordinal);
            Assert.Contains("[REDACTED:", redacted, StringComparison.Ordinal);
        }

        /// <summary>
        /// Text that is not a parseable pack is still scanned. Failing open on a credential check
        /// is the wrong direction to fail, and the output pane is editable.
        /// </summary>
        [Fact]
        public void Unparseable_text_is_still_scanned_rather_than_skipped()
        {
            var matches = SecretGuard.Scan("here is a key I pasted: AKIAIOSFODNN7EXAMPLE");

            Assert.Contains(matches, m => m.Kind == SecretKind.AwsAccessKeyId);
        }

        /// <summary>
        /// Why MainForm.BtnCompress_Click must run the gate against the PRE-compression text.
        /// </summary>
        /// <remarks>
        /// SecretGuard.Scan is a text scan. Compression replaces the output pane in place with
        /// gzip+base64, in which no pattern matches, so Decide returns Pass and the pack is
        /// released untouched — through "compress the pack so it fits the context window, then
        /// paste it", a workflow the product itself teaches in its help. This test pins the
        /// blindness rather than trusting it not to exist: if a future scanner did see into
        /// base64, the ordering would stop mattering and this test would say so by failing.
        /// </remarks>
        [Fact]
        public void Compressed_output_is_opaque_to_the_secret_scanner()
        {
            var pack = Framed((@"C:\src\.env", "AWS_ACCESS_KEY_ID=AKIAIOSFODNN7EXAMPLE"));

            Assert.NotEmpty(SecretGuard.Scan(pack));

            var compressed = CompressionUtils.CompressToBase64(pack);

            Assert.DoesNotContain("AKIAIOSFODNN7EXAMPLE", compressed, StringComparison.Ordinal);
            Assert.Empty(SecretGuard.Scan(compressed));
            Assert.Equal(SecretGateAction.Pass,
                SecretGuard.Decide(SecretGuard.Scan(compressed).Count, warnOnSecrets: true, redactSecrets: true));
        }

        /// <summary>
        /// The consequence: gating first and compressing the gate's output carries the redaction
        /// through, which is what the Compress handler now does.
        /// </summary>
        [Fact]
        public void Redacting_before_compressing_keeps_the_credential_out_of_the_blob()
        {
            var pack = Framed((@"C:\src\.env", "AWS_ACCESS_KEY_ID=AKIAIOSFODNN7EXAMPLE"));

            var redacted = SecretGuard.Redact(pack, SecretGuard.Scan(pack));
            var compressed = CompressionUtils.CompressToBase64(redacted);

            var roundTripped = CompressionUtils.DecompressFromBase64(compressed);

            Assert.DoesNotContain("AKIAIOSFODNN7EXAMPLE", roundTripped, StringComparison.Ordinal);
            Assert.Contains("[REDACTED:", roundTripped, StringComparison.Ordinal);
        }

        [Theory]
        [InlineData(0, true, true, SecretGateAction.Pass)]
        [InlineData(3, true, true, SecretGateAction.Ask)]
        [InlineData(3, true, false, SecretGateAction.Ask)]
        [InlineData(3, false, true, SecretGateAction.RedactSilently)]
        [InlineData(3, false, false, SecretGateAction.Pass)]
        public void The_gate_decides_from_the_two_settings(
            int matches, bool warn, bool redact, SecretGateAction expected)
        {
            Assert.Equal(expected, SecretGuard.Decide(matches, warn, redact));
        }

        /// <summary>Both protections default on. The failure they prevent is not recoverable.</summary>
        [Fact]
        public void Secret_protections_default_on()
        {
            var settings = new AppSettings();
            Assert.True(settings.RedactSecrets);
            Assert.True(settings.WarnOnSecrets);
        }

        /// <summary>The dialog must never render the value itself, only a masked preview.</summary>
        [Fact]
        public void The_warning_dialog_shows_a_masked_preview_not_the_value()
        {
            var pack = Framed((@"C:\src\.env", "AWS_ACCESS_KEY_ID=AKIAIOSFODNN7EXAMPLE"));
            var matches = SecretGuard.Scan(pack);

            StaRunner.Run(() =>
            {
                using var dlg = new SecretWarningForm(matches, redactByDefault: true);

                var list = (ListView)FindControl(dlg, "lstMatches");
                foreach (ListViewItem item in list.Items)
                {
                    foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                        Assert.DoesNotContain("AKIAIOSFODNN7EXAMPLE", sub.Text, StringComparison.Ordinal);
                }

                Assert.NotEmpty(list.Items);
                Assert.All(list.Items.Cast<ListViewItem>(), i => Assert.True(i.Checked));
            });
        }

        [Fact]
        public void Keeping_everything_leaves_the_redaction_set_empty()
        {
            var matches = SecretGuard.Scan(Framed((@"C:\.env", "API_KEY=abcdefgh12345678")));
            Assert.NotEmpty(matches);

            StaRunner.Run(() =>
            {
                using var dlg = new SecretWarningForm(matches, redactByDefault: false);

                Assert.Empty(dlg.Redacted);
                Assert.Equal(matches.Count, dlg.Kept.Count);
            });
        }

        // ---------------------------------------------------------------- settings transfer

        [Fact]
        public void Exported_settings_round_trip_to_an_equal_object()
        {
            var original = new AppSettings
            {
                MaxFileSizeBytes = 4096,
                SkipBinaryFiles = false,
                AutoDetectEncoding = false,
                UseGitIgnoreFiles = false,
                UseDockerIgnoreFiles = true,
                WatchFolderForChanges = true,
                RegexSearch = true,
                CaseSensitiveSearch = true,
                WholeWordSearch = true,
                Mode = Theming.ThemeMode.Dark,
                RedactSecrets = false,
                WarnOnSecrets = false,
                TokenModelId = TokenBudget.Gemini.Id,
                CustomTokenBudget = 12_345,
            };
            original.RecentFolders.Add(@"C:\code\one");
            original.RecentSearches.Add("TODO");
            original.Presets.Add(new Preset
            {
                Name = "API",
                FolderPath = @"C:\code\api",
                Extensions = { ".cs", ".json" },
                IgnorePatterns = { "bin/", "*.tmp" },
                IncludeSubfolders = false,
            });
            PromptTemplateStore.Load(original);
            original.PromptTemplates.Add(new PromptTemplate { Name = "mine", Body = "{files}" });

            var json = SettingsTransfer.Export(original);

            var restored = new AppSettings();
            SettingsTransfer.Import(restored, json);

            Assert.Equal(original.MaxFileSizeBytes, restored.MaxFileSizeBytes);
            Assert.Equal(original.SkipBinaryFiles, restored.SkipBinaryFiles);
            Assert.Equal(original.AutoDetectEncoding, restored.AutoDetectEncoding);
            Assert.Equal(original.UseGitIgnoreFiles, restored.UseGitIgnoreFiles);
            Assert.Equal(original.UseDockerIgnoreFiles, restored.UseDockerIgnoreFiles);
            Assert.Equal(original.WatchFolderForChanges, restored.WatchFolderForChanges);
            Assert.Equal(original.RegexSearch, restored.RegexSearch);
            Assert.Equal(original.CaseSensitiveSearch, restored.CaseSensitiveSearch);
            Assert.Equal(original.WholeWordSearch, restored.WholeWordSearch);
            Assert.Equal(original.Mode, restored.Mode);
            Assert.Equal(original.RedactSecrets, restored.RedactSecrets);
            Assert.Equal(original.WarnOnSecrets, restored.WarnOnSecrets);
            Assert.Equal(original.TokenModelId, restored.TokenModelId);
            Assert.Equal(original.CustomTokenBudget, restored.CustomTokenBudget);

            Assert.Equal(original.RecentFolders, restored.RecentFolders);
            Assert.Equal(original.RecentSearches, restored.RecentSearches);

            var preset = Assert.Single(restored.Presets);
            Assert.Equal("API", preset.Name);
            Assert.Equal(new[] { ".cs", ".json" }, preset.Extensions);
            Assert.Equal(new[] { "bin/", "*.tmp" }, preset.IgnorePatterns);
            Assert.False(preset.IncludeSubfolders);

            Assert.Equal(original.PromptTemplates.Count, restored.PromptTemplates.Count);
            Assert.Contains(restored.PromptTemplates, t => t.Name == "mine");
            Assert.Contains(restored.PromptTemplates, t => t.BuiltIn == PromptBuiltIn.Claude);

            // Re-exporting the imported object must produce the identical document.
            Assert.Equal(json, SettingsTransfer.Export(restored));
        }

        [Fact]
        public void Settings_export_and_import_through_a_file()
        {
            using var dir = new TempDir();
            var path = Path.Combine(dir.Path, "exported.json");

            var original = new AppSettings { CustomTokenBudget = 777 };
            original.Presets.Add(new Preset { Name = "P" });

            SettingsTransfer.ExportToFile(original, path);

            var restored = new AppSettings();
            SettingsTransfer.ImportFromFile(restored, path);

            Assert.Equal(777, restored.CustomTokenBudget);
            Assert.Single(restored.Presets);
        }

        /// <summary>
        /// Window geometry describes one machine's monitors. Carrying it in a shared configuration
        /// file is how somebody else's window ends up off the edge of your screen.
        /// </summary>
        [Fact]
        public void Machine_specific_settings_are_deliberately_not_exported()
        {
            var original = new AppSettings
            {
                WindowBounds = new WindowBoundsSetting { X = 3000, Y = 2000, Width = 800, Height = 600 },
                SplitterDistance = 999,
                HasCompletedFirstRun = true,
            };

            var restored = new AppSettings();
            SettingsTransfer.Import(restored, SettingsTransfer.Export(original));

            Assert.Null(restored.WindowBounds);
            Assert.Equal(0, restored.SplitterDistance);
            Assert.False(restored.HasCompletedFirstRun);
        }

        [Fact]
        public void Importing_a_file_that_is_not_settings_reports_rather_than_throwing_raw()
        {
            var settings = new AppSettings();
            Assert.Throws<InvalidDataException>(() => SettingsTransfer.Import(settings, "{ not json at all"));
        }

        /// <summary>A settings file written before WS5 existed must still load with defaults.</summary>
        [Fact]
        public void A_settings_file_without_the_ws5_fields_still_loads()
        {
            using var dir = new TempDir();
            var path = Path.Combine(dir.Path, "settings.json");
            File.WriteAllText(path, "{ \"RecentFolders\": [ \"C:\\\\code\" ] }");

            var loaded = AppSettings.Load(path);

            Assert.Single(loaded.RecentFolders);
            Assert.True(loaded.RedactSecrets);
            Assert.True(loaded.WarnOnSecrets);
            Assert.Equal("claude", loaded.TokenModelId);
            Assert.Equal(0, loaded.CustomTokenBudget);
            Assert.Empty(loaded.PromptTemplates);
        }

        [Fact]
        public void The_ws5_settings_survive_the_settings_file()
        {
            using var dir = new TempDir();
            var path = Path.Combine(dir.Path, "settings.json");

            var saved = new AppSettings
            {
                RedactSecrets = false,
                WarnOnSecrets = false,
                TokenModelId = TokenBudget.Gpt.Id,
                CustomTokenBudget = 4242,
            };
            saved.PromptTemplates.Add(new PromptTemplate { Name = "t", Body = "{files}", Format = PromptBodyFormat.Xml });
            saved.Save(path);

            var loaded = AppSettings.Load(path);

            Assert.False(loaded.RedactSecrets);
            Assert.False(loaded.WarnOnSecrets);
            Assert.Equal(TokenBudget.Gpt.Id, loaded.TokenModelId);
            Assert.Equal(4242, loaded.CustomTokenBudget);
            var template = Assert.Single(loaded.PromptTemplates);
            Assert.Equal("t", template.Name);
            Assert.Equal(PromptBodyFormat.Xml, template.Format);
        }

        // ---------------------------------------------------------------- help

        [Fact]
        public void Every_declared_help_topic_has_its_markdown_embedded()
        {
            Assert.True(HelpTopics.AllResourcesPresent(),
                "a declared topic has no embedded resource; check the EmbeddedResource glob");

            Assert.All(HelpTopics.All, t =>
            {
                var text = HelpTopics.Read(t);
                Assert.False(string.IsNullOrWhiteSpace(text));
                Assert.DoesNotContain("not available in this build", text, StringComparison.Ordinal);
            });
        }

        [Fact]
        public void The_round_trip_has_its_own_help_section()
        {
            Assert.Contains(HelpTopics.All, t => t.Id == HelpTopics.ApplyingAnswersBack);

            var text = HelpTopics.Read(HelpTopics.Find(HelpTopics.ApplyingAnswersBack)!);
            Assert.Contains("Rejected", text, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("backup", text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// F1's resolver: a control inherits the topic of the nearest tagged ancestor, so tagging
        /// a pane once covers everything inside it.
        /// </summary>
        [Fact]
        public void The_help_resolver_walks_up_to_the_nearest_tagged_ancestor()
        {
            StaRunner.Run(() =>
            {
                using var root = new Panel { Name = "root" };
                var middle = new Panel { Name = "middle" };
                var leaf = new TextBox { Name = "leaf" };

                root.Controls.Add(middle);
                middle.Controls.Add(leaf);

                HelpTopics.Set(root, HelpTopics.GettingStarted);
                Assert.Equal(HelpTopics.GettingStarted, HelpTopics.ResolveFor(leaf)?.Id);

                // The nearer tag wins.
                HelpTopics.Set(middle, HelpTopics.Searching);
                Assert.Equal(HelpTopics.Searching, HelpTopics.ResolveFor(leaf)?.Id);
            });
        }

        [Fact]
        public void The_help_resolver_returns_null_when_nothing_is_tagged()
        {
            StaRunner.Run(() =>
            {
                using var orphan = new Panel();
                Assert.Null(HelpTopics.ResolveFor(orphan));
                Assert.Null(HelpTopics.ResolveFor(null));
            });
        }

        /// <summary>At least six containers carry a topic, per the acceptance criterion.</summary>
        [Fact]
        public void The_main_window_tags_at_least_six_containers_with_a_help_topic()
        {
            StaRunner.Run(() =>
            {
                using var form = new MainForm();
                int tagged = HelpTopics.CountTagged(form);

                Assert.True(tagged >= 6, $"only {tagged} containers carry a help topic");
            });
        }

        [Fact]
        public void The_help_window_opens_at_the_requested_topic()
        {
            StaRunner.Run(() =>
            {
                using var help = new HelpForm(HelpTopics.Troubleshooting);
                Assert.Equal(HelpTopics.Troubleshooting, help.CurrentTopicId);
            });
        }

        [Fact]
        public void An_unknown_topic_falls_back_rather_than_opening_blank()
        {
            StaRunner.Run(() =>
            {
                using var help = new HelpForm("no-such-topic");
                Assert.False(string.IsNullOrEmpty(help.CurrentTopicId));
            });
        }

        // ---------------------------------------------------------------- About diagnostics

        /// <summary>
        /// The diagnostics blob is a support tool, not an exfiltration channel. The natural next
        /// step after copying it is pasting it into an email or a public issue tracker, so it must
        /// not carry anything about what the user scanned.
        /// </summary>
        [Fact]
        public void Copy_diagnostics_never_includes_a_scanned_path_or_file_name()
        {
            var previous = CrashLogger.ScanRoot;
            try
            {
                CrashLogger.ScanRoot = @"C:\Clients\Acme\secret-project";

                var report = DiagnosticsReport.Build();

                Assert.DoesNotContain("Acme", report, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("secret-project", report, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain(@"C:\Clients", report, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                CrashLogger.ScanRoot = previous;
            }
        }

        [Fact]
        public void Copy_diagnostics_carries_what_a_support_request_actually_needs()
        {
            var report = DiagnosticsReport.Build();

            Assert.Contains("Version", report, StringComparison.Ordinal);
            Assert.Contains("OS", report, StringComparison.Ordinal);
            Assert.Contains(".NET", report, StringComparison.Ordinal);
            Assert.Contains("Culture", report, StringComparison.Ordinal);
            Assert.Contains("Settings folder", report, StringComparison.Ordinal);
            Assert.Contains("Log folder", report, StringComparison.Ordinal);
        }

        /// <summary>A bare year is not a copyright notice; the holder is required.</summary>
        [Fact]
        public void About_shows_a_copyright_holder_a_version_and_third_party_notices()
        {
            Assert.False(string.IsNullOrWhiteSpace(AboutInfo.CopyrightHolder));
            Assert.Contains(AboutInfo.CopyrightHolder, AboutInfo.Copyright, StringComparison.Ordinal);
            Assert.Matches(@"©\s*\d{4}\s+\S", AboutInfo.Copyright);

            Assert.False(string.IsNullOrWhiteSpace(AboutInfo.ThirdPartyNotices));
            Assert.Contains("MIT", AboutInfo.ThirdPartyNotices, StringComparison.Ordinal);

            StaRunner.Run(() =>
            {
                using var about = new AboutForm();

                Assert.Contains(AboutInfo.CopyrightHolder,
                    ((Label)FindControl(about, "lblCopyright")).Text, StringComparison.Ordinal);

                Assert.Contains(AppVersion.Full,
                    ((Label)FindControl(about, "lblVersion")).Text, StringComparison.Ordinal);

                var notices = (TextBox)FindControl(about, "txtNotices");
                Assert.False(string.IsNullOrWhiteSpace(notices.Text));
                Assert.True(notices.ReadOnly);

                FindControl(about, "btnCopyDiagnostics");
                FindControl(about, "btnOpenSettings");
                FindControl(about, "btnOpenLogs");
            });
        }

        // ---------------------------------------------------------------- exclusion rules

        [Fact]
        public void The_rule_editor_reports_which_rule_excludes_a_tested_path()
        {
            var candidates = new[]
            {
                @"C:\proj\src\a.cs",
                @"C:\proj\bin\b.dll",
                @"C:\proj\src\temp.tmp",
            };

            StaRunner.Run(() =>
            {
                using var dlg = new ExclusionRuleEditorForm(
                    new[] { "bin/", "*.tmp" }, @"C:\proj", candidates);

                var list = (ListView)FindControl(dlg, "lstRules");
                Assert.Equal(2, list.Items.Count);

                // Each rule reports what it removes on its own, which is the whole point of the
                // editor: the previous comma-separated box gave no feedback at all.
                Assert.All(list.Items.Cast<ListViewItem>(), i =>
                    Assert.Contains("excludes", i.SubItems[1].Text, StringComparison.Ordinal));

                var summary = (Label)FindControl(dlg, "lblSummary");
                Assert.Contains("excluded", summary.Text, StringComparison.Ordinal);
            });
        }

        [Fact]
        public void The_rule_editor_keeps_the_rules_it_was_given()
        {
            StaRunner.Run(() =>
            {
                using var dlg = new ExclusionRuleEditorForm(
                    new[] { "bin/", "", "  ", "*.tmp" }, "", Array.Empty<string>());

                // Blank entries are dropped rather than becoming rules that match everything.
                Assert.Equal(new[] { "bin/", "*.tmp" }, dlg.Rules);
            });
        }

        // ---------------------------------------------------------------- helpers

        private static Control FindControl(Control root, string name)
        {
            var found = FindControlOrNull(root, name);
            Assert.True(found != null, $"control '{name}' not found under {root.GetType().Name}");
            return found!;
        }

        private static Control? FindControlOrNull(Control root, string name)
        {
            foreach (Control child in root.Controls)
            {
                if (child.Name == name) return child;
                var deeper = FindControlOrNull(child, name);
                if (deeper != null) return deeper;
            }
            return null;
        }
    }
}

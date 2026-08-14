using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace CodeShuttle.Diagnostics
{
    public sealed class UpdateInfo
    {
        public Version Current { get; init; } = new(0, 0);
        public Version Latest { get; init; } = new(0, 0);
        public string TagName { get; init; } = "";
        public string HtmlUrl { get; init; } = "";
        public bool UpdateAvailable => Latest > Current;
    }

    /// <summary>
    /// Lightweight GitHub-releases checker. Compares the latest release tag to the running
    /// assembly version. Best-effort: no network = no notice.
    /// </summary>
    public static class UpdateChecker
    {
        private static readonly HttpClient _http = CreateClient();

        // Cached: JsonSerializerOptions is expensive to construct and caches its own metadata,
        // so a fresh instance per call defeats that cache (CA1869).
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        // Repository to check. Adjust if you fork.
        public const string Owner = "aelmi";
        public const string Repo = "CodeShuttle";

        private static HttpClient CreateClient()
        {
            var c = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
            c.DefaultRequestHeaders.UserAgent.ParseAdd("CodeShuttle-UpdateChecker/1.0");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            return c;
        }

        public static async Task<UpdateInfo?> CheckAsync(CancellationToken ct = default)
        {
            try
            {
                var url = $"https://api.github.com/repos/{Owner}/{Repo}/releases/latest";
                using var resp = await _http.GetAsync(url, ct).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode) return null;

                await using var stream = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
                var release = await JsonSerializer.DeserializeAsync<GhRelease>(stream, _json, ct)
                    .ConfigureAwait(false);
                if (release == null || string.IsNullOrEmpty(release.TagName)) return null;

                // The releases/latest endpoint excludes drafts but INCLUDES prereleases, so
                // without this filter publishing a single beta would offer that beta to every
                // stable user as an update. Both flags are checked defensively.
                if (release.Prerelease || release.Draft) return null;

                // Must come from the informational version. The assembly's binding version is
                // pinned at 1.0.0.0, which made this check report "update available" every launch.
                var current = ParseVersion(AppVersion.Display);
                var latest = ParseVersion(release.TagName);

                return new UpdateInfo
                {
                    Current = current,
                    Latest = latest,
                    TagName = release.TagName ?? "",
                    HtmlUrl = release.HtmlUrl ?? $"https://github.com/{Owner}/{Repo}/releases"
                };
            }
            catch
            {
                return null;
            }
        }

        // Accepts "v1.2.3", "1.2.3", "1.2.3.4", "1.2", "v1.2-beta" etc.
        private static Version ParseVersion(string? tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return new Version(0, 0);
            var s = tag.TrimStart('v', 'V');
            // strip anything after a '-' or '+' so prereleases parse cleanly
            int cut = s.AsSpan().IndexOfAny('-', '+');
            if (cut >= 0) s = s.Substring(0, cut);
            return Version.TryParse(s, out var v) ? v : new Version(0, 0);
        }

        private sealed class GhRelease
        {
            [JsonPropertyName("tag_name")] public string? TagName { get; set; }
            [JsonPropertyName("html_url")] public string? HtmlUrl { get; set; }
            [JsonPropertyName("prerelease")] public bool Prerelease { get; set; }
            [JsonPropertyName("draft")] public bool Draft { get; set; }
        }
    }
}

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FileContentToolkit.Diagnostics
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

        // Repository to check. Adjust if you fork.
        public const string Owner = "aelmi";
        public const string Repo = "FileContentToolkit";

        private static HttpClient CreateClient()
        {
            var c = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
            c.DefaultRequestHeaders.UserAgent.ParseAdd("FileContentToolkit-UpdateChecker/1.0");
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
                var release = await JsonSerializer.DeserializeAsync<GhRelease>(stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct).ConfigureAwait(false);
                if (release == null || string.IsNullOrEmpty(release.TagName)) return null;

                var current = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 0);
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
            int cut = s.IndexOfAny(new[] { '-', '+' });
            if (cut >= 0) s = s.Substring(0, cut);
            return Version.TryParse(s, out var v) ? v : new Version(0, 0);
        }

        private sealed class GhRelease
        {
            [JsonPropertyName("tag_name")] public string? TagName { get; set; }
            [JsonPropertyName("html_url")] public string? HtmlUrl { get; set; }
        }
    }
}

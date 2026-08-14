using System;
using System.Linq;
using CodeShuttle.Filters;
using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// The app packs .env files straight into an AI prompt. Detection is deliberately biased
    /// toward recall: a false positive costs one click, a false negative leaks a production key
    /// into a third-party chat transcript.
    /// </summary>
    public class SecretScannerTests
    {
        /// <summary>WS2 acceptance criterion 14.</summary>
        [Fact]
        public void DetectsAwsAccessKeyId()
        {
            var matches = SecretScanner.Scan("const key = \"AKIAIOSFODNN7EXAMPLE\";", "config.js");

            Assert.Contains(matches, m => m.Kind == SecretKind.AwsAccessKeyId && m.Value == "AKIAIOSFODNN7EXAMPLE");
        }

        /// <summary>WS2 acceptance criterion 14.</summary>
        [Fact]
        public void DetectsPemPrivateKeyHeader()
        {
            const string content = "-----BEGIN RSA PRIVATE KEY-----\nMIIEow...\n-----END RSA PRIVATE KEY-----";

            var matches = SecretScanner.Scan(content, "id_rsa");

            Assert.Contains(matches, m => m.Kind == SecretKind.PrivateKey);
            Assert.Equal(1, matches.First(m => m.Kind == SecretKind.PrivateKey).Line);
        }

        [Theory]
        [InlineData("-----BEGIN PRIVATE KEY-----")]
        [InlineData("-----BEGIN EC PRIVATE KEY-----")]
        [InlineData("-----BEGIN OPENSSH PRIVATE KEY-----")]
        public void DetectsEveryPemPrivateKeyVariant(string header)
        {
            Assert.Contains(SecretScanner.Scan(header, "key.pem"), m => m.Kind == SecretKind.PrivateKey);
        }

        [Fact]
        public void DetectsApiKeyAssignment()
        {
            var matches = SecretScanner.Scan("api_key = \"sk_live_9fT2xQ8mVb3nP7wL\"", "settings.py");

            Assert.Contains(matches, m => m.Kind == SecretKind.ApiKeyAssignment);
        }

        [Fact]
        public void DetectsConnectionStringPassword()
        {
            const string content = "Server=db;Database=app;User Id=sa;Password=Hunter2Hunter2;";

            Assert.Contains(SecretScanner.Scan(content, "appsettings.json"),
                m => m.Kind == SecretKind.ConnectionStringPassword);
        }

        [Fact]
        public void DetectsJsonWebToken()
        {
            const string jwt = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.dozjgNryP4J3jVmNHl0w5N_XgL0n3I9PlFUP0THsR8U";

            Assert.Contains(SecretScanner.Scan($"const token = '{jwt}';", "auth.js"),
                m => m.Kind == SecretKind.JsonWebToken);
        }

        [Fact]
        public void DetectsHighEntropyEnvValue()
        {
            var matches = SecretScanner.Scan("SESSION_SECRET=xQ9vB2mK7pL4nR8tZ3wY6jH1sD5fG0aC", ".env");

            Assert.Contains(matches, m => m.Kind == SecretKind.HighEntropyEnvValue);
        }

        [Fact]
        public void IgnoresLowEntropyAndPlaceholderValues()
        {
            var matches = SecretScanner.Scan(string.Join("\n", new[]
            {
                "API_KEY=your_api_key_here",
                "PASSWORD=changeme",
                "TOKEN=${VAULT_TOKEN}",
                "DEBUG=true",
                "LOG_LEVEL=aaaaaaaaaaaaaaaaaaaaaaa"
            }), ".env");

            Assert.Empty(matches);
        }

        [Fact]
        public void CleanSourceProducesNoMatches()
        {
            const string content = "public class Widget\n{\n    public int Count { get; set; }\n}";

            Assert.Empty(SecretScanner.Scan(content, "Widget.cs"));
        }

        [Fact]
        public void ReportsTheCorrectLineNumber()
        {
            var content = "line one\nline two\nkey = \"AKIAIOSFODNN7EXAMPLE\"\nline four";

            var match = SecretScanner.Scan(content, "x.cs").First(m => m.Kind == SecretKind.AwsAccessKeyId);

            Assert.Equal(3, match.Line);
        }

        [Fact]
        public void PreviewNeverExposesTheWholeSecret()
        {
            var match = SecretScanner.Scan("key = \"AKIAIOSFODNN7EXAMPLE\"", "x.cs")
                                     .First(m => m.Kind == SecretKind.AwsAccessKeyId);

            Assert.DoesNotContain(match.Value, match.Preview, StringComparison.Ordinal);
            Assert.StartsWith("AKIA", match.Preview, StringComparison.Ordinal);
        }

        [Fact]
        public void RedactReplacesTheValueWithAMarker()
        {
            const string content = "const key = \"AKIAIOSFODNN7EXAMPLE\";";
            var matches = SecretScanner.Scan(content, "config.js");

            var redacted = SecretScanner.Redact(content, matches);

            Assert.DoesNotContain("AKIAIOSFODNN7EXAMPLE", redacted, StringComparison.Ordinal);
            Assert.Contains("[REDACTED:", redacted, StringComparison.Ordinal);
        }

        [Fact]
        public void ShannonEntropySeparatesRandomFromRepetitive()
        {
            Assert.True(SecretScanner.ShannonEntropy("xQ9vB2mK7pL4nR8tZ3wY6jH1sD5fG0aC") >= SecretScanner.EntropyThreshold);
            Assert.True(SecretScanner.ShannonEntropy("aaaaaaaaaaaaaaaaaaaaaaaa") < SecretScanner.EntropyThreshold);
        }

        [Fact]
        public void NullAndEmptyInputAreHandled()
        {
            Assert.Empty(SecretScanner.Scan(null, "x.cs"));
            Assert.Empty(SecretScanner.Scan("", "x.cs"));
        }
    }
}

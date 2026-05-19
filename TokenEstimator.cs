namespace FileContentToolkit
{
    /// <summary>
    /// Cheap token-count estimator. Uses the common 4-characters-per-token heuristic that
    /// matches GPT and Claude tokenizers within roughly 10% for prose / code. Good enough
    /// to give the user a sense of "will this fit in my LLM context window".
    /// </summary>
    public static class TokenEstimator
    {
        public const double CharsPerToken = 4.0;

        public static int Estimate(string? text)
            => string.IsNullOrEmpty(text) ? 0 : (int)((text.Length + CharsPerToken - 1) / CharsPerToken);
    }
}

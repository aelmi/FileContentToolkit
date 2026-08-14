namespace CodeShuttle
{
    /// <summary>
    /// Cheap token-count estimator, giving the user a sense of "will this fit in my LLM context
    /// window".
    ///
    /// The ratio is deliberately pessimistic. The often-quoted 4.0 chars/token holds for prose;
    /// real code sits nearer 3.0–3.5 because of punctuation and identifiers. Since the whole
    /// point of the number is "will this fit", under-estimating is the failure the user actually
    /// feels, so 3.3 is used instead.
    /// </summary>
    public static class TokenEstimator
    {
        public const double CharsPerToken = 3.3;

        public static int Estimate(string? text)
            => string.IsNullOrEmpty(text) ? 0 : (int)((text.Length + CharsPerToken - 1) / CharsPerToken);
    }
}

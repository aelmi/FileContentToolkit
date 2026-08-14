using Xunit;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Serialises every test class that calls <c>AppSettings.Load</c>.
    /// <para>
    /// <c>AppSettings.LastLoadError</c> is static and <c>Load()</c> resets it to null on entry, so two
    /// classes loading settings in parallel can null the field between another test's arrange and assert.
    /// That produced a genuine 1-in-6 flake. xunit parallelises across classes by default; sharing one
    /// collection puts these three in the same pool and removes the race without slowing anything
    /// measurable (they are all sub-millisecond file round-trips).
    /// </para>
    /// </summary>
    [CollectionDefinition(Name)]
    public sealed class AppSettingsCollection
    {
        public const string Name = "AppSettings state";
    }
}

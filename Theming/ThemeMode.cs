namespace CodeShuttle.Theming
{
    /// <summary>
    /// Replaces the old <c>AppSettings.DarkMode</c> boolean, which could not express "follow the
    /// system". <see cref="System"/> is defined so the preference can be stored and migrated, but
    /// the registry watcher that would honour it is deferred; it currently resolves to
    /// <see cref="Light"/>.
    /// </summary>
    public enum ThemeMode
    {
        Light = 0,
        Dark = 1,
        System = 2,
    }
}

using System;
using System.Windows.Forms;
using CodeShuttle.Dialogs;

namespace CodeShuttle.UI
{
    /// <summary>
    /// Convenience wrapper around <see cref="MessageDialog"/>, mirroring <see cref="ThemedPrompt"/>
    /// so callers don't manage the dialog's lifetime.
    /// </summary>
    /// <remarks>
    /// Use this — not <c>Toast</c> — whenever the thing the user asked for did not happen. Toast
    /// is for outcomes that need no acknowledgement; anything a user might want to read twice,
    /// copy, or act on belongs here.
    /// </remarks>
    public static class AppMessage
    {
        public static void Error(IWin32Window? owner, string title, string message, string? details = null) =>
            Show(owner, MessageKind.Error, title, message, details);

        public static void Warning(IWin32Window? owner, string title, string message, string? details = null) =>
            Show(owner, MessageKind.Warning, title, message, details);

        public static void Info(IWin32Window? owner, string title, string message, string? details = null) =>
            Show(owner, MessageKind.Info, title, message, details);

        /// <summary>
        /// Reports an exception with its type and stack folded into the copyable details, so a
        /// user reporting a fault can hand over something actionable instead of a paraphrase.
        /// </summary>
        public static void Error(IWin32Window? owner, string title, string message, Exception ex) =>
            Show(owner, MessageKind.Error, title, message, ex?.ToString());

        private static void Show(IWin32Window? owner, MessageKind kind, string title, string message, string? details)
        {
            using var dlg = new MessageDialog(kind, title, message, details);
            dlg.ShowDialog(owner);
        }
    }
}

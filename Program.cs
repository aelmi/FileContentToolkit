using CodeShuttle.Diagnostics;

namespace CodeShuttle
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Catch and log unhandled exceptions before they tear the process down.
            CrashLogger.Install();

            // An unhandled UI exception used to be logged and swallowed with no dialog at all:
            // the app carried on in an undefined state and the user was never told a log existed.
            CrashLogger.CrashLogged += ShowCrashNotice;

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        private static void ShowCrashNotice(string logPath, Exception? ex)
        {
            try
            {
                var message =
                    "Something went wrong and CodeShuttle may not be in a reliable state. " +
                    "Saving your work and restarting is recommended.\n\n" +
                    (ex?.Message ?? "No further detail is available.") +
                    "\n\nA report was written to:\n" + logPath +
                    "\n\nOpen the log folder?";

                var answer = MessageBox.Show(message, "CodeShuttle — unexpected error",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);

                if (answer == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = CrashLogger.LogsDirectory,
                        UseShellExecute = true
                    });
                }
            }
            catch
            {
                // Reporting a crash must never itself crash.
            }
        }
    }
}

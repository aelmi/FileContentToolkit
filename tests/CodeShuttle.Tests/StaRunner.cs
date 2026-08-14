using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace CodeShuttle.Tests
{
    /// <summary>
    /// Runs a block on a single-threaded-apartment thread.
    /// </summary>
    /// <remarks>
    /// WinForms controls create their window handles on an apartment-threaded COM context, so
    /// instantiating a Form on xunit's MTA worker throws. The usual answer is the
    /// <c>Xunit.StaFact</c> package, but this workstream is not permitted to add a NuGet
    /// dependency, and one short-lived thread per test costs less than a package does.
    ///
    /// The original exception is rethrown with its stack intact, so a failure inside the block
    /// still points at the assertion that failed rather than at this helper.
    /// </remarks>
    internal static class StaRunner
    {
        public static void Run(Action action)
        {
            ExceptionDispatchInfo? captured = null;

            var thread = new Thread(() =>
            {
                try { action(); }
                catch (Exception ex) { captured = ExceptionDispatchInfo.Capture(ex); }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            // Generous, but a hang here means a dialog is trying to go modal, which is a real
            // defect and should fail the run rather than block it forever.
            if (!thread.Join(TimeSpan.FromSeconds(60)))
                throw new TimeoutException("STA test body did not complete within 60 seconds.");

            captured?.Throw();
        }
    }
}

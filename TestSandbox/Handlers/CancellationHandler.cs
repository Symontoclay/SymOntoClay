using SymOntoClay.CoreHelper.Cancellation;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System.Threading;

namespace TestSandbox.Handlers
{
    public class CancellationHandler
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            //Case2();
            Case1();

            _logger.Info("End");
        }

        private void Case2()
        {
            //var source = new CancellationTokenSource();

            //CancellationTokenSourceContext
        }

        private void Case1()
        {
            var context = new CancellationTokenContext(CancellationToken.None);

            _logger.Info($"context = {context}");

            var cancellationToken = context.Token;

            _logger.Info($"cancellationToken = {cancellationToken}");
        }
    }
}

using SymOntoClay.Common.Cancellation;
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

            Case3();
            //Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case3()
        {
            var source1 = new CancellationTokenSource();
            var context1 = new CancellationTokenSourceContext(source1);

            _logger.Info($"context1 = {context1}");

            var source2 = new CancellationTokenSource();
            var context2 = new CancellationTokenSourceContext(source2);

            _logger.Info($"context2 = {context2}");

            var context = new CancellationLinkedTokenSourceContext(context1, context2);

            _logger.Info($"context = {context}");

            var cancellationToken = context.Token;

            _logger.Info($"cancellationToken = {cancellationToken}");
        }

        private void Case2()
        {
            var source = new CancellationTokenSource();

            var context = new CancellationTokenSourceContext(source);

            _logger.Info($"context = {context}");

            var cancellationToken = context.Token;

            _logger.Info($"cancellationToken = {cancellationToken}");
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

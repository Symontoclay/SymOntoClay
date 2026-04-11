using SymOntoClay.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal;
using System.Threading;

namespace TestSandbox.Handlers
{
    public class GameComponentGuardHander
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case1();

            _logger.Info("End");
        }

        private void Case1()
        {
            var objectName = "Obj1";

            var autoResetEvent = new ManualResetEvent(true);

            var state = ComponentState.Disposed;

            _logger.Info($"state = {state}");

            var result = GameComponentGuard.Check("3FEEC3AC-4636-471B-B500-11C880A4D81B", objectName, ref state, autoResetEvent);

            _logger.Info($"result = {result}");
        }
    }
}

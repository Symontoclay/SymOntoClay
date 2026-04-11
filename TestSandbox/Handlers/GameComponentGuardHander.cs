using SymOntoClay.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal;

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
            var state = ComponentState.Created;
        }
    }
}

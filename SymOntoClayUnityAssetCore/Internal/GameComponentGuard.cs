using SymOntoClay.Core.Internal;
using System;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public static class GameComponentGuard
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool Check(string pointId, ref ComponentState componentState, EventWaitHandle waitEvent)
        {
#if DEBUG
            _logger.Info($"pointId = {pointId}");
            _logger.Info($"componentState = {componentState}");
#endif

            waitEvent?.WaitOne();



            throw new NotImplementedException("C3FDDEE0-5572-4585-B71E-7CD64C9F244E");
        }
    }
}

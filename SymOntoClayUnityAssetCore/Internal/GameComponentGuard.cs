using SymOntoClay.Core.Internal;
using System;
using System.Linq;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public static class GameComponentGuard
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool Check(string pointId, string objectName, ref ComponentState componentState, EventWaitHandle waitEvent, params ComponentState[] leaveIf)
        {
#if DEBUG
            //_logger.Info($"pointId = {pointId}");
            //_logger.Info($"componentState = {componentState}");
            //_logger.Info($"leaveIf.Length = {leaveIf.Length}");
#endif

            waitEvent?.WaitOne();

            if (componentState == ComponentState.Disposed)
            {
                throw new ObjectDisposedException(objectName, pointId);
            }

            //throw new NotImplementedException("C3FDDEE0-5572-4585-B71E-7CD64C9F244E");
            return !leaveIf.Contains(componentState);
        }
    }
}

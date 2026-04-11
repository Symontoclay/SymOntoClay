using System;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public static class GameComponentGuard
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool Check()
        {
#if DEBUG
            _logger.Info("Run");
#endif

            throw new NotImplementedException("C3FDDEE0-5572-4585-B71E-7CD64C9F244E");
        }
    }
}

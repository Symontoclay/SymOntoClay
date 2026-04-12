using NLog;
using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal.DateAndTime;
using SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache;
using SymOntoClay.UnityAsset.Core.Internal.ModulesStorage;
using SymOntoClay.UnityAsset.Core.Internal.Storage;
using System;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class SerializedWorldContext
    {
        public SerializedWorldContext(IWorldCoreContext coreContext)
        {
            _coreContext = coreContext;
        }

        private IWorldCoreContext _coreContext;

        public DateTimeProvider DateTimeProvider { get; private set; }

        public void Init()
        {
            DateTimeProvider = new DateTimeProvider(_coreContext.Logger, _coreContext.SyncContext, _coreContext.AsyncEventsThreadPool, _coreContext.GetCancellationContext());
            //LogicQueryParseAndCache = new LogicQueryParseAndCache(_coreContext);
            //ModulesStorage = new ModulesStorageComponent(_coreContext);
            //StandaloneStorage = new StandaloneStorageComponent(_coreContext);
            //ModulesStorage.Init(_coreContext.StandaloneStorage.StandaloneStorage.Context);
        }

        public void LoadFromSourceCode()
        {
            DateTimeProvider.LoadFromSourceCode();
            //throw new NotImplementedException("C0D7FA6C-3CD2-496F-BF2F-A79F7F12B074");
        }
    }
}

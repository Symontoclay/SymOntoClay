using NLog;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal.DateAndTime;
using SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache;
using SymOntoClay.UnityAsset.Core.Internal.ModulesStorage;
using SymOntoClay.UnityAsset.Core.Internal.Storage;
using System;
using System.Collections.Generic;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class SerializedWorldContext: Disposable
    {
        public SerializedWorldContext(IWorldCoreContext coreContext)
        {
            _coreContext = coreContext;
        }

        private IWorldCoreContext _coreContext;

        public DateTimeProvider DateTimeProvider { get; private set; }
        public LogicQueryParseAndCache LogicQueryParseAndCache { get; private set; }
        public ModulesStorageComponent ModulesStorage { get; private set; }
        public StandaloneStorageComponent StandaloneStorage { get; private set; }

        private readonly object _worldComponentsListLockObj = new object();
        private readonly List<ISerializedWorldCoreComponent> _worldComponentsList = new List<ISerializedWorldCoreComponent>();

        public void Init()
        {
            DateTimeProvider = new DateTimeProvider(_coreContext.Logger, _coreContext.SyncContext, _coreContext.AsyncEventsThreadPool, _coreContext.GetCancellationContext());
            LogicQueryParseAndCache = new LogicQueryParseAndCache(_coreContext);
            ModulesStorage = new ModulesStorageComponent(_coreContext);
            StandaloneStorage = new StandaloneStorageComponent(_coreContext);
            ModulesStorage.Init(StandaloneStorage.StandaloneStorage.Context);
        }

        public void LoadFromSourceCode()
        {
            DateTimeProvider.LoadFromSourceCode();
            ModulesStorage.LoadFromSourceCode();
            StandaloneStorage.LoadFromSourceCode();
        }

        public void AddSerializedWorldComponent(ISerializedWorldCoreComponent component)
        {
            lock (_worldComponentsListLockObj)
            {
                if (_worldComponentsList.Contains(component))
                {
                    return;
                }

                _worldComponentsList.Add(component);
            }
        }

        public void AddPublicFactsStorage(IStorage publicFactsStorage)
        {
            StandaloneStorage.StandaloneStorage.WorldPublicFactsStorage.AddConsolidatedStorage(_coreContext.Logger, publicFactsStorage);
        }

        public void RemoveGameComponent(IStorage publicFactsStorage)
        {
            StandaloneStorage.StandaloneStorage.WorldPublicFactsStorage.RemoveConsolidatedStorage(_coreContext.Logger, publicFactsStorage);
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            lock (_worldComponentsListLockObj)
            {
                foreach (var item in _worldComponentsList)
                {
                    item.Dispose();
                }
            }

            base.OnDisposing();
        }
    }
}

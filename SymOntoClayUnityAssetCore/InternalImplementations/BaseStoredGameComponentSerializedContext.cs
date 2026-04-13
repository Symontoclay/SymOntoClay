using SymOntoClay.Common.Disposing;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Runtime;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public class BaseStoredGameComponentSerializedContext : Disposable
    {
        public BaseStoredGameComponentSerializedContext(BaseStoredGameComponentSettings settings, IWorldCoreGameComponentContext worldContext, KindOfWorldItem kindOfWorldItem, BaseStoredGameComponent gameComponent)
        {
            var standaloneStorageSettings = new StandaloneStorageSettings();
            standaloneStorageSettings.Id = settings.Id;
            standaloneStorageSettings.IsWorld = false;
            standaloneStorageSettings.AppFile = settings.HostFile;
            standaloneStorageSettings.MonitorNode = gameComponent.MonitorNode;
            standaloneStorageSettings.StandardFactsBuilder = worldContext.StandardFactsBuilder;

            standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage;
            standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage;
            standaloneStorageSettings.LogicQueryParseAndCache = worldContext.LogicQueryParseAndCache;

            standaloneStorageSettings.Categories = settings.Categories;
            standaloneStorageSettings.EnableCategories = settings.EnableCategories;

            standaloneStorageSettings.ThreadingSettings = kindOfWorldItem switch
            {
                KindOfWorldItem.Player => worldContext.PlayerDefaultThreadingSettings,
                KindOfWorldItem.GameObject => worldContext.GameObjectDefaultThreadingSettings,
                KindOfWorldItem.Place => worldContext.PlaceDefaultThreadingSettings,
                _ => throw new ArgumentOutOfRangeException(nameof(kindOfWorldItem), kindOfWorldItem, null)
            };

            standaloneStorageSettings.CancellationContext = worldContext.GetCancellationContext();

            HostStorage = new StandaloneStorage(standaloneStorageSettings);
        }

        public StandaloneStorage HostStorage { get; private set; }

        public void LoadFromSourceCode()
        {
            HostStorage.LoadFromSourceCode();
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            HostStorage.Dispose();

            base.OnDisposing();
        }
    }
}

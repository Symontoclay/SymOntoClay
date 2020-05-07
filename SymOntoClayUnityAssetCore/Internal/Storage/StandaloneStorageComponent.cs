using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Storage
{
    public class StandaloneStorageComponent: BaseWorldCoreComponent
    {
        public StandaloneStorageComponent(WorldSettings settings, IWorldCoreContext coreContext)
            : base(coreContext)
        {
            var standaloneStorageSettings = new StandaloneStorageSettings();
            standaloneStorageSettings.Id = "world";
            standaloneStorageSettings.Logger = coreContext.Logger;
            standaloneStorageSettings.ModulesStorage = coreContext.ModulesStorage;
            standaloneStorageSettings.Dictionary = coreContext.SharedDictionary;
            standaloneStorageSettings.AppFile = settings.HostFile;

            Log($"standaloneStorageSettings = {standaloneStorageSettings}");

            _standaloneStorage = new StandaloneStorage(standaloneStorageSettings);
        }

        private readonly StandaloneStorage _standaloneStorage;

        public IStandaloneStorage StandaloneStorage => _standaloneStorage;

        public void LoadFromSourceCode()
        {
            _standaloneStorage.LoadFromSourceCode();
        }
    }
}

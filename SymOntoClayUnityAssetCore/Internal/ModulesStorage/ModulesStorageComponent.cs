using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.ModulesStorage
{
    public class ModulesStorageComponent : BaseWorldCoreComponent
    {
        public ModulesStorageComponent(IWorldCoreContext coreContext)
            : base(coreContext)
        {
            var modulesStorageSettings = new ModulesStorageSettings();
            modulesStorageSettings.Logger = Logger;
            modulesStorageSettings.Dictionary = coreContext.SharedDictionary;

            Log($"modulesStorageSettings = {modulesStorageSettings}");

            _modulesStorage = new SymOntoClay.Core.ModulesStorage(modulesStorageSettings);
        }

        private readonly SymOntoClay.Core.ModulesStorage _modulesStorage;

        public SymOntoClay.Core.IModulesStorage ModulesStorage => _modulesStorage;
    }
}

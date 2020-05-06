using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.BipedNPC
{
    public class BipedNPCGameComponent: BaseGameComponent
    {
        public BipedNPCGameComponent(BipedNPCSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings.Id, worldContext)
        {
            var standaloneStorageSettings = new StandaloneStorageSettings();
            standaloneStorageSettings.Id = settings.Id;
            standaloneStorageSettings.AppFile = settings.HostFile;
            standaloneStorageSettings.Logger = Logger;
            standaloneStorageSettings.Dictionary = worldContext.SharedDictionary;
            standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage;
            standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage;

            Log($"standaloneStorageSettings = {standaloneStorageSettings}");

            _hostStorage = new StandaloneStorage(standaloneStorageSettings);

            var coreEngineSettings = new EngineSettings();
            coreEngineSettings.Id = settings.Id;
            coreEngineSettings.AppFile = settings.LogicFile;
            coreEngineSettings.Logger = Logger;
            coreEngineSettings.SyncContext = worldContext.SyncContext;
            coreEngineSettings.Dictionary = worldContext.SharedDictionary;
            coreEngineSettings.ModulesStorage = worldContext.ModulesStorage;
            coreEngineSettings.ParentStorage = _hostStorage;

            Log($"coreEngineSettings = {coreEngineSettings}");

            _coreEngine = new Engine(coreEngineSettings);           
        }

        private readonly StandaloneStorage _hostStorage;
        private readonly Engine _coreEngine;

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
#if IMAGINE_WORKING
            Log("Do");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public override void BeginStarting()
        {
#if IMAGINE_WORKING
            Log("Do");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public override bool IsWaited
        {
            get
            {
#if IMAGINE_WORKING
                Log("Do");
                return true;
#else
            throw new NotImplementedException();
#endif
            }
        }
    }
}

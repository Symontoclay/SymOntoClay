using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.BipedNPC
{
    public class BipedNPCGameComponent: BaseGameComponent
    {
        public BipedNPCGameComponent(BipedNPCSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings.Id, worldContext)
        {
            try
            {
#if DEBUG
                Log($"settings = {settings}");
                Log($"worldContext.TmpDir = {worldContext.TmpDir}");
#endif
                var tmpDir = Path.Combine(worldContext.TmpDir, settings.Id);

                Directory.CreateDirectory(worldContext.TmpDir);

                var standaloneStorageSettings = new StandaloneStorageSettings();
                standaloneStorageSettings.Id = settings.Id;
                standaloneStorageSettings.IsWorld = false;
                standaloneStorageSettings.AppFile = settings.HostFile;
                standaloneStorageSettings.Logger = Logger;
                standaloneStorageSettings.Dictionary = worldContext.SharedDictionary;
                standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage;
                standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage;

#if DEBUG
                Log($"standaloneStorageSettings = {standaloneStorageSettings}");
#endif
                _hostStorage = new StandaloneStorage(standaloneStorageSettings);

                var coreEngineSettings = new EngineSettings();
                coreEngineSettings.Id = settings.Id;
                coreEngineSettings.AppFile = settings.LogicFile;
                coreEngineSettings.Logger = Logger;
                coreEngineSettings.SyncContext = worldContext.SyncContext;
                coreEngineSettings.Dictionary = worldContext.SharedDictionary;
                coreEngineSettings.ModulesStorage = worldContext.ModulesStorage;
                coreEngineSettings.ParentStorage = _hostStorage;
                coreEngineSettings.TmpDir = tmpDir;

#if DEBUG
                Log($"coreEngineSettings = {coreEngineSettings}");
#endif
                _coreEngine = new Engine(coreEngineSettings);
            }
            catch (Exception e)
            {
                Log(e.ToString());

                throw e;
            }     
        }

        private readonly StandaloneStorage _hostStorage;
        private readonly Engine _coreEngine;

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            //try
            //{
                _hostStorage.LoadFromSourceCode();
                _coreEngine.LoadFromSourceCode();
            //}
            //catch(Exception e)
            //{
            //    Log(e.ToString());

            //    throw e;
            //}
        }

        /// <inheritdoc/>
        public override void BeginStarting()
        {
            try
            {
                _coreEngine.BeginStarting();
            }
            catch (Exception e)
            {
                Log(e.ToString());

                throw e;
            }           
        }

        /// <inheritdoc/>
        public override bool IsWaited
        {
            get
            {
                return _coreEngine.IsWaited;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _hostStorage.Dispose();
            _coreEngine.Dispose();

            base.OnDisposed();
        }
    }
}

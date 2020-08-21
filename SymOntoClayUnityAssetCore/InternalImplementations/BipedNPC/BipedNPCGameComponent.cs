using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.BipedNPC
{
    public class BipedNPCGameComponent: BaseManualControllingGameComponent
    {
        public BipedNPCGameComponent(BipedNPCSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
            try
            {
#if DEBUG
                Log($"settings = {settings}");
                Log($"worldContext.TmpDir = {worldContext.TmpDir}");
#endif
                var tmpDir = Path.Combine(worldContext.TmpDir, settings.Id);

                Directory.CreateDirectory(worldContext.TmpDir);

                var coreEngineSettings = new EngineSettings();
                coreEngineSettings.Id = settings.Id;
                coreEngineSettings.AppFile = settings.LogicFile;
                coreEngineSettings.Logger = Logger;
                coreEngineSettings.SyncContext = worldContext.SyncContext;
                coreEngineSettings.Dictionary = worldContext.SharedDictionary;
                coreEngineSettings.ModulesStorage = worldContext.ModulesStorage;
                coreEngineSettings.ParentStorage = HostStorage;
                coreEngineSettings.TmpDir = tmpDir;
                coreEngineSettings.HostListener = this;
                coreEngineSettings.DateTimeProvider = worldContext.DateTimeProvider;

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

        private readonly Engine _coreEngine;

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            try
            {
                _coreEngine.LoadFromSourceCode();
            }
            catch(Exception e)
            {
                Log(e.ToString());

                throw e;
            }
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
            _coreEngine.Dispose();

            base.OnDisposed();
        }
    }
}

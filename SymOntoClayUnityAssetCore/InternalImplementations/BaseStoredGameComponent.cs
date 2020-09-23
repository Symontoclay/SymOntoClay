using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public abstract class BaseStoredGameComponent: BaseGameComponent
    {
        protected BaseStoredGameComponent(BaseStoredGameComponentSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
            try
            {
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
                HostStorage = new StandaloneStorage(standaloneStorageSettings);
            }
            catch (Exception e)
            {
                Log(e.ToString());

                throw e;
            }
        }

        protected StandaloneStorage HostStorage { get; private set; }

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            //try
            //{
                HostStorage.LoadFromSourceCode();
            //}
            //catch (Exception e)
            //{
            //    Log(e.ToString());

            //    throw e;
            //}
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            HostStorage.Dispose();

            base.OnDisposed();
        }
    }
}

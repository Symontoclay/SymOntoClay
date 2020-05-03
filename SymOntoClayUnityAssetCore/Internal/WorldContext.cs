using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.Images;
using SymOntoClay.UnityAsset.Core.Internal.Logging;
using SymOntoClay.UnityAsset.Core.Internal.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class WorldContext: IWorldCoreContext, IWorldCoreGameComponentContext
    {
        //TODO: fix me!
        public void SetSettings(WorldSettings settings)
        {
            WorldSettingsValidator.Validate(settings);

            CreateLogging(settings);
            CreateComponents(settings);
            //throw new NotImplementedException();
        }

        private void CreateLogging(WorldSettings settings)
        {
            CoreLogger = new CoreLogger(settings.Logging, this);
        }

        private void CreateComponents(WorldSettings settings)
        {
            ImagesRegistry = new ImagesRegistry(this);
        }

        public CoreLogger CoreLogger { get; private set; }

        public ILogger Logger => CoreLogger?.WordCoreLogger;

        public ImagesRegistry ImagesRegistry { get; private set; }

        private readonly object _wordComponentsListLockObj = new object();
        private List<IWorldCoreComponent> _wordComponentsList = new List<IWorldCoreComponent>();

        void IWorldCoreContext.AddWorldComponent(IWorldCoreComponent component)
        {
            lock(_wordComponentsListLockObj)
            {
                if(_wordComponentsList.Contains(component))
                {
                    return;
                }

                _wordComponentsList.Add(component);
            }
        }

        private List<IGameComponent> _gameComponentsList = new List<IGameComponent>();

        public bool EnableLogging { get => CoreLogger.EnableLogging; set => CoreLogger.EnableLogging = value; }

        public bool EnableRemoteConnection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Load(IRunTimeImageInfo imageInfo)
        {
            throw new NotImplementedException();
        }

        public void Load(string id)
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool IsActive { get => throw new NotImplementedException(); }

        public IRunTimeImageInfo CreateImage(RunTimeImageSettings settings)
        {
            throw new NotImplementedException();
        }

        public IRunTimeImageInfo CreateImage()
        {
            throw new NotImplementedException();
        }

        public IRunTimeImageInfo CurrentImage { get => throw new NotImplementedException(); }

        public IList<IRunTimeImageInfo> GetImages()
        {
            throw new NotImplementedException();
        }

        public void DeleteImage(IRunTimeImageInfo imageInfo)
        {
            throw new NotImplementedException();
        }

        public void DeleteImage(string id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsDisposed { get => throw new NotImplementedException(); }
    }
}

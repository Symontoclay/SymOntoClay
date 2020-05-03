using SymOntoClay.UnityAsset.Core.Internal.Logging;
using SymOntoClay.UnityAsset.Core.Internal.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class WorldContext: IWorlCoreContext, IWorlCoreGameComponentContext
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
        public ImagesRegistry ImagesRegistry { get; private set; }

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

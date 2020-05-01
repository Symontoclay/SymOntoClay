using SymOntoClay.UnityAsset.Core.Internal.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class WorldContext: IWorlCoreContext, IWorlCoreComponentContext
    {
        //TODO: fix me!
        public void SetSettings(WorldSettings settings)
        {
            WorldSettingsValidator.Validate(settings);

            //throw new NotImplementedException();
        }

        public bool EnableLogging { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

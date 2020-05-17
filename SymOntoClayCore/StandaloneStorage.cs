using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class StandaloneStorage: BaseComponent, IStandaloneStorage, ISerializableEngine
    {
        public StandaloneStorage(StandaloneStorageSettings settings)
            : base(settings.Logger)
        {
            _settings = settings;

            Log($"_settings = {_settings}");
        }

        private readonly StandaloneStorageSettings _settings;
        private RealStorage _storage;

        IStorage IStandaloneStorage.Storage
        {
            get
            {
                lock (_stateLockObj)
                {
                    if (_state == ComponentState.Disposed)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    return _storage;
                }
            }
        }

        /// <inheritdoc/>
        public void LoadFromSourceCode()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                var storageSettings = new RealStorageSettings();
                storageSettings.Logger = Logger;
                storageSettings.EntityDictionary = _settings.Dictionary;

                if(_settings?.ParentStorage?.Storage != null)
                {
                    storageSettings.ParentsStorages = new List<IStorage>() { _settings.ParentStorage.Storage };
                }

                if (_settings.IsWorld)
                {
                    _storage = new WorldStorage(storageSettings);
                }
                else
                {
                    _storage = new HostStorage(storageSettings);
                }

#if IMAGINE_WORKING
                Log("Do");
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <inheritdoc/>
        public void LoadFromImage(string path)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

#if IMAGINE_WORKING
                Log("Do");
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <inheritdoc/>
        public void SaveToImage(string path)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

#if IMAGINE_WORKING
                Log("Do");
#else
                throw new NotImplementedException();
#endif
            }
        }
    }
}

using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Dict;
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class SharedDictionary : BaseComponent, IEntityDictionary, ISerializableEngine
    {
        public SharedDictionary(SharedDictionarySettings settings)
            :base(settings.Logger)
        {
            _dictionary = new EntityDictionary(settings.Logger);
        }

        private readonly EntityDictionary _dictionary;

        public string Name
        {
            get
            {
                lock (_stateLockObj)
                {
                    if (_state == ComponentState.Disposed)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    return _dictionary.Name;
                }
            }
        }

        /// <inheritdoc/>
        public ulong GetKey(string name)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                return _dictionary.GetKey(name);
            }
        }

        /// <inheritdoc/>
        public string GetName(ulong key)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                return _dictionary.GetName(key);
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

                _dictionary.LoadFromSourceCode();
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

                _dictionary.LoadFromImage(path);
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

                _dictionary.SaveToImage(path);
            }
        }
    }
}

/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

#if DEBUG
        /// <inheritdoc/>
        public string GetDbgStr()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                return _dictionary.GetDbgStr();
            }
        }
#endif
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

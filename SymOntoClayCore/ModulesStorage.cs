/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core
{
    public class ModulesStorage : BaseComponent, IModulesStorage, ISerializableEngine
    {
        public ModulesStorage(ModulesStorageSettings settings)
            : base(settings.Logger)
        {
            Log($"settings = {settings}");
        }

        private readonly object _lockObj = new object();

        private readonly Dictionary<StrongIdentifierValue, IStorage> _storagesDict = new Dictionary<StrongIdentifierValue, IStorage>();

        /// <inheritdoc/>
        public void LoadFromSourceCode()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                //throw new NotImplementedException();

#if IMAGINE_WORKING
                //Log("Do");
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
                //Log("Do");
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
                //Log("Do");
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <inheritdoc/>
        public IList<IStorage> Import(IList<StrongIdentifierValue> namesList)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }
            }

            lock(_lockObj)
            {
#if DEBUG
                Log($"namesList = {namesList.WriteListToString()}");
#endif

                var result = new List<IStorage>();

                foreach (var name in namesList)
                {
#if DEBUG
                    Log($"name = {name}");
#endif

                    if(_storagesDict.ContainsKey(name))
                    {
                        result.Add(_storagesDict[name]);
                    }
                    else
                    {
                        result.Add(LoadLib(name));
                    }
                }

                return result;
            }
        }

        private IStorage LoadLib(StrongIdentifierValue name)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            throw new NotImplementedException();
        }
    }
}

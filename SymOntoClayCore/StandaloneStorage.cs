/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
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

            _additionalSourceCodePaths = settings.AdditionalSourceCodePaths;

            _context = EngineContextHelper.CreateAndInitMainStorageContext(settings);
        }
        
        private readonly MainStorageContext _context;

        /// <inheritdoc/>
        public IMainStorageContext Context => _context;
        private readonly IList<string> _additionalSourceCodePaths;

        private IStorageComponent _storageComponent;
        private IStorage _storage;
        private IStorage _publicFactsStorage;
        private ConsolidatedPublicFactsStorage _worldPublicFactsStorage;

        /// <inheritdoc/>
        public IStorageComponent StorageComponent
        {
            get
            {
                lock (_stateLockObj)
                {
                    if (_state == ComponentState.Disposed)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    return _storageComponent;
                }
            }
        }

        /// <inheritdoc/>
        public IStorage Storage
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
        public IStorage PublicFactsStorage
        {
            get
            {
                lock (_stateLockObj)
                {
                    if (_state == ComponentState.Disposed)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    return _publicFactsStorage;
                }
            }
        }

        /// <inheritdoc/>
        public ConsolidatedPublicFactsStorage WorldPublicFactsStorage
        {
            get
            {
                lock (_stateLockObj)
                {
                    if (_state == ComponentState.Disposed)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    return _worldPublicFactsStorage;
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

                EngineContextHelper.LoadFromSourceCode(_context);

                if (!_additionalSourceCodePaths.IsNullOrEmpty())
                {
                    _context.LoaderFromSourceCode.LoadFromPaths(_additionalSourceCodePaths);
                }

                _storageComponent = _context.Storage;
                _storage = _storageComponent.GlobalStorage;
                _publicFactsStorage = _storageComponent.PublicFactsStorage;
                _worldPublicFactsStorage = _storageComponent.WorldPublicFactsStorage;

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
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <inheritdoc/>
        public string InsertPublicFact(string text)
        {
            return _storageComponent.InsertPublicFact(text);
        }

        /// <inheritdoc/>
        public string InsertPublicFact(RuleInstance fact)
        {
            return _storageComponent.InsertPublicFact(fact);
        }

        /// <inheritdoc/>
        public void RemovePublicFact(string id)
        {
            _storageComponent.RemovePublicFact(id);
        }

        /// <inheritdoc/>
        public void AddCategory(string category)
        {
            _storageComponent.AddCategory(category);
        }

        /// <inheritdoc/>
        public void AddCategories(List<string> categories)
        {
            _storageComponent.AddCategories(categories);
        }

        /// <inheritdoc/>
        public void RemoveCategory(string category)
        {
            _storageComponent.RemoveCategory(category);
        }

        /// <inheritdoc/>
        public void RemoveCategories(List<string> categories)
        {
            _storageComponent.RemoveCategories(categories);
        }

        /// <inheritdoc/>
        public bool EnableCategories { get => _storageComponent.EnableCategories; set => _storageComponent.EnableCategories = value; }
    }
}

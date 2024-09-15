/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public class StandaloneStorage: BaseComponent, IStandaloneStorage, IDirectStandaloneStorage, ISerializableEngine
    {
        public StandaloneStorage(StandaloneStorageSettings settings)
            : base(settings.MonitorNode)
        {
            _internalStandaloneStorage = new InternalStandaloneStorage(settings);
        }

        private InternalStandaloneStorage _internalStandaloneStorage;

        /// <inheritdoc/>
        public IMainStorageContext Context => _internalStandaloneStorage.Context;


        /// <inheritdoc/>
        public IStorageComponent StorageComponent
        {
            get
            {
                return _internalStandaloneStorage.StorageComponent;
            }
        }

        /// <inheritdoc/>
        public IStorage Storage
        {
            get
            {
                return _internalStandaloneStorage.Storage;
            }
        }

        /// <inheritdoc/>
        public IStorage PublicFactsStorage
        {
            get
            {
                return _internalStandaloneStorage.PublicFactsStorage;
            }
        }

        /// <inheritdoc/>
        public ConsolidatedPublicFactsStorage WorldPublicFactsStorage
        {
            get
            {
                return _internalStandaloneStorage.WorldPublicFactsStorage;
            }
        }

        /// <inheritdoc/>
        public void LoadFromSourceCode()
        {
            _internalStandaloneStorage.LoadFromSourceCode();
        }

        /// <inheritdoc/>
        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _internalStandaloneStorage.InsertPublicFact(logger, text);
        }

        /// <inheritdoc/>
        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _internalStandaloneStorage.InsertPublicFact(logger, fact);
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return _internalStandaloneStorage.RemovePublicFact(logger, id);
        }

        /// <inheritdoc/>
        public ISyncMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return _internalStandaloneStorage.AddCategory(logger, category);
        }

        /// <inheritdoc/>
        public ISyncMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            return _internalStandaloneStorage.AddCategories(logger, categories);
        }

        void IDirectStandaloneStorage.DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalStandaloneStorage.DirectAddCategories(logger, categories);
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return _internalStandaloneStorage.RemoveCategory(logger, category);
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            return _internalStandaloneStorage.RemoveCategories(logger, categories);
        }

        void IDirectStandaloneStorage.DirectRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalStandaloneStorage.DirectRemoveCategories(logger, categories);
        }

        /// <inheritdoc/>
        public bool EnableCategories { get => _internalStandaloneStorage.EnableCategories; set => _internalStandaloneStorage.EnableCategories = value; }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _internalStandaloneStorage.Dispose();

            base.OnDisposed();
        }
    }
}

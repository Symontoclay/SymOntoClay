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
        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, string text)
        {
            throw new NotSupportedException("BB4ECC8C-A536-48A5-8B9A-579804F54513");
        }

        /// <inheritdoc/>
        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _internalStandaloneStorage.InsertPublicFact(logger, text);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            throw new NotSupportedException("31339C37-BFBE-471D-A4A3-A236AF11C8DD");
        }

        /// <inheritdoc/>
        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _internalStandaloneStorage.InsertPublicFact(logger, fact);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePublicFact(IMonitorLogger logger, string id)
        {
            throw new NotSupportedException("A09FF186-D3E7-4263-829F-9EAD2E74A4F5");
        }

        /// <inheritdoc/>
        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return _internalStandaloneStorage.RemovePublicFact(logger, id);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategory(IMonitorLogger logger, string category)
        {
            throw new NotSupportedException("EE9B9270-71C7-46C2-ADC8-2B9BE3B3115D");
        }

        /// <inheritdoc/>
        public IMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return _internalStandaloneStorage.AddCategory(logger, category);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotSupportedException("86A196A2-49A9-481E-BD28-C6066F9999E0");
        }

        /// <inheritdoc/>
        public IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            return _internalStandaloneStorage.AddCategories(logger, categories);
        }

        void IDirectStandaloneStorage.DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalStandaloneStorage.DirectAddCategories(logger, categories);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategory(IMonitorLogger logger, string category)
        {
            throw new NotSupportedException("1CF43D34-ED37-4766-BE44-35DCCD8E873D");
        }

        /// <inheritdoc/>
        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return _internalStandaloneStorage.RemoveCategory(logger, category);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotSupportedException("C3A95594-0441-42FE-B1CD-4F34ED41886D");
        }

        /// <inheritdoc/>
        public IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            return _internalStandaloneStorage.RemoveCategories(logger, categories);
        }

        void IDirectStandaloneStorage.DirectRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalStandaloneStorage.DirectRemoveCategories(logger, categories);
        }

        /// <inheritdoc/>
        public bool EnableCategories { get => _internalStandaloneStorage.EnableCategories; set => _internalStandaloneStorage.EnableCategories = value; }
    }
}

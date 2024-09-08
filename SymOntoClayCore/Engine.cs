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
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public class Engine : BaseComponent, ISerializableEngine, IDirectEngine
    {
        public Engine(EngineSettings settings)
            : base(settings.MonitorNode)
        {
            _internalEngine = new InternalEngine(settings);
        }

        private InternalEngine _internalEngine;

        /// <summary>
        /// Gets engine context. Only for debugging and testing!
        /// </summary>
        public IEngineContext EngineContext => _internalEngine.EngineContext;

        /// <inheritdoc/>
        public void LoadFromSourceCode()
        {
            _internalEngine.LoadFromSourceCode();
        }

        /// <inheritdoc/>
        public void BeginStarting()
        {
            _internalEngine.BeginStarting();
        }

        /// <inheritdoc/>
        public bool IsWaited
        {
            get
            {
                return _internalEngine.IsWaited;
            }
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.InsertPublicFact(logger, text);
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _internalEngine.InsertPublicFact(logger, factName, text);
        }

        string IDirectEngine.DirectInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _internalEngine.DirectInsertPublicFact(logger, factName, text);
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _internalEngine.InsertPublicFact(logger, fact);
        }

        string IDirectEngine.DirectInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _internalEngine.DirectInsertPublicFact(logger, fact);
        }

        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return _internalEngine.RemovePublicFact(logger, id);
        }

        void IDirectEngine.DirectRemovePublicFact(IMonitorLogger logger, string id)
        {
            _internalEngine.DirectRemovePublicFact(logger, id);
        }

        public IMethodResponse<string> InsertFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.InsertFact(logger, text);
        }

        public IMethodResponse<string> InsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _internalEngine.InsertFact(logger, factName, text);
        }

        string IDirectEngine.DirectInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _internalEngine.DirectInsertFact(logger, factName, text);
        }

        public IMethodResponse RemoveFact(IMonitorLogger logger, string id)
        {
            return _internalEngine.RemoveFact(logger, id);
        }

        void IDirectEngine.DirectRemoveFact(IMonitorLogger logger, string id)
        {
            _internalEngine.DirectRemoveFact(logger, id);
        }

        public IStorage PublicFactsStorage => _internalEngine.PublicFactsStorage;

        public IMethodResponse AddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            return _internalEngine.AddVisibleStorage(logger, storage);
        }

        void IDirectEngine.DirectAddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _internalEngine.DirectAddVisibleStorage(logger, storage);
        }

        public IMethodResponse RemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            return _internalEngine.RemoveVisibleStorage(logger, storage);
        }

        void IDirectEngine.DirectRemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _internalEngine.DirectRemoveVisibleStorage(logger, storage);
        }

        public IMethodResponse<string> InsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.InsertPerceptedFact(logger, text);
        }

        string IDirectEngine.DirectInsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.DirectInsertPerceptedFact(logger, text);
        }

        public IMethodResponse RemovePerceptedFact(IMonitorLogger logger, string id)
        {
            return _internalEngine.RemovePerceptedFact(logger, id);
        }

        void IDirectEngine.DirectRemovePerceptedFact(IMonitorLogger logger, string id)
        {
            _internalEngine.DirectRemovePerceptedFact(logger, id);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldInsertListenedFact(IMonitorLogger logger, string text)
        {
            throw new NotSupportedException("8641EFFD-60C0-4E88-869D-E0A568D334B5");
        }

        public IMethodResponse InsertListenedFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.InsertListenedFact(logger, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldInsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            throw new NotSupportedException("02EA437B-878B-42B8-93EC-43A89B967021");
        }

        public IMethodResponse InsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _internalEngine.InsertListenedFact(logger, fact);
        }

        public IMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return _internalEngine.AddCategory(logger, category);
        }

        public IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            return _internalEngine.AddCategories(logger, categories);
        }

        void IDirectEngine.DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalEngine.DirectAddCategories(logger, categories);
        }

        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return _internalEngine.RemoveCategory(logger, category);
        }

        public IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            return _internalEngine.RemoveCategories(logger, categories);
        }

        void IDirectEngine.DirectRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalEngine.DirectRemoveCategories(logger, categories);
        }

        public bool EnableCategories { get => _internalEngine.EnableCategories; set => _internalEngine.EnableCategories = value; }

        [Obsolete("Serialization Refactoring", true)]
        public void OldDie()
        {
            throw new NotSupportedException("C3E95372-D2A5-4B14-8512-38308EEA61E0");
        }

        public IMethodResponse Die()
        {
            return _internalEngine.Die();
        }

        void IDirectEngine.DirectDie()
        {
            _internalEngine.DirectDie();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _internalEngine.Dispose();

            base.OnDisposed();
        }
    }
}

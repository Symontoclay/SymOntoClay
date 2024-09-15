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

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.InsertPublicFact(logger, text);
        }

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _internalEngine.InsertPublicFact(logger, factName, text);
        }

        string IDirectEngine.DirectInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _internalEngine.DirectInsertPublicFact(logger, factName, text);
        }

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _internalEngine.InsertPublicFact(logger, fact);
        }

        string IDirectEngine.DirectInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _internalEngine.DirectInsertPublicFact(logger, fact);
        }

        public ISyncMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return _internalEngine.RemovePublicFact(logger, id);
        }

        void IDirectEngine.DirectRemovePublicFact(IMonitorLogger logger, string id)
        {
            _internalEngine.DirectRemovePublicFact(logger, id);
        }

        public ISyncMethodResponse<string> InsertFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.InsertFact(logger, text);
        }

        public ISyncMethodResponse<string> InsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _internalEngine.InsertFact(logger, factName, text);
        }

        string IDirectEngine.DirectInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _internalEngine.DirectInsertFact(logger, factName, text);
        }

        public ISyncMethodResponse RemoveFact(IMonitorLogger logger, string id)
        {
            return _internalEngine.RemoveFact(logger, id);
        }

        void IDirectEngine.DirectRemoveFact(IMonitorLogger logger, string id)
        {
            _internalEngine.DirectRemoveFact(logger, id);
        }

        public IStorage PublicFactsStorage => _internalEngine.PublicFactsStorage;

        public ISyncMethodResponse AddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            return _internalEngine.AddVisibleStorage(logger, storage);
        }

        void IDirectEngine.DirectAddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _internalEngine.DirectAddVisibleStorage(logger, storage);
        }

        public ISyncMethodResponse RemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            return _internalEngine.RemoveVisibleStorage(logger, storage);
        }

        void IDirectEngine.DirectRemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _internalEngine.DirectRemoveVisibleStorage(logger, storage);
        }

        public ISyncMethodResponse<string> InsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.InsertPerceptedFact(logger, text);
        }

        string IDirectEngine.DirectInsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.DirectInsertPerceptedFact(logger, text);
        }

        public ISyncMethodResponse RemovePerceptedFact(IMonitorLogger logger, string id)
        {
            return _internalEngine.RemovePerceptedFact(logger, id);
        }

        void IDirectEngine.DirectRemovePerceptedFact(IMonitorLogger logger, string id)
        {
            _internalEngine.DirectRemovePerceptedFact(logger, id);
        }

        public ISyncMethodResponse InsertListenedFact(IMonitorLogger logger, string text)
        {
            return _internalEngine.InsertListenedFact(logger, text);
        }

        public ISyncMethodResponse InsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _internalEngine.InsertListenedFact(logger, fact);
        }

        public ISyncMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return _internalEngine.AddCategory(logger, category);
        }

        public ISyncMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            return _internalEngine.AddCategories(logger, categories);
        }

        void IDirectEngine.DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalEngine.DirectAddCategories(logger, categories);
        }

        public ISyncMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return _internalEngine.RemoveCategory(logger, category);
        }

        public ISyncMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            return _internalEngine.RemoveCategories(logger, categories);
        }

        void IDirectEngine.DirectRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalEngine.DirectRemoveCategories(logger, categories);
        }

        public bool EnableCategories { get => _internalEngine.EnableCategories; set => _internalEngine.EnableCategories = value; }

        public ISyncMethodResponse Die()
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

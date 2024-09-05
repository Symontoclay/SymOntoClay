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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core
{
    public class StandaloneStorage: BaseComponent, IStandaloneStorage, ISerializableEngine
    {
        public StandaloneStorage(StandaloneStorageSettings settings)
            : base(settings.MonitorNode)
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

        private List<(StrongIdentifierValue, string)> _deferredPublicFactsTexts = new List<(StrongIdentifierValue, string)>();
        private List<RuleInstance> _deferredPublicFactsInstances = new List<RuleInstance>();
        private List<string> _defferedRemovedPublicFacts = new List<string>();
        private List<string> _deferredAddedCategories = new List<string>();
        private List<string> _deferredRemovedCategories = new List<string>();

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

                if (_deferredPublicFactsTexts.Any())
                {
                    foreach (var item in _deferredPublicFactsTexts)
                    {
                        _storageComponent.OldInsertPublicFact(Logger, item.Item1, item.Item2);
                    }

                    _deferredPublicFactsTexts.Clear();
                    _deferredPublicFactsTexts = null;
                }
                else
                {
                    _deferredPublicFactsTexts = null;
                }

                if (_deferredPublicFactsInstances.Any())
                {
                    foreach(var fact in _deferredPublicFactsInstances)
                    {
                        _storageComponent.InsertPublicFact(Logger, fact);
                    }
                    _deferredPublicFactsInstances.Clear();
                    _deferredPublicFactsInstances = null;
                }
                else
                {
                    _deferredPublicFactsInstances = null;
                }

                if (_defferedRemovedPublicFacts.Any())
                {
                    foreach (var item in _defferedRemovedPublicFacts)
                    {
                        _storageComponent.RemovePublicFact(Logger, item);
                    }

                    _defferedRemovedPublicFacts.Clear();
                    _defferedRemovedPublicFacts = null;
                }
                else
                {
                    _defferedRemovedPublicFacts = null;
                }

                if (_deferredAddedCategories.Any())
                {
                    _storageComponent.AddCategories(Logger, _deferredAddedCategories);

                    _deferredAddedCategories.Clear();
                    _deferredAddedCategories = null;
                }
                else
                {
                    _deferredAddedCategories = null;
                }

                if (_deferredRemovedCategories.Any())
                {
                    _storageComponent.RemoveCategories(Logger, _deferredRemovedCategories);

                    _deferredRemovedCategories.Clear();
                    _deferredRemovedCategories = null;
                }
                else
                {
                    _deferredRemovedCategories = null;
                }

                _state = ComponentState.Loaded;
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
                throw new NotImplementedException("A55EFC61-5254-4207-84F8-BD27EEEBFEED");
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
                throw new NotImplementedException("F4A132AB-8E1A-4766-9104-305B9FE1DBAF");
#endif
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, string text)
        {
            lock (_stateLockObj)
            {
                if(_storageComponent == null)
                {
                    var factName = NameHelper.CreateRuleOrFactName();
                    _deferredPublicFactsTexts.Add((factName, text));
                    return factName.NameValue;
                }

                return _storageComponent.InsertPublicFact(logger, text);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    if(fact.Name == null)
                    {
                        fact.Name = NameHelper.CreateRuleOrFactName();
                    }

                    _deferredPublicFactsInstances.Add(fact);
                    return fact.Name.NameValue;
                }

                return _storageComponent.InsertPublicFact(logger, fact);
            }                
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePublicFact(IMonitorLogger logger, string id)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _defferedRemovedPublicFacts.Add(id);
                    return;
                }

                _storageComponent.RemovePublicFact(logger, id);
            }            
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategory(IMonitorLogger logger, string category)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredAddedCategories.Add(category);
                    return;
                }

                _storageComponent.AddCategory(logger, category);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredAddedCategories.AddRange(categories);
                    return;
                }

                _storageComponent.AddCategories(logger, categories);
            }            
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategory(IMonitorLogger logger, string category)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredRemovedCategories.Add(category);
                    return;
                }

                _storageComponent.RemoveCategory(logger, category);
            }            
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredRemovedCategories.AddRange(categories);
                    return;
                }

                _storageComponent.RemoveCategories(logger, categories);
            }            
        }

        /// <inheritdoc/>
        public bool EnableCategories { get => _storageComponent.EnableCategories; set => _storageComponent.EnableCategories = value; }
    }
}

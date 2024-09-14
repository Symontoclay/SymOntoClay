using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal
{
    public class InternalStandaloneStorage : BaseComponent
    {
        public InternalStandaloneStorage(StandaloneStorageSettings settings)
            : base(settings.MonitorNode)
        {
            _additionalSourceCodePaths = settings.AdditionalSourceCodePaths;

            _context = EngineContextHelper.CreateAndInitMainStorageContext(settings);
        }

        private MainStorageContext _context;

        public IMainStorageContext Context => _context;

        private IList<string> _additionalSourceCodePaths;

        private IStorageComponent _storageComponent;
        private IStorage _storage;
        private IStorage _publicFactsStorage;
        private ConsolidatedPublicFactsStorage _worldPublicFactsStorage;

        private List<(StrongIdentifierValue, string)> _deferredPublicFactsTexts = new List<(StrongIdentifierValue, string)>();
        private List<RuleInstance> _deferredPublicFactsInstances = new List<RuleInstance>();
        private List<string> _defferedRemovedPublicFacts = new List<string>();
        private List<string> _deferredAddedCategories = new List<string>();
        private List<string> _deferredRemovedCategories = new List<string>();

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
                        _storageComponent.InsertPublicFact(Logger, item.Item1, item.Item2);
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
                    foreach (var fact in _deferredPublicFactsInstances)
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

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, string text)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    var factName = NameHelper.CreateRuleOrFactName();
                    _deferredPublicFactsTexts.Add((factName, text));
                    return factName.NameValue;
                }

                return _storageComponent.InsertPublicFact(logger, text);
            }
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {

        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    if (fact.Name == null)
                    {
                        fact.Name = NameHelper.CreateRuleOrFactName();
                    }

                    _deferredPublicFactsInstances.Add(fact);
                    return fact.Name.NameValue;
                }

                return _storageComponent.InsertPublicFact(logger, fact);
            }
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {

        }

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

        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {

        }

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

        public IMethodResponse AddCategory(IMonitorLogger logger, string category)
        {

        }

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

        public IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {

        }

        public void DirectAddCategories(IMonitorLogger logger, List<string> categories);

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

        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {

        }

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

        public IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {

        }

        public void DirectRemoveCategories(IMonitorLogger logger, List<string> categories);

        public bool EnableCategories { get => _storageComponent.EnableCategories; set => _storageComponent.EnableCategories = value; }
    }
}

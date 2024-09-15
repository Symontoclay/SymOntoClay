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

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    var factName = NameHelper.CreateRuleOrFactName();
                    _deferredPublicFactsTexts.Add((factName, text));
                    return new CompletedMethodResponse<string>(factName.NameValue);
                }

                return NInsertPublicFact(logger, text);
            }
        }

        public string NInsertPublicFact(IMonitorLogger logger, string text)
        {
            return _storageComponent.InsertPublicFact(logger, text);
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
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
                    return new CompletedMethodResponse<string>(fact.Name.NameValue);
                }

                return NInsertPublicFact(logger, fact);
            }
        }

        public string NInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _storageComponent.InsertPublicFact(logger, fact);
        }

        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _defferedRemovedPublicFacts.Add(id);
                    return CompletedMethodResponse.Instance;
                }

                NRemovePublicFact(logger, id);
            }
        }

        public void NRemovePublicFact(IMonitorLogger logger, string id)
        {
            _storageComponent.RemovePublicFact(logger, id);
        }

        public IMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredAddedCategories.Add(category);
                    return CompletedMethodResponse.Instance;
                }

                NAddCategory(logger, category);
            }
        }

        public void NAddCategory(IMonitorLogger logger, string category)
        {
            _storageComponent.AddCategory(logger, category);
        }

        public IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredAddedCategories.AddRange(categories);
                    return CompletedMethodResponse.Instance;
                }

                NAddCategories(logger, categories);
            }
        }

        public void DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredAddedCategories.AddRange(categories);
                    return;
                }

                NAddCategories(logger, categories);
            }
        }

        public void NAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _storageComponent.AddCategories(logger, categories);
        }

        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredRemovedCategories.Add(category);
                    return CompletedMethodResponse.Instance;
                }

                NRemoveCategory(logger, category);
            }
        }

        public void NRemoveCategory(IMonitorLogger logger, string category)
        {
            _storageComponent.RemoveCategory(logger, category);
        }

        public IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredRemovedCategories.AddRange(categories);
                    return CompletedMethodResponse.Instance;
                }

                NRemoveCategories(logger, categories);
            }
        }

        public void DirectRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredRemovedCategories.AddRange(categories);
                    return;
                }

                NRemoveCategories(logger, categories);
            }
        }

        public void NRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _storageComponent.RemoveCategories(logger, categories);
        }

        public bool EnableCategories { get => _storageComponent.EnableCategories; set => _storageComponent.EnableCategories = value; }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _context.Dispose();

            base.OnDisposed();
        }
    }
}

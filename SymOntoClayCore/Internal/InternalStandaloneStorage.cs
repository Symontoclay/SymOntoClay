using Newtonsoft.Json.Linq;
using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
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
            _activeObjectContext = _context.ActiveObjectContext;
            _serializationAnchor = new SerializationAnchor();
        }

        private MainStorageContext _context;
        private readonly IActiveObjectContext _activeObjectContext;
        private SerializationAnchor _serializationAnchor;

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

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    var factName = NameHelper.CreateRuleOrFactName();
                    _deferredPublicFactsTexts.Add((factName, text));
                    return new CompletedSyncMethodResponse<string>(factName.NameValue);
                }

                return LoggedSyncFunctorWithResult<InternalStandaloneStorage, string, string>.Run(logger, "9A943BBE-0E7A-49FB-B59B-18257B4B0BEE", this, text,
                    string(IMonitorLogger loggerValue, InternalStandaloneStorage instanceValue, string textValue) => { 
                        return instanceValue.NInsertPublicFact(loggerValue, textValue);
                    },
                    _activeObjectContext, _serializationAnchor).ToMethodResponse();
            }
        }

        public string NInsertPublicFact(IMonitorLogger logger, string text)
        {
            return _storageComponent.InsertPublicFact(logger, text);
        }

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
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
                    return new CompletedSyncMethodResponse<string>(fact.Name.NameValue);
                }

                return LoggedSyncFunctorWithResult<InternalStandaloneStorage, RuleInstance, string>.Run(logger, "F6F736D8-8365-4D55-930A-51CFCED6985B", this, fact,
                    string(IMonitorLogger loggerValue, InternalStandaloneStorage instanceValue, RuleInstance factValue) => {
                        return instanceValue.NInsertPublicFact(loggerValue, factValue);
                    },
                    _activeObjectContext, _serializationAnchor).ToMethodResponse();
            }
        }

        public string NInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _storageComponent.InsertPublicFact(logger, fact);
        }

        public ISyncMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _defferedRemovedPublicFacts.Add(id);
                    return CompletedSyncMethodResponse.Instance;
                }

                return LoggedSyncFunctorWithoutResult<InternalStandaloneStorage, string>.Run(logger, "E906C9AD-3469-4956-80D9-FB7A5F0D07D8", this, id,
                    (IMonitorLogger loggerValue, InternalStandaloneStorage instanceValue, string idValue) => {
                        instanceValue.NRemovePublicFact(loggerValue, idValue);
                    },
                    _activeObjectContext, _serializationAnchor).ToMethodResponse();
            }
        }

        public void NRemovePublicFact(IMonitorLogger logger, string id)
        {
            _storageComponent.RemovePublicFact(logger, id);
        }

        public ISyncMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredAddedCategories.Add(category);
                    return CompletedSyncMethodResponse.Instance;
                }

                return LoggedSyncFunctorWithoutResult<InternalStandaloneStorage, string>.Run(logger, "98D58843-1D5F-4662-999B-8AB34225B7FF", this, category,
                    (IMonitorLogger loggerValue, InternalStandaloneStorage instanceValue, string categoryValue) => {
                        instanceValue.NAddCategory(loggerValue, categoryValue);
                    },
                    _activeObjectContext, _serializationAnchor).ToMethodResponse();
            }
        }

        public void NAddCategory(IMonitorLogger logger, string category)
        {
            _storageComponent.AddCategory(logger, category);
        }

        public ISyncMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredAddedCategories.AddRange(categories);
                    return CompletedSyncMethodResponse.Instance;
                }

                return LoggedSyncFunctorWithoutResult<InternalStandaloneStorage, List<string>>.Run(logger, "82EC25C9-7430-467D-9C45-D1669632C719", this, categories,
                    (IMonitorLogger loggerValue, InternalStandaloneStorage instanceValue, List<string> categoriesValue) => {
                        instanceValue.NAddCategories(loggerValue, categoriesValue);
                    },
                    _activeObjectContext, _serializationAnchor).ToMethodResponse();
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

        public ISyncMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredRemovedCategories.Add(category);
                    return CompletedSyncMethodResponse.Instance;
                }

                return LoggedSyncFunctorWithoutResult<InternalStandaloneStorage, string>.Run(logger, "ED81F452-0401-42E9-BBAB-FABF9EBABF28", this, category,
                    (IMonitorLogger loggerValue, InternalStandaloneStorage instanceValue, string categoryValue) => {
                        instanceValue.NRemoveCategory(loggerValue, categoryValue);
                    },
                    _activeObjectContext, _serializationAnchor).ToMethodResponse();
            }
        }

        public void NRemoveCategory(IMonitorLogger logger, string category)
        {
            _storageComponent.RemoveCategory(logger, category);
        }

        public ISyncMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_stateLockObj)
            {
                if (_storageComponent == null)
                {
                    _deferredRemovedCategories.AddRange(categories);
                    return CompletedSyncMethodResponse.Instance;
                }

                return LoggedSyncFunctorWithoutResult<InternalStandaloneStorage, List<string>>.Run(logger, "00C6AD56-50FB-4D9C-899F-2F641E6829BF", this, categories,
                    (IMonitorLogger loggerValue, InternalStandaloneStorage instanceValue, List<string> categoriesValue) => {
                        instanceValue.NRemoveCategories(loggerValue, categoriesValue);
                    },
                    _activeObjectContext, _serializationAnchor).ToMethodResponse();
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
            _serializationAnchor.Dispose();

            _context.Dispose();

            base.OnDisposed();
        }
    }
}

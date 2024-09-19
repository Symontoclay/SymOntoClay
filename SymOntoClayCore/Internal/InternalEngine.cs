using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal
{
    public class InternalEngine : BaseComponent
    {
        public InternalEngine(EngineSettings settings)
            : base(settings.MonitorNode)
        {
            _context = EngineContextHelper.CreateAndInitContext(settings);
            _activeObjectContext = _context.ActiveObjectContext;
            _serializationAnchor = new SerializationAnchor();
        }

        private EngineContext _context;
        private IActiveObjectContext _activeObjectContext;
        private SerializationAnchor _serializationAnchor;

        public IEngineContext EngineContext => _context;

        public void LoadFromSourceCode()
        {
            lock (_stateLockObj)
            {
                switch (_state)
                {
                    case ComponentState.Loaded:
                    case ComponentState.Stopped:
                        throw new NotImplementedException("DCC81752-0D31-41D3-AEA8-7BC59C178460");

                    case ComponentState.Started:
                        throw new NotImplementedException("6A15197F-4CDB-4FAC-B50D-FB0C650EFAE6");

                    case ComponentState.Disposed:
                        throw new ObjectDisposedException(null);
                }

                EngineContextHelper.LoadFromSourceCode(_context);

                _state = ComponentState.Loaded;
            }
        }

        public void BeginStarting()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

#if IMAGINE_WORKING
#else
                throw new NotImplementedException("F8B0FEF1-C39D-469C-A8CA-BA94CD1C3EF0");
#endif
            }
        }

        public bool IsWaited
        {
            get
            {
                lock (_stateLockObj)
                {
                    if (_state == ComponentState.Disposed)
                    {
                        throw new ObjectDisposedException(null);
                    }

#if IMAGINE_WORKING

                    return true;
#else
                    throw new NotImplementedException("BF0906EC-518F-48E4-9000-5AFD17E65BCA");
#endif
                }
            }
        }

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return LoggedSyncFunctorWithResult<InternalEngine, string, string>.Run(logger, "5B25A0FF-9EB3-490B-895D-3617CFD9DD1B", this, text,
                string (IMonitorLogger loggerValue, InternalEngine instanceValue, string textValue) => { 
                    return instanceValue.NInsertPublicFact(loggerValue, textValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public string NInsertPublicFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertPublicFact(logger, text);
        }

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return LoggedSyncFunctorWithResult<InternalEngine, StrongIdentifierValue, string, string>.Run(logger, "99B30FB7-7425-46CA-9320-70DF92727FD0", this, factName, text,
                string (IMonitorLogger loggerValue, InternalEngine instanceValue, StrongIdentifierValue factNameValue, string textValue) => {
                    return instanceValue.NInsertPublicFact(loggerValue, factNameValue, textValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public string DirectInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return NInsertPublicFact(logger, factName, text);
        }

        public string NInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _context.Storage.InsertPublicFact(logger, factName, text);
        }

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return LoggedSyncFunctorWithResult<InternalEngine, RuleInstance, string>.Run(logger, "6DF4CC30-42D7-4955-A866-67C7EB132F80", this, fact,
                string (IMonitorLogger loggerValue, InternalEngine instanceValue, RuleInstance factValue) => {
                    return instanceValue.NInsertPublicFact(loggerValue, factValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public string DirectInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return NInsertPublicFact(logger, fact);
        }

        public string NInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _context.Storage.InsertPublicFact(logger, fact);
        }

        public ISyncMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, string>.Run(logger, "7D2C2F53-F0C0-481F-AF43-2510EF145784", this, id,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, string idValue) => {
                    instanceValue.NRemovePublicFact(loggerValue, idValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void DirectRemovePublicFact(IMonitorLogger logger, string id)
        {
            NRemovePublicFact(logger, id);
        }

        public void NRemovePublicFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemovePublicFact(logger, id);
        }

        public ISyncMethodResponse<string> InsertFact(IMonitorLogger logger, string text)
        {
            return LoggedSyncFunctorWithResult<InternalEngine, string, string>.Run(logger, "F27B76F3-DF59-45B1-9B4A-D64A0C544588", this, text,
                string (IMonitorLogger loggerValue, InternalEngine instanceValue, string textValue) => {
                    return instanceValue.NInsertFact(loggerValue, textValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public string NInsertFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertFact(logger, text);
        }

        public ISyncMethodResponse<string> InsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return LoggedSyncFunctorWithResult<InternalEngine, StrongIdentifierValue, string, string>.Run(logger, "6415CB6F-6697-4BA1-B661-C986397F5450", this, factName, text,
                string(IMonitorLogger loggerValue, InternalEngine instanceValue, StrongIdentifierValue factNameValue, string textValue) => {
                    return instanceValue.NInsertFact(loggerValue, factNameValue, textValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public string DirectInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return NInsertFact(logger, factName, text);
        }

        public string NInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _context.Storage.InsertFact(logger, factName, text);
        }

        public ISyncMethodResponse RemoveFact(IMonitorLogger logger, string id)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, string>.Run(logger, "2C287D0D-F572-4525-8262-D9D614CA0E47", this, id,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, string idValue) => {
                    instanceValue.NRemoveFact(loggerValue, idValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void DirectRemoveFact(IMonitorLogger logger, string id)
        {
            NRemoveFact(logger, id);
        }

        public void NRemoveFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemoveFact(logger, id);
        }

        public IStorage PublicFactsStorage => _context.Storage.PublicFactsStorage;

        public ISyncMethodResponse AddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, IStorage>.Run(logger, "A8A23D7C-1A13-4126-9369-346927C6BDEE", this, storage,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, IStorage storageValue) => {
                    instanceValue.NAddVisibleStorage(loggerValue, storageValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void DirectAddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            NAddVisibleStorage(logger, storage);
        }

        public void NAddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _context.Storage.AddVisibleStorage(logger, storage);
        }

        public ISyncMethodResponse RemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, IStorage>.Run(logger, "A403D8B7-2390-4ACE-A41C-43848013200B", this, storage,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, IStorage storageValue) => {
                    instanceValue.NRemoveVisibleStorage(loggerValue, storageValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void DirectRemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            NRemoveVisibleStorage(logger, storage);
        }

        public void NRemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _context.Storage.RemoveVisibleStorage(logger, storage);
        }

        public ISyncMethodResponse<string> InsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return LoggedSyncFunctorWithResult<InternalEngine, string, string>.Run(logger, "CEDE7288-D779-4F86-B334-C6D84E82FD16", this, text,
                string(IMonitorLogger loggerValue, InternalEngine instanceValue, string textValue) => {
                    return instanceValue.NInsertPerceptedFact(loggerValue, textValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public string DirectInsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return NInsertPerceptedFact(logger, text);
        }

        public string NInsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertPerceptedFact(logger, text);
        }

        public ISyncMethodResponse RemovePerceptedFact(IMonitorLogger logger, string id)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, string>.Run(logger, "6CDD5CF7-4870-4C62-9DC6-A620F352B439", this, id,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, string idValue) => {
                    instanceValue.NRemovePerceptedFact(loggerValue, idValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void DirectRemovePerceptedFact(IMonitorLogger logger, string id)
        {
            NRemovePerceptedFact(logger, id);
        }

        public void NRemovePerceptedFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemovePerceptedFact(logger, id);
        }

        public ISyncMethodResponse InsertListenedFact(IMonitorLogger logger, string text)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, string>.Run(logger, "2B86544E-1FFB-4895-91DB-0185E717FCC1", this, text,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, string textValue) => {
                    instanceValue.NInsertListenedFact(loggerValue, textValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NInsertListenedFact(IMonitorLogger logger, string text)
        {
            _context.Storage.InsertListenedFact(logger, text);
        }

        public ISyncMethodResponse InsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, RuleInstance>.Run(logger, "C07E04DF-6455-406A-BC74-3F5236F380BF", this, fact,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, RuleInstance factValue) => {
                    instanceValue.NInsertListenedFact(loggerValue, factValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NInsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            _context.Storage.InsertListenedFact(logger, fact);
        }

        public ISyncMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, string>.Run(logger, "FF6E8585-CA95-440E-8824-0FEEC13286A6", this, category,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, string categoryValue) => {
                    instanceValue.NAddCategory(loggerValue, categoryValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NAddCategory(IMonitorLogger logger, string category)
        {
            _context.Storage.AddCategory(logger, category);
        }

        public ISyncMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, List<string>>.Run(logger, "7A2E1229-34FD-4D8A-937C-5490815B588D", this, categories,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, List<string> categoriesValue) => {
                    instanceValue.NAddCategories(loggerValue, categoriesValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            NAddCategories(logger, categories);
        }

        public void NAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _context.Storage.AddCategories(logger, categories);
        }

        public ISyncMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, string>.Run(logger, "EAA752D6-E54D-45F9-A179-DB4A881C273E", this, category,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, string categoryValue) => {
                    instanceValue.NRemoveCategory(loggerValue, categoryValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NRemoveCategory(IMonitorLogger logger, string category)
        {
            _context.Storage.RemoveCategory(logger, category);
        }

        public ISyncMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine, List<string>>.Run(logger, "E502A605-89C3-4FCB-BC7B-393CF1478A79", this, categories,
                (IMonitorLogger loggerValue, InternalEngine instanceValue, List<string> categoriesValue) => {
                    instanceValue.NRemoveCategories(loggerValue, categoriesValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void DirectRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            NRemoveCategories(logger, categories);
        }

        public void NRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _context.Storage.RemoveCategories(logger, categories);
        }

        public bool EnableCategories { get => _context.Storage.EnableCategories; set => _context.Storage.EnableCategories = value; }

        public ISyncMethodResponse Die()
        {
            return LoggedSyncFunctorWithoutResult<InternalEngine>.Run(Logger, "56B53133-0391-44D3-B2E7-AA1A4FF40547", this,
                (IMonitorLogger loggerValue, InternalEngine instanceValue) => {
                    instanceValue.NDie();
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void DirectDie()
        {
            NDie();
        }

        public void NDie()
        {
            _context.Die();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _serializationAnchor.Dispose();

            _context.Dispose();

            base.OnDisposed();
        }
    }
}

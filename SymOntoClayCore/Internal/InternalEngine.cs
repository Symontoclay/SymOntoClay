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

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
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

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
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

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
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

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePublicFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemovePublicFact(logger, id);
        }

        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id);
        public void DirectRemovePublicFact(IMonitorLogger logger, string id);

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertFact(logger, text);
        }

        public IMethodResponse<string> InsertFact(IMonitorLogger logger, string text);

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _context.Storage.InsertFact(logger, factName, text);
        }

        public IMethodResponse<string> InsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text);
        public string DirectInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text);

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemoveFact(logger, id);
        }

        public IMethodResponse RemoveFact(IMonitorLogger logger, string id);
        public void DirectRemoveFact(IMonitorLogger logger, string id);

        public IStorage PublicFactsStorage => _context.Storage.PublicFactsStorage;

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _context.Storage.AddVisibleStorage(logger, storage);
        }

        public IMethodResponse AddVisibleStorage(IMonitorLogger logger, IStorage storage);
        public void DirectAddVisibleStorage(IMonitorLogger logger, IStorage storage);

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _context.Storage.RemoveVisibleStorage(logger, storage);
        }

        public IMethodResponse RemoveVisibleStorage(IMonitorLogger logger, IStorage storage);
        public void DirectRemoveVisibleStorage(IMonitorLogger logger, IStorage storage);

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertPerceptedFact(logger, text);
        }

        public IMethodResponse<string> InsertPerceptedFact(IMonitorLogger logger, string text);
        public string DirectInsertPerceptedFact(IMonitorLogger logger, string text);

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePerceptedFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemovePerceptedFact(logger, id);
        }

        public IMethodResponse RemovePerceptedFact(IMonitorLogger logger, string id);
        public void DirectRemovePerceptedFact(IMonitorLogger logger, string id);

        [Obsolete("Serialization Refactoring", true)]
        public void OldInsertListenedFact(IMonitorLogger logger, string text)
        {
            _context.Storage.InsertListenedFact(logger, text);
        }

        public IMethodResponse InsertListenedFact(IMonitorLogger logger, string text);

        [Obsolete("Serialization Refactoring", true)]
        public void OldInsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            _context.Storage.InsertListenedFact(logger, fact);
        }

        public IMethodResponse InsertListenedFact(IMonitorLogger logger, RuleInstance fact);

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategory(IMonitorLogger logger, string category)
        {
            _context.Storage.AddCategory(logger, category);
        }

        public IMethodResponse AddCategory(IMonitorLogger logger, string category);

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _context.Storage.AddCategories(logger, categories);
        }

        public IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories);
        public void DirectAddCategories(IMonitorLogger logger, List<string> categories);

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategory(IMonitorLogger logger, string category)
        {
            _context.Storage.RemoveCategory(logger, category);
        }

        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category);

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _context.Storage.RemoveCategories(logger, categories);
        }

        public IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories);
        public void DirectRemoveCategories(IMonitorLogger logger, List<string> categories);

        public bool EnableCategories { get => _context.Storage.EnableCategories; set => _context.Storage.EnableCategories = value; }

        [Obsolete("Serialization Refactoring", true)]
        public void OldDie()
        {
            _context.Die();
        }

        public IMethodResponse Die();
        public void DirectDie();

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _context.Dispose();

            base.OnDisposed();
        }
    }
}

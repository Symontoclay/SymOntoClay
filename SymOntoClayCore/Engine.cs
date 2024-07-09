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

using Newtonsoft.Json.Linq;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Serialization.Functors;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace SymOntoClay.Core
{
    public class Engine : BaseComponent, ISerializableEngine
    {
        public Engine(EngineSettings settings)
            : base(settings.MonitorNode)
        {
            _context = EngineContextHelper.CreateAndInitContext(settings);
            _activeObjectContext = _context.ActiveObjectContext;
            _threadPool = _context.AsyncEventsThreadPool;
        }

        private readonly EngineContext _context;
        private readonly IActiveObjectContext _activeObjectContext;
        private readonly ICustomThreadPool _threadPool;

        /// <summary>
        /// Gets engine context. Onkly for debugging and testing!
        /// </summary>
        public IEngineContext EngineContext => _context;

        /// <inheritdoc/>
        public void LoadFromSourceCode()
        {
            lock (_stateLockObj)
            {
                switch(_state)
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
                throw new NotImplementedException("78748188-426E-437B-B569-B7991C41081E");
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
                throw new NotImplementedException("CB081D88-0E42-4C37-BD2D-746CDD6317A8");
#endif
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
            return _context.Storage.InsertPublicFact(logger, text);
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _context.Storage.InsertPublicFact(logger, factName, text);
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _context.Storage.InsertPublicFact(logger, fact);
        }

        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return LoggedFunctorWithoutResult<string>.Run(logger, id, (loggerValue, idValue) => {
                _context.Storage.RemovePublicFact(loggerValue, idValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IMethodResponse<string> InsertFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertFact(logger, text);
        }

        public IMethodResponse<string> InsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _context.Storage.InsertFact(logger, factName, text);
        }

        public IMethodResponse RemoveFact(IMonitorLogger logger, string id)
        {
            return LoggedFunctorWithoutResult<string>.Run(logger, id, (loggerValue, idValue) => {
                _context.Storage.RemoveFact(loggerValue, idValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IStorage PublicFactsStorage => _context.Storage.PublicFactsStorage;

        public IMethodResponse AddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            return LoggedFunctorWithoutResult<IStorage>.Run(logger, storage, (loggerValue, storageValue) => {
                _context.Storage.AddVisibleStorage(loggerValue, storageValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IMethodResponse RemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            return LoggedFunctorWithoutResult<IStorage>.Run(logger, storage, (loggerValue, storageValue) => {
                _context.Storage.RemoveVisibleStorage(loggerValue, storageValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IMethodResponse<string> InsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertPerceptedFact(logger, text);
        }

        public IMethodResponse RemovePerceptedFact(IMonitorLogger logger, string id)
        {
            return LoggedFunctorWithoutResult<string>.Run(logger, id, (loggerValue, idValue) =>
            {
                _context.Storage.RemovePerceptedFact(loggerValue, idValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IMethodResponse InsertListenedFact(IMonitorLogger logger, string text)
        {
            return LoggedFunctorWithoutResult<string>.Run(logger, text, (loggerValue, textValue) => {
                _context.Storage.InsertListenedFact(loggerValue, textValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IMethodResponse InsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            return LoggedFunctorWithoutResult<RuleInstance>.Run(logger, fact, (loggerValue, factValue) => {
                _context.Storage.InsertListenedFact(loggerValue, factValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return LoggedFunctorWithoutResult<string>.Run(logger, category, (loggerValue, categoryValue) => {
                _context.Storage.AddCategory(loggerValue, categoryValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            return LoggedFunctorWithoutResult<List<string>>.Run(logger, categories, (loggerValue, categoriesValue) => {
                _context.Storage.AddCategories(loggerValue, categoriesValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return LoggedFunctorWithoutResult<string>.Run(logger, category, (loggerValue, categoryValue) => {
                _context.Storage.RemoveCategory(loggerValue, categoryValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            return LoggedFunctorWithoutResult<List<string>>.Run(logger, categories, (loggerValue, categoriesValue) => {
                _context.Storage.RemoveCategories(loggerValue, categoriesValue);
            }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        public bool EnableCategories { get => _context.Storage.EnableCategories; set => _context.Storage.EnableCategories = value; }

        public IMethodResponse Die()
        {
            return BaseFunctorWithoutResult.Run(Logger, () => {
                _context.Die();
            },_activeObjectContext, _threadPool).ToMethodResponse();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _context.Dispose();

            base.OnDisposed();
        }
    }
}

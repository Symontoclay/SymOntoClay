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

using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public class Engine : BaseComponent, ISerializableEngine
    {
        public Engine(EngineSettings settings)
            : base(settings.MonitorNode)
        {
            _context = EngineContextHelper.CreateAndInitContext(settings);
        }

        private readonly EngineContext _context;

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

        public string InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.OldInsertPublicFact(logger, text);
        }

        public string InsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _context.Storage.OldInsertPublicFact(logger, factName, text);
        }

        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _context.Storage.InsertPublicFact(logger, fact);
        }

        public void RemovePublicFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemovePublicFact(logger, id);
        }

        public string InsertFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertFact(logger, text);
        }

        public string InsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _context.Storage.InsertFact(logger, factName, text);
        }

        public void RemoveFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemoveFact(logger, id);
        }

        public IStorage PublicFactsStorage => _context.Storage.PublicFactsStorage;

        public void AddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _context.Storage.AddVisibleStorage(logger, storage);
        }

        public void RemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _context.Storage.RemoveVisibleStorage(logger, storage);
        }

        public string InsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertPerceptedFact(logger, text);
        }

        public void RemovePerceptedFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemovePerceptedFact(logger, id);
        }

        public void InsertListenedFact(IMonitorLogger logger, string text)
        {
            _context.Storage.InsertListenedFact(logger, text);
        }

        public void InsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            _context.Storage.InsertListenedFact(logger, fact);
        }
        
        public void AddCategory(IMonitorLogger logger, string category)
        {
            _context.Storage.AddCategory(logger, category);
        }

        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            _context.Storage.AddCategories(logger, categories);
        }

        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            _context.Storage.RemoveCategory(logger, category);
        }

        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _context.Storage.RemoveCategories(logger, categories);
        }

        public bool EnableCategories { get => _context.Storage.EnableCategories; set => _context.Storage.EnableCategories = value; }

        public void Die()
        {
            _context.Die();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _context.Dispose();

            base.OnDisposed();
        }
    }
}

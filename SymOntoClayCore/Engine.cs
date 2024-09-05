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

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertPublicFact(logger, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _context.Storage.InsertPublicFact(logger, factName, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _context.Storage.InsertPublicFact(logger, fact);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePublicFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemovePublicFact(logger, id);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertFact(logger, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _context.Storage.InsertFact(logger, factName, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemoveFact(logger, id);
        }

        public IStorage PublicFactsStorage => _context.Storage.PublicFactsStorage;

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _context.Storage.AddVisibleStorage(logger, storage);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveVisibleStorage(IMonitorLogger logger, IStorage storage)
        {
            _context.Storage.RemoveVisibleStorage(logger, storage);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPerceptedFact(IMonitorLogger logger, string text)
        {
            return _context.Storage.InsertPerceptedFact(logger, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePerceptedFact(IMonitorLogger logger, string id)
        {
            _context.Storage.RemovePerceptedFact(logger, id);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldInsertListenedFact(IMonitorLogger logger, string text)
        {
            _context.Storage.InsertListenedFact(logger, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldInsertListenedFact(IMonitorLogger logger, RuleInstance fact)
        {
            _context.Storage.InsertListenedFact(logger, fact);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategory(IMonitorLogger logger, string category)
        {
            _context.Storage.AddCategory(logger, category);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _context.Storage.AddCategories(logger, categories);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategory(IMonitorLogger logger, string category)
        {
            _context.Storage.RemoveCategory(logger, category);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _context.Storage.RemoveCategories(logger, categories);
        }

        public bool EnableCategories { get => _context.Storage.EnableCategories; set => _context.Storage.EnableCategories = value; }

        [Obsolete("Serialization Refactoring", true)]
        public void OldDie()
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

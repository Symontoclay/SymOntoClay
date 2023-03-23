/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public class Engine : BaseComponent, ISerializableEngine
    {
        public Engine(EngineSettings settings)
            : base(settings.Logger)
        {
#if DEBUG
            //Log($"settings = {settings}");
#endif

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
                        throw new NotImplementedException();

                    case ComponentState.Started:
                        throw new NotImplementedException();

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
                //Log("Do");
#else
                throw new NotImplementedException();
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
                //Log("Do");
#else
                throw new NotImplementedException();
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
                //Log("Do");
#else
                throw new NotImplementedException();
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
                    //Log("Do");

                    return true;
#else
                    throw new NotImplementedException();
#endif
                }
            }
        }

        public string InsertPublicFact(string text)
        {
            return _context.Storage.InsertPublicFact(text);
        }

        public string InsertPublicFact(RuleInstance fact)
        {
            return _context.Storage.InsertPublicFact(fact);
        }

        public void RemovePublicFact(string id)
        {
            _context.Storage.RemovePublicFact(id);
        }

        public string InsertFact(string text)
        {
            return _context.Storage.InsertFact(text);
        }

        public void RemoveFact(string id)
        {
            _context.Storage.RemoveFact(id);
        }

        public IStorage PublicFactsStorage => _context.Storage.PublicFactsStorage;

        public void AddVisibleStorage(IStorage storage)
        {
            _context.Storage.AddVisibleStorage(storage);
        }

        public void RemoveVisibleStorage(IStorage storage)
        {
            _context.Storage.RemoveVisibleStorage(storage);
        }

        public string InsertPerceptedFact(string text)
        {
            return _context.Storage.InsertPerceptedFact(text);
        }

        public void RemovePerceptedFact(string id)
        {
            _context.Storage.RemovePerceptedFact(id);
        }

        public void InsertListenedFact(string text)
        {
            _context.Storage.InsertListenedFact(text);
        }

        public void InsertListenedFact(RuleInstance fact)
        {
            _context.Storage.InsertListenedFact(fact);
        }
        
        public void AddCategory(string category)
        {
            _context.Storage.AddCategory(category);
        }

        public void AddCategories(List<string> categories)
        {
            _context.Storage.AddCategories(categories);
        }

        public void RemoveCategory(string category)
        {
            _context.Storage.RemoveCategory(category);
        }

        public void RemoveCategories(List<string> categories)
        {
            _context.Storage.RemoveCategories(categories);
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

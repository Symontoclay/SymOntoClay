using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.InheritanceEngine;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.LogicalEngine;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.States;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.TriggerExecution;
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class Engine : BaseComponent, ISerializableEngine
    {
        public Engine(EngineSettings settings)
            : base(settings.Logger)
        {
#if DEBUG
            Log($"settings = {settings}");
#endif

            _context = EngineContextHelper.CreateAndInitContext(settings);
        }

        private readonly EngineContext _context;

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

                _context.CommonNamesStorage.LoadFromSourceCode();
                _context.Storage.LoadFromSourceCode();
                _context.StatesStorage.LoadFromSourceCode();
                _context.LoaderFromSourceCode.LoadFromSourceFiles();

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
                Log("Do");
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
                Log("Do");
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
                Log("Do");
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
                    Log("Do");

                    return true;
#else
                    throw new NotImplementedException();
#endif
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _context.Dispose();

            base.OnDisposed();
        }
    }
}

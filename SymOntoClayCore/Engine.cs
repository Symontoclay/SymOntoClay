using SymOntoClay.Core.Internal;
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
        }

        /// <inheritdoc/>
        public void LoadFromSourceCode()
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
    }
}

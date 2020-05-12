using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public abstract class BaseWorldCoreComponent : IWorldCoreComponent
    {
        private readonly IWorldCoreContext _coreContext;
        private readonly IEntityLogger _logger;

        protected BaseWorldCoreComponent(IWorldCoreContext coreContext)
        {
            coreContext.AddWorldComponent(this);
            _coreContext = coreContext;
            _logger = _coreContext.Logger;
        }

        protected IEntityLogger Logger => _logger;

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Log(string message)
        {
            _logger.Log(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Warning(string message)
        {
            _logger.Warning(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Error(string message)
        {
            _logger.Error(message);
        }

        protected ComponentState _componentState = ComponentState.Created;
        protected readonly object _stateLockObj = new object();

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get
            {
                lock(_stateLockObj)
                {
                    return _componentState == ComponentState.Disposed;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_stateLockObj)
            {
                if(_componentState == ComponentState.Disposed)
                {
                    return;
                }

                _componentState = ComponentState.Disposed;
            }

            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}

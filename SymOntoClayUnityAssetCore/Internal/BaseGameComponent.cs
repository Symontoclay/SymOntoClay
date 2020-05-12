using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public abstract class BaseGameComponent : IGameComponent
    {
        private readonly IWorldCoreGameComponentContext _worldContext;
        private readonly IEntityLogger _logger;

        protected BaseGameComponent(string id, IWorldCoreGameComponentContext worldContext)
        {
            worldContext.AddGameComponent(this);
            _worldContext = worldContext;
            _logger = _worldContext.CreateLogger(id);
        }

        public IEntityLogger Logger => _logger;

        /// <inheritdoc/>
        public virtual void LoadFromSourceCode()
        {
        }

        /// <inheritdoc/>
        public virtual void BeginStarting()
        {
        }

        /// <inheritdoc/>
        public virtual bool IsWaited { get; }

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
                lock (_stateLockObj)
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
                if (_componentState == ComponentState.Disposed)
                {
                    return;
                }

                _componentState = ComponentState.Disposed;
            }

            _worldContext.RemoveGameComponent(this);

            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
        }
    }
}

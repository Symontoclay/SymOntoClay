/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public abstract class BaseGameComponent : IGameComponent
    {
        private readonly IWorldCoreGameComponentContext _worldContext;
        private readonly IEntityLogger _logger;
        private readonly IInvokerInMainThread _invokerInMainThread;
        private readonly int _instanceId;

        protected BaseGameComponent(BaseGameComponentSettings settings, IWorldCoreGameComponentContext worldContext)
        {
            _instanceId = settings.InstanceId;
            worldContext.AddGameComponent(this);
            _worldContext = worldContext;
            _invokerInMainThread = worldContext.InvokerInMainThread;
            _logger = _worldContext.CreateLogger(settings.Id);
        }

        /// <inheritdoc/>
        public int InstanceId => _instanceId;

        public IEntityLogger Logger => _logger;

        /// <inheritdoc/>
        public bool EnableLogging { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public void RunInMainThread(Action function)
        {
            var invocableInMainThreadObj = new InvocableInMainThread(function, _invokerInMainThread);
            invocableInMainThreadObj.Run();
        }

        /// <inheritdoc/>
        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            var invocableInMainThreadObj = new InvocableInMainThreadObj<TResult>(function, _invokerInMainThread);
            return invocableInMainThreadObj.Run();
        }

        /// <inheritdoc/>
        public abstract IStorage PublicFactsStorage { get; }

        /// <inheritdoc/>
        public abstract string IdForFacts { get; }

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

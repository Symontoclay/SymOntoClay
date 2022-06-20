/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public abstract class BaseGameComponent : IGameComponent
    {
        protected readonly IWorldCoreGameComponentContext _worldContext;
        private readonly IEntityLogger _logger;
        private readonly IInvokerInMainThread _invokerInMainThread;
        private readonly int _instanceId;

        protected BaseGameComponent(BaseGameComponentSettings settings, IWorldCoreGameComponentContext worldContext)
        {
            _instanceId = settings.InstanceId;
            _id = settings.Id;
            _idForFacts = settings.IdForFacts;

            worldContext.AddGameComponent(this);
            _worldContext = worldContext;
            _invokerInMainThread = worldContext.InvokerInMainThread;
            _logger = _worldContext.CreateLogger(settings.Id);
        }

        private readonly string _idForFacts;
        private readonly string _id;

        /// <inheritdoc/>
        public int InstanceId => _instanceId;

        /// <inheritdoc/>
        public string Id => _id;

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
        public string IdForFacts => _idForFacts;

        /// <inheritdoc/>
        void IGameComponent.AddPublicFactsStorageOfOtherGameComponent(IStorage storage)
        {
            Task.Run(() => { OnAddPublicFactsStorageOfOtherGameComponent(storage); });
        }

        protected virtual void OnAddPublicFactsStorageOfOtherGameComponent(IStorage storage)
        {
        }

        /// <inheritdoc/>
        void IGameComponent.RemovePublicFactsStorageOfOtherGameComponent(IStorage storage)
        {
            Task.Run(() => { OnRemovePublicFactsStorageOfOtherGameComponent(storage); });
        }

        protected virtual void OnRemovePublicFactsStorageOfOtherGameComponent(IStorage storage)
        {
        }

        /// <inheritdoc/>
        public abstract bool CanBeTakenBy(IEntity subject);

        /// <inheritdoc/>
        public abstract Vector3? GetPosition();

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

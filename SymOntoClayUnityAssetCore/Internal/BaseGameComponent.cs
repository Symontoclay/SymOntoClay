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

using NLog;
using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.InternalImplementations;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public abstract class BaseGameComponent : IGameComponent
    {
        protected readonly IWorldCoreGameComponentContext _worldContext;
        private readonly IMonitorLogger _logger;
        private readonly IMonitorNode _monitorNode;
        private readonly IInvokerInMainThread _invokerInMainThread;
        private readonly int _instanceId;

        protected BaseGameComponent(BaseGameComponentSettings settings, IWorldCoreGameComponentContext worldContext, KindOfWorldItem kindOfWorldItem)
        {
            _instanceId = settings.InstanceId;
            _id = settings.Id;
            _idForFacts = settings.IdForFacts;

            worldContext.AddGameComponent(this);
            _worldContext = worldContext;
            _invokerInMainThread = worldContext.InvokerInMainThread;

            _cancellationTokenSource = new CancellationTokenSource();
            _linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, worldContext.GetCancellationToken());

            var threadingSettings = settings.ThreadingSettings?.AsyncEvents;
            var worldThreadingSettings = worldContext.GetDefaultThreadingSettings(kindOfWorldItem);

            AsyncEventsThreadPool = new CustomThreadPool(threadingSettings?.MinThreadsCount ?? (worldThreadingSettings?.AsyncEvents?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount),
                threadingSettings?.MaxThreadsCount ?? (worldThreadingSettings?.AsyncEvents?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount),
                _linkedCancellationTokenSource.Token);

            _monitorNode = _worldContext.Motitor.CreateMotitorNode("852f0d28-15ca-4671-8779-66e00d23a386", settings.Id);
            _logger = _monitorNode;

            _standardFactsBuilder = worldContext.StandardFactsBuilder;
        }

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _linkedCancellationTokenSource;
        private readonly IStandardFactsBuilder _standardFactsBuilder;
        private readonly string _idForFacts;
        private readonly string _id;

        /// <inheritdoc/>
        public ICustomThreadPool AsyncEventsThreadPool { get; private set; }

        /// <inheritdoc/>
        public CancellationToken GetCancellationToken()
        {
            return _linkedCancellationTokenSource.Token;
        }

        /// <inheritdoc/>
        public int InstanceId => _instanceId;

        /// <inheritdoc/>
        public string Id => _id;

        public IMonitorLogger Logger => _logger;
        public IMonitorNode MonitorNode => _monitorNode;

        /// <inheritdoc/>
        public bool EnableLogging { get => throw new NotImplementedException("35A98EF2-0336-45E0-B0EF-856BB6655BA3"); set => throw new NotImplementedException("44FC9E0D-FFDC-4681-909D-5C09DE5FB882"); }

        /// <inheritdoc/>
        public void RunInMainThread(Action function)
        {
            _invokerInMainThread.RunInMainThread(function);
        }

        /// <inheritdoc/>
        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _invokerInMainThread.RunInMainThread(function);
        }

        public IStandardFactsBuilder StandardFactsBuilder => _standardFactsBuilder;

        /// <inheritdoc/>
        public abstract IStorage PublicFactsStorage { get; }

        /// <inheritdoc/>
        public string IdForFacts => _idForFacts;

        /// <inheritdoc/>
        public abstract bool CanBeTakenBy(IMonitorLogger logger, IEntity subject);

        /// <inheritdoc/>
        public abstract Vector3? GetPosition(IMonitorLogger logger);

        /// <inheritdoc/>
        public virtual void LoadFromSourceCode()
        {
        }

        /// <inheritdoc/>
        public virtual void BeginStarting()
        {
        }

        /// <inheritdoc/>
        public virtual void EndStarting()
        {
        }

        /// <inheritdoc/>
        public virtual bool IsWaited { get; }

        [MethodForLoggingSupport]
        protected void Trace(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Trace(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Debug(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Debug(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Info(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Info(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Warn(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Warn(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Error(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Error(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Error(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Error(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Fatal(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Fatal(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        protected void Fatal(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Fatal(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
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

            _cancellationTokenSource?.Cancel();

            _worldContext.RemoveGameComponent(this);

            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
        }
    }
}

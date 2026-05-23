using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Runtime.CompilerServices;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public abstract class BaseSerializedWorldCoreComponent : ISerializedWorldCoreComponent
    {
        private readonly IWorldCoreContext _coreContext;
        private readonly IMonitorLogger _logger;

        protected BaseSerializedWorldCoreComponent(IWorldCoreContext coreContext)
        {
            coreContext.AddSerializedWorldComponent(this);
            _coreContext = coreContext;
            _logger = _coreContext.Logger;
        }
        
        protected IMonitorLogger Logger => _logger;

        [MethodForLoggingSupport]
        protected void Output(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Output(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

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

            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}

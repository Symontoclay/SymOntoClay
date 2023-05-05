using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners.Handlers
{
    public class HandlersRegistry: IDisposable
    {
        public void SetLogger(IEntityLogger logger)
        {
            _logger = logger;
        }

        protected IEntityLogger _logger;

        public void AddSyncHandler(string methodName, Action handler)
        {
            lock(_lockObj)
            {
                if(_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                AddHandler(PrepareMethodName(methodName), handler, _syncHandlersDict);
            }
        }

        public void AddAsyncHandler(string methodName, Action handler)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                AddHandler(PrepareMethodName(methodName), handler, _asyncHandlersDict);
            }
        }

        public void RemoveHandler(string methodName, Action handler)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                methodName = PrepareMethodName(methodName);

                TryRemoveHandler(methodName, handler, _syncHandlersDict);
                TryRemoveHandler(methodName, handler, _asyncHandlersDict);
            }
        }

        private void AddHandler(string methodName, Action handler, Dictionary<string, List<Action>> handlersDict)
        {
            if(handlersDict.ContainsKey(methodName))
            {
                var list = handlersDict[methodName];

                if(!list.Contains(handler))
                {
                    list.Add(handler);
                }
            }
            else
            {
                handlersDict[methodName] = new List<Action>() { handler };
            }
        }

        private void TryRemoveHandler(string methodName, Action handler, Dictionary<string, List<Action>> handlersDict)
        {
            if(handlersDict.ContainsKey(methodName))
            {
                handlersDict[methodName].Remove(handler);
            }
        }

        public void Emit(string methodName)
        {
            lock(_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(string.Empty);
                }

                methodName = PrepareMethodName(methodName);

                if(_asyncHandlersDict.ContainsKey(methodName))
                {
                    var list = _asyncHandlersDict[methodName];

                    foreach (var handler in list)
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                handler.Invoke();
                            }
                            catch (Exception e)
                            {
                                _logger.Error(e.ToString());
                            }
                        });
                    }
                }

                if(_syncHandlersDict.ContainsKey(methodName))
                {
                    var list = _syncHandlersDict[methodName];

                    foreach(var handler in list)
                    {
                        handler.Invoke();
                    }
                }
            }
        }

        private string PrepareMethodName(string methodName)
        {
            return methodName.ToLower().Trim();
        }

        private readonly Dictionary<string, List<Action>> _syncHandlersDict = new Dictionary<string, List<Action>>();
        private readonly Dictionary<string, List<Action>> _asyncHandlersDict = new Dictionary<string, List<Action>>();

        private readonly object _lockObj = new object();
        private bool _disposed = false;

        /// <inheritdoc/>
        public void Dispose()
        {
            lock(_lockObj)
            {
                if(_disposed)
                {
                    return;
                }

                _disposed = true;

                _syncHandlersDict.Clear();
                _asyncHandlersDict.Clear();
            }
        }
    }
}

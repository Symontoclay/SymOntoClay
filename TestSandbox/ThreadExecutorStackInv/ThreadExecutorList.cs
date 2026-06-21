using SymOntoClay.Common.Disposing;
using SymOntoClay.Core.Internal.CodeExecution;
using System.Collections.Generic;

namespace TestSandbox.ThreadExecutorStackInv
{
    public class ThreadExecutorList: Disposable,
        IOnCompletedThreadExecutorListItemHandler
    {
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();

        public void Add(IThreadExecutor threadExecutor)
        {
            lock(_lockObj)
            {
                if(_existingItems.Contains(threadExecutor))
                {
                    return;
                }

                _existingItems.Add(threadExecutor);

                var item = new ThreadExecutorListItem(threadExecutor);
                _items.Add(item);
                item.AddOnCompletedHandler(this);
            }
        }

        private object _lockObj = new object();
        private HashSet<IThreadExecutor> _existingItems = new HashSet<IThreadExecutor>();
        private List<ThreadExecutorListItem> _items = new List<ThreadExecutorListItem>();

        void IOnCompletedThreadExecutorListItemHandler.Invoke(ThreadExecutorListItem sender)
        {
            sender.RemoveOnCompletedHandler(this);
            _items.Remove(sender);
            _existingItems.Remove(sender.Executor);
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            _existingItems.Clear();

            foreach(var item in _items)
            {
                item.Dispose();
            }

            _items.Clear();
        }
    }
}

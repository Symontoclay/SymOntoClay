using SymOntoClay.Core.Internal.CodeExecution;
using System.Collections.Generic;
using System.Linq;

namespace TestSandbox.ThreadExecutorStackInv
{
    public class ThreadExecutorList
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
            }
        }

        private object _lockObj = new object();
        private HashSet<IThreadExecutor> _existingItems = new HashSet<IThreadExecutor>();
        private List<ThreadExecutorListItem> _items = new List<ThreadExecutorListItem>();
    }
}

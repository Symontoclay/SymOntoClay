using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using System;

namespace TestSandbox.ThreadExecutorStackInv
{
    public class ThreadExecutorListItem: IOnCompletedThreadExecutorHandler
    {
        public ThreadExecutorListItem(IThreadExecutor threadExecutor) 
        {
            _threadExecutor = threadExecutor;
            _threadExecutor.AddOnCompletedHandler(this);
        }

        private readonly IThreadExecutor _threadExecutor;

        void IOnCompletedThreadExecutorHandler.Invoke()
        {
            throw new NotImplementedException("C0C9EAE8-4542-43EC-A1B1-C59BFB3AEFC7");
        }
    }
}

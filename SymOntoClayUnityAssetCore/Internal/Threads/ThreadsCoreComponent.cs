using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal.Threads
{
    public class ThreadsCoreComponent: BaseWorldCoreComponent, ISyncContext
    {
        public ThreadsCoreComponent(IWorldCoreContext coreContext)
            : base(coreContext)
        {
        }

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        void ISyncContext.Wait()
        {
            _autoResetEvent.WaitOne();
        }

        public void Lock()
        {
            _autoResetEvent.Reset();
        }

        public void UnLock()
        {
            _autoResetEvent.Set();
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _autoResetEvent.Dispose();
        }
    }
}

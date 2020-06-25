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

        /// <inheritdoc/>
        void ISyncContext.Wait()
        {
            throw new NotImplementedException();
            _autoResetEvent.WaitOne();
        }

        public void Lock()
        {
            //throw new NotImplementedException();

            _autoResetEvent.Reset();
        }

        public void UnLock()
        {
            //throw new NotImplementedException();

            _autoResetEvent.Set();
        }

        /// <inheritdoc/>
        protected override void OnDispose()
        {
            base.OnDispose();

            _autoResetEvent.Dispose();
        }
    }
}

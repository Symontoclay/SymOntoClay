using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal.Threads
{
    public class ThreadsCoreComponent: BaseWorldCoreComponent, IActivePeriodicObjectCommonContext
    {
        public ThreadsCoreComponent(IWorldCoreContext coreContext)
            : base(coreContext)
        {
        }

        private readonly ActivePeriodicObjectCommonContext _commonActiveContext = new ActivePeriodicObjectCommonContext();

        /// <inheritdoc/>
        bool IActivePeriodicObjectCommonContext.IsNeedWating => _commonActiveContext.IsNeedWating;

        /// <inheritdoc/>
        AutoResetEvent IActivePeriodicObjectCommonContext.AutoResetEvent => _commonActiveContext.AutoResetEvent;

        public void Lock()
        {
            _commonActiveContext.Lock();
        }

        public void UnLock()
        {
            _commonActiveContext.UnLock();
        }

        ///// <inheritdoc/>
        //protected override void OnDispose()
        //{
        //    base.OnDispose();

        //    _commonActiveContext.Dispose();
        //}
    }
}

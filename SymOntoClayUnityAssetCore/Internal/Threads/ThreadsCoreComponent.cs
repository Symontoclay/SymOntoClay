/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

        private readonly object _lockObj = new object();
        private bool _isLocked;

        private readonly ActivePeriodicObjectCommonContext _commonActiveContext = new ActivePeriodicObjectCommonContext();

        /// <inheritdoc/>
        bool IActivePeriodicObjectCommonContext.IsNeedWating => _commonActiveContext.IsNeedWating;

        /// <inheritdoc/>
        EventWaitHandle IActivePeriodicObjectCommonContext.WaitEvent => _commonActiveContext.WaitEvent;

        public void Lock()
        {
            lock (_lockObj)
            {
                if(_isLocked)
                {
                    return;
                }

                _isLocked = true;

                _commonActiveContext.Lock();
            }            
        }

        public void UnLock()
        {
            lock (_lockObj)
            {
                if(!_isLocked)
                {
                    return;
                }

                _isLocked = false;

                _commonActiveContext.UnLock();
            }            
        }

        ///// <inheritdoc/>
        //protected override void OnDispose()
        //{
        //    base.OnDispose();

        //    _commonActiveContext.Dispose();
        //}
    }
}

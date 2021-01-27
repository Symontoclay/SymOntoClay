/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    public class ActivePeriodicObjectCommonContext : IActivePeriodicObjectCommonContext
    {
        private readonly ManualResetEvent _autoResetEvent = new ManualResetEvent(true);
        private volatile bool _isNeedWating;

        /// <inheritdoc/>
        public bool IsNeedWating => _isNeedWating;

        /// <inheritdoc/>
        public EventWaitHandle WaitEvent => _autoResetEvent;

        public void Lock()
        {
            _autoResetEvent.Reset();
            _isNeedWating = true;
        }

        public void UnLock()
        {
            _isNeedWating = false;
            _autoResetEvent.Set();
        }
    }
}

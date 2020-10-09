/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal.DateAndTime
{
    public class DateTimeProvider: BaseLoggedComponent, IDateTimeProvider, IDisposable
    {
        public DateTimeProvider(IEntityLogger logger, IActivePeriodicObjectCommonContext syncContext)
            : base(logger)
        {
            var activeContext = new ActivePeriodicObjectContext(syncContext);

            _activeObject = new AsyncActivePeriodicObject(activeContext)
            {
                PeriodicMethod = NRun
            };
        }

        private readonly object _lockObj = new object();
        private readonly IActivePeriodicObject _activeObject;
        private long _ticks = 0;

        public void LoadFromSourceCode()
        {
            lock(_lockObj)
            {
                _ticks = 0;
            }
        }

        /// <inheritdoc/>
        public long CurrentTiks 
        {
            get
            {
                lock(_lockObj)
                {
                    return _ticks;
                }
            }
        }

        private bool NRun()
        {
            lock (_lockObj)
            {
                _ticks++;
            }

            Thread.Sleep(100);

            return true;
        }

        /// <inheritdoc/>
        public Value Start()
        {
            return _activeObject.Start();
        }

        /// <inheritdoc/>
        public void Stop()
        {
            _activeObject.Stop();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _activeObject.Dispose();
        }
    }
}

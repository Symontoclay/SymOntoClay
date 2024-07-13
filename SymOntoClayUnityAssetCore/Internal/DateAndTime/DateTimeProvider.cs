/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal.DateAndTime
{
    public class DateTimeProvider: BaseLoggedComponent, IDateTimeProvider, IDisposable
    {
        public DateTimeProvider(IMonitorLogger logger, IActiveObjectCommonContext syncContext, ICustomThreadPool threadPool, CancellationToken cancellationToken)
            : base(logger)
        {
            var activeContext = new ActiveObjectContext(syncContext, cancellationToken);

            _activeObject = new AsyncActivePeriodicObject(activeContext, threadPool, logger)
            {
                PeriodicMethod = NRun
            };
        }

        private readonly object _lockObj = new object();
        private readonly IActivePeriodicObject _activeObject;
        private long _ticks = 0;

        /// <inheritdoc/>
        public float TicksToSecondsMultiplicator => 0.001F;

        /// <inheritdoc/>
        public float SecondsToTicksMultiplicator => 1000F;

        /// <inheritdoc/>
        public float TicksToMillisecondsMultiplicator => 1F;

        /// <inheritdoc/>
        public float MillisecondsToTicksMultiplicator => 1F;

        private int _millisecondsTimeout = 100;

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

        private bool NRun(CancellationToken cancellationToken)
        {
            lock (_lockObj)
            {
                _ticks += _millisecondsTimeout;
            }

            Thread.Sleep(_millisecondsTimeout);

            return true;
        }

        /// <inheritdoc/>
        public IThreadTask Start()
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

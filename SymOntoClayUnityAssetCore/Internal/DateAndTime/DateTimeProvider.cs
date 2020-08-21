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

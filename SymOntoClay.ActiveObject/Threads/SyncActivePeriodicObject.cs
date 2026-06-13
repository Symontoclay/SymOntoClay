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

using SymOntoClay.ActiveObject.EventsCollections;
using SymOntoClay.ActiveObject.EventsInterfaces;
using SymOntoClay.ActiveObject.Pointers;
using SymOntoClay.Common.Cancellation;
using SymOntoClay.Threading;
using System;
using System.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    /// <summary>
    /// Executes PeriodicMethod in the same Thread as a caller.
    /// It is helpful for short calculations.
    /// The loop of the class must not be blocked with AutoResetEvent.
    /// </summary>
    //[Obsolete("Use AsyncActivePeriodicObject", true)]
    public class SyncActivePeriodicObject : IActivePeriodicObject
    {
        public SyncActivePeriodicObject(ICancellationContext cancellationContext)
        {
            _cancellationContext = cancellationContext;
        }

        private readonly ICancellationContext _cancellationContext;

        /// <inheritdoc/>
        public IObjectWithPeriodicMethod ObjectWithPeriodicMethod { get; set; }

        private volatile bool _isActive;

        /// <inheritdoc/>
        public bool IsActive => _isActive;

        /// <inheritdoc/>
        public bool IsWaited => false;

        private IThreadTaskPointer _taskValue = new ThreadTaskPointer();

        /// <inheritdoc/>
        public IThreadTaskPointer TaskValue => _taskValue;

        /// <inheritdoc/>
        public void AddOnCompletedHandler(IOnCompletedActiveObjectHandler handler)
        {
            _onCompletedHandlersCollection.AddHandler(handler);
        }

        /// <inheritdoc/>
        public void RemoveOnCompletedHandler(IOnCompletedActiveObjectHandler handler)
        {
            _onCompletedHandlersCollection.RemoveHandler(handler);
        }

        private OnCompletedActiveObjectHandlersCollection _onCompletedHandlersCollection = new OnCompletedActiveObjectHandlersCollection();

        /// <inheritdoc/>
        public IThreadTaskPointer Start()
        {
            _isActive = true;

            while (true)
            {
                if (_cancellationContext.IsCancellationRequested)
                {
                    _onCompletedHandlersCollection.Emit();
                    return _taskValue;
                }

                if (!ObjectWithPeriodicMethod.PeriodicHandler(_cancellationContext))
                {
                    _isActive = false;
                    _onCompletedHandlersCollection.Emit();
                    return _taskValue;
                }
            }
        }

        /// <inheritdoc/>
        public void Stop()
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _onCompletedHandlersCollection.Clear();
        }
    }
}

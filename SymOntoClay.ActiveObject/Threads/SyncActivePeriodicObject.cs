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

using SymOntoClay.Threading;
using System.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    /// <summary>
    /// Executes PeriodicMethod in the same Thread as a caller.
    /// It is helpful for short calculations.
    /// The loop of the class must not be blocked with AutoResetEvent.
    /// </summary>
    public class SyncActivePeriodicObject : IActivePeriodicObject
    {
        public SyncActivePeriodicObject(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        private readonly CancellationToken _cancellationToken;

        /// <inheritdoc/>
        public PeriodicDelegate PeriodicMethod { get; set; }

        private volatile bool _isActive;

        /// <inheritdoc/>
        public bool IsActive => _isActive;

        /// <inheritdoc/>
        public bool IsWaited => false;

        private IThreadTask _taskValue = null;

        /// <inheritdoc/>
        public IThreadTask TaskValue => _taskValue;

        /// <inheritdoc/>
        public IThreadTask Start()
        {
            _isActive = true;

            while (true)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return _taskValue;
                }

                if (!PeriodicMethod(_cancellationToken))
                {
                    _isActive = false;
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
        }
    }
}

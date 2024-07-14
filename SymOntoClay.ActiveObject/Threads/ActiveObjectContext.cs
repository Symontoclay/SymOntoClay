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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SymOntoClay.ActiveObject.Threads
{
    public class ActiveObjectContext : IActiveObjectContext, IDisposable
    {
        public ActiveObjectContext(IActiveObjectCommonContext commonContext, CancellationToken cancellationToken)
        {
            _commonContext = commonContext;
            _cancellationToken = cancellationToken;
        }

        private readonly IActiveObjectCommonContext _commonContext;
        private readonly CancellationToken _cancellationToken;

        /// <inheritdoc/>
        public bool IsNeedWating => _commonContext.IsNeedWating;

        /// <inheritdoc/>
        public EventWaitHandle WaitEvent => _commonContext.WaitEvent;

        private object _lockObj = new object();

        private List<IActivePeriodicObject> _periodicChildren = new List<IActivePeriodicObject>();
        private List<IActiveOnceObject> _onceChildren = new List<IActiveOnceObject>();

        /// <inheritdoc/>
        void IActiveObjectContext.AddChildActiveObject(IActivePeriodicObject activeObject)
        {
            lock (_lockObj)
            {
                if (_periodicChildren.Contains(activeObject))
                {
                    return;
                }

                _periodicChildren.Add(activeObject);
            }
        }

        /// <inheritdoc/>
        void IActiveObjectContext.RemoveChildActiveObject(IActivePeriodicObject activeObject)
        {
            lock (_lockObj)
            {
                if (_periodicChildren.Contains(activeObject))
                {
                    _periodicChildren.Remove(activeObject);
                }
            }
        }

        /// <inheritdoc/>
        void IActiveObjectContext.AddChildActiveObject(IActiveOnceObject activeObject)
        {
            lock (_lockObj)
            {
                if (_onceChildren.Contains(activeObject))
                {
                    _onceChildren.Remove(activeObject);
                }
            }
        }

        /// <inheritdoc/>
        void IActiveObjectContext.RemoveChildActiveObject(IActiveOnceObject activeObject)
        {
            lock (_lockObj)
            {
                if (_onceChildren.Contains(activeObject))
                {
                    _onceChildren.Remove(activeObject);
                }
            }
        }

        /// <inheritdoc/>
        public void WaitWhenAllIsNotWaited()
        {
            lock (_lockObj)
            {
                while ((!_periodicChildren.All(p => p.IsWaited)) && (!_onceChildren.All(p => p.IsWaited)))
                {
                }
            }
        }

        /// <inheritdoc/>
        public void StartAll()
        {
            lock (_lockObj)
            {
                foreach (var child in _periodicChildren)
                {
                    child.Start();
                }

                foreach (var child in _onceChildren)
                {
                    child.Start();
                }
            }
        }

        /// <inheritdoc/>
        public void StopAll()
        {
            lock (_lockObj)
            {
                foreach (var child in _periodicChildren)
                {
                    child.Stop();
                }
            }
        }

        /// <inheritdoc/>
        public CancellationToken Token => _cancellationToken;

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
            }

            var tmpPeriodicChildren = _periodicChildren.ToList();
            var tmpOnceChildren = _onceChildren.ToList();

            foreach (var child in tmpPeriodicChildren)
            {
                child.Dispose();
            }

            foreach (var child in tmpOnceChildren)
            {
                child.Dispose();
            }
        }
    }
}

/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    public class ActivePeriodicObjectContext : IActivePeriodicObjectContext, IDisposable
    {
        public ActivePeriodicObjectContext(IActivePeriodicObjectCommonContext commonContext)
        {
            _commonContext = commonContext;
        }

        private readonly IActivePeriodicObjectCommonContext _commonContext;

        /// <inheritdoc/>
        public bool IsNeedWating => _commonContext.IsNeedWating;

        /// <inheritdoc/>
        public EventWaitHandle WaitEvent => _commonContext.WaitEvent;

        private object _lockObj = new object();

        private List<IActivePeriodicObject> _children = new List<IActivePeriodicObject>();

        /// <inheritdoc/>
        void IActivePeriodicObjectContext.AddChildActiveObject(IActivePeriodicObject activeObject)
        {
            lock (_lockObj)
            {
                if (_children.Contains(activeObject))
                {
                    return;
                }

                _children.Add(activeObject);
            }
        }

        /// <inheritdoc/>
        void IActivePeriodicObjectContext.RemoveChildActiveObject(IActivePeriodicObject activeObject)
        {
            lock (_lockObj)
            {
                if (_children.Contains(activeObject))
                {
                    _children.Remove(activeObject);
                }
            }
        }

        /// <inheritdoc/>
        public void WaitWhenAllIsNotWaited()
        {
            lock (_lockObj)
            {
                while (!_children.All(p => p.IsWaited))
                {
                }
            }
        }

        /// <inheritdoc/>
        public void StartAll()
        {
            lock (_lockObj)
            {
                foreach (var child in _children)
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
                foreach (var child in _children)
                {
                    child.Stop();
                }
            }
        }

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lockObj)
            {
                if(_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
            }

            var tmpChildren = _children.ToList();

            foreach (var child in tmpChildren)
            {
                child.Dispose();
            }
        }
    }
}

/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread
{
    public class InvokerInMainThread : IInvokerInMainThread
    {
        private static int MainThreadId = 1;

        /// <inheritdoc/>
        public void RunInMainThread(Action function)
        {
            if(Thread.CurrentThread.ManagedThreadId == MainThreadId)
            {
                function();
                return;
            }

            var invocableInMainThreadObj = new InvocableInMainThread(function, this);
            invocableInMainThreadObj.Run();
        }

        /// <inheritdoc/>
        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            if (Thread.CurrentThread.ManagedThreadId == MainThreadId)
            {
                return function();
            }

            var invocableInMainThreadObj = new InvocableInMainThreadObj<TResult>(function, this);
            return invocableInMainThreadObj.Run();
        }

        /// <inheritdoc/>
        public void SetInvocableObj(IInvocableInMainThread invokableObj)
        {
            lock (_lockObj)
            {
                _queue.Enqueue(invokableObj);
            }
        }

        public void Update()
        {
            List<IInvocableInMainThread> invocableList = null;

            lock (_lockObj)
            {
                if (_queue.Count > 0)
                {
                    invocableList = _queue.ToList();
                    _queue.Clear();
                }
            }

            if (invocableList == null)
            {
                return;
            }

            foreach (var invocable in invocableList)
            {
                try
                {
                    invocable.Invoke();
                }
                catch{
                }
            }
        }

        private readonly object _lockObj = new object();
        private readonly Queue<IInvocableInMainThread> _queue = new Queue<IInvocableInMainThread>();
    }
}

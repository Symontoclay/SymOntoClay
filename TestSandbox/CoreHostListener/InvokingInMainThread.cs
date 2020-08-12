using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    public class InvokingInMainThread : IInvokingInMainThread
    {
        public void SetInvocableObj(IInvocableInMainThreadObj invokableObj)
        {
            lock (_lockObj)
            {
                _queue.Enqueue(invokableObj);
            }
        }

        public void Update()
        {
            List<IInvocableInMainThreadObj> invocableList = null;

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
                invocable.Invoke();
            }
        }

        private object _lockObj = new object();
        private Queue<IInvocableInMainThreadObj> _queue = new Queue<IInvocableInMainThreadObj>();
    }
}

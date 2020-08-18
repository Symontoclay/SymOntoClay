using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread
{
    public class InvokingInMainThread : IInvokingInMainThread
    {
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
                invocable.Invoke();
            }
        }

        private readonly object _lockObj = new object();
        private readonly Queue<IInvocableInMainThread> _queue = new Queue<IInvocableInMainThread>();
    }
}

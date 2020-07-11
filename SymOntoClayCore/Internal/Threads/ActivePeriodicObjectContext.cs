using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    public class ActivePeriodicObjectContext : IActivePeriodicObjectContext
    {
        public ActivePeriodicObjectContext(IActivePeriodicObjectCommonContext commonContext)
        {
            _commonContext = commonContext;
        }

        private readonly IActivePeriodicObjectCommonContext _commonContext;

        /// <inheritdoc/>
        public bool IsNeedWating => _commonContext.IsNeedWating;

        /// <inheritdoc/>
        public AutoResetEvent AutoResetEvent => _commonContext.AutoResetEvent;

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
    }
}

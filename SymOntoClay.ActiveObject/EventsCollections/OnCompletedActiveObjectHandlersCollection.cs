using SymOntoClay.ActiveObject.EventsInterfaces;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.EventsCollections
{
    public class OnCompletedActiveObjectHandlersCollection
    {
        public void AddHandler(IOnCompletedActiveObjectHandler handler)
        {
            lock (_lockObj)
            {
                if (_handlers.Contains(handler))
                {
                    return;
                }

                _handlers.Add(handler);
            }
        }

        public void RemoveHandler(IOnCompletedActiveObjectHandler handler)
        {
            lock (_lockObj)
            {
                if (_handlers.Contains(handler))
                {
                    _handlers.Remove(handler);
                }
            }
        }

        public void Emit()
        {
            lock (_lockObj)
            {
                foreach (var handler in _handlers.ToArray())
                {
                    handler.Invoke();
                }
            }
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                _handlers.Clear();
            }
        }

        private object _lockObj = new object();
        private List<IOnCompletedActiveObjectHandler> _handlers = new List<IOnCompletedActiveObjectHandler>();
    }
}

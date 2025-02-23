using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors
{
    public class SerializationAnchor : ISerializationAnchor
    {
        public SerializationAnchor()
        {
            _lockObj = new object();
        }

        private object _lockObj;

        private List<IBaseFunctor> _functors = new List<IBaseFunctor>();

        /// <inheritdoc/>
        public void AddFunctor(IBaseFunctor functor)
        {
            lock (_lockObj)
            {
                if (_functors.Contains(functor))
                {
                    return;
                }

                _functors.Add(functor);
            }
        }

        /// <inheritdoc/>
        public void RemoveFunctor(IBaseFunctor functor)
        {
            lock (_lockObj)
            {
                if (_functors.Contains(functor))
                {
                    _functors.Remove(functor);
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _functors.Clear();
        }
    }
}

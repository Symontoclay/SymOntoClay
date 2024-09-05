using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    [SocSerialization]
    public partial class SerializationAnchor : ISerializationAnchor
    {
        public SerializationAnchor()
        {
            _lockObj = new object();
        }

        [SocNoSerializable]
        private object _lockObj;

        private List<IBaseFunctor> _functors = new List<IBaseFunctor>();

        /// <inheritdoc/>
        public void AddFunctor(IBaseFunctor functor)
        {
            lock(_lockObj)
            {
                if(_functors.Contains(functor))
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
    }
}

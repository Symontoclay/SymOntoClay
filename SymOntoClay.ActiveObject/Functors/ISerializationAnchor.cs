using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public interface ISerializationAnchor : IDisposable
    {
        void AddFunctor(IBaseFunctor functor);
        void RemoveFunctor(IBaseFunctor functor);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public interface ISerializationAnchor: IDisposable
    {
        void AddFunctor(IBaseFunctor functor);
        void RemoveFunctor(IBaseFunctor functor);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public interface ISerializationAnchor
    {
        void AddFunctor(IBaseFunctor functor);
        void RemoveFunctor(IBaseFunctor functor);
    }
}

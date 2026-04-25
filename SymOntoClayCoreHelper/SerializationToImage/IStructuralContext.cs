using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IStructuralContext
    {
        StructuralAdvice GetAdvice(Type type);
    }
}

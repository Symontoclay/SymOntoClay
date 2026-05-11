using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IStructuralContext
    {
        KindOfStructuralContext Kind { get; }
        StructuralAdvice GetAdvice(Type type);
        KindOfStructuralObject GetKindOfStructuralObject(Type type);
    }
}

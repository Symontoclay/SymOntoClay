using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ISerializedTypesPool
    {
        int NullTypeId { get; }
        int GetOrRegisterType(Type type);
        KindOfSerializedValue GetKindOfSerializedValue(Type type);
    }
}

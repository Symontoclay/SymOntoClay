using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ISerializedTypesPool
    {
        int GetOrRegisterType(Type type);
    }
}

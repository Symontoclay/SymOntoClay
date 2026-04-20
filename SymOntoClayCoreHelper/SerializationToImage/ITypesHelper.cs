using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ITypesHelper
    {
        KindOfSerializedValue GetKindOfSerializedValue(Type type);
        string ToString(object obj);
    }
}

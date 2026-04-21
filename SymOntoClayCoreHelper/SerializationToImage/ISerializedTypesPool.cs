using System;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ISerializedTypesPool
    {
        int NullTypeId { get; }
        int GetOrRegisterType(Type type);
        void SaveToStream(Stream stream);
        void LoadFromStream(Stream stream);
    }
}

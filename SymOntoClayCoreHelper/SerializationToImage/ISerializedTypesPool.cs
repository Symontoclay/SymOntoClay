using System;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ISerializedTypesPool
    {
        int NullTypeId { get; }
        int GetOrRegisterType(Type type);
        Type GetTypeValue(int typeId);
        void Save(BinaryWriter writer);
        void Load(BinaryReader reader);
    }
}

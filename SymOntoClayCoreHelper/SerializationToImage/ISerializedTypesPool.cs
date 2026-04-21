using System;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ISerializedTypesPool
    {
        int NullTypeId { get; }
        int GetOrRegisterType(Type type);
        void Save(BinaryWriter writer);
        void Load(BinaryReader reader);
    }
}

using SymOntoClay.Common;
using System;

namespace SymOntoClay.Serialization
{
    public interface ISerializable : IObjectToString
    {
        Type GetPlainObjectType();
        void OnWritePlainObject(object plainObject, ISerializer serializer);
        void OnReadPlainObject(object plainObject, IDeserializer deserializer);
    }
}

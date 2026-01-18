using System;

namespace SymOntoClay.CoreHelper.SerializerAdapters
{
    public static class SerializerAdapterFactory
    {
        public static ISerializerAdapter Create(KindOfSerialization kindOfSerialization)
        {
            switch(kindOfSerialization)
            {
                case KindOfSerialization.Json:
                    return new JsonSerializerAdapter();

                case KindOfSerialization.Bson:
                    return new BsonSerializerAdapter();

                case KindOfSerialization.MessagePack:
                    return new MessagePackSerializerAdapter();

                default:
                    throw new NotSupportedException(kindOfSerialization.ToString());
            }
        }
    }
}

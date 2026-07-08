using Newtonsoft.Json;
using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedValue
    {
        public SerializedValue()
        {
        }

        public SerializedValue(KindOfSerializedValue kindOfSerializedValue, long id, int typeId)
            : this(kindOfSerializedValue, id, typeId, string.Empty)
        {
        }

        public SerializedValue(KindOfSerializedValue kindOfSerializedValue, long id, int typeId, string literal)
        {
            KindOfSerializedValue = kindOfSerializedValue;
            Id = id;
            TypeId = typeId;
            Literal = literal;
        }

        [JsonProperty]
        public KindOfSerializedValue KindOfSerializedValue { get; private set; }

        [JsonProperty]
        public long Id { get; private set; }

        [JsonProperty]
        public int TypeId { get; private set; }

        [JsonProperty]
        public string Literal { get; private set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(KindOfSerializedValue, Id, TypeId, Literal);
        }

        public override string ToString() => $"({nameof(KindOfSerializedValue)}: {KindOfSerializedValue}, {nameof(Id)}: {Id}, {nameof(TypeId)}: {TypeId}, {nameof(Literal)}: '{Literal}')";
    }
}

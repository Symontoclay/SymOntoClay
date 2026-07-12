using Newtonsoft.Json;
using System;
using System.Drawing;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedValue: IEquatable<SerializedValue>
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

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(KindOfSerializedValue, Id, TypeId, Literal);
        }

        /// <inheritdoc/>
        public bool Equals(SerializedValue other)
        {
            if (other == null)
            {
                return false;
            }

            if(KindOfSerializedValue != other.KindOfSerializedValue)
            {
                return false;
            }

            if(Id != other.Id)
            {
                return false;
            }

            if(TypeId != other.TypeId)
            {
                return false;
            }

            if(Literal != other.Literal)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is SerializedValue other && Equals(other);
        }

        /// <inheritdoc/>
        public override string ToString() => $"({nameof(KindOfSerializedValue)}: {KindOfSerializedValue}, {nameof(Id)}: {Id}, {nameof(TypeId)}: {TypeId}, {nameof(Literal)}: '{Literal}')";

        public static bool operator ==(SerializedValue left, SerializedValue right) => left.Equals(right);
        public static bool operator !=(SerializedValue left, SerializedValue right) => !left.Equals(right);
    }
}

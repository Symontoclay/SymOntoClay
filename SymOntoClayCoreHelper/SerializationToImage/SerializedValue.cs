namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedValue
    {
        public SerializedValue()
        {
        }

        public SerializedValue(KindOfSerializedValue kindOfSerializedValue, long id, int typeId, string literal)
        {
            KindOfSerializedValue = kindOfSerializedValue;
            Id = id;
            TypeId = typeId;
            Literal = literal;
        }

        public KindOfSerializedValue KindOfSerializedValue { get; private set; }
        public long Id { get; private set; }
        public int TypeId { get; private set; }
        public string Literal { get; private set; }

        //public override int GetHashCode()
        //{
        //    return Id.GetHashCode();
        //}

        //public override string ToString() => $"({nameof(Id)}: '{Id}', {nameof(TypeId)}: '{TypeId}')";
    }
}

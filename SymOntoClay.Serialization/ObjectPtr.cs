namespace SymOntoClay.Serialization
{
    public class ObjectPtr
    {
        public ObjectPtr()
        {
        }

        public ObjectPtr(bool isNull)
            : this(null, null, isNull, false, string.Empty)
        {
        }

        public ObjectPtr(string id, string typeName)
            : this(id, typeName, false, false, string.Empty)
        {
        }

        public ObjectPtr(string id, string typeName, bool isNull, bool isPrimitive, string primitiveValue)
        {
            Id = id;
            TypeName = typeName;
            IsNull = isNull;
            IsPrimitive = isPrimitive;
            PrimitiveValue = primitiveValue;
        }

        public string Id { get; set; }
        public string TypeName { get; set; }
        public bool IsNull { get; set; }
        public bool IsPrimitive { get; set; }
        public string PrimitiveValue { get; set; }

        public override string ToString() => $"({nameof(Id)}: '{Id}', {nameof(TypeName)}: '{TypeName}', {nameof(IsNull)}: {IsNull}, {nameof(IsPrimitive)}: {IsPrimitive}, {nameof(PrimitiveValue)} : '{PrimitiveValue}')";

        public static ObjectPtr NullPtr = new ObjectPtr(true);

        public static ObjectPtr FromPrimitive(string id, string typeName, string primitiveValue)
        {
            return new ObjectPtr(id: id, typeName: typeName, isNull: false, isPrimitive: true, primitiveValue: primitiveValue);
        }
    }
}

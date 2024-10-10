namespace SymOntoClay.Serialization
{
    public struct ObjectPtr
    {
        public ObjectPtr(bool isNull)
            : this(null, null, isNull)
        {
        }

        public ObjectPtr(string id, string typeName)
            : this(id, typeName, false)
        {
        }

        public ObjectPtr(string id, string typeName, bool isNull)
        {
            Id = id;
            TypeName = typeName;
            IsNull = isNull;
        }

        public string Id { get; set; }
        public string TypeName { get; set; }
        public bool IsNull { get; set; }

        public override string ToString() => $"({nameof(Id)}: '{Id}', {nameof(TypeName)}: '{TypeName}', {nameof(IsNull)}: {IsNull})";

        public static ObjectPtr NullPtr = new ObjectPtr(true);
    }
}

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

        public string Id { get; private set; }
        public string TypeName { get; private set; }
        public bool IsNull { get; private set; }

        public override string ToString() => $"({nameof(Id)}: '{Id}', {nameof(TypeName)}: '{TypeName}', {nameof(IsNull)}: {IsNull})";
    }
}

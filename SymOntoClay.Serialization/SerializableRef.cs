namespace SymOntoClay.Serialization
{
    //[SocSerialization]
    public partial class SerializableRef<T>
    {
        public SerializableRef(T value)
        {
            Value = value;
        }

        [SocNoSerializable]
        public T Value { get; set; }
    }
}

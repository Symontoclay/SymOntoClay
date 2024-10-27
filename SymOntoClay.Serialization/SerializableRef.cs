namespace SymOntoClay.Serialization
{
    public class SerializableRef<T>
    {
        public SerializableRef(T value)
        {
            Value = value;
        }

        [SocNoSerializable]
        public T Value { get; set; }
    }
}

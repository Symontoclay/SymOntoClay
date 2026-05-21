using SymOntoClay.Common.SerializationToImage.Attributes;

namespace SymOntoClay.CoreHelper
{
    public class Pointer<T>
    {
        public Pointer()
        {
        }

        public Pointer(T value)
        {
            Value = value;
        }

        [SystemNoSerializedMember]
        public T Value { get; set; }
    }
}

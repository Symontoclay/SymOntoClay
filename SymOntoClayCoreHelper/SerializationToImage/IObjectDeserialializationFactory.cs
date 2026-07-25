namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IObjectDeserialializationFactory
    {
        object GetValue(SerializedValue serializedValue);
    }
}

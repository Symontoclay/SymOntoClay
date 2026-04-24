namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IObjectSerializer
    {
        SerializedValue GetSerializedValue(object obj);
    }
}

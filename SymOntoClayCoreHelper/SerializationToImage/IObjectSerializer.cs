namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IObjectSerializer
    {
        SerializedValue SerializeValue(object obj);
    }
}

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ISerializedObjectsPool
    {
        bool IsSerialized(object obj);
        bool TryGetSerializedValue(object obj, out SerializedValue serializedValue);
        SerializedValue RegSerializedValue(object obj);
    }
}

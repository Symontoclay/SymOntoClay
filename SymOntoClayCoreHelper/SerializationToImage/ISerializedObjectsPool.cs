namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ISerializedObjectsPool
    {
        bool IsSerialized(object obj, bool ignorePreregistered);
        bool TryGetSerializedValue(object obj, out SerializedValue serializedValue);
        bool TryGetObject(SerializedValue serializedValue, out object obj);
        SerializedValue RegSerializedValue(object obj, SerializedObjectsPoolMode mode);
        SerializedValue GetOrRegSerializedValue(object obj, SerializedObjectsPoolMode mode);
    }
}

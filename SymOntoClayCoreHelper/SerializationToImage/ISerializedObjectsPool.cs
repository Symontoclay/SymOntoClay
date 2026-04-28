namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ISerializedObjectsPool
    {
        bool IsSerialized(object obj, bool ignorePreregistered);
        bool TryGetSerializedValue(object obj, out SerializedValue serializedValue);
        SerializedValue RegSerializedValue(object obj, SerializedObjectsPoolMode mode);
    }
}

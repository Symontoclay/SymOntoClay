namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IObjectDeserializer
    {
        object DeserializeValue(SerializedValue serializedValue, ObjMemberRef objMember = null);
    }
}

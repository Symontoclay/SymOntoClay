namespace SymOntoClay.Serialization
{
    public interface ISerializer
    {
        void Serialize(ISerializable serializable);
        ObjectPtr GetSerializedObjectPtr(object obj);
    }
}

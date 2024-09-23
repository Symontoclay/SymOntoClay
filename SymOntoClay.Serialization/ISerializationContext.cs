namespace SymOntoClay.Serialization
{
    public interface ISerializationContext: ISerializedObjectsPool
    {
        string DirName { get; }
    }
}

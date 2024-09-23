namespace SymOntoClay.Serialization
{
    public interface ISerializationContext: ISerializedObjectsPool
    {
        string HeapDirName { get; }
        string RootDirName { get; }
    }
}

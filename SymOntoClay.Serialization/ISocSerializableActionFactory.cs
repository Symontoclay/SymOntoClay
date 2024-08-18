namespace SymOntoClay.Serialization
{
    public interface ISocSerializableActionFactory
    {
        string Id { get; }
        object GetAction(int index);
    }
}

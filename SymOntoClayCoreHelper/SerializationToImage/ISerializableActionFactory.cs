namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface ISerializableActionFactory
    {
        string Id { get; }
        object GetAction(int index);
    }
}

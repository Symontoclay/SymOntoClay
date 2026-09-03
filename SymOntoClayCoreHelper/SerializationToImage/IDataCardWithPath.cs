namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardWithPath: IDataCardWithHeader
    {
        string Path { get; }

        bool ShouldBeReplacedDuringDeserialization { get; }
    }
}

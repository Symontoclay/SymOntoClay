namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardWithHeader: IDataCard
    {
        SerializedValue Header { get; }
    }
}

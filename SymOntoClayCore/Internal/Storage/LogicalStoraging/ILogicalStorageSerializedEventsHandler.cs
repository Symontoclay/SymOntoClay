namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public interface ILogicalStorageSerializedEventsHandler
    {
        void NRealStorageContext_OnAddParentStorage(IStorage storage);
        void NRealStorageContext_OnRemoveParentStorage(IStorage storage);
    }
}

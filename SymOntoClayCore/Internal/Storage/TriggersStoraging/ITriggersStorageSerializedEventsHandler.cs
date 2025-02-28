namespace SymOntoClay.Core.Internal.Storage.TriggersStoraging
{
    public interface ITriggersStorageSerializedEventsHandler
    {
        void NRealStorageContext_OnRemoveParentStorage(IStorage storage);
        void NRealStorageContext_OnAddParentStorage(IStorage storage);
    }
}

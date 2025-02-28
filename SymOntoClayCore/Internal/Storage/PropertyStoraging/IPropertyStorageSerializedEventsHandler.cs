namespace SymOntoClay.Core.Internal.Storage.PropertyStoraging
{
    public interface IPropertyStorageSerializedEventsHandler
    {
        void NRealStorageContext_OnRemoveParentStorage(IStorage storage);
        void NRealStorageContext_OnAddParentStorage(IStorage storage);
    }
}

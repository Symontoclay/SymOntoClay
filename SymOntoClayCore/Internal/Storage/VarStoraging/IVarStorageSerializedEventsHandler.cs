namespace SymOntoClay.Core.Internal.Storage.VarStoraging
{
    public interface IVarStorageSerializedEventsHandler
    {
        void NRealStorageContext_OnRemoveParentStorage(IStorage storage);
        void NRealStorageContext_OnAddParentStorage(IStorage storage);
    }
}

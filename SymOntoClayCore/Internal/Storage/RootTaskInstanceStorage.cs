namespace SymOntoClay.Core.Internal.Storage
{
    public class RootTaskInstanceStorage : RealStorage
    {
        public RootTaskInstanceStorage(RealStorageSettings settings)
            : base(KindOfStorage.RootTaskInstance, settings)
        {
        }
    }
}

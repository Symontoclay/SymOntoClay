namespace SymOntoClay.Core.Internal.Storage
{
    public class AppInstanceStorage : RealStorage
    {
        public AppInstanceStorage(RealStorageSettings settings)
            : base(KindOfStorage.AppInstance, settings)
        {
        }
    }
}

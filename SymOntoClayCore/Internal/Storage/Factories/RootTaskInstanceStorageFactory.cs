namespace SymOntoClay.Core.Internal.Storage.Factories
{
    public class RootTaskInstanceStorageFactory : IStorageFactory
    {
        /// <inheritdoc/>
        public IStorage CreateStorage(RealStorageSettings settings)
        {
            return new RootTaskInstanceStorage(settings);
        }
    }
}

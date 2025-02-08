namespace SymOntoClay.Core.Internal.Storage.Factories
{
    public class CompoundTaskInstanceStorageFactory : IStorageFactory
    {
        /// <inheritdoc/>
        public IStorage CreateStorage(RealStorageSettings settings)
        {
            return new CompoundTaskInstanceStorage(settings);
        }
    }
}

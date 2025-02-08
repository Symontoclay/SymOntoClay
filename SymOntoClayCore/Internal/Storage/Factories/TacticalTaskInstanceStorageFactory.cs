namespace SymOntoClay.Core.Internal.Storage.Factories
{
    public class TacticalTaskInstanceStorageFactory : IStorageFactory
    {
        /// <inheritdoc/>
        public IStorage CreateStorage(RealStorageSettings settings)
        {
            return new TacticalTaskInstanceStorage(settings);
        }
    }
}

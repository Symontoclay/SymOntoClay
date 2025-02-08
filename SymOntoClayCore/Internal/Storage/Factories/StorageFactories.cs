namespace SymOntoClay.Core.Internal.Storage.Factories
{
    public class StorageFactories: IStorageFactories
    {
        private readonly IStorageFactory _appInstanceStorageFactory = new AppInstanceStorageFactory();
        private readonly IStorageFactory _objectStorageFactory = new ObjectStorageFactory();
        private readonly IStorageFactory _stateStorageFactory = new StateStorageFactory();
        private readonly IStorageFactory _actionStorageFactory = new ActionStorageFactory();
        private readonly IStorageFactory _rootTaskInstanceStorageFactory = new RootTaskInstanceStorageFactory();
        private readonly IStorageFactory _strategicTaskInstanceStorageFactory = new StrategicTaskInstanceStorageFactory();
        private readonly IStorageFactory _tacticalTaskInstanceStorageFactory = new TacticalTaskInstanceStorageFactory();
        private readonly IStorageFactory _compoundTaskInstanceStorageFactory = new CompoundTaskInstanceStorageFactory();

        /// <inheritdoc/>
        public IStorageFactory AppInstanceStorageFactory => _appInstanceStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory ObjectStorageFactory => _objectStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory StateStorageFactory => _stateStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory ActionStorageFactory => _actionStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory RootTaskInstanceStorageFactory => _rootTaskInstanceStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory StrategicTaskInstanceStorageFactory => _strategicTaskInstanceStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory TacticalTaskInstanceStorageFactory => _tacticalTaskInstanceStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory CompoundTaskInstanceStorageFactory => _compoundTaskInstanceStorageFactory;
    }
}

namespace SymOntoClay.BaseTestLib
{
    public class NewBehaviorTestEngineInstanceBuilder: INewBehaviorTestEngineInstanceBuilder
    {
        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder UseDefaultRootDirectory()
        {
            _rootDir = AdvancedBehaviorTestEngineInstance.RoorDir;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder UseCustomRootDirectory(string rootDir)
        {
            _rootDir = rootDir;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder DontUseStandardLibrary()
        {
            _useStandardLibrary = KindOfUsingStandardLibrary.None;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder SetUsingStandardLibrary(KindOfUsingStandardLibrary useStandardLibrary)
        {
            _useStandardLibrary = useStandardLibrary;

            return this;
        }

        INewBehaviorTestEngineInstanceBuilder DontUseTimeoutToEnd();
        INewBehaviorTestEngineInstanceBuilder UseDefaultTimeoutToEnd();
        INewBehaviorTestEngineInstanceBuilder UseTimeoutToEnd(int timeoutToEnd);
        INewBehaviorTestEngineInstanceBuilder DisableHtnPlanExecution();
        INewBehaviorTestEngineInstanceBuilder EnableHtnPlanExecution();
        INewBehaviorTestEngineInstanceBuilder SethHtnIterationsMaxCount(int htnIterationsMaxCount);
        INewBehaviorTestEngineInstanceBuilder DontUsePlatformListener();
        INewBehaviorTestEngineInstanceBuilder UsePlatformListener(object platformListener);

        private string _rootDir = AdvancedBehaviorTestEngineInstance.RoorDir;
        private KindOfUsingStandardLibrary _useStandardLibrary = KindOfUsingStandardLibrary.None;
    }
}

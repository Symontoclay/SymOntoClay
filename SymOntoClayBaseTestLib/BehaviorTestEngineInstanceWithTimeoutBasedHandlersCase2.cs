namespace SymOntoClay.BaseTestLib
{
    public class BehaviorTestEngineInstanceWithTimeoutBasedHandlersCase2 : BaseBehaviorTestEngineInstance
    {
        public BehaviorTestEngineInstanceWithTimeoutBasedHandlersCase2(string fileContent,
            object platformListener,
            Action<string> logHandler,
            Action<string> errorHandler,
            int timeoutToEnd,
            string rootDir,
            KindOfUsingStandardLibrary useStandardLibrary,
            int? htnIterationsMaxCount,
            bool enableNLP,
            bool enableCategories,
            List<string> categories)
            : base(fileContent, rootDir, useStandardLibrary, enableNLP, enableCategories, categories)
        {
            _platformListener = platformListener;
            _logHandler = logHandler;
            _errorHandler = errorHandler;
            _timeoutToEnd = timeoutToEnd;
            _htnIterationsMaxCount = htnIterationsMaxCount;
        }

        private readonly object _platformListener;
        private readonly Action<string> _logHandler;
        private readonly Action<string> _errorHandler;
        private readonly int _timeoutToEnd;
        private readonly int? _htnIterationsMaxCount;

        /// <inheritdoc/>
        public override bool Run()
        {
            var result = true;

            _internalInstance.CreateAndStartNPC(message => {
                    _logHandler(message);
                },
                errorMsg => {
                    result = false;
                    _errorHandler(errorMsg);
                },
                _platformListener,
                _htnIterationsMaxCount);

            Thread.Sleep(_timeoutToEnd);

            return result;
        }
    }
}

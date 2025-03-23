namespace SymOntoClay.BaseTestLib
{
    public class BehaviorTestEngineInstanceWithTimeoutBasedHandlersCase1 : BaseBehaviorTestEngineInstance
    {
        public BehaviorTestEngineInstanceWithTimeoutBasedHandlersCase1(string fileContent,
            object platformListener,
            Action<int, string> logHandler,
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
        private readonly Action<int, string> _logHandler;
        private readonly Action<string> _errorHandler;
        private readonly int _timeoutToEnd;
        private readonly int? _htnIterationsMaxCount;

        /// <inheritdoc/>
        public override bool Run()
        {
            var n = 0;

            var result = true;

            _internalInstance.CreateAndStartNPC(message => {
                    n++;
                    _logHandler(n, message);
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

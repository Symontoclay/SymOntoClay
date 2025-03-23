namespace SymOntoClay.BaseTestLib
{
    public class BehaviorTestEngineInstanceWithTimeoutBasedOnlyLogHandlerCase1 : BaseBehaviorTestEngineInstance
    {
        public BehaviorTestEngineInstanceWithTimeoutBasedOnlyLogHandlerCase1(string fileContent,
            object platformListener,
            Action<int, string> logHandler,
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
            _timeoutToEnd = timeoutToEnd;
            _htnIterationsMaxCount = htnIterationsMaxCount;
        }

        private readonly object _platformListener;
        private readonly Action<int, string> _logHandler;
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
                },
                _platformListener,
                _htnIterationsMaxCount);

            Thread.Sleep(_timeoutToEnd);

            return result;
        }
    }
}

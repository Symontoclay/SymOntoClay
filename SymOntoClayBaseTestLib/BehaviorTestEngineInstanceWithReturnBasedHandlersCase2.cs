namespace SymOntoClay.BaseTestLib
{
    public class BehaviorTestEngineInstanceWithReturnBasedHandlersCase2 : BaseBehaviorTestEngineInstance
    {
        public BehaviorTestEngineInstanceWithReturnBasedHandlersCase2(string fileContent,
            object platformListener,
            Func<string, bool> logHandler,
            Action<string> errorHandler,
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
            _htnIterationsMaxCount = htnIterationsMaxCount;
        }

        private readonly object _platformListener;
        private readonly Func<string, bool> _logHandler;
        private readonly Action<string> _errorHandler;
        private readonly int? _htnIterationsMaxCount;

        /// <inheritdoc/>
        public override bool Run()
        {
            var result = true;

            var needRun = true;

            var lastIterationTime = DateTime.Now;

            _internalInstance.CreateAndStartNPC(message => {
                lastIterationTime = DateTime.Now;

                if (!_logHandler(message))
                {
                    needRun = false;
                }
            },
                errorMsg => {
                    result = false;
                    needRun = false;
                    _errorHandler(errorMsg);
                },
                _platformListener,
                _htnIterationsMaxCount);

            while (needRun)
            {
                if (BehaviorTestEngineInstanceHelper.ShouldBeCancelledByTimeout(lastIterationTime))
                {
                    throw new TimeoutException("6C903E7A-68E6-4454-B5B5-2B2DB5729D74");
                }

                Thread.Sleep(100);
            }

            return result;
        }
    }
}

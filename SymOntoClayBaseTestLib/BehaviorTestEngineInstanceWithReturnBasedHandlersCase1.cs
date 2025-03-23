namespace SymOntoClay.BaseTestLib
{
    public class BehaviorTestEngineInstanceWithReturnBasedHandlersCase1 : BaseBehaviorTestEngineInstance
    {
        public BehaviorTestEngineInstanceWithReturnBasedHandlersCase1(string fileContent,
            object platformListener,
            Func<int, string, bool> logHandler,
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
        private readonly Func<int, string, bool> _logHandler;
        private readonly Action<string> _errorHandler;
        private readonly int? _htnIterationsMaxCount;

        /// <inheritdoc/>
        public override bool Run()
        {
            var n = 0;

            var result = true;

            var needRun = true;

            _internalInstance.CreateAndStartNPC(message => {
                    n++;
                    if (!_logHandler(n, message))
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
                Thread.Sleep(100);
            }

            return result;
        }
    }
}

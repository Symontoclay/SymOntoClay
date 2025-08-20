namespace SymOntoClay.BaseTestLib
{
    public class BehaviorTestEngineInstanceWithReturnBasedOnlyLogHandlerCase2 : BaseBehaviorTestEngineInstance
    {
        public BehaviorTestEngineInstanceWithReturnBasedOnlyLogHandlerCase2(string fileContent,
            object platformListener,
            Func<string, bool> logHandler,
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
            _htnIterationsMaxCount = htnIterationsMaxCount;
        }

        private readonly object _platformListener;
        private readonly Func<string, bool> _logHandler;
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
                },
                _platformListener,
                _htnIterationsMaxCount);

            while (needRun)
            {
                if (BehaviorTestEngineInstanceHelper.ShouldBeCancelledByTimeout(lastIterationTime))
                {
                    throw new TimeoutException("1D2BCAF6-FEB1-4CCF-AA80-82AE3E11582F");
                }

                Thread.Sleep(100);
            }

            return result;
        }
    }
}

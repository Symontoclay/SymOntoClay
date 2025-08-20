namespace SymOntoClay.BaseTestLib
{
    public class BehaviorTestEngineInstanceWithReturnBasedOnlyLogHandlerCase1: BaseBehaviorTestEngineInstance
    {
        public BehaviorTestEngineInstanceWithReturnBasedOnlyLogHandlerCase1(string fileContent,
            object platformListener,
            Func<int, string, bool> logHandler,
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
        private readonly Func<int, string, bool> _logHandler;
        private readonly int? _htnIterationsMaxCount;

        /// <inheritdoc/>
        public override bool Run()
        {
            var n = 0;
            
            var result = true;

            var needRun = true;
            var lastIterationTime = DateTime.Now;

            _internalInstance.CreateAndStartNPC(message => {
                    n++;
                    lastIterationTime = DateTime.Now;

                    if (!_logHandler(n, message))
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
                if(BehaviorTestEngineInstanceHelper.ShouldBeCancelledByTimeout(lastIterationTime))
                {
                    throw new TimeoutException("1E2CCFED-5386-4CF9-9748-9228D1A23F63");
                }

                Thread.Sleep(100);
            }

            return result;
        }
    }
}

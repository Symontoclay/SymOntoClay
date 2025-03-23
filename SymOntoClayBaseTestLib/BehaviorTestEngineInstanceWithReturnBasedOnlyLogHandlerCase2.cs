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

            _internalInstance.CreateAndStartNPC(message => {
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
                Thread.Sleep(100);
            }

            return result;
        }
    }
}

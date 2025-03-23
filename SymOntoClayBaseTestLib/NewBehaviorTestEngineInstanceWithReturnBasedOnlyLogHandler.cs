using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SymOntoClay.BaseTestLib
{
    public class NewBehaviorTestEngineInstanceWithReturnBasedOnlyLogHandler: NewBaseBehaviorTestEngineInstance
    {
        public NewBehaviorTestEngineInstanceWithReturnBasedOnlyLogHandler(string fileContent,
            object platformListener,
            Func<int, string, bool> logHandler,
            string rootDir,
            KindOfUsingStandardLibrary useStandardLibrary,
            int? htnIterationsMaxCount)
            : base(fileContent, rootDir, useStandardLibrary)
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

            _internalInstance.CreateAndStartNPC(message => {
                    n++;
                    if (!_logHandler(n, message))
                    {
                        needRun = false;
                    }
                },
                errorMsg => throw new Exception(errorMsg),
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

using NLog;

namespace SymOntoClay.BaseTestLib
{
    public class BehaviorTestEngineInstanceWithTimeoutBasedOnlyLogHandlerCase1 : BaseBehaviorTestEngineInstance
    {
        private static readonly Logger _globalLogger = LogManager.GetCurrentClassLogger();

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
#if DEBUG
            //_globalLogger.Info($"fileContent = {fileContent}");
            //_globalLogger.Info($"htnIterationsMaxCount = {htnIterationsMaxCount}");
            //_globalLogger.Info($"platformListener == null = {platformListener == null}");
#endif

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
#if DEBUG
                    //_globalLogger.Info($"errorMsg = {errorMsg}");
#endif
                    result = false;
                },
                _platformListener,
                _htnIterationsMaxCount);

            Thread.Sleep(_timeoutToEnd);

            return result;
        }
    }
}

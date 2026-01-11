/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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

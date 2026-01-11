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

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
    public class BehaviorTestEngineInstanceBuilder: IBehaviorTestEngineInstanceBuilder
    {
        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder UseDefaultRootDirectory()
        {
            _rootDir = AdvancedBehaviorTestEngineInstance.RoorDir;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder UseCustomRootDirectory(string rootDir)
        {
            _rootDir = rootDir;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder DontUseStandardLibrary()
        {
            _useStandardLibrary = KindOfUsingStandardLibrary.None;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder SetUsingStandardLibrary(KindOfUsingStandardLibrary useStandardLibrary)
        {
            _useStandardLibrary = useStandardLibrary;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder DontUseTimeoutToEnd()
        {
            _timeoutToEnd = null;
            _usedReturnBasedPart = true;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder UseDefaultTimeoutToEnd()
        {
            _timeoutToEnd = _defaultTimeoutToEnd;
            _usedTimeoutBasedPart = true;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder UseTimeoutToEnd(int timeoutToEnd)
        {
            _timeoutToEnd = timeoutToEnd;
            _usedTimeoutBasedPart = true;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder DisableHtnPlanExecution()
        {
            _htnIterationsMaxCount = 0;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder EnableHtnPlanExecution()
        {
            _htnIterationsMaxCount = null;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder SethHtnIterationsMaxCount(int htnIterationsMaxCount)
        {
            _htnIterationsMaxCount = htnIterationsMaxCount;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder EnableNlp()
        {
            _enableNLP = true;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder DisableNlp()
        {
            _enableNLP = false;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder DisableCategories()
        {
            _enableCategories = false;
            _categories = null;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder UseCategories(List<string> categories)
        {
            _enableCategories = true;
            _categories = categories;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder DontUsePlatformListener()
        {
            _platformListener = new object();

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder UsePlatformListener(object platformListener)
        {
            _platformListener = platformListener;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder TestedCode(string fileContent)
        {
            _fileContent = fileContent;

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder LogHandler(Action<int, string> handler)
        {
            _timeoutBasedLogHandler1 = handler;

            _logHandlerIsDefined = true;
            _usedTimeoutBasedPart = true;
            SetDefaultTimeoutIfNeeds();

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder LogHandler(Func<int, string, bool> handler)
        {
            _returnBasedLogHandler1 = handler;

            _logHandlerIsDefined = true;
            _usedReturnBasedPart = true;
            SetNullTimeoutIfNeeds();

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder LogHandler(Action<string> handler)
        {
            _timeoutBasedLogHandler2 = handler;

            _logHandlerIsDefined = true;
            _usedTimeoutBasedPart = true;
            SetDefaultTimeoutIfNeeds();

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder LogHandler(Func<string, bool> handler)
        {
            _returnBasedLogHandler2 = handler;

            _logHandlerIsDefined = true;
            _usedReturnBasedPart = true;
            SetNullTimeoutIfNeeds();

            return this;
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstanceBuilder ErrorHandler(Action<string> handler)
        {
            _errorHandler = handler;

            _errorHandlerIsDefined = true;
            SetDefaultTimeoutIfNeeds();

            return this;
        }

        private void SetDefaultTimeoutIfNeeds()
        {
            if(!_timeoutToEnd.HasValue)
            {
                _timeoutToEnd = _defaultTimeoutToEnd;
            }
        }

        private void SetNullTimeoutIfNeeds()
        {
            if(_timeoutToEnd.HasValue)
            {
                _timeoutToEnd = null;
            }
        }

        /// <inheritdoc/>
        public IBehaviorTestEngineInstance Build()
        {
            ValidateConfiguration();

            if(_usedTimeoutBasedPart)
            {
                if(_timeoutBasedLogHandler1 == null)
                {
                    if(_errorHandler == null)
                    {
                        return BuildTimeoutBasedOnlyLogHandlerCase2(_timeoutBasedLogHandler2);
                    }
                    else
                    {
                        return BuildTimeoutBasedCase2(_timeoutBasedLogHandler2, _errorHandler);
                    }
                }
                else
                {
                    if (_errorHandler == null)
                    {
                        return BuildTimeoutBasedOnlyLogHandlerCase1(_timeoutBasedLogHandler1);
                    }
                    else
                    {
                        return BuildTimeoutBasedCase1(_timeoutBasedLogHandler1, _errorHandler);
                    }
                }
            }
            else
            {
                if(_returnBasedLogHandler1 == null)
                {
                    if (_errorHandler == null)
                    {
                        return BuildReturnBasedOnlyLogHandlerCase2(_returnBasedLogHandler2);
                    }
                    else
                    {
                        return BuildReturnBasedCase2(_returnBasedLogHandler2, _errorHandler);
                    }
                }
                else
                {
                    if (_errorHandler == null)
                    {
                        return BuildReturnBasedOnlyLogHandlerCase1(_returnBasedLogHandler1);
                    }
                    else
                    {
                        return BuildReturnBasedCase1(_returnBasedLogHandler1, _errorHandler);
                    }
                }
            }
        }

        private void ValidateConfiguration()
        {
            if(string.IsNullOrWhiteSpace(_fileContent))
            {
                throw new Exception("Tested code is not defined.");
            }

            if(_usedTimeoutBasedPart && _usedReturnBasedPart)
            {
                throw new Exception("Timeout based and return based handlers are mixed.");
            }

            if(!_logHandlerIsDefined)
            {
                throw new Exception("Log handler is not defined.");
            }
        }

        private const int _defaultTimeoutToEnd = 5000;

        private string _rootDir = AdvancedBehaviorTestEngineInstance.RoorDir;
        private KindOfUsingStandardLibrary _useStandardLibrary = KindOfUsingStandardLibrary.None;
        private int? _timeoutToEnd;
        private int? _htnIterationsMaxCount;

        private bool _enableNLP;
        private bool _enableCategories;
        private List<string> _categories;

        private object _platformListener = new object();
        private string _fileContent;

        private bool _usedTimeoutBasedPart;
        private bool _usedReturnBasedPart;

        private bool _logHandlerIsDefined;
        private bool _errorHandlerIsDefined;

        private Action<int, string> _timeoutBasedLogHandler1;
        private Func<int, string, bool> _returnBasedLogHandler1;

        private Action<string> _timeoutBasedLogHandler2;
        private Func<string, bool> _returnBasedLogHandler2;

        private Action<string> _errorHandler;

#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private IBehaviorTestEngineInstance BuildTimeoutBasedOnlyLogHandlerCase1(Action<int, string> logHandler)
        {
            return new BehaviorTestEngineInstanceWithTimeoutBasedOnlyLogHandlerCase1(_fileContent,
                _platformListener,
                logHandler,
                _timeoutToEnd.Value,
                _rootDir,
                _useStandardLibrary,
                _htnIterationsMaxCount,
                _enableNLP,
                _enableCategories,
                _categories);
        }

        private IBehaviorTestEngineInstance BuildTimeoutBasedOnlyLogHandlerCase2(Action<string> logHandler)
        {
            return new BehaviorTestEngineInstanceWithTimeoutBasedOnlyLogHandlerCase2(_fileContent,
                _platformListener,
                logHandler,
                _timeoutToEnd.Value,
                _rootDir,
                _useStandardLibrary,
                _htnIterationsMaxCount,
                _enableNLP,
                _enableCategories,
                _categories);
        }

        private IBehaviorTestEngineInstance BuildReturnBasedOnlyLogHandlerCase1(Func<int, string, bool> logHandler)
        {
            return new BehaviorTestEngineInstanceWithReturnBasedOnlyLogHandlerCase1(_fileContent,
                _platformListener,
                logHandler,
                _rootDir,
                _useStandardLibrary,
                _htnIterationsMaxCount,
                _enableNLP,
                _enableCategories,
                _categories);
        }

        private IBehaviorTestEngineInstance BuildReturnBasedOnlyLogHandlerCase2(Func<string, bool> logHandler)
        {
            return new BehaviorTestEngineInstanceWithReturnBasedOnlyLogHandlerCase2(_fileContent,
                _platformListener,
                logHandler,
                _rootDir,
                _useStandardLibrary,
                _htnIterationsMaxCount,
                _enableNLP,
                _enableCategories,
                _categories);
        }

        private IBehaviorTestEngineInstance BuildTimeoutBasedCase1(Action<int, string> logHandler, Action<string> errorHandler)
        {
            return new BehaviorTestEngineInstanceWithTimeoutBasedHandlersCase1(_fileContent,
                _platformListener,
                logHandler,
                errorHandler,
                _timeoutToEnd.Value,
                _rootDir,
                _useStandardLibrary,
                _htnIterationsMaxCount,
                _enableNLP,
                _enableCategories,
                _categories);
        }

        private IBehaviorTestEngineInstance BuildTimeoutBasedCase2(Action<string> logHandler, Action<string> errorHandler)
        {
            return new BehaviorTestEngineInstanceWithTimeoutBasedHandlersCase2(_fileContent,
                _platformListener,
                logHandler,
                errorHandler,
                _timeoutToEnd.Value,
                _rootDir,
                _useStandardLibrary,
                _htnIterationsMaxCount,
                _enableNLP,
                _enableCategories,
                _categories);
        }

        private IBehaviorTestEngineInstance BuildReturnBasedCase1(Func<int, string, bool> logHandler, Action<string> errorHandler)
        {
            return new BehaviorTestEngineInstanceWithReturnBasedHandlersCase1(_fileContent,
                _platformListener,
                logHandler,
                errorHandler,
                _rootDir,
                _useStandardLibrary,
                _htnIterationsMaxCount,
                _enableNLP,
                _enableCategories,
                _categories);
        }

        private IBehaviorTestEngineInstance BuildReturnBasedCase2(Func<string, bool> logHandler, Action<string> errorHandler)
        {
            return new BehaviorTestEngineInstanceWithReturnBasedHandlersCase2(_fileContent,
                _platformListener,
                logHandler,
                errorHandler,
                _rootDir,
                _useStandardLibrary,
                _htnIterationsMaxCount,
                _enableNLP,
                _enableCategories,
                _categories);
        }
    }
}

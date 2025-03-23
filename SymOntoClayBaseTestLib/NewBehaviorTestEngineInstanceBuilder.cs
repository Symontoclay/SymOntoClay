namespace SymOntoClay.BaseTestLib
{
    public class NewBehaviorTestEngineInstanceBuilder: INewBehaviorTestEngineInstanceBuilder
    {
        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder UseDefaultRootDirectory()
        {
            _rootDir = AdvancedBehaviorTestEngineInstance.RoorDir;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder UseCustomRootDirectory(string rootDir)
        {
            _rootDir = rootDir;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder DontUseStandardLibrary()
        {
            _useStandardLibrary = KindOfUsingStandardLibrary.None;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder SetUsingStandardLibrary(KindOfUsingStandardLibrary useStandardLibrary)
        {
            _useStandardLibrary = useStandardLibrary;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder DontUseTimeoutToEnd()
        {
            _timeoutToEnd = null;
            _usedReturnBasedPart = true;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder UseDefaultTimeoutToEnd()
        {
            _timeoutToEnd = _defaultTimeoutToEnd;
            _usedTimeoutBasedPart = true;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder UseTimeoutToEnd(int timeoutToEnd)
        {
            _timeoutToEnd = timeoutToEnd;
            _usedTimeoutBasedPart = true;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder DisableHtnPlanExecution()
        {
            _htnIterationsMaxCount = 0;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder EnableHtnPlanExecution()
        {
            _htnIterationsMaxCount = null;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder SethHtnIterationsMaxCount(int htnIterationsMaxCount)
        {
            _htnIterationsMaxCount = htnIterationsMaxCount;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder EnableNlp()
        {
            _enableNLP = true;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder DisableNlp()
        {
            _enableNLP = false;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder DisableCategories()
        {
            _enableCategories = false;
            _categories = null;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder UseCategories(List<string> categories)
        {
            _enableCategories = true;
            _categories = categories;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder DontUsePlatformListener()
        {
            _platformListener = new object();

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder UsePlatformListener(object platformListener)
        {
            _platformListener = platformListener;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder TestedCode(string fileContent)
        {
            _fileContent = fileContent;

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder LogHandler(Action<int, string> handler)
        {
            _timeoutBasedLogHandler1 = handler;

            _logHandlerIsDefined = true;
            _usedTimeoutBasedPart = true;
            SetDefaultTimeoutIfNeeds();

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder LogHandler(Func<int, string, bool> handler)
        {
            _returnBasedLogHandler1 = handler;

            _logHandlerIsDefined = true;
            _usedReturnBasedPart = true;
            SetNullTimeoutIfNeeds();

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder LogHandler(Action<string> handler)
        {
            _timeoutBasedLogHandler2 = handler;

            _logHandlerIsDefined = true;
            _usedTimeoutBasedPart = true;
            SetDefaultTimeoutIfNeeds();

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder LogHandler(Func<string, bool> handler)
        {
            _returnBasedLogHandler2 = handler;

            _logHandlerIsDefined = true;
            _usedReturnBasedPart = true;
            SetNullTimeoutIfNeeds();

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder ErrorHandler(Action<string> handler)
        {
            _timeoutBasedErrorHandler = handler;

            _errorHandlerIsDefined = true;
            _usedTimeoutBasedPart = true;
            SetDefaultTimeoutIfNeeds();

            return this;
        }

        /// <inheritdoc/>
        public INewBehaviorTestEngineInstanceBuilder ErrorHandler(Func<string, bool> handler)
        {
            _returnBasedErrorHandler = handler;

            _errorHandlerIsDefined = true;
            _usedReturnBasedPart = true;
            SetNullTimeoutIfNeeds();

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
        public INewBehaviorTestEngineInstance Build()
        {
            ValidateConfiguration();

            if(_usedTimeoutBasedPart)
            {
                if(_timeoutBasedLogHandler1 == null)
                {
                    if(_timeoutBasedErrorHandler == null)
                    {
                        return BuildTimeoutBasedOnlyLogHandlerCase2(_timeoutBasedLogHandler2);
                    }
                    else
                    {
                        return BuildTimeoutBasedCase2(_timeoutBasedLogHandler2, _timeoutBasedErrorHandler);
                    }
                }
                else
                {
                    if (_timeoutBasedErrorHandler == null)
                    {
                        return BuildTimeoutBasedOnlyLogHandlerCase1(_timeoutBasedLogHandler1);
                    }
                    else
                    {
                        return BuildTimeoutBasedCase1(_timeoutBasedLogHandler1, _timeoutBasedErrorHandler);
                    }
                }
            }
            else
            {
                if(_returnBasedLogHandler1 == null)
                {
                    if (_returnBasedErrorHandler == null)
                    {
                        return BuildReturnBasedOnlyLogHandlerCase2(_returnBasedLogHandler2);
                    }
                    else
                    {
                        return BuildReturnBasedCase2(_returnBasedLogHandler2, _returnBasedErrorHandler);
                    }
                }
                else
                {
                    if (_returnBasedErrorHandler == null)
                    {
                        return BuildReturnBasedOnlyLogHandlerCase1(_returnBasedLogHandler1);
                    }
                    else
                    {
                        return BuildReturnBasedCase1(_returnBasedLogHandler1, _returnBasedErrorHandler);
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

        private Action<string> _timeoutBasedErrorHandler;
        private Func<string, bool> _returnBasedErrorHandler;

#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private INewBehaviorTestEngineInstance BuildTimeoutBasedOnlyLogHandlerCase1(Action<int, string> logHandler)
        {
            throw new NotImplementedException();
        }

        private INewBehaviorTestEngineInstance BuildTimeoutBasedOnlyLogHandlerCase2(Action<string> logHandler)
        {
            throw new NotImplementedException("B741FB62-A8BE-434F-9533-94BF94242F80");
        }

        private INewBehaviorTestEngineInstance BuildReturnBasedOnlyLogHandlerCase1(Func<int, string, bool> logHandler)
        {
            return new NewBehaviorTestEngineInstanceWithReturnBasedOnlyLogHandler(_fileContent,
                _platformListener,
                logHandler,
                _rootDir,
                _useStandardLibrary,
                _htnIterationsMaxCount,
                _enableNLP,
                _enableCategories,
                _categories);
        }

        private INewBehaviorTestEngineInstance BuildReturnBasedOnlyLogHandlerCase2(Func<string, bool> logHandler)
        {
            throw new NotImplementedException();
        }

        private INewBehaviorTestEngineInstance BuildTimeoutBasedCase1(Action<int, string> logHandler, Action<string> errorHandler)
        {
            throw new NotImplementedException();
        }

        private INewBehaviorTestEngineInstance BuildTimeoutBasedCase2(Action<string> logHandler, Action<string> errorHandler)
        {
            throw new NotImplementedException();
        }

        private INewBehaviorTestEngineInstance BuildReturnBasedCase1(Func<int, string, bool> logHandler, Func<string, bool> errorHandler)
        {
            throw new NotImplementedException();
        }

        private INewBehaviorTestEngineInstance BuildReturnBasedCase2(Func<string, bool> logHandler, Func<string, bool> errorHandler)
        {
            throw new NotImplementedException();
        }
    }
}

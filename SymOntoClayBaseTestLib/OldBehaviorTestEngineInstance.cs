/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
    [Obsolete("Use Builder")]
    public class OldBehaviorTestEngineInstance : IDisposable
    {
        public const int DefaultTimeoutToEnd = 5000;

#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public OldBehaviorTestEngineInstance()
            : this(AdvancedBehaviorTestEngineInstance.RoorDir)
        {
        }
        
        public OldBehaviorTestEngineInstance(KindOfUsingStandardLibrary useStandardLibrary)
            : this(AdvancedBehaviorTestEngineInstance.RoorDir, useStandardLibrary)
        {
        }

        private AdvancedBehaviorTestEngineInstance _internalInstance;

        public OldBehaviorTestEngineInstance(string rootDir, KindOfUsingStandardLibrary useStandardLibrary)
        {
            _internalInstance = new AdvancedBehaviorTestEngineInstance(rootDir, useStandardLibrary);
        }

        public OldBehaviorTestEngineInstance(string rootDir)
        {
            _internalInstance = new AdvancedBehaviorTestEngineInstance(rootDir);
        }

        public void WriteFile(string fileContent)
        {
            _internalInstance.WriteFile(fileContent);
        }

        public static bool Run(string fileContent, Action<int, string> logChannel, KindOfUsingStandardLibrary useStandardLibrary, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent, logChannel, useStandardLibrary, new object(), timeoutToEnd);
        }

        public static bool Run(string fileContent, Action<int, string> logChannel, int timeoutToEnd = DefaultTimeoutToEnd, int? htnPlanExecutionIterationsMaxCount = null)
        {
            return Run(fileContent, logChannel, new object(), timeoutToEnd, htnPlanExecutionIterationsMaxCount);
        }

        public static bool Run(string fileContent, Func<int, string, bool> logChannel, int? htnPlanExecutionIterationsMaxCount = null)
        {
            return Run(fileContent, logChannel, new object(), htnPlanExecutionIterationsMaxCount);
        }

        public static bool Run(string fileContent, Action<int, string> logChannel, KindOfUsingStandardLibrary useStandardLibrary, object platformListener,
            int timeoutToEnd = DefaultTimeoutToEnd, int? htnPlanExecutionIterationsMaxCount = null)
        {
            var n = 0;

            return Run(fileContent,
                message => { n++; logChannel(n, message); },
                error => { throw new Exception(error); },
                useStandardLibrary,
                platformListener,
                timeoutToEnd,
                htnPlanExecutionIterationsMaxCount);
        }

        public static bool Run(string fileContent, Func<int, string, bool> logChannel, KindOfUsingStandardLibrary useStandardLibrary, object platformListener,
            int? htnPlanExecutionIterationsMaxCount = null)
        {
            var n = 0;

            return Run(fileContent,
                message => 
                { 
                    n++; 
                    return logChannel(n, message); 
                },
                error => { throw new Exception(error); },
                useStandardLibrary,
                platformListener,
                htnPlanExecutionIterationsMaxCount);
        }

        public static bool Run(string fileContent, Action<int, string> logChannel, object platformListener,
            int timeoutToEnd = DefaultTimeoutToEnd, int? htnPlanExecutionIterationsMaxCount = null)
        {
            var n = 0;

            return Run(fileContent,
                message => { n++; logChannel(n, message); },
                error => { throw new Exception(error); },
                platformListener,
                timeoutToEnd,
                htnPlanExecutionIterationsMaxCount);
        }

        public static bool Run(string fileContent, Func<int, string, bool> logChannel, object platformListener,
            int? htnPlanExecutionIterationsMaxCount = null)
        {
            var n = 0;

            return Run(fileContent,
                message => 
                { 
                    n++; 
                    return logChannel(n, message); 
                },
                error => { throw new Exception(error); },
                platformListener,
                htnPlanExecutionIterationsMaxCount);
        }

        public static bool Run(string fileContent, Action<string> logChannel, KindOfUsingStandardLibrary useStandardLibrary, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent,
                message => { logChannel(message); },
                error => { throw new Exception(error); },
                useStandardLibrary,
                timeoutToEnd);
        }

        public static bool Run(string fileContent, Func<string, bool> logChannel, KindOfUsingStandardLibrary useStandardLibrary)
        {
            return Run(fileContent,
                message => { return logChannel(message); },
                error => { throw new Exception(error); },
                useStandardLibrary);
        }

        public static bool Run(string fileContent, Action<string> logChannel, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent,
                message => { logChannel(message); },
                error => { throw new Exception(error); },
                timeoutToEnd);
        }

        public static bool Run(string fileContent, Func<string, bool> logChannel)
        {
            return Run(fileContent,
                message => { return logChannel(message); },
                error => { throw new Exception(error); });
        }

        public static bool Run(string fileContent, Action<string> logChannel, Action<string> error, KindOfUsingStandardLibrary useStandardLibrary, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent, logChannel, error, useStandardLibrary, new object(), timeoutToEnd);
        }

        public static bool Run(string fileContent, Func<string, bool> logChannel, Func<string, bool> error, KindOfUsingStandardLibrary useStandardLibrary)
        {
            return Run(fileContent, logChannel, error, useStandardLibrary, new object());
        }

        public static bool Run(string fileContent, Action<string> logChannel, Action<string> error, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent, logChannel, error, new object(), timeoutToEnd);
        }

        public static bool Run(string fileContent, Func<string, bool> logChannel, Func<string, bool> error)
        {
            return Run(fileContent, logChannel, error, new object());
        }

        public static bool Run(string fileContent, Action<string> logChannel, Action<string> error, KindOfUsingStandardLibrary useStandardLibrary, object platformListener,
            int timeoutToEnd = DefaultTimeoutToEnd, int? htnPlanExecutionIterationsMaxCount = null)
        {
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                throw new Exception("Argument 'fileContent' can not be null or empty!");
            }

            using (var behaviorTestEngineInstance = new OldBehaviorTestEngineInstance(useStandardLibrary))
            {
                behaviorTestEngineInstance.WriteFile(fileContent);

                return behaviorTestEngineInstance.Run(timeoutToEnd, 
                    htnPlanExecutionIterationsMaxCount,
                    message => { logChannel(message); },
                    errorMsg => { error(errorMsg); },
                    platformListener);
            }
        }

        public static bool Run(string fileContent, Func<string, bool> logChannel, Func<string, bool> error, KindOfUsingStandardLibrary useStandardLibrary, object platformListener,
            int? htnPlanExecutionIterationsMaxCount = null)
        {
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                throw new Exception("Argument 'fileContent' can not be null or empty!");
            }

            using (var behaviorTestEngineInstance = new OldBehaviorTestEngineInstance(useStandardLibrary))
            {
                behaviorTestEngineInstance.WriteFile(fileContent);

                return behaviorTestEngineInstance.Run(
                    htnPlanExecutionIterationsMaxCount,
                    message => { return logChannel(message); },
                    errorMsg => { return error(errorMsg); },
                    platformListener);
            }
        }

        public static bool Run(string fileContent, Action<string> logChannel, Action<string> error, object platformListener,
            int timeoutToEnd = DefaultTimeoutToEnd, int? htnPlanExecutionIterationsMaxCount = null)
        {
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                throw new Exception("Argument 'fileContent' can not be null or empty!");
            }

            using (var behaviorTestEngineInstance = new OldBehaviorTestEngineInstance())
            {
                behaviorTestEngineInstance.WriteFile(fileContent);

                return behaviorTestEngineInstance.Run(timeoutToEnd,
                    htnPlanExecutionIterationsMaxCount,
                    message => { logChannel(message); },
                    errorMsg => { error(errorMsg); },
                    platformListener);
            }
        }

        public static bool Run(string fileContent, Func<string, bool> logChannel, Func<string, bool> error, object platformListener,
            int? htnPlanExecutionIterationsMaxCount = null)
        {
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                throw new Exception("Argument 'fileContent' can not be null or empty!");
            }

            using (var behaviorTestEngineInstance = new OldBehaviorTestEngineInstance())
            {
                behaviorTestEngineInstance.WriteFile(fileContent);

                return behaviorTestEngineInstance.Run(
                    htnPlanExecutionIterationsMaxCount,
                    message => { return logChannel(message); },
                    errorMsg => { return error(errorMsg); },
                    platformListener);
            }
        }

        public bool Run(int timeoutToEnd, int? htnPlanExecutionIterationsMaxCount, Action<string> logChannel, Action<string> error)
        {
            return Run(timeoutToEnd, htnPlanExecutionIterationsMaxCount, logChannel, error, new object());
        }

        public bool Run(int? htnPlanExecutionIterationsMaxCount, Func<string, bool> logChannel, Func<string, bool> error)
        {
            return Run(htnPlanExecutionIterationsMaxCount, logChannel, error, new object());
        }

        public bool Run(int timeoutToEnd, int? htnPlanExecutionIterationsMaxCount, Action<string> logChannel, Action<string> error, object platformListener)
        {
            var result = true;

            _internalInstance.CreateAndStartNPC(message => { logChannel(message); },
                errorMsg => { result = false; error(errorMsg); },
                platformListener, 
                htnPlanExecutionIterationsMaxCount);

            Thread.Sleep(timeoutToEnd);

            return result;
        }

        public bool Run(int? htnPlanExecutionIterationsMaxCount, Func<string, bool> logChannel, Func<string, bool> error, object platformListener)
        {
            var result = true;

            var needRun = true;

            _internalInstance.CreateAndStartNPC(message => { 
                    if(!logChannel(message))
                    {
                        needRun = false;
                    }
                },
                errorMsg => { 
                    result = false;
                    if(error(errorMsg))
                    {
                        needRun = false;
                    }
                },
                platformListener,
                htnPlanExecutionIterationsMaxCount);

            while (needRun)
            {
                Thread.Sleep(100);
            }

            return result;
        }

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            _internalInstance.Dispose();
        }
    }
}

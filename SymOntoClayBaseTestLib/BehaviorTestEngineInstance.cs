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

using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.World;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.ProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SymOntoClay.BaseTestLib
{
    public class BehaviorTestEngineInstance : IDisposable
    {
        public const int DefaultTimeoutToEnd = 5000;

        public BehaviorTestEngineInstance()
            : this(AdvancedBehaviorTestEngineInstance.RoorDir)
        {
        }
        
        public BehaviorTestEngineInstance(KindOfUsingStandardLibrary useStandardLibrary)
            : this(AdvancedBehaviorTestEngineInstance.RoorDir, useStandardLibrary)
        {
        }

        private AdvancedBehaviorTestEngineInstance _internalInstance;

        public BehaviorTestEngineInstance(string rootDir, KindOfUsingStandardLibrary useStandardLibrary)
        {
            _internalInstance = new AdvancedBehaviorTestEngineInstance(rootDir, useStandardLibrary);
        }

        public BehaviorTestEngineInstance(string rootDir)
        {
            _internalInstance = new AdvancedBehaviorTestEngineInstance(rootDir);
        }

        public void WriteFile(string fileContent)
        {
            _internalInstance.WriteFile(fileContent);
        }

        public void WriteFile(string relativeFileName, string fileContent)
        {
            _internalInstance.WriteFile(relativeFileName, fileContent);
        }

        public static bool Run(string fileContent, Action<int, string> logChannel, KindOfUsingStandardLibrary useStandardLibrary, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent, logChannel, useStandardLibrary, new object(), timeoutToEnd);
        }

        public static bool Run(string fileContent, Action<int, string> logChannel, int timeoutToEnd = DefaultTimeoutToEnd, int? htnPlanExecutionIterationsMaxCount = null)
        {
            return Run(fileContent, logChannel, new object(), timeoutToEnd, htnPlanExecutionIterationsMaxCount);
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

        public static bool Run(string fileContent, Action<string> logChannel, KindOfUsingStandardLibrary useStandardLibrary, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent,
                message => { logChannel(message); },
                error => { throw new Exception(error); },
                useStandardLibrary,
                timeoutToEnd);
        }

        public static bool Run(string fileContent, Action<string> logChannel, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent,
                message => { logChannel(message); },
                error => { throw new Exception(error); },
                timeoutToEnd);
        }

        public static bool Run(string fileContent, Action<string> logChannel, Action<string> error, KindOfUsingStandardLibrary useStandardLibrary, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent, logChannel, error, useStandardLibrary, new object(), timeoutToEnd);
        }

        public static bool Run(string fileContent, Action<string> logChannel, Action<string> error, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent, logChannel, error, new object(), timeoutToEnd);
        }

        public static bool Run(string fileContent, Action<string> logChannel, Action<string> error, KindOfUsingStandardLibrary useStandardLibrary, object platformListener,
            int timeoutToEnd = DefaultTimeoutToEnd, int? htnPlanExecutionIterationsMaxCount = null)
        {
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                throw new Exception("Argument 'fileContent' can not be null or empty!");
            }

            using (var behaviorTestEngineInstance = new BehaviorTestEngineInstance(useStandardLibrary))
            {
                behaviorTestEngineInstance.WriteFile(fileContent);

                return behaviorTestEngineInstance.Run(timeoutToEnd, 
                    htnPlanExecutionIterationsMaxCount,
                    message => { logChannel(message); },
                    errorMsg => { error(errorMsg); },
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

            using (var behaviorTestEngineInstance = new BehaviorTestEngineInstance())
            {
                behaviorTestEngineInstance.WriteFile(fileContent);

                return behaviorTestEngineInstance.Run(timeoutToEnd,
                    htnPlanExecutionIterationsMaxCount,
                    message => { logChannel(message); },
                    errorMsg => { error(errorMsg); },
                    platformListener);
            }
        }

        public bool Run(int timeoutToEnd, int? htnPlanExecutionIterationsMaxCount, Action<string> logChannel, Action<string> error)
        {
            return Run(timeoutToEnd, htnPlanExecutionIterationsMaxCount, logChannel, error, new object());
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

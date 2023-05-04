/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.BaseTestLib;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners
{
    public abstract class BaseHostListener: ILoggedTestHostListener
    {
        public void AddOnMethodEnterSyncHandler(string methodName, Action handler)
        {
            //throw new NotImplementedException();
        }

        [SupportHostListenerMethod]
        protected void EmitOnMethodEnter(string methodName, string methodImplName)
        {
#if DEBUG
            _logger.Log($"methodName = {methodName}");
            _logger.Log($"methodImplName = {methodImplName}");
#endif

            var className = string.Empty;
            var methodName_1 = string.Empty;
            var framesToSkip = 0;

            while (true)
            {
                var frame = new StackFrame(framesToSkip, false);

                var method = frame.GetMethod();

                if(method == null)
                {
                    break;
                }

#if DEBUG
                _logger.Log($"method.Name = {method.Name}");
                _logger.Log($"method.GetType().FullName = {method.GetType().FullName}");
#endif

                var supportHostListenerMethodAttribute = method?.GetCustomAttribute<SupportHostListenerMethodAttribute>();

                if( supportHostListenerMethodAttribute != null )
                {
                    framesToSkip++;
                    continue;
                }

                var endPointInfo = EndpointDescriber.GetBaseEndpointInfo(method);

#if DEBUG
                _logger.Log($"endPointInfo = {endPointInfo}");
#endif

                var declaringType = method.DeclaringType;

                if (declaringType == null)
                {
                    break;
                }

                if (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                framesToSkip++;
                className = declaringType.FullName;
                methodName_1 = method.Name;
            }

            //throw new NotImplementedException();
        }

        protected static ToHumanizedStringJsonConverter _customConverter = new ToHumanizedStringJsonConverter();

        public void SetLogger(IEntityLogger logger)
        {
            _logger = logger;
        }

        protected IEntityLogger _logger;

        private static object _lockObj = new object();

        private static int _methodId;

        protected int GetMethodId()
        {
            lock (_lockObj)
            {
                _methodId++;
                return _methodId;
            }
        }

        [DebuggerHidden]
        protected static void Sleep(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            var delta = 10;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Thread.Sleep(delta);

                millisecondsTimeout = millisecondsTimeout - delta;

                if (millisecondsTimeout <= 0)
                {
                    return;
                }
            }
        }
    }
}

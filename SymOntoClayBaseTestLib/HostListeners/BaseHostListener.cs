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
using SymOntoClay.BaseTestLib.HostListeners.Handlers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
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
        public void AddEnterSyncHandler(string methodName, Action handler)
        {
            _onEnterHandlersRegistry.AddSyncHandler(methodName, handler);
        }

        public void AddOnEndPointEnterSyncHandler(string methodName, Action handler)
        {
            _onEnterHandlersRegistry.AddEndPointSyncHandler(methodName, handler);
        }

        public void AddMethodImplEnterSyncHandler(string methodName, Action handler)
        {
            _onEnterHandlersRegistry.AddMethodImplSyncHandler(methodName, handler);
        }

        public void AddEnterAsyncHandler(string methodName, Action handler)
        {
            _onEnterHandlersRegistry.AddAsyncHandler(methodName, handler);
        }

        public void AddEndPointEnterAsyncHandler(string methodName, Action handler)
        {
            _onEnterHandlersRegistry.AddEndPointAsyncHandler(methodName, handler);
        }

        public void AddMethodImplEnterAsyncHandler(string methodName, Action handler)
        {
            _onEnterHandlersRegistry.AddMethodImplAsyncHandler(methodName, handler);
        }

        public void AddLeaveSyncHandler(string methodName, Action handler)
        {
            _onLeaveHandlersRegistry.AddSyncHandler(methodName, handler);
        }

        public void AddOnEndPointLeaveSyncHandler(string methodName, Action handler)
        {
            _onLeaveHandlersRegistry.AddEndPointSyncHandler(methodName, handler);
        }

        public void AddMethodImplLeaveSyncHandler(string methodName, Action handler)
        {
            _onLeaveHandlersRegistry.AddMethodImplSyncHandler(methodName, handler);
        }

        public void AddLeaveAsyncHandler(string methodName, Action handler)
        {
            _onLeaveHandlersRegistry.AddAsyncHandler(methodName, handler);
        }

        public void AddEndPointLeaveAsyncHandler(string methodName, Action handler)
        {
            _onLeaveHandlersRegistry.AddEndPointAsyncHandler(methodName, handler);
        }

        public void AddMethodImplLeaveAsyncHandler(string methodName, Action handler)
        {
            _onLeaveHandlersRegistry.AddMethodImplAsyncHandler(methodName, handler);
        }

        public void RemoveHandler(string methodName, Action handler)
        {
            _onEnterHandlersRegistry.RemoveHandler(methodName, handler);
            _onLeaveHandlersRegistry.RemoveHandler(methodName, handler);
        }

        private HostListenerHandlersRegistry _onEnterHandlersRegistry = new HostListenerHandlersRegistry();
        private HostListenerHandlersRegistry _onLeaveHandlersRegistry = new HostListenerHandlersRegistry();

        [SupportHostListenerMethod]
        protected void EmitOnEnter()
        {
            var methodNamesResult = GetMethodNames();

            var endPointName = methodNamesResult.Item1;
            var methodName = methodNamesResult.Item2;

            _onEnterHandlersRegistry.Emit(endPointName, methodName);
        }

        [SupportHostListenerMethod]
        protected void EmitOnLeave()
        {
            var methodNamesResult = GetMethodNames();

            var endPointName = methodNamesResult.Item1;
            var methodName = methodNamesResult.Item2;

            _onLeaveHandlersRegistry.Emit(endPointName, methodName);
        }

        [SupportHostListenerMethod]
        private (string, string) GetMethodNames()
        {
            var framesToSkip = 0;

            while (true)
            {
                var frame = new StackFrame(framesToSkip, false);

                var method = frame.GetMethod();

                if (method == null)
                {
                    break;
                }

                var supportHostListenerMethodAttribute = method?.GetCustomAttribute<SupportHostListenerMethodAttribute>();

                if (supportHostListenerMethodAttribute != null)
                {
                    framesToSkip++;
                    continue;
                }

                var endPointInfo = EndpointDescriber.GetBaseEndpointInfo(method);

                if (endPointInfo == null)
                {
                    break;
                }

                return (endPointInfo.Name, endPointInfo.OriginalName);
            }

            return (string.Empty, string.Empty);
        }

        protected static ToHumanizedStringJsonConverter _customConverter = new ToHumanizedStringJsonConverter();

        public void SetLogger(IMonitorLogger logger)
        {
            //_logger = logger;
            _onEnterHandlersRegistry.SetLogger(logger);
        }

        //protected IMonitorLogger _logger;

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

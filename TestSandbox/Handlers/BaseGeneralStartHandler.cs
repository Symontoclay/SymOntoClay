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

using SymOntoClay.BaseTestLib;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.UnityAsset.Core;
using System;
using TestSandbox.Helpers;

namespace TestSandbox.Handlers
{
    public abstract class BaseGeneralStartHandler: IDisposable
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        protected BaseGeneralStartHandler()
            : this(new UnityTestEngineContextFactorySettings())
        {
        }

        protected BaseGeneralStartHandler(UnityTestEngineContextFactorySettings factorySettings)
        {
#if DEBUG
            //_globalLogger.Info($"factorySettings = {factorySettings}");
#endif

            _world = TstEngineContextHelper.CreateWorld(factorySettings);
        }

        protected static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        protected IWorld _world;
        protected IHumanoidNPC _npc;

        protected void CreateMainNPC(UnityTestEngineContextFactorySettings factorySettings)
        {
            _npc = CreateNPC(factorySettings);       
        }
        
        protected IHumanoidNPC CreateNPC(UnityTestEngineContextFactorySettings factorySettings)
        {
            return TstEngineContextHelper.CreateNPC(_world, factorySettings);
        }

        private bool _isDisposed;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            _world?.Dispose();
        }
    }
}

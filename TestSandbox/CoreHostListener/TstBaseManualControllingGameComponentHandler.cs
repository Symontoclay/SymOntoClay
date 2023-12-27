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

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject;
using SymOntoClay.DefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;

namespace TestSandbox.CoreHostListener
{
    public class TstBaseManualControllingGameComponentHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("E999ED21-C52E-4177-844D-1D68533EC648", "Begin");

            var complexContext = TstEngineContextHelper.CreateAndInitContext();

            var context = complexContext.EngineContext;
            var worldContext = complexContext.WorldContext;


            var platformListener = new TstPlatformHostListener();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Apps\PeaceKeeper\PeaceKeeper.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            _logger.Info("052D2883-886A-4C97-93A7-5EAB717F7871", $"npcSettings = {npcSettings}");

            var tstBaseManualControllingGameComponent = new TstBaseManualControllingGameComponent(npcSettings, worldContext);

            var methodName = NameHelper.CreateName("go");

            var command = new Command();
            command.Name = methodName;
            command.ParamsDict = new Dictionary<StrongIdentifierValue, Value>();

            var param1Value = new WaypointValue(25, 36, context);
            var param1Name = NameHelper.CreateName("to");

            command.ParamsDict[param1Name] = param1Value;

            var param2Value = new NumberValue(12.4);
            var param2Name = NameHelper.CreateName("speed");

            command.ParamsDict[param2Name] = param2Value;

            _logger.Info("E211BABC-4838-41D7-A23F-51C6BEBDD451", $"command = {command}");


            methodName = NameHelper.CreateName("shoot");

            command = new Command();
            command.Name = methodName;

            ExecuteCommand(tstBaseManualControllingGameComponent, command);

            var gameObjectSettings = new GameObjectSettings();
            gameObjectSettings.Id = "#120ED339-6313-459A-900D-92F809CEBDC5";

            var gunPlatformHostListener = new TstGunPlatformHostListener();

            gameObjectSettings.HostListener = gunPlatformHostListener;

            _logger.Info("B7D3223B-B02D-451E-AA93-C2971308297C", $"gameObjectSettings = {gameObjectSettings}");

            var gameObject = new GameObjectImplementation(gameObjectSettings, worldContext);

            tstBaseManualControllingGameComponent.AddToManualControl(gameObject, 12);

            methodName = NameHelper.CreateName("shoot");

            command = new Command();
            command.Name = methodName;

            ExecuteCommand(tstBaseManualControllingGameComponent, command);

            var manualControlledObjectsList = tstBaseManualControllingGameComponent.GetManualControlledObjects();

            _logger.Info("4C649695-DFA2-4D1C-9419-9FE5A9B7A9E0", $"manualControlledObjectsList.Count = {manualControlledObjectsList.Count}");

            tstBaseManualControllingGameComponent.RemoveFromManualControl(gameObject);

            manualControlledObjectsList = tstBaseManualControllingGameComponent.GetManualControlledObjects();

            _logger.Info("007DA309-84AB-4578-B2E1-59C7BC790337", $"manualControlledObjectsList.Count = {manualControlledObjectsList.Count}");

            _logger.Info("741D700F-3C2D-4E6B-B0AF-D0C70BA51F62", "End");
        }

        private void ExecuteCommand(TstBaseManualControllingGameComponent tstBaseManualControllingGameComponent, ICommand command)
        {
            var result = tstBaseManualControllingGameComponent.CreateProcess(_logger, string.Empty, command, null, null);

            _logger.Info("42C7917C-89F0-487E-A7E4-32A994FB98B8", $"result = {result}");

            if (result.IsSuccessful)
            {
                var processInfo = result.Process;

                _logger.Info("3C3BAA91-5333-44C4-955C-AE8EEAD4B83A", $"processInfo = {processInfo}");

                processInfo.Start(_logger);

                Thread.Sleep(5000);

                _logger.Info("70D31FCD-3A07-4CE7-AA79-1CEF5CFACD2A", "Cancel");

                processInfo.Cancel(_logger, "CCE32AB3-7969-4686-BFBD-7A9E65805273", ReasonOfChangeStatus.Unknown);

                Thread.Sleep(1000);
            }
        }
    }
}

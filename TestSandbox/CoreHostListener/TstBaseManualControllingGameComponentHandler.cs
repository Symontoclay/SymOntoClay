/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

namespace TestSandbox.CoreHostListener
{
    public class TstBaseManualControllingGameComponentHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var complexContext = TstEngineContextHelper.CreateAndInitContext();

            var context = complexContext.EngineContext;
            var worldContext = complexContext.WorldContext;

            //var dictionary = context.Dictionary;

            var platformListener = new TstPlatformHostListener();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            //npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Apps\PeaceKeeper\PeaceKeeper.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            _logger.Log($"npcSettings = {npcSettings}");

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

            _logger.Log($"command = {command}");

            //ExecuteCommand(tstBaseManualControllingGameComponent, command);

            methodName = NameHelper.CreateName("shoot");

            command = new Command();
            command.Name = methodName;

            ExecuteCommand(tstBaseManualControllingGameComponent, command);

            var gameObjectSettings = new GameObjectSettings();
            gameObjectSettings.Id = "#120ED339-6313-459A-900D-92F809CEBDC5";

            var gunPlatformHostListener = new TstGunPlatformHostListener();

            gameObjectSettings.HostListener = gunPlatformHostListener;

            _logger.Log($"gameObjectSettings = {gameObjectSettings}");

            var gameObject = new GameObjectImplementation(gameObjectSettings, worldContext);

            tstBaseManualControllingGameComponent.AddToManualControl(gameObject, 12);

            methodName = NameHelper.CreateName("shoot");

            command = new Command();
            command.Name = methodName;

            ExecuteCommand(tstBaseManualControllingGameComponent, command);

            var manualControlledObjectsList = tstBaseManualControllingGameComponent.GetManualControlledObjects();

            //_logger.Log($"manualControlledObjectsList = {manualControlledObjectsList.WriteListToString()}");
            _logger.Log($"manualControlledObjectsList.Count = {manualControlledObjectsList.Count}");

            tstBaseManualControllingGameComponent.RemoveFromManualControl(gameObject);

            manualControlledObjectsList = tstBaseManualControllingGameComponent.GetManualControlledObjects();

            _logger.Log($"manualControlledObjectsList.Count = {manualControlledObjectsList.Count}");

            _logger.Log("End");
        }

        private void ExecuteCommand(TstBaseManualControllingGameComponent tstBaseManualControllingGameComponent, ICommand command)
        {
            var result = tstBaseManualControllingGameComponent.CreateProcess(command);

            _logger.Log($"result = {result}");

            if (result.IsSuccessful)
            {
                var processInfo = result.Process;

                _logger.Log($"processInfo = {processInfo}");

                processInfo.Start();

                Thread.Sleep(5000);

                _logger.Log("Cancel");

                processInfo.Cancel();

                Thread.Sleep(1000);
            }
        }
    }
}

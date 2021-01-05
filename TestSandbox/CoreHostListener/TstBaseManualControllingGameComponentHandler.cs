/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject;
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

            var dictionary = context.Dictionary;

            var platformListener = new TstPlatformHostListener();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Apps\PeaceKeeper\PeaceKeeper.npc");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new TstPlatformSupport();

            _logger.Log($"npcSettings = {npcSettings}");

            var tstBaseManualControllingGameComponent = new TstBaseManualControllingGameComponent(npcSettings, worldContext);

            var methodName = NameHelper.CreateName("go", dictionary);

            var command = new Command();
            command.Name = methodName;
            command.ParamsDict = new Dictionary<StrongIdentifierValue, Value>();

            var param1Value = new WaypointValue(new Vector2(25, 36), context);
            var param1Name = NameHelper.CreateName("to", dictionary);

            command.ParamsDict[param1Name] = param1Value;

            var param2Value = new NumberValue(12.4);
            var param2Name = NameHelper.CreateName("speed", dictionary);

            command.ParamsDict[param2Name] = param2Value;

            _logger.Log($"command = {command}");

            //ExecuteCommand(tstBaseManualControllingGameComponent, command);

            methodName = NameHelper.CreateName("shoot", dictionary);

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

            methodName = NameHelper.CreateName("shoot", dictionary);

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

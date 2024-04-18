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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.MonoBehaviourTesting
{
    public class TstGameObject : TstMonoBehaviour
    {
        private readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        private IGameObject _gameObject;
        private string _id;

        public override void Awake()
        {
            _logger.Info("3D5AF0B4-068B-468D-86BD-B091862A9B0A", "Begin");

            var platformListener = new TstPlatformHostListener();

            var settings = new GameObjectSettings();

            _id = "#a";
            settings.Id = _id;
            settings.InstanceId = 2;
            settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Things\Barrel\Barrel.sobj");
            settings.HostListener = platformListener;

            _gameObject = WorldFactory.WorldInstance.GetGameObject(settings);

            _logger.Info("930C4944-BC9E-4F83-8258-04F3983BB94F", "End");
        }

        public override void Stop()
        {
            _logger.Info("EC1CDFDD-C28B-44A7-9EC9-CDF80092C928", "Begin");

            _gameObject.Dispose();

            _logger.Info("2845399D-87AB-4471-804F-A6DBED9435CC", "End");
        }
    }
}

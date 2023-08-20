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

using Newtonsoft.Json;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CoreHostListener
{
    public class TstPlatformHostListener
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        private static object _lockObj = new object();

        private static int _methodId;

        private int GetMethodId()
        {
            lock(_lockObj)
            {
                _methodId++;
                return _methodId;
            }
        }

        [DebuggerHidden]
        [BipedEndpoint("*")]
        public void GenericCall(CancellationToken cancellationToken, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            _logger.Info($"methodName = '{methodName}'");
            _logger.Info($"isNamedParameters = {isNamedParameters}");
            _logger.Info($"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented)}");
            _logger.Info($"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented)}");
        }













        [DebuggerHidden]
        [BipedEndpoint("Go2", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl_2(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] IEntity entity,
            float speed = 12)
        {
            var name = GetMethodId();

            _logger.Info($"Begin {name}");

            _logger.Info($"{name} entity.InstanceId = {entity.InstanceId}");
            _logger.Info($"{name} entity.Id = {entity.Id}");
            _logger.Info($"{name} entity.Position = {entity.Position}");
            _logger.Info($"{name} speed = {speed}");

            var n = 0;

            while (true)
            {
                n++;

                if (n > 10/*00*/)
                {
                    break;
                }

                Thread.Sleep(1000);

                _logger.Info($"{name} Hi! n = {n}");

                _logger.Info($"{name} Hi! cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

                cancellationToken.ThrowIfCancellationRequested();
            }


            _logger.Info($"End {name}");
        }

        [DebuggerHidden]
        [BipedEndpoint("Go3", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl_3(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            var name = GetMethodId();

            _logger.Info($"Begin {name}");

            _logger.Info($"{name} navTarget.Kind = {navTarget.Kind}");

            var entity = navTarget.Entity;

            _logger.Info($"{name} entity.InstanceId = {entity.InstanceId}");
            _logger.Info($"{name} entity.Id = {entity.Id}");
            _logger.Info($"{name} entity.Position = {entity.Position}");
            _logger.Info($"{name} speed = {speed}");

            var n = 0;

            while (true)
            {
                n++;

                if (n > 10/*00*/)
                {
                    break;
                }

                Thread.Sleep(1000);

                _logger.Info($"{name} Hi! n = {n}");

                _logger.Info($"{name} Hi! cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

                cancellationToken.ThrowIfCancellationRequested();
            }


            _logger.Info($"End {name}");
        }













        [DebuggerHidden]
        [BipedEndpoint("Take", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeImpl(CancellationToken cancellationToken, IEntity entity)
        {
            var name = GetMethodId();

            _logger.Info($"Begin {name}");

            _logger.Info($"(entity == null) = {entity == null}");

            if(entity == null)
            {
                return;
            }

            entity.Specify(EntityConstraints.CanBeTaken, /*EntityConstraints.OnlyVisible,*/ EntityConstraints.Nearest);

            entity.Resolve();

            _logger.Info($"{name} entity.InstanceId = {entity.InstanceId}");
            _logger.Info($"{name} entity.Id = {entity.Id}");
            _logger.Info($"{name} entity.Position = {entity.Position}");

            _logger.Info($"End {name}");
        }

        [BipedEndpoint]
        private void JumpImpl(CancellationToken cancellationToken)
        {
            _logger.Info("Begin");

            _logger.Info("End");
        }

        [BipedEndpoint("Aim", DeviceOfBiped.LeftHand, DeviceOfBiped.RightHand)]
        private void AimToImpl(CancellationToken cancellationToken)
        {
            _logger.Info("AimToImpl Begin");

            _logger.Info("AimToImpl End");
        }

        [FriendsEndpoints("Aim")]
        [BipedEndpoint("Fire", DeviceOfBiped.LeftHand, DeviceOfBiped.RightHand)]
        private void FireImpl(CancellationToken cancellationToken)
        {
            _logger.Info("FireImpl Begin");

            for(var i = 0; i < 100; i++)
            {
                _logger.Info($"FireImpl {i}");

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Info("FireImpl Cancel");
                    return;
                }

                Thread.Sleep(100);
            }

            _logger.Info("FireImpl End");
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float? direction)
        {
#if DEBUG
            var methodId = GetMethodId();

            _logger.Info($"RotateImpl Begin {methodId}; direction = {direction}");
#endif



#if DEBUG
            _logger.Info($"RotateImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateToEntityImpl(CancellationToken cancellationToken, IEntity entity,
            float speed = 2)
        {
            _logger.Info("RotateToEntityImpl Begin");
        }
    }
}

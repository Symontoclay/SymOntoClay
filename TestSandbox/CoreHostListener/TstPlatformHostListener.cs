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
        private static readonly IEntityLogger _logger = new LoggerImpementation();

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
            _logger.Log($"methodName = '{methodName}'");
            _logger.Log($"isNamedParameters = {isNamedParameters}");
            _logger.Log($"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented)}");
            _logger.Log($"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented)}");
        }













        [DebuggerHidden]
        [BipedEndpoint("Go2", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl_2(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] IEntity entity,
            float speed = 12)
        {
            var name = GetMethodId();

            _logger.Log($"Begin {name}");

            _logger.Log($"{name} entity.InstanceId = {entity.InstanceId}");
            _logger.Log($"{name} entity.Id = {entity.Id}");
            _logger.Log($"{name} entity.Position = {entity.Position}");
            _logger.Log($"{name} speed = {speed}");

            var n = 0;

            while (true)
            {
                n++;

                if (n > 10/*00*/)
                {
                    break;
                }

                Thread.Sleep(1000);

                _logger.Log($"{name} Hi! n = {n}");

                _logger.Log($"{name} Hi! cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

                cancellationToken.ThrowIfCancellationRequested();
            }


            _logger.Log($"End {name}");
        }

        [DebuggerHidden]
        [BipedEndpoint("Go3", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl_3(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            var name = GetMethodId();

            _logger.Log($"Begin {name}");

            _logger.Log($"{name} navTarget.Kind = {navTarget.Kind}");

            var entity = navTarget.Entity;

            _logger.Log($"{name} entity.InstanceId = {entity.InstanceId}");
            _logger.Log($"{name} entity.Id = {entity.Id}");
            _logger.Log($"{name} entity.Position = {entity.Position}");
            _logger.Log($"{name} speed = {speed}");

            var n = 0;

            while (true)
            {
                n++;

                if (n > 10/*00*/)
                {
                    break;
                }

                Thread.Sleep(1000);

                _logger.Log($"{name} Hi! n = {n}");

                _logger.Log($"{name} Hi! cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

                cancellationToken.ThrowIfCancellationRequested();
            }


            _logger.Log($"End {name}");
        }













        [DebuggerHidden]
        [BipedEndpoint("Take", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeImpl(CancellationToken cancellationToken, IEntity entity)
        {
            var name = GetMethodId();

            _logger.Log($"Begin {name}");

            _logger.Log($"(entity == null) = {entity == null}");

            if(entity == null)
            {
                return;
            }

            entity.Specify(EntityConstraints.CanBeTaken, /*EntityConstraints.OnlyVisible,*/ EntityConstraints.Nearest);

            entity.Resolve();

            _logger.Log($"{name} entity.InstanceId = {entity.InstanceId}");
            _logger.Log($"{name} entity.Id = {entity.Id}");
            _logger.Log($"{name} entity.Position = {entity.Position}");

            _logger.Log($"End {name}");
        }

        [BipedEndpoint]
        private void JumpImpl(CancellationToken cancellationToken)
        {
            _logger.Log("Begin");

            _logger.Log("End");
        }

        [BipedEndpoint("Aim", DeviceOfBiped.LeftHand, DeviceOfBiped.RightHand)]
        private void AimToImpl(CancellationToken cancellationToken)
        {
            _logger.Log("AimToImpl Begin");

            _logger.Log("AimToImpl End");
        }

        [FriendsEndpoints("Aim")]
        [BipedEndpoint("Fire", DeviceOfBiped.LeftHand, DeviceOfBiped.RightHand)]
        private void FireImpl(CancellationToken cancellationToken)
        {
            _logger.Log("FireImpl Begin");

            for(var i = 0; i < 100; i++)
            {
                _logger.Log($"FireImpl {i}");

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Log("FireImpl Cancel");
                    return;
                }

                Thread.Sleep(100);
            }

            _logger.Log("FireImpl End");
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float? direction)
        {
#if DEBUG
            var methodId = GetMethodId();

            _logger.Log($"RotateImpl Begin {methodId}; direction = {direction}");
#endif



#if DEBUG
            _logger.Log($"RotateImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateToEntityImpl(CancellationToken cancellationToken, IEntity entity,
            float speed = 2)
        {
            _logger.Log("RotateToEntityImpl Begin");
        }
    }
}

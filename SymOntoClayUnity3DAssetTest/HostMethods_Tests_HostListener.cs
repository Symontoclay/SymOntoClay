/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.Core.Tests.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class HostMethods_Tests_HostListener: ILoggedTestHostListener
    {
        public void SetLogger(IEntityLogger logger)
        {
            _logger = logger;
        }

        private IEntityLogger _logger;

        private static object _lockObj = new object();

        private static int _methodId;

        private int GetMethodId()
        {
            lock (_lockObj)
            {
                _methodId++;
                return _methodId;
            }
        }

        [BipedEndpoint("Boo", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void BooImpl(CancellationToken cancellationToken)
        {
            _logger.Log("BooImpl Begin");
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float? direction)
        {
            _logger.Log("RotateImpl Begin");
            _logger.Log(direction.ToString());
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateToEntityImpl(CancellationToken cancellationToken, IEntity entity,
            float speed = 2)
        {
            _logger.Log("RotateToEntityImpl Begin");
        }

        [BipedEndpoint("Take", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeImpl(CancellationToken cancellationToken, IEntity entity)
        {
            _logger.Log("TakeImpl Begin");
            _logger.Log($"(entity == null) = {entity == null}");

            if (entity == null)
            {
                return;
            }

            entity.Specify(EntityConstraints.CanBeTaken, EntityConstraints.Nearest);

            entity.Resolve();

            _logger.Log(entity.InstanceId.ToString());
            _logger.Log(entity.Id);
            _logger.Log(entity.Position.ToString());

            _logger.Log($"TakeImpl End");
        }

        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl_2(CancellationToken cancellationToken,
            [EndpointParam("Direction", KindOfEndpointParam.Position)] Vector3 point,
            float speed = 12)
        {
            _logger.Log($"GoToImpl_2");
            _logger.Log(point.ToString());
        }
    }
}

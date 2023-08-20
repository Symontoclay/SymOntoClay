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
using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSandbox.CoreHostListener
{
    public class TstBattleRoyaleHostListener : BaseHostListener
    {
        [DebuggerHidden]
        [BipedEndpoint("*")]
        public void GenericCall(CancellationToken cancellationToken, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            EmitOnEnter();

            _logger.Info("6D53EA4E-73A7-48D3-AD9F-5F3C6C6831BD", $"methodName = '{methodName}'");
            _logger.Info("1715AE7E-31BD-4FE3-BD1D-BCABDFA80F79", $"isNamedParameters = {isNamedParameters}");
            _logger.Info("14431596-FE75-4B14-A891-4C954C52D50E", $"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented, _customConverter)}");
            _logger.Info("581668DB-8C93-4531-A87A-E02FFA5FFA17", $"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented, _customConverter)}");

            EmitOnLeave();
        }

        private int? _remainingDistance;
        private int _wholeDistance = 1000;
        private int _goDelta = 100;

        [DebuggerHidden]
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            EmitOnEnter();

            _logger.Info("C4E768E8-D74D-4DC0-A3D4-F66C3FE40D56", $"GoToImpl Begin");
            _logger.Info("164A6E53-4663-4B62-B20A-7CF5F433E81D", $"navTarget.Kind = {navTarget.Kind}");
            var entity = navTarget.Entity;
            _logger.Info("1327F5B6-7A7F-4F0C-AE4B-F943667C7E1A", $"entity.InstanceId = {entity.InstanceId}");
            _logger.Info("EB150AE3-C540-4EF5-94D9-4BD54F06B155", $"entity.Id = {entity.Id}");
            _logger.Info("25EFB07E-9668-472F-9E74-D39BB8CD15EC", $"entity.Position = {entity.Position}");
            _logger.Info("505688E5-8375-41E6-A8DB-02026E4E1FB2", $"_remainingDistance = {_remainingDistance}");

            if (!_remainingDistance.HasValue)
            {
                _remainingDistance = _wholeDistance;
                Sleep(10000, cancellationToken);
            }
            else
            {
                _remainingDistance = _remainingDistance - _goDelta;

                _logger.Info($"_remainingDistance (after) = {_remainingDistance}");

                if (_remainingDistance > 0)
                {
                    Sleep(10000, cancellationToken);
                }
                else
                {
                    _logger.Info($"It completed!!!!!");

                    _remainingDistance = null;
                }
            }

            _logger.Info($"cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

            _logger.Info($"GoToImpl End");

            EmitOnLeave();
        }

        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken)
        {
            EmitOnEnter();

            _logger.Info("StopImpl Begin");

            EmitOnLeave();
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float? direction)
        {
            EmitOnEnter();

            _logger.Info("RotateImpl Begin");
            _logger.Info(direction.ToString());

            EmitOnLeave();
        }

        [DebuggerHidden]
        [BipedEndpoint("Aim to", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void AimToImpl(CancellationToken cancellationToken, IEntity entity)
        {
            _logger.Info("AimToImpl Begin");
            _logger.Info($"entity.InstanceId = {entity.InstanceId}");
            _logger.Info($"entity.Id = {entity.Id}");
            _logger.Info($"entity.Position = {entity.Position}");
        }

        [DebuggerHidden]
        [BipedEndpoint("Ready For Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ReadyForShootImpl(CancellationToken cancellationToken)
        {
            _logger.Info("ReadyForShootImpl Begin");
        }

        [DebuggerHidden]
        [FriendsEndpoints("Aim to")]
        [BipedEndpoint("Start Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StartShootImpl(CancellationToken cancellationToken)
        {
            _logger.Info("StartShootImpl Begin");
        }

        [DebuggerHidden]
        [FriendsEndpoints("Aim to")]
        [BipedEndpoint("Stop Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StopShootImpl(CancellationToken cancellationToken)
        {
            _logger.Info("StopShootImpl Begin");
        }
    }
}

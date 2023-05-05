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

            _logger.Log($"methodName = '{methodName}'");
            _logger.Log($"isNamedParameters = {isNamedParameters}");
            _logger.Log($"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented, _customConverter)}");
            _logger.Log($"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented, _customConverter)}");
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

            _logger.Log($"GoToImpl Begin");
            _logger.Log($"navTarget.Kind = {navTarget.Kind}");
            var entity = navTarget.Entity;
            _logger.Log($"entity.InstanceId = {entity.InstanceId}");
            _logger.Log($"entity.Id = {entity.Id}");
            _logger.Log($"entity.Position = {entity.Position}");
            _logger.Log($"_remainingDistance = {_remainingDistance}");

            if (!_remainingDistance.HasValue)
            {
                _remainingDistance = _wholeDistance;
                Sleep(10000, cancellationToken);
            }
            else
            {
                _remainingDistance = _remainingDistance - _goDelta;

                _logger.Log($"_remainingDistance (after) = {_remainingDistance}");

                if (_remainingDistance > 0)
                {
                    Sleep(10000, cancellationToken);
                }
                else
                {
                    _logger.Log($"It completed!!!!!");

                    _remainingDistance = null;
                }
            }

            _logger.Log($"cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

            _logger.Log($"GoToImpl End");
        }

        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken)
        {
            EmitOnEnter();

            _logger.Log("StopImpl Begin");
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float? direction)
        {
            EmitOnEnter();

            _logger.Log("RotateImpl Begin");
            _logger.Log(direction.ToString());
        }
    }
}

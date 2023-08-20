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
    public class TstBattleRoyaleHostListener2 : BaseHostListener
    {
        [DebuggerHidden]
        [BipedEndpoint("*")]
        public void GenericCall(CancellationToken cancellationToken, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            _logger.Info($"methodName = '{methodName}'");
            _logger.Info($"isNamedParameters = {isNamedParameters}");
            _logger.Info($"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented, _customConverter)}");
            _logger.Info($"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented, _customConverter)}");
        }

        private bool _isFirstCall;

        [DebuggerHidden]
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            _logger.Info($"GoToImpl Begin");
            _logger.Info($"navTarget.Kind = {navTarget.Kind}");
            var entity = navTarget.Entity;
            _logger.Info($"entity.InstanceId = {entity.InstanceId}");
            _logger.Info($"entity.Id = {entity.Id}");
            _logger.Info($"entity.Position = {entity.Position}");
            _logger.Info($"_isFirstCall = {_isFirstCall}");

            if (!_isFirstCall)
            {
                _isFirstCall = true;
                Sleep(10000, cancellationToken);
            }
            else
            {
                _isFirstCall = false;

                _logger.Info($"It completed!!!!!");
            }

            _logger.Info($"cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

            _logger.Info($"GoToImpl End");
        }

        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken)
        {
            _logger.Info("StopImpl Begin");
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float? direction)
        {
            _logger.Info("RotateImpl Begin");
            _logger.Info(direction.ToString());
        }
    }
}

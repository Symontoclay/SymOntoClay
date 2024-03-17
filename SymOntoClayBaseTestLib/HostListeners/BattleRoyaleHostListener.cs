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
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners
{
    public class BattleRoyaleHostListener : BaseHostListener
    {
        [DebuggerHidden]
        [BipedEndpoint("*")]
        public void GenericCall(CancellationToken cancellationToken, IMonitorLogger logger, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            logger.Output("C3EBA1CC-341A-41C3-BA7B-26D7B0ABE3CB", $"methodName = '{methodName}'");
        }

        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            logger.Output("38E3E03A-4065-4AF2-B326-6FBB4787480D", "StopImpl Begin");
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, IMonitorLogger logger, float? direction)
        {
            logger.Output("6A1ABDD2-FEFB-4C94-A5AE-F8E85B875B74", "RotateImpl Begin");
            logger.Output("71727657-A89A-4E90-8194-CCE27110DA39", direction.ToString());
        }

        private bool _isFirstCall;

        [DebuggerHidden]
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken, IMonitorLogger logger,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            logger.Output("A49A38AA-70E2-4108-BD8B-C9460560A031", $"GoToImpl Begin");
            logger.Output("8514AA8A-C510-4507-8C2A-EBE885D82737", navTarget.Kind.ToString());
            var entity = navTarget.Entity;
            logger.Output("A0A6CFA1-8290-44D0-B990-1263D4733793", entity.InstanceId.ToString());
            logger.Output("F3077726-584A-45E0-AC30-5EC391C4B779", entity.Id);
            logger.Output("B98D35DD-7515-404F-98F7-94742923D798", entity.Position.ToString());
            if (!_isFirstCall)
            {
                _isFirstCall = true;
                Sleep(10000, cancellationToken);
            }
            else
            {
                _isFirstCall = false;
            }
            logger.Output("D5FDA8BE-1EE5-41E5-BE0F-B8779EE2039A", $"GoToImpl End");
        }
    }
}

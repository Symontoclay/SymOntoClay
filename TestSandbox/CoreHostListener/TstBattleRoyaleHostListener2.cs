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

using Newtonsoft.Json;
using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.Core;
using SymOntoClay.Monitor.Common;
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
        public void GenericCall(CancellationToken cancellationToken, IMonitorLogger logger, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            logger.Info("08901E08-2681-413A-A285-705442594E9B", $"methodName = '{methodName}'");
            logger.Info("3F7293DE-85B2-4BC7-B763-94318811E5AC", $"isNamedParameters = {isNamedParameters}");
            logger.Info("33910F7F-F62A-4EF9-B987-1BA6E44BA4A7", $"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented, _customConverter)}");
            logger.Info("A2378CCB-C12F-4737-8872-840B9F59A4E6", $"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented, _customConverter)}");
        }

        private bool _isFirstCall;

        [DebuggerHidden]
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken, IMonitorLogger logger,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            logger.Info("C404C995-403F-4DC4-A68F-3678FDBB66B5", $"GoToImpl Begin");
            logger.Info("BEA559BF-A353-42CF-9617-794E74D8644C", $"navTarget.Kind = {navTarget.Kind}");
            var entity = navTarget.Entity;
            logger.Info("51B66F4B-E01F-4464-8502-196439A6465D", $"entity.InstanceId = {entity.InstanceId}");
            logger.Info("E2218C0D-A645-4DFA-9772-E90ACC853D1A", $"entity.Id = {entity.Id}");
            logger.Info("41265F04-C163-4E84-8404-84B397CAD84A", $"entity.Position = {entity.Position}");
            logger.Info("27EB941F-A0DA-44A2-9470-A69C22FD2036", $"_isFirstCall = {_isFirstCall}");

            if (!_isFirstCall)
            {
                _isFirstCall = true;
                Sleep(10000, cancellationToken);
            }
            else
            {
                _isFirstCall = false;

                logger.Info("8C6E35EB-5395-4663-BAB7-3BF1A1D8196A", $"It completed!!!!!");
            }

            logger.Info("DABDF048-6461-48A6-849D-5DC16F938722", $"cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

            logger.Info("B7EB7277-57A9-44B2-A8F3-84E0431F18BA", $"GoToImpl End");
        }

        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            logger.Info("04F887D9-BC4B-40FD-9C19-76258EC8E887", "StopImpl Begin");
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, IMonitorLogger logger, float? direction)
        {
            logger.Info("FA60A1F5-2317-4F1C-8CD1-2ADE43B94CC5", "RotateImpl Begin");
            logger.Info("4E9418F4-D9C1-4674-87F7-C5235F345C9E", direction.ToString());
        }
    }
}

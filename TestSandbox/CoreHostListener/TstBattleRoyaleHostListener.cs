/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
    public class TstBattleRoyaleHostListener : BaseHostListener
    {
        [DebuggerHidden]
        [BipedEndpoint("*")]
        public void GenericCall(CancellationToken cancellationToken, IMonitorLogger logger, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            EmitOnEnter();

            logger.Info("6D53EA4E-73A7-48D3-AD9F-5F3C6C6831BD", $"methodName = '{methodName}'");
            logger.Info("1715AE7E-31BD-4FE3-BD1D-BCABDFA80F79", $"isNamedParameters = {isNamedParameters}");
            logger.Info("14431596-FE75-4B14-A891-4C954C52D50E", $"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented, _customConverter)}");
            logger.Info("581668DB-8C93-4531-A87A-E02FFA5FFA17", $"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented, _customConverter)}");

            EmitOnLeave();
        }

        private int? _remainingDistance;
        private int _wholeDistance = 1000;
        private int _goDelta = 100;

        [DebuggerHidden]
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken, IMonitorLogger logger,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            EmitOnEnter();

            logger.Info("C4E768E8-D74D-4DC0-A3D4-F66C3FE40D56", $"GoToImpl Begin");
            logger.Info("164A6E53-4663-4B62-B20A-7CF5F433E81D", $"navTarget.Kind = {navTarget.Kind}");
            var entity = navTarget.Entity;
            logger.Info("1327F5B6-7A7F-4F0C-AE4B-F943667C7E1A", $"entity.InstanceId = {entity.InstanceId}");
            logger.Info("EB150AE3-C540-4EF5-94D9-4BD54F06B155", $"entity.Id = {entity.Id}");
            logger.Info("25EFB07E-9668-472F-9E74-D39BB8CD15EC", $"entity.Position = {entity.Position}");
            logger.Info("505688E5-8375-41E6-A8DB-02026E4E1FB2", $"_remainingDistance = {_remainingDistance}");

            if (!_remainingDistance.HasValue)
            {
                _remainingDistance = _wholeDistance;
                Sleep(10000, cancellationToken);
            }
            else
            {
                _remainingDistance = _remainingDistance - _goDelta;

                logger.Info("0C8929D6-FC00-48DA-81F7-19C40FF700EC", $"_remainingDistance (after) = {_remainingDistance}");

                if (_remainingDistance > 0)
                {
                    Sleep(10000, cancellationToken);
                }
                else
                {
                    logger.Info("D9CD494C-EC04-47CF-87DC-ED3C96FFE619", $"It completed!!!!!");

                    _remainingDistance = null;
                }
            }

            logger.Info("F5536E3C-B7EF-4AE1-91CD-94C5B81F29A1", $"cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

            logger.Info("C91D8079-7CFB-4D82-AF55-39F2892B1FB8", $"GoToImpl End");

            EmitOnLeave();
        }

        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            EmitOnEnter();

            logger.Info("B14713CD-C569-4EED-8A0A-826F00692F03", "StopImpl Begin");

            EmitOnLeave();
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, IMonitorLogger logger, float? direction)
        {
            EmitOnEnter();

            logger.Info("26ABD22B-A834-4D2E-A019-F21DDDA45079", "RotateImpl Begin");
            logger.Info("BA0E1FA1-E73A-4C30-8B31-B784DD319636", direction.ToString());

            EmitOnLeave();
        }

        [DebuggerHidden]
        [BipedEndpoint("Aim to", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void AimToImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
            logger.Info("127DD07F-83BA-4BCA-B4C3-ABEDACB76265", "AimToImpl Begin");
            logger.Info("60FC1D44-87D4-4E17-A6DD-1294844F8015", $"entity.InstanceId = {entity.InstanceId}");
            logger.Info("A583D18F-CDF9-420E-8EF6-26B84DC12BE1", $"entity.Id = {entity.Id}");
            logger.Info("5588E9C5-F08C-47BD-BCD9-9330DA17137A", $"entity.Position = {entity.Position}");
        }

        [DebuggerHidden]
        [BipedEndpoint("Ready For Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void ReadyForShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            logger.Info("50886639-E150-4C06-9ACA-B78D5815BCFA", "ReadyForShootImpl Begin");
        }

        [DebuggerHidden]
        [FriendsEndpoints("Aim to")]
        [BipedEndpoint("Start Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StartShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            logger.Info("7AFDDD2F-1830-4791-93FC-EC2AA4B59129", "StartShootImpl Begin");
        }

        [DebuggerHidden]
        [FriendsEndpoints("Aim to")]
        [BipedEndpoint("Stop Shoot", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void StopShootImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            logger.Info("7523B37F-5F8F-424F-87A3-8CB4FC2AF94A", "StopShootImpl Begin");
        }
    }
}

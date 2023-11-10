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
using SymOntoClay.Monitor.NLog;
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
        public void GenericCall(CancellationToken cancellationToken, IMonitorLogger logger, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            logger.Info("F4F160FE-82E3-4A6F-A2D4-BDC3A980668E", $"methodName = '{methodName}'");
            logger.Info("C01856F4-0812-4FF0-8BC8-BFE7424AE637", $"isNamedParameters = {isNamedParameters}");
            logger.Info("88931B1B-1797-4A14-97C4-7BF190EE5270", $"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented)}");
            logger.Info("20B22369-AC91-4A0D-AB1A-C9627DF4A267", $"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented)}");
        }

        [DebuggerHidden]
        [BipedEndpoint("Go2", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl_2(CancellationToken cancellationToken, IMonitorLogger logger,
            [EndpointParam("To", KindOfEndpointParam.Position)] IEntity entity,
            float speed = 12)
        {
            var name = GetMethodId();

            logger.Info("480DD174-5637-41D3-9CA1-975DB652FAF2", $"Begin {name}");

            logger.Info("1220943D-E3C3-4836-B863-F46D5AD5F92C", $"{name} entity.InstanceId = {entity.InstanceId}");
            logger.Info("8B2B7978-91AF-40D6-AD13-8C2383D9B4AD", $"{name} entity.Id = {entity.Id}");
            logger.Info("B6A0C4E2-3AEE-4505-917C-4B779400AF1F", $"{name} entity.Position = {entity.Position}");
            logger.Info("CCA0DBB4-C37A-4404-B3E0-0C8FB3C0FD28", $"{name} speed = {speed}");

            var n = 0;

            while (true)
            {
                n++;

                if (n > 10/*00*/)
                {
                    break;
                }

                Thread.Sleep(1000);

                logger.Info("9167BFF3-6AAD-4168-8A30-1FD88D0ECB7D", $"{name} Hi! n = {n}");

                logger.Info("B4493AEC-5DC1-4784-9B3C-663F40D44D83", $"{name} Hi! cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

                cancellationToken.ThrowIfCancellationRequested();
            }


            logger.Info("08B06A4E-D1F6-46E9-816C-4586EB2C9814", $"End {name}");
        }

        [DebuggerHidden]
        [BipedEndpoint("Go3", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl_3(CancellationToken cancellationToken, IMonitorLogger logger,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            var name = GetMethodId();

            logger.Info("07519B1E-8030-474B-856F-1896B0843897", $"Begin {name}");

            logger.Info("80830510-ECEC-45FB-A0CE-6E6AD9799571", $"{name} navTarget.Kind = {navTarget.Kind}");

            var entity = navTarget.Entity;

            logger.Info("C75C4FAC-9AA1-48C7-919C-2AAB7350DFF1", $"{name} entity.InstanceId = {entity.InstanceId}");
            logger.Info("76146FD5-34DB-4B93-8787-1D8D00C338B8", $"{name} entity.Id = {entity.Id}");
            logger.Info("23F0E6BF-9BBD-4E38-8D00-B7262C3C3822", $"{name} entity.Position = {entity.Position}");
            logger.Info("5B0742F0-A362-473B-B8E8-16D68F27D015", $"{name} speed = {speed}");

            var n = 0;

            while (true)
            {
                n++;

                if (n > 10/*00*/)
                {
                    break;
                }

                Thread.Sleep(1000);

                logger.Info("D3A91577-8AAD-48BF-BCE5-335EAC537CCE", $"{name} Hi! n = {n}");

                logger.Info("49621894-127B-4232-A80F-D350DFF82DC7", $"{name} Hi! cancellationToken.IsCancellationRequested = {cancellationToken.IsCancellationRequested}");

                cancellationToken.ThrowIfCancellationRequested();
            }

            logger.Info("3C79F6E8-2E06-4F56-B76A-CB27B66A4AC9", $"End {name}");
        }

        [DebuggerHidden]
        [BipedEndpoint("Take", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
            var name = GetMethodId();

            logger.Info("B17B110A-0E73-4EA4-AE67-B3715B79ED2F", $"Begin {name}");

            logger.Info("246F9A99-8A1C-4D71-BDB5-0366B97F9058", $"(entity == null) = {entity == null}");

            if(entity == null)
            {
                return;
            }

            entity.Specify(logger, EntityConstraints.CanBeTaken, /*EntityConstraints.OnlyVisible,*/ EntityConstraints.Nearest);

            entity.Resolve(logger);

            logger.Info("E1EC0AAE-3AF0-42C0-A0C2-D18CD5E8229C", $"{name} entity.InstanceId = {entity.InstanceId}");
            logger.Info("6B1CB60C-57D4-4C09-9CCA-CD2215B1A9A2", $"{name} entity.Id = {entity.Id}");
            logger.Info("B2537EE6-BCCA-445C-A890-CA5FE82D67AC", $"{name} entity.Position = {entity.Position}");

            logger.Info("C4C1779D-0714-498D-9D6C-242BB06821B8", $"End {name}");
        }

        [BipedEndpoint]
        private void JumpImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            logger.Info("EC5A9B0B-3627-4141-8F65-8E6163313A3A", "Begin");

            logger.Info("65CB2066-6C91-45B2-A790-DFDB5B116440", "End");
        }

        [BipedEndpoint("Aim", DeviceOfBiped.LeftHand, DeviceOfBiped.RightHand)]
        private void AimToImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            logger.Info("C383ABA2-1138-4A5D-9CF9-D3876897DA0E", "AimToImpl Begin");

            logger.Info("127E488D-EEB1-46BC-A9C7-1D62EC03D2FC", "AimToImpl End");
        }

        [FriendsEndpoints("Aim")]
        [BipedEndpoint("Fire", DeviceOfBiped.LeftHand, DeviceOfBiped.RightHand)]
        private void FireImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            logger.Info("476E9B3A-FF4E-42CD-BBDB-F5266D66B43B", "FireImpl Begin");

            for(var i = 0; i < 100; i++)
            {
                logger.Info("80973732-B1EF-471F-A3F4-DDA7260CD07D", $"FireImpl {i}");

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.Info("C732A698-909A-42D0-9797-C228B6BF87B6", "FireImpl Cancel");
                    return;
                }

                Thread.Sleep(100);
            }

            logger.Info("298CD9B0-20F2-4EA8-9699-9D5A3A7C37B1", "FireImpl End");
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, IMonitorLogger logger, float? direction)
        {
#if DEBUG
            var methodId = GetMethodId();

            logger.Info("BBF926CA-710E-4937-B113-4001DE08DCF0", $"RotateImpl Begin {methodId}; direction = {direction}");
#endif



#if DEBUG
            logger.Info("AA97362C-1452-4D45-9894-E1F3E86736B4", $"RotateImpl End {methodId}");
#endif
        }

        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateToEntityImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity,
            float speed = 2)
        {
            logger.Info("57114BFE-8ADB-4518-A48D-C21BD7C4BDFC", "RotateToEntityImpl Begin");
        }
    }
}

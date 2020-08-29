using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CoreHostListener
{
    public class TstPlatformHostListener
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken, 
            [EndpointParam("To", KindOfEndpointParam.Position)] Vector3 point,
            float speed = 12)
        {
            _logger.Log("Begin");

            _logger.Log($"point = {point}");
            _logger.Log($"speed = {speed}");

            var n = 0;

            while (true)
            {
                n++;

                if(n > 10)
                {
                    break;
                }

                Thread.Sleep(1000);

                _logger.Log($"Hi! n = {n}");

                cancellationToken.ThrowIfCancellationRequested();
            }

            //Thread.Sleep(5000);

            _logger.Log("End");
        }

        [BipedEndpoint]
        private void JumpImpl(CancellationToken cancellationToken)
        {
            _logger.Log("Begin");

            _logger.Log("End");
        }

        [BipedEndpoint("Aim", true, DeviceOfBiped.Head, DeviceOfBiped.LeftHand, DeviceOfBiped.RightHand)]
        private void AimTo(CancellationToken cancellationToken)
        {
            _logger.Log("Begin");

            _logger.Log("End");
        }
    }
}

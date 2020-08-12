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
    public class TstPlatformHostListener : IPlatformHostListener
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public IPlatformCommandCallResult CallCommand(IPlatformCommand command)
        {
            _logger.Log($"command = {command}");

            throw new NotImplementedException();
        }

        public IPlatformCommandInfo GetCommandInfo(IPlatformCommand command)
        {
            _logger.Log($"command = {command}");

            throw new NotImplementedException();
        }

        [BipedEndpoint("Go")]
        public void GoToImpl(CancellationToken cancellationToken, 
            [EndpointParam("To", KindOfEndpointParam.Position)] Vector3 point,
            float speed = 12)
        {
            _logger.Log("Begin");

            _logger.Log($"point = {point}");
            _logger.Log($"speed = {speed}");

            while(true)
            {
                Thread.Sleep(1000);

                _logger.Log("Hi!");

                cancellationToken.ThrowIfCancellationRequested();
            }

            //_logger.Log("End");
        }

        [BipedEndpoint]
        private void JumpImpl(CancellationToken cancellationToken)
        {
            _logger.Log("Begin");

            _logger.Log("End");
        }

        [BipedEndpoint("Aim", true, BipedDevices.Head, BipedDevices.LeftArm, BipedDevices.RightArm)]
        private void AimTo(CancellationToken cancellationToken)
        {
            _logger.Log("Begin");

            _logger.Log("End");
        }
    }
}

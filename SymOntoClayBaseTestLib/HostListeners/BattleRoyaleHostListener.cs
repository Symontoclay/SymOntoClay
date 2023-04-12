using Newtonsoft.Json;
using SymOntoClay.Core;
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
        public void GenericCall(CancellationToken cancellationToken, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            _logger.Log($"methodName = '{methodName}'");
        }

        [BipedEndpoint("Stop", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void StopImpl(CancellationToken cancellationToken)
        {
            _logger.Log("StopImpl Begin");
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float? direction)
        {
            _logger.Log("RotateImpl Begin");
            _logger.Log(direction.ToString());
        }

        private bool _isFirstCall;

        [DebuggerHidden]
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            _logger.Log($"GoToImpl Begin");
            _logger.Log(navTarget.Kind.ToString());
            var entity = navTarget.Entity;
            _logger.Log(entity.InstanceId.ToString());
            _logger.Log(entity.Id);
            _logger.Log(entity.Position.ToString());
            if (!_isFirstCall)
            {
                _isFirstCall = true;
                Sleep(10000, cancellationToken);
            }
            else
            {
                _isFirstCall = false;
            }
            _logger.Log($"GoToImpl End");
        }
    }
}

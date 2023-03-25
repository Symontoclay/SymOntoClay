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
            _logger.Log($"methodName = '{methodName}'");
            _logger.Log($"isNamedParameters = {isNamedParameters}");
            _logger.Log($"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented)}");
            _logger.Log($"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented)}");
        }

        private int? _remainingDistance;
        private int _wholeDistance = 1000;
        private int _goDelta = 100;

        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
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
                Thread.Sleep(10000);
            }
            else
            {
                _remainingDistance = _remainingDistance - _goDelta;

                if (_remainingDistance > 0)
                {
                    Thread.Sleep(10000);
                }
                else
                {
                    _remainingDistance = null;
                }
            }

            //Thread.Sleep(10000);

            _logger.Log($"GoToImpl End");
        }
    }
}

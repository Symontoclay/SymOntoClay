using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.BaseTestLib.HostListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSandbox.CoreHostListener
{
    public class TstGoHostListener : BaseHostListener
    {
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

            Thread.Sleep(10000);

            _logger.Log($"GoToImpl End");
        }
    }
}

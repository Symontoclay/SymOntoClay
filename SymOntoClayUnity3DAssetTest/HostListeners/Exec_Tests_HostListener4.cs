using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.HostListeners
{
    public class Exec_Tests_HostListener4 : BaseHostListener
    {
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
            _logger.Log($"GoToImpl End");
        }
    }
}

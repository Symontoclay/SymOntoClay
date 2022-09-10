using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.HostListeners
{
    public class Exec_Tests_HostListener1 : BaseHostListener
    {
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl_2(CancellationToken cancellationToken,
            [EndpointParam("Direction", KindOfEndpointParam.Position)] Vector3 point,
            float speed = 12)
        {
            _logger.Log($"GoToImpl_2");
            _logger.Log(point.ToString());
        }
    }
}

using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSandbox.CoreHostListener
{
    public class TstAsyncGoHostListener : BaseHostListener
    {
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public async Task GoToImpl(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            _logger.Log($"GoToImpl Begin");
            _logger.Log($"navTarget.Kind = {navTarget.Kind}");
            var entity = navTarget.Entity;
            _logger.Log($"entity.InstanceId = {entity.InstanceId}");
            _logger.Log($"entity.Id = {entity.Id}");
            _logger.Log($"entity.Position = {entity.Position}");

            await SomeMethod();

            _logger.Log($"GoToImpl End");
        }

        private Task SomeMethod()
        {
            return Task.Run(() => { Thread.Sleep(10000); });
        }

        /*
         [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public async void GoToImpl(CancellationToken cancellationToken,
        [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget target,
        float speed = 12)
         */
    }
}

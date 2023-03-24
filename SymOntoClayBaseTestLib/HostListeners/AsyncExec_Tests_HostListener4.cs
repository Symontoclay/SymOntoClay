using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners
{
    public class AsyncExec_Tests_HostListener4 : BaseHostListener
    {
        public AsyncExec_Tests_HostListener4()
            : this(null)
        {
        }

        public AsyncExec_Tests_HostListener4(int? millisecondsTimeout) 
        {
            _millisecondsTimeout = millisecondsTimeout;
        }

        private readonly int? _millisecondsTimeout;

        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public async Task GoToImpl(CancellationToken cancellationToken,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            _logger.Log($"GoToImpl Begin");
            _logger.Log(navTarget.Kind.ToString());
            var entity = navTarget.Entity;
            _logger.Log(entity.InstanceId.ToString());
            _logger.Log(entity.Id);
            _logger.Log(entity.Position.ToString());

            await SomeMethod();

            _logger.Log($"GoToImpl End");
        }

        private Task SomeMethod()
        {
            return Task.Run(() => { 
                if(_millisecondsTimeout.HasValue)
                {
                    Thread.Sleep(_millisecondsTimeout.Value);
                }                
            });
        }
    }
}

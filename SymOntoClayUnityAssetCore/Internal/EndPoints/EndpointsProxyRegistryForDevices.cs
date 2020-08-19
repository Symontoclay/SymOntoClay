using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class EndpointsProxyRegistryForDevices : BaseLoggedComponent, IEndpointsRegistry
    {
        public EndpointsProxyRegistryForDevices(IEntityLogger logger, IEndpointsRegistry sourceEndpointsRegistry, IList<int> devices)
            : base(logger)
        {
            _sourceEndpointsRegistry = sourceEndpointsRegistry;
            _devices = devices;
        }

        private readonly IEndpointsRegistry _sourceEndpointsRegistry;
        private readonly IList<int> _devices;

        /// <inheritdoc/>
        public IList<IEndpointInfo> GetEndpointsInfoListDirectly(string endPointName, int paramsCount)
        {
            if(_devices.IsNullOrEmpty())
            {
                return _sourceEndpointsRegistry.GetEndpointsInfoListDirectly(endPointName, paramsCount);
            }

            var initialResultList = _sourceEndpointsRegistry.GetEndpointsInfoListDirectly(endPointName, paramsCount);

            if(initialResultList == null)
            {
                return null;
            }

            var result = new List<IEndpointInfo>();

            foreach(var initialEndPointInfo in initialResultList)
            {
                var endPointInfo = new EndpointInfo(initialEndPointInfo);
                endPointInfo.Devices = _devices.ToList();
                result.Add(endPointInfo);
            }

            return result;
        }
    }
}

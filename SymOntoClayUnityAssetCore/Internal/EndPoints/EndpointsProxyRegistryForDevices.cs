/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
            Devices = devices;
        }

        private readonly IEndpointsRegistry _sourceEndpointsRegistry;
        public IList<int> Devices { get; set; } = new List<int>();

        /// <inheritdoc/>
        public IList<IEndpointInfo> GetEndpointsInfoListDirectly(string endPointName, int paramsCount)
        {
            if(Devices.IsNullOrEmpty())
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
                endPointInfo.Devices = Devices.ToList();
                result.Add(endPointInfo);
            }

            return result;
        }
    }
}

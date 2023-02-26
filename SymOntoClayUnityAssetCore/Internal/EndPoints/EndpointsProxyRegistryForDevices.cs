/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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

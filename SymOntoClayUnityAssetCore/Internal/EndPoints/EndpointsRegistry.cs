/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class EndpointsRegistry : BaseLoggedComponent, IEndpointsRegistry
    {
        public EndpointsRegistry(IMonitorLogger logger)
            : base(logger)
        {
        }

        private readonly object _lockObj = new object();
        private readonly Dictionary<string, Dictionary<int, List<IEndpointInfo>>> _endPointsDict = new Dictionary<string, Dictionary<int, List<IEndpointInfo>>>();
        private IEndpointInfo _generallCallEndPoint;

        public void AddEndpointsRange(IList<IEndpointInfo> platformEndpointsList)
        {
            lock (_lockObj)
            {
                foreach (var endpointInfo in platformEndpointsList)
                {
                    AddEndpoint(endpointInfo);
                }
            }
        }

        public void AddEndpoint(IEndpointInfo endpointInfo)
        {
            if(endpointInfo.KindOfEndpoint == KindOfEndpointInfo.GenericCall)
            {
                _generallCallEndPoint = endpointInfo;
                return;
            }

            var endPointName = endpointInfo.Name;

            var paramsCountList = GetParamsCountList(endpointInfo);

            lock (_lockObj)
            {
                Dictionary<int, List<IEndpointInfo>> targetDict = null;

                if (_endPointsDict.ContainsKey(endPointName))
                {
                    targetDict = _endPointsDict[endPointName];
                }
                else
                {
                    targetDict = new Dictionary<int, List<IEndpointInfo>>();
                    _endPointsDict[endPointName] = targetDict;
                }

                foreach (var count in paramsCountList)
                {
                    List<IEndpointInfo> targetList = null;

                    if (targetDict.ContainsKey(count))
                    {
                        targetList = targetDict[count];
                    }
                    else
                    {
                        targetList = new List<IEndpointInfo>();
                        targetDict[count] = targetList;
                    }

                    if (!targetList.Contains(endpointInfo))
                    {
                        targetList.Add(endpointInfo);
                    }
                }
            }
        }

        private List<int> GetParamsCountList(IEndpointInfo endpointInfo)
        {
            var result = new List<int>();

            var argumentsList = endpointInfo.Arguments;

#if DEBUG
            //Info("FA4480A1-55F6-46A8-ADC0-860786DEC851", $"endpointInfo.Name = {endpointInfo.Name}");
            //Info("6ECA534D-4E3D-4F6A-BD6E-6E5DE360AC33", $"argumentsList = {argumentsList.WriteListToString()}");
#endif

            if (!argumentsList.Any())
            {
                result.Add(0);
                return result;
            }

            var totalCount = argumentsList.Count(p => !p.IsSystemDefiend);
            var argumentsWithoutDefaultValueCount = argumentsList.Count(p => !p.IsSystemDefiend && !p.HasDefaultValue);

            if (totalCount == argumentsWithoutDefaultValueCount)
            {
                result.Add(totalCount);
                return result;
            }

            for (var i = argumentsWithoutDefaultValueCount; i <= totalCount; i++)
            {
                result.Add(i);
            }

            return result;
        }

        /// <inheritdoc/>
        public IList<IEndpointInfo> GetEndpointsInfoListDirectly(string endPointName, int paramsCount)
        {
            lock (_lockObj)
            {
                if (_endPointsDict.ContainsKey(endPointName))
                {
                    var targetDict = _endPointsDict[endPointName];

                    if (targetDict.ContainsKey(paramsCount))
                    {
                        return targetDict[paramsCount];
                    }
                }

                return new List<IEndpointInfo> { _generallCallEndPoint };
            }
        }
    }
}

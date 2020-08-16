using Newtonsoft.Json;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    public class EndpointsRegistry: BaseLoggedComponent
    {
        public EndpointsRegistry(IEntityLogger logger)
            : base(logger)
        {
        }

        private readonly object _lockObj = new object();
        private readonly Dictionary<string, Dictionary<int, List<EndpointInfo>>> _endPointsDict = new Dictionary<string, Dictionary<int, List<EndpointInfo>>>();

        public void AddEndpointsRange(List<EndpointInfo> platformEndpointsList)
        {
            foreach(var endpointInfo in platformEndpointsList)
            {
                AddEndpoint(endpointInfo);
            }
        }

        public void AddEndpoint(EndpointInfo endpointInfo)
        {
            lock(_lockObj)
            {
#if DEBUG
                Log($"endpointInfo = {endpointInfo}");
#endif

                var paramsCountList = GetParamsCountList(endpointInfo);

#if DEBUG
                Log($"paramsCountList = {JsonConvert.SerializeObject(paramsCountList, Formatting.Indented)}");
#endif

                var endPointName = endpointInfo.Name;

#if DEBUG
                Log($"endPointName = {endPointName}");
#endif

                Dictionary<int, List<EndpointInfo>> targetDict = null;

                if(_endPointsDict.ContainsKey(endPointName))
                {
                    targetDict = _endPointsDict[endPointName];
                }
                else
                {
                    targetDict = new Dictionary<int, List<EndpointInfo>>();
                    _endPointsDict[endPointName] = targetDict;
                }

                foreach(var count in paramsCountList)
                {
#if DEBUG
                    Log($"count = {count}");
#endif

                    List<EndpointInfo> targetList = null;

                    if(targetDict.ContainsKey(count))
                    {
                        targetList = targetDict[count];
                    }
                    else
                    {
                        targetList = new List<EndpointInfo>();
                        targetDict[count] = targetList;
                    }

                    if(!targetList.Contains(endpointInfo))
                    {
                        targetList.Add(endpointInfo);
                    }
                }
            }
        }

        private List<int> GetParamsCountList(EndpointInfo endpointInfo)
        {
            var result = new List<int>();

            var argumentsList = endpointInfo.Arguments;

            if (!argumentsList.Any())
            {
                result.Add(0);
                return result;
            }

            var totalCount = argumentsList.Count;
            var argumentsWithoutDefaultValueCount = argumentsList.Count(p => !p.HasDefaultValue);

#if DEBUG
            Log($"totalCount = {totalCount}");
            Log($"argumentsWithoutDefaultValueCount = {argumentsWithoutDefaultValueCount}");
#endif

            if(totalCount == argumentsWithoutDefaultValueCount)
            {
                result.Add(totalCount);
                return result;
            }

            for(var i = argumentsWithoutDefaultValueCount; i <= totalCount; i++)
            {
                result.Add(i);
            }

            return result;
        }
    }
}

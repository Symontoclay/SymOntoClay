using Newtonsoft.Json;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class EndpointsRegistry : BaseLoggedComponent, IEndpointsRegistry
    {
        public EndpointsRegistry(IEntityLogger logger)
            : base(logger)
        {
        }

        private readonly object _lockObj = new object();
        private readonly Dictionary<string, Dictionary<int, List<IEndpointInfo>>> _endPointsDict = new Dictionary<string, Dictionary<int, List<IEndpointInfo>>>();

        public void AddEndpointsRange(IList<IEndpointInfo> platformEndpointsList)
        {
            foreach (var endpointInfo in platformEndpointsList)
            {
                AddEndpoint(endpointInfo);
            }
        }

        public void AddEndpoint(IEndpointInfo endpointInfo)
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
#if DEBUG
                    Log($"count = {count}");
#endif

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

            if (!argumentsList.Any())
            {
                result.Add(0);
                return result;
            }

            var totalCount = argumentsList.Count(p => !p.IsSystemDefiend);
            var argumentsWithoutDefaultValueCount = argumentsList.Count(p => !p.IsSystemDefiend && !p.HasDefaultValue);

#if DEBUG
            Log($"totalCount = {totalCount}");
            Log($"argumentsWithoutDefaultValueCount = {argumentsWithoutDefaultValueCount}");
#endif

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

                return null;
            }
        }
    }
}

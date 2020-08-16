using Newtonsoft.Json;
using SymOntoClay.Core;
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

            var totalCount = argumentsList.Count(p => !p.IsSystemDefiend);
            var argumentsWithoutDefaultValueCount = argumentsList.Count(p => !p.IsSystemDefiend && !p.HasDefaultValue);

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

        public EndpointInfo GetEndpointInfo(ICommand command)
        {
#if DEBUG
            Log($"command = {command}");
            Log($"command.ParamsCount = {command.ParamsCount}");
#endif

            var endPointsList = NGetEndpointInfoList(command);

#if DEBUG
            Log($"endPointsList = {endPointsList.WriteListToString()}");
#endif

            if(endPointsList == null)
            {
                return null;
            }

            var kindOfCommandParameters = command.KindOfCommandParameters;

            switch(kindOfCommandParameters)
            {
                case KindOfCommandParameters.NoParameters:
                    return endPointsList.SingleOrDefault();

                case KindOfCommandParameters.ParametersByDict:
                    return NGetEndpointInfoByParametersByDict(endPointsList, command);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommandParameters), kindOfCommandParameters, null);
            }
        }

        private List<EndpointInfo> NGetEndpointInfoList(ICommand command)
        {
            var endPointName = command.Name.NameValue;

#if DEBUG
            Log($"endPointName = {endPointName}");
#endif

            lock (_lockObj)
            {
                if(_endPointsDict.ContainsKey(endPointName))
                {
                    var targetDict = _endPointsDict[endPointName];

                    if(targetDict.ContainsKey(command.ParamsCount))
                    {
                        return targetDict[command.ParamsCount];
                    }
                }

                return null;
            }
        }

        private EndpointInfo NGetEndpointInfoByParametersByDict(List<EndpointInfo> endPointsList, ICommand command)
        {
            var resultList = new List<EndpointInfo>();

            var commandParamsDict = command.ParamsDict.ToDictionary(p => p.Key.NameValue.ToLower(), p => p.Value);

            foreach(var endPointInfo in endPointsList)
            {
#if DEBUG
                Log($"endPointInfo = {endPointInfo}");
#endif

                var argumentsDict = endPointInfo.Arguments.Where(p => !p.IsSystemDefiend).ToDictionary(p => p.Name, p => p);

                var isFitEndpoint = true;

                foreach(var commandParamItem in commandParamsDict)
                {
#if DEBUG
                    Log($"commandParamItem.Key = {commandParamItem.Key}");
#endif
                    if (!argumentsDict.ContainsKey(commandParamItem.Key))
                    {
                        isFitEndpoint = false;
                        break;
                    }

                    var targetCommandValue = commandParamItem.Value;

#if DEBUG
                    Log($"targetCommandValue = {targetCommandValue}");
#endif

                    var targetArgument = argumentsDict[commandParamItem.Key];

#if DEBUG
                    Log($"targetArgument = {targetArgument}");
#endif

                    throw new NotImplementedException();
                }

#if DEBUG
                Log($"isFitEndpoint = {isFitEndpoint}");
#endif

                if(!isFitEndpoint)
                {
                    break;
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}

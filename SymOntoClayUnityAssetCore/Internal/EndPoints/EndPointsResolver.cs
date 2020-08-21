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
    public class EndPointsResolver : BaseLoggedComponent
    {
        public EndPointsResolver(IEntityLogger logger, IPlatformTypesConvertorsRegistry platformTypesConvertorsRegistry)
            : base(logger)
        {
            _platformTypesConvertorsRegistry = platformTypesConvertorsRegistry;
        }

        private readonly IPlatformTypesConvertorsRegistry _platformTypesConvertorsRegistry;

        public IEndpointInfo GetEndpointInfo(ICommand command, IList<IEndpointsRegistry> endpointsRegistries)
        {
#if DEBUG
            //Log($"command = {command}");
            //Log($"command.ParamsCount = {command.ParamsCount}");
#endif
            var endPointsList = new List<IEndpointInfo>();

            var endPointName = command.Name.NameValue;

#if DEBUG
            //Log($"endPointName = {endPointName}");
#endif

            var paramsCount = command.ParamsCount;

            foreach (var endpointsRegistry in endpointsRegistries.ToList())
            {
                var targetEndPointsList = endpointsRegistry.GetEndpointsInfoListDirectly(endPointName, paramsCount);

                if(targetEndPointsList != null)
                {
                    endPointsList.AddRange(targetEndPointsList);
                }               
            }

            endPointsList = endPointsList.Distinct().ToList();

#if DEBUG
            //Log($"endPointsList = {endPointsList.WriteListToString()}");
#endif

            if (endPointsList == null)
            {
                return null;
            }

            var kindOfCommandParameters = command.KindOfCommandParameters;

            switch (kindOfCommandParameters)
            {
                case KindOfCommandParameters.NoParameters:
                    return endPointsList.SingleOrDefault();

                case KindOfCommandParameters.ParametersByDict:
                    return NGetEndpointInfoByParametersByDict(endPointsList, command);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommandParameters), kindOfCommandParameters, null);
            }
        }

        private IEndpointInfo NGetEndpointInfoByParametersByDict(IList<IEndpointInfo> endPointsList, ICommand command)
        {
            var resultList = new List<IEndpointInfo>();

            var commandParamsDict = command.ParamsDict.ToDictionary(p => p.Key.NameValue.ToLower(), p => p.Value);

            foreach (var endPointInfo in endPointsList)
            {
#if DEBUG
                //Log($"endPointInfo = {endPointInfo}");
#endif

                var argumentsDict = endPointInfo.Arguments.Where(p => !p.IsSystemDefiend).ToDictionary(p => p.Name, p => p);

                var isFitEndpoint = true;

                foreach (var commandParamItem in commandParamsDict)
                {
#if DEBUG
                    //Log($"commandParamItem.Key = {commandParamItem.Key}");
#endif
                    if (!argumentsDict.ContainsKey(commandParamItem.Key))
                    {
                        isFitEndpoint = false;
                        break;
                    }

                    var targetCommandValue = commandParamItem.Value;

#if DEBUG
                    //Log($"targetCommandValue = {targetCommandValue}");
#endif

                    var targetArgument = argumentsDict[commandParamItem.Key];

#if DEBUG
                    //Log($"targetArgument = {targetArgument}");
#endif

                    if (!_platformTypesConvertorsRegistry.CanConvert(targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType))
                    {
                        isFitEndpoint = false;
                        break;
                    }
                }

#if DEBUG
                //Log($"isFitEndpoint = {isFitEndpoint}");
#endif

                if (!isFitEndpoint)
                {
                    break;
                }

                resultList.Add(endPointInfo);
            }

            return resultList.FirstOrDefault();
        }
    }
}

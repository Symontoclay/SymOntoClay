/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
            var endPointsList = new List<IEndpointInfo>();

            var endPointName = command.Name.NameValue;

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
                var argumentsDict = endPointInfo.Arguments.Where(p => !p.IsSystemDefiend).ToDictionary(p => p.Name, p => p);

                var isFitEndpoint = true;

                foreach (var commandParamItem in commandParamsDict)
                {
                    if (!argumentsDict.ContainsKey(commandParamItem.Key))
                    {
                        isFitEndpoint = false;
                        break;
                    }

                    var targetCommandValue = commandParamItem.Value;

                    var targetArgument = argumentsDict[commandParamItem.Key];

                    if (!_platformTypesConvertorsRegistry.CanConvert(targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType))
                    {
                        isFitEndpoint = false;
                        break;
                    }
                }

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

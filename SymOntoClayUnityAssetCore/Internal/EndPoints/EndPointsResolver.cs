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

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class EndPointsResolver : BaseLoggedComponent
    {
        public EndPointsResolver(IMonitorLogger logger, IPlatformTypesConvertersRegistry platformTypesConvertorsRegistry)
            : base(logger)
        {
            _platformTypesConvertorsRegistry = platformTypesConvertorsRegistry;
        }

        private readonly IPlatformTypesConvertersRegistry _platformTypesConvertorsRegistry;

        public IEndpointInfo GetEndpointInfo(IMonitorLogger logger, string callMethodId, ICommand command, IList<IEndpointsRegistry> endpointsRegistries, IPackedSynonymsResolver synonymsResolver)
        {
            logger.HostMethodResolving("AA671383-16F9-4805-849A-AA978959A28D", callMethodId);

            var endPointsList = new List<IEndpointInfo>();

            var endPointName = NameHelper.UnShieldString(command.Name.NameValue);

            var paramsCount = command.ParamsCount;
            
            var synonymsList = synonymsResolver?.GetSynonyms(logger, NameHelper.CreateName(endPointName)).Select(p => p.NameValue).ToList();

            logger.SystemExpr("69BD7A53-01CC-4105-A37D-BE674676622A", callMethodId, nameof(paramsCount), paramsCount);

            foreach (var endpointsRegistry in endpointsRegistries.ToList())
            {
                var targetEndPointsList = endpointsRegistry.GetEndpointsInfoListDirectly(endPointName, paramsCount);

                if (targetEndPointsList != null)
                {
                    endPointsList.AddRange(targetEndPointsList);
                }

                if(!synonymsList.IsNullOrEmpty())
                {
                    foreach(var synonym in synonymsList)
                    {
                        targetEndPointsList = endpointsRegistry.GetEndpointsInfoListDirectly(synonym, paramsCount);

                        if (targetEndPointsList != null)
                        {
                            endPointsList.AddRange(targetEndPointsList);
                        }
                    }
                }
            }

            endPointsList = endPointsList.Where(p => p != null).Distinct().ToList();

            if (endPointsList == null)
            {
                logger.EndHostMethodResolving("ECCFFB47-7D66-4739-93AC-D006CBAF0570", callMethodId);

                return null;
            }

            if(endPointsList.Any(p => p.KindOfEndpoint == KindOfEndpointInfo.GenericCall))
            {
                var result = endPointsList.FirstOrDefault(p => p.KindOfEndpoint == KindOfEndpointInfo.GenericCall);

                logger.EndHostMethodResolving("51AB7328-4AD2-41B2-ACBA-7372E93B0A07", callMethodId);

                return result;
            }

            var kindOfCommandParameters = command.KindOfCommandParameters;

            switch (kindOfCommandParameters)
            {
                case KindOfCommandParameters.NoParameters:
                    {
                        var result = endPointsList.SingleOrDefault();

                        logger.EndHostMethodResolving("1A37CE56-1FBD-4FA8-AD29-71FFCD6EC79C", callMethodId);

                        return result;
                    }

                case KindOfCommandParameters.ParametersByDict:
                    {
                        var result = NGetEndpointInfoByParametersByDict(logger, endPointsList, command, synonymsResolver);

                        logger.EndHostMethodResolving("7936DE46-9012-4D74-B950-A1121E0BCBF5", callMethodId);

                        return result;
                    }

                case KindOfCommandParameters.ParametersByList:
                    {
                        var result = NGetEndpointInfoByParametersByList(logger, endPointsList, command);

                        logger.EndHostMethodResolving("92CD4B4A-1C4D-463D-AE58-98014CD77CF1", callMethodId);

                        return result;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommandParameters), kindOfCommandParameters, null);
            }
        }

        private IEndpointInfo NGetEndpointInfoByParametersByList(IMonitorLogger logger, IList<IEndpointInfo> endPointsList, ICommand command)
        {
            var resultList = new List<IEndpointInfo>();

            foreach (var endPointInfo in endPointsList)
            {
                var argumentsList = endPointInfo.Arguments.Where(p => !p.IsSystemDefiend);

                var isFitEndpoint = true;

                var argumentsListEnumerator = argumentsList.GetEnumerator();

                foreach (var commandParamItem in command.ParamsList)
                {
                    if(!argumentsListEnumerator.MoveNext())
                    {
                        isFitEndpoint = false;
                        break;
                    }

                    var targetArgument = argumentsListEnumerator.Current;

                    if (!_platformTypesConvertorsRegistry.CanConvert(logger, commandParamItem.GetType(), targetArgument.ParameterInfo.ParameterType))
                    {
                        isFitEndpoint = false;
                        break;
                    }
                }

                if (isFitEndpoint)
                {
                    resultList.Add(endPointInfo);
                }                
            }

            return resultList.FirstOrDefault();
        }

        private IEndpointInfo NGetEndpointInfoByParametersByDict(IMonitorLogger logger, IList<IEndpointInfo> endPointsList, ICommand command, IPackedSynonymsResolver synonymsResolver)
        {
            var resultList = new List<IEndpointInfo>();

            var commandParamsDict = command.ParamsDict.ToDictionary(p => p.Key.NameValue.ToLower(), p => p.Value);

            foreach (var endPointInfo in endPointsList)
            {
                var argumentsDict = endPointInfo.Arguments.Where(p => !p.IsSystemDefiend).ToDictionary(p => p.Name, p => p);

                var isFitEndpoint = true;

                foreach (var commandParamItem in commandParamsDict)
                {
                    var realParamName = commandParamItem.Key;

                    if (!isFitEndpoint)
                    {
                        continue;
                    }

                    if (!argumentsDict.ContainsKey(commandParamItem.Key))
                    {
                        var synonymsList = synonymsResolver?.GetSynonyms(logger, NameHelper.CreateName(commandParamItem.Key)).Select(p => p.NameValue).ToList();

                        var isSynonymFit = false;

                        if(!synonymsList.IsNullOrEmpty())
                        {
                            foreach(var synonym in synonymsList)
                            {
                                if(isSynonymFit)
                                {
                                    continue;
                                }

                                if(argumentsDict.ContainsKey(synonym))
                                {
                                    isSynonymFit = true;
                                    realParamName = synonym;
                                }
                            }
                        }

                        if(!isSynonymFit)
                        {
                            isFitEndpoint = false;
                            continue;
                        }
                    }

                    var targetCommandValue = commandParamItem.Value;

                    var targetArgument = argumentsDict[realParamName];

                    if (!_platformTypesConvertorsRegistry.CanConvert(logger, targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType))
                    {
                        isFitEndpoint = false;
                    }
                }

                if (!isFitEndpoint)
                {
                    continue;
                }

                resultList.Add(endPointInfo);
            }

            return resultList.FirstOrDefault();
        }
    }
}

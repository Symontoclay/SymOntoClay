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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var endPointName = NameHelper.UnShieldString(command.Name.NameValue).ToLower();

#if DEBUG
            Info("4AE25975-C786-4A77-9A2D-53BFC95D78F7", $"endPointName = {endPointName}");
#endif

            var paramsCount = command.ParamsCount;

#if DEBUG
            Info("74EC3E96-9D2E-4500-BCFF-201611B8576A", $"paramsCount = {paramsCount}");
#endif

            var synonymsList = synonymsResolver?.GetSynonyms(logger, NameHelper.CreateName(endPointName)).Select(p => p.NameValue.ToLower()).ToList();

            logger.SystemExpr("69BD7A53-01CC-4105-A37D-BE674676622A", callMethodId, nameof(paramsCount), paramsCount);

#if DEBUG
            Info("57203E59-2511-429B-9F22-6A4164300B76", $"endpointsRegistries.Count = {endpointsRegistries.Count}");
#endif

            foreach (var endpointsRegistry in endpointsRegistries.ToList())
            {
#if DEBUG
                Info("3ED94741-E2D2-4D4D-961B-29D2B3B67897", $"endpointsRegistry.GetType().FullName = {endpointsRegistry.GetType().FullName}");
#endif

                var targetEndPointsList = endpointsRegistry.GetEndpointsInfoListDirectly(endPointName, paramsCount);

#if DEBUG
                Info("58426F70-7D09-4946-97A2-9F425BA57650", $"targetEndPointsList?.Count = {targetEndPointsList?.Count}");
#endif

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

#if DEBUG
            Info("749E7C2E-A164-4C6F-9926-6F04D0E9C9FC", $"endPointsList?.Count = {endPointsList?.Count}");
#endif

            if (endPointsList == null)
            {
                logger.EndHostMethodResolving("ECCFFB47-7D66-4739-93AC-D006CBAF0570", callMethodId);

                return null;
            }

#if DEBUG
            Info("39A8E292-9722-4724-B4F1-AEC71F5B541E", $"endPointsList.Any(p => p.KindOfEndpoint == KindOfEndpointInfo.GenericCall) = {endPointsList.Any(p => p.KindOfEndpoint == KindOfEndpointInfo.GenericCall)}");
#endif

            if (endPointsList.Any(p => p.KindOfEndpoint == KindOfEndpointInfo.GenericCall))
            {
                var result = endPointsList.FirstOrDefault(p => p.KindOfEndpoint == KindOfEndpointInfo.GenericCall);

                logger.EndHostMethodResolving("51AB7328-4AD2-41B2-ACBA-7372E93B0A07", callMethodId);

                return result;
            }

            var kindOfCommandParameters = command.KindOfCommandParameters;

#if DEBUG
            Info("30578E55-9BC7-4F83-8F58-D9EC895D2723", $"kindOfCommandParameters = {kindOfCommandParameters}");
#endif

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

#if DEBUG
                Info("EC0AA984-ADD6-4694-9DDD-2170875B5260", $"argumentsDict.Count = {argumentsDict.Count}");
                foreach(var tmpArgItem in argumentsDict)
                {
                    Info("C356D6B7-1703-45A1-B51D-1DF1A15D9D36", $"tmpArgItem.Key = {tmpArgItem.Key}");
                }
#endif

                var isFitEndpoint = true;

                foreach (var commandParamItem in commandParamsDict)
                {
#if DEBUG
                    Info("BA39750A-5B9E-4394-8BE0-FF84FB65B1A5", $"commandParamItem.Key = {commandParamItem.Key}");
#endif

                    var realParamName = PrepareParamName(commandParamItem.Key);

#if DEBUG
                    Info("6F545C5A-05C9-4DE4-9DFD-38B811E0816F", $"realParamName = {realParamName}");
#endif

                    if (!isFitEndpoint)
                    {
                        continue;
                    }

                    if (!argumentsDict.ContainsKey(realParamName))
                    {
#if DEBUG
                        Info("80C4DEBE-D1F2-428B-BC58-15A6C7429B36", $"!argumentsDict.ContainsKey(realParamName)");
#endif

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

                                var preparedSynonym = PrepareParamName(synonym);

                                if (argumentsDict.ContainsKey(preparedSynonym))
                                {
                                    isSynonymFit = true;
                                    realParamName = preparedSynonym;
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

#if DEBUG
                    Info("07D4C7B0-546D-4E7E-8E0A-472AFBD1D87A", $"targetCommandValue = {targetCommandValue}");
#endif

                    var targetArgument = argumentsDict[realParamName];

#if DEBUG
                    Info("0E27BDD9-E655-428B-AA0C-A18AF1843003", $"targetCommandValue.GetType().FullName = {targetCommandValue.GetType().FullName}");
                    Info("FA3BE48C-C640-46D2-9868-912515AA0822", $"targetArgument.ParameterInfo.ParameterType.FullName = {targetArgument.ParameterInfo.ParameterType.FullName}");
#endif

                    if (!_platformTypesConvertorsRegistry.CanConvert(logger, targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType))
                    {
#if DEBUG
                        Info("BE665CF5-FB58-4297-8199-A747EE88BA7D", $"!_platformTypesConvertorsRegistry.CanConvert(logger, targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType)");
#endif

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

        private string PrepareParamName(string paramName)
        {
            return paramName.Replace("`", string.Empty).ToLower();
        }
    }
}

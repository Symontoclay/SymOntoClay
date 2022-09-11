/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class EndPointsResolver : BaseLoggedComponent
    {
        public EndPointsResolver(IEntityLogger logger, IPlatformTypesConvertersRegistry platformTypesConvertorsRegistry)
            : base(logger)
        {
            _platformTypesConvertorsRegistry = platformTypesConvertorsRegistry;
        }

        private readonly IPlatformTypesConvertersRegistry _platformTypesConvertorsRegistry;

        public IEndpointInfo GetEndpointInfo(ICommand command, IList<IEndpointsRegistry> endpointsRegistries, IPackedSynonymsResolver synonymsResolver)
        {
            var endPointsList = new List<IEndpointInfo>();

            var endPointName = NameHelper.UnShieldString(command.Name.NameValue);

            var paramsCount = command.ParamsCount;
            
#if DEBUG
            //Log($"endPointName = {endPointName}");
            //Log($"paramsCount = {paramsCount}");
#endif

            var synonymsList = synonymsResolver?.GetSynonyms(NameHelper.CreateName(endPointName)).Select(p => p.NameValue).ToList();

            foreach (var endpointsRegistry in endpointsRegistries.ToList())
            {
                var targetEndPointsList = endpointsRegistry.GetEndpointsInfoListDirectly(endPointName, paramsCount);

                if(targetEndPointsList != null)
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

            endPointsList = endPointsList.Distinct().ToList();

#if DEBUG
            //Log($"endPointsList = {endPointsList.WriteListToString()}");
#endif

            if (endPointsList == null)
            {
                return null;
            }

            var kindOfCommandParameters = command.KindOfCommandParameters;

#if DEBUG
            //Log($"kindOfCommandParameters = {kindOfCommandParameters}");
#endif

            switch (kindOfCommandParameters)
            {
                case KindOfCommandParameters.NoParameters:
                    return endPointsList.SingleOrDefault();

                case KindOfCommandParameters.ParametersByDict:
                    return NGetEndpointInfoByParametersByDict(endPointsList, command, synonymsResolver);

                case KindOfCommandParameters.ParametersByList:
                    return NGetEndpointInfoByParametersByList(endPointsList, command);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommandParameters), kindOfCommandParameters, null);
            }
        }

        private IEndpointInfo NGetEndpointInfoByParametersByList(IList<IEndpointInfo> endPointsList, ICommand command)
        {
#if DEBUG
            //Log($"command = {command}");
#endif

            var resultList = new List<IEndpointInfo>();

#if DEBUG
            //Log($"endPointsList.Count = {endPointsList.Count}");
#endif

            foreach (var endPointInfo in endPointsList)
            {
#if DEBUG
                //Log($"endPointInfo.Name = {endPointInfo.Name}");
#endif

                var argumentsList = endPointInfo.Arguments.Where(p => !p.IsSystemDefiend);

#if DEBUG
                //Log($"argumentsList.Count() = {argumentsList.Count()}");
#endif

                var isFitEndpoint = true;

                var argumentsListEnumerator = argumentsList.GetEnumerator();

                foreach (var commandParamItem in command.ParamsList)
                {
#if DEBUG
                    //Log($"commandParamItem.GetType().FullName = {commandParamItem.GetType().FullName}");
                    //Log($"commandParamItem = {commandParamItem.ToHumanizedString()}");
#endif

                    if(!argumentsListEnumerator.MoveNext())
                    {
                        isFitEndpoint = false;
                        break;
                    }

                    var targetArgument = argumentsListEnumerator.Current;

#if DEBUG
                    //Log($"targetArgument.ParameterInfo.ParameterType.FullName = {targetArgument.ParameterInfo.ParameterType.FullName}");
#endif

                    if (!_platformTypesConvertorsRegistry.CanConvert(commandParamItem.GetType(), targetArgument.ParameterInfo.ParameterType))
                    {
                        isFitEndpoint = false;
                        break;
                    }
                }

#if DEBUG
                //Log($"isFitEndpoint = {isFitEndpoint}");
#endif

                if (isFitEndpoint)
                {
                    resultList.Add(endPointInfo);
                }                
            }

            return resultList.FirstOrDefault();
        }

        private IEndpointInfo NGetEndpointInfoByParametersByDict(IList<IEndpointInfo> endPointsList, ICommand command, IPackedSynonymsResolver synonymsResolver)
        {
            var resultList = new List<IEndpointInfo>();

            var commandParamsDict = command.ParamsDict.ToDictionary(p => p.Key.NameValue.ToLower(), p => p.Value);

#if DEBUG
            //Log($"endPointsList.Count = {endPointsList.Count}");
#endif

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

                    var realParamName = commandParamItem.Key;

                    if (!isFitEndpoint)
                    {
                        continue;
                    }

                    if (!argumentsDict.ContainsKey(commandParamItem.Key))
                    {
#if DEBUG
                        //Log($"!argumentsDict.ContainsKey(commandParamItem.Key)");
#endif

                        var synonymsList = synonymsResolver?.GetSynonyms(NameHelper.CreateName(commandParamItem.Key)).Select(p => p.NameValue).ToList();

#if DEBUG
                        //Log($"synonymsList = {synonymsList.WritePODListToString()}");
#endif

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

#if DEBUG
                        //Log($"isSynonymFit = {isSynonymFit}");
#endif

                        if(!isSynonymFit)
                        {
                            isFitEndpoint = false;
                            continue;
                        }
                    }

                    var targetCommandValue = commandParamItem.Value;

#if DEBUG
                    //Log($"targetCommandValue = {targetCommandValue}");
#endif

                    var targetArgument = argumentsDict[realParamName];

#if DEBUG
                    //Log($"targetArgument = {targetArgument}");
                    //Log($"targetCommandValue.GetType() = {targetCommandValue.GetType()}");
                    //Log($"targetArgument.ParameterInfo.ParameterType = {targetArgument.ParameterInfo.ParameterType}");
#endif

                    if (!_platformTypesConvertorsRegistry.CanConvert(targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType))
                    {
                        isFitEndpoint = false;
                    }
                }

#if DEBUG
                //Log($"isFitEndpoint = {isFitEndpoint}");
#endif

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

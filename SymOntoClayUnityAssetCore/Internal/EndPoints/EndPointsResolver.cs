/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

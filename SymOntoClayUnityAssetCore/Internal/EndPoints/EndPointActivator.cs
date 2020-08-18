﻿using Newtonsoft.Json;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class EndPointActivator : BaseLoggedComponent
    {
        public EndPointActivator(IEntityLogger logger, IPlatformTypesConvertorsRegistry platformTypesConvertorsRegistry, IInvokingInMainThread invokingInMainThread)
            : base(logger)
        {
            _platformTypesConvertorsRegistry = platformTypesConvertorsRegistry;
            _invokingInMainThread = invokingInMainThread;
        }

        private readonly IPlatformTypesConvertorsRegistry _platformTypesConvertorsRegistry;
        private readonly IInvokingInMainThread _invokingInMainThread;

        public IProcessInfo Activate(EndpointInfo endpointInfo, ICommand command, object platformListener)
        {
#if DEBUG
            Log($"endpointInfo = {endpointInfo}");
            Log($"command = {command}");
#endif

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var paramsList = MapParams(cancellationToken, endpointInfo, command);

#if DEBUG
            Log($"paramsList = {JsonConvert.SerializeObject(paramsList, Formatting.Indented)}");
#endif

            Task task = null;

            if (endpointInfo.NeedMainThread)
            {
                task = CreateTaskForMainThread(cancellationToken, endpointInfo, paramsList, platformListener);
            }
            else
            {
                task = CreateTaskForUsualThread(cancellationToken, endpointInfo, paramsList, platformListener);
            }

            var processInfo = new PlatformProcessInfo(task, cancellationTokenSource, endpointInfo.Devices);

#if DEBUG
            Log($"processInfo = {processInfo}");
#endif

            return processInfo;
        }

        private Task CreateTaskForMainThread(CancellationToken cancellationToken, EndpointInfo endpointInfo, object[] paramsList, object platformListener)
        {
            var task = new Task(() =>
            {
                try
                {
                    var invocableInMainThreadObj = new InvocableInMainThread(() => {
                        endpointInfo.MethodInfo.Invoke(platformListener, paramsList);
                    }, _invokingInMainThread);

                    invocableInMainThreadObj.Run();
                }
                catch (TargetInvocationException)
                {
                }
                catch (Exception e)
                {
#if DEBUG
                    Log($"e = {e}");
#endif
                }

            }, cancellationToken);

            return task;
        }

        private Task CreateTaskForUsualThread(CancellationToken cancellationToken, EndpointInfo endpointInfo, object[] paramsList, object platformListener)
        {
            var task = new Task(() =>
            {
                try
                {
                    endpointInfo.MethodInfo.Invoke(platformListener, paramsList);
                }
                catch (TargetInvocationException)
                {
                }
                catch (Exception e)
                {
#if DEBUG
                    Log($"e = {e}");
#endif
                }

            }, cancellationToken);

            return task;
        }

        private object[] MapParams(CancellationToken cancellationToken, EndpointInfo endpointInfo, ICommand command)
        {
            var kindOfCommandParameters = command.KindOfCommandParameters;

            switch (kindOfCommandParameters)
            {
                case KindOfCommandParameters.NoParameters:
                    return new List<object>() { cancellationToken }.ToArray();

                case KindOfCommandParameters.ParametersByDict:
                    return MapParamsByParametersByDict(cancellationToken, endpointInfo, command);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommandParameters), kindOfCommandParameters, null);
            }
        }

        private object[] MapParamsByParametersByDict(CancellationToken cancellationToken, EndpointInfo endpointInfo, ICommand command)
        {
            var commandParamsDict = command.ParamsDict.ToDictionary(p => p.Key.NameValue.ToLower(), p => p.Value);
            var argumentsDict = endpointInfo.Arguments.Where(p => !p.IsSystemDefiend).ToDictionary(p => p.Name, p => p);

            var resultList = new List<object>();
            resultList.Add(cancellationToken);

            foreach (var commandParamItem in commandParamsDict)
            {
#if DEBUG
                Log($"commandParamItem.Key = {commandParamItem.Key}");
#endif

                var targetCommandValue = commandParamItem.Value;

#if DEBUG
                Log($"targetCommandValue = {targetCommandValue}");
#endif

                var targetArgument = argumentsDict[commandParamItem.Key];

#if DEBUG
                Log($"targetArgument = {targetArgument}");
#endif

                var targetValue = _platformTypesConvertorsRegistry.Convert(targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType, targetCommandValue);

#if DEBUG
                Log($"targetValue = {targetValue}");
#endif

                resultList.Add(targetValue);
            }

            return resultList.ToArray();
        }
    }
}

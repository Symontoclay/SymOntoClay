using Newtonsoft.Json;
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
        public EndPointActivator(IEntityLogger logger, IPlatformTypesConvertorsRegistry platformTypesConvertorsRegistry, IInvokerInMainThread invokingInMainThread)
            : base(logger)
        {
            _platformTypesConvertorsRegistry = platformTypesConvertorsRegistry;
            _invokingInMainThread = invokingInMainThread;
        }

        private readonly IPlatformTypesConvertorsRegistry _platformTypesConvertorsRegistry;
        private readonly IInvokerInMainThread _invokingInMainThread;

        public IProcessInfo Activate(IEndpointInfo endpointInfo, ICommand command)
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
            var processInfo = new PlatformProcessInfo(cancellationTokenSource, endpointInfo.Devices);

            if (endpointInfo.NeedMainThread)
            {
                task = CreateTaskForMainThread(cancellationToken, endpointInfo, paramsList, processInfo);
            }
            else
            {
                task = CreateTaskForUsualThread(cancellationToken, endpointInfo, paramsList, processInfo);
            }

            
            processInfo.SetTask(task);
#if DEBUG
            Log($"processInfo = {processInfo}");
#endif

            return processInfo;
        }

        private Task CreateTaskForMainThread(CancellationToken cancellationToken, IEndpointInfo endpointInfo, object[] paramsList, PlatformProcessInfo processInfo)
        {
            var platformListener = endpointInfo.Object;

            var task = new Task(() =>
            {
#if DEBUG
                Log($">>>>> processInfo = {processInfo}");
#endif

                try
                {
                    var invocableInMainThreadObj = new InvocableInMainThread(() => {
                        endpointInfo.MethodInfo.Invoke(platformListener, paramsList);
                    }, _invokingInMainThread);

                    invocableInMainThreadObj.Run();

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException)
                {
                }
                catch (Exception e)
                {
#if DEBUG
                    Log($"e = {e}");
#endif

                    processInfo.Status = ProcessStatus.Faulted;
                }

            }, cancellationToken);

            return task;
        }

        private Task CreateTaskForUsualThread(CancellationToken cancellationToken, IEndpointInfo endpointInfo, object[] paramsList, PlatformProcessInfo processInfo)
        {
            var platformListener = endpointInfo.Object;

            var task = new Task(() =>
            {
                try
                {
#if DEBUG
                    Log($"+++++++++++++ processInfo = {processInfo}");
#endif

                    endpointInfo.MethodInfo.Invoke(platformListener, paramsList);

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException)
                {
                }
                catch (Exception e)
                {
#if DEBUG
                    Log($"e = {e}");
#endif

                    processInfo.Status = ProcessStatus.Faulted;
                }

            }, cancellationToken);

            return task;
        }

        private object[] MapParams(CancellationToken cancellationToken, IEndpointInfo endpointInfo, ICommand command)
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

        private object[] MapParamsByParametersByDict(CancellationToken cancellationToken, IEndpointInfo endpointInfo, ICommand command)
        {
            var commandParamsDict = command.ParamsDict.ToDictionary(p => p.Key.NameValue.ToLower(), p => p.Value);
            var argumentsDict = endpointInfo.Arguments.Where(p => !p.IsSystemDefiend).ToDictionary(p => p.Name, p => p);

            var resultList = new List<object>();
            resultList.Add(cancellationToken);

            foreach (var argumentItem in argumentsDict)
            {
                var argumentName = argumentItem.Key;

                var argumentInfo = argumentItem.Value;

#if DEBUG
                Log($"argumentName = {argumentName}");
                Log($"argumentInfo = {argumentInfo}");
#endif

                if(commandParamsDict.ContainsKey(argumentName))
                {
                    var targetCommandValue = commandParamsDict[argumentName];

#if DEBUG
                    Log($"targetCommandValue = {targetCommandValue}");
#endif

                    var targetValue = _platformTypesConvertorsRegistry.Convert(targetCommandValue.GetType(), argumentInfo.ParameterInfo.ParameterType, targetCommandValue);

#if DEBUG
                    Log($"targetValue = {targetValue}");
#endif

                    resultList.Add(targetValue);

                    continue;
                }

                if(argumentInfo.HasDefaultValue)
                {
                    resultList.Add(argumentInfo.DefaultValue);
                }
            }

            //            foreach (var commandParamItem in commandParamsDict)
            //            {
            //#if DEBUG
            //                Log($"commandParamItem.Key = {commandParamItem.Key}");
            //#endif

            //                var targetCommandValue = commandParamItem.Value;

            //#if DEBUG
            //                Log($"targetCommandValue = {targetCommandValue}");
            //#endif

            //                var targetArgument = argumentsDict[commandParamItem.Key];

            //#if DEBUG
            //                Log($"targetArgument = {targetArgument}");
            //#endif

            //                var targetValue = _platformTypesConvertorsRegistry.Convert(targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType, targetCommandValue);

            //#if DEBUG
            //                Log($"targetValue = {targetValue}");
            //#endif

            //                resultList.Add(targetValue);
            //            }

            //throw new NotImplementedException();

            return resultList.ToArray();
        }
    }
}

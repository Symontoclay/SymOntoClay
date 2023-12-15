/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
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
        public EndPointActivator(IMonitorLogger logger, IPlatformTypesConvertersRegistry platformTypesConvertorsRegistry, IInvokerInMainThread invokingInMainThread)
            : base(logger)
        {
            _platformTypesConvertorsRegistry = platformTypesConvertorsRegistry;
            _invokingInMainThread = invokingInMainThread;
        }

        private readonly IPlatformTypesConvertersRegistry _platformTypesConvertorsRegistry;
        private readonly IInvokerInMainThread _invokingInMainThread;

        public IProcessInfo Activate(IMonitorLogger logger, string callMethodId, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            logger.HostMethodActivation("D5CAB261-7931-433C-971F-3054EFCF9AC7", callMethodId);

#if DEBUG
            //Log($"endpointInfo = {endpointInfo}");
            //Log($"command = {command}");
#endif

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var mapParamsResult = MapParams(cancellationToken, logger, endpointInfo, command, context, localContext);

#if DEBUG
            //logger.Info("E2184458-2DA5-4E34-A224-BEA16B24A5F2", $"mapParamsResult.Item2 = {mapParamsResult.Item2.WriteDict_3_ToString()}");
#endif

            var paramsList = mapParamsResult.Item1;
#if DEBUG
            //Log($"paramsList?.Length = {paramsList?.Length}");
#endif

            Task task = null;
            var processInfo = new PlatformProcessInfo(cancellationTokenSource, endpointInfo.Name, mapParamsResult.Item2, endpointInfo.Devices, endpointInfo.Friends, callMethodId);

#if DEBUG
            //Log($"processInfo != null = {processInfo != null}");
            //Log($"endpointInfo.NeedMainThread = {endpointInfo.NeedMainThread}");
#endif

            if (endpointInfo.NeedMainThread)
            {
                task = CreateTaskForMainThread(cancellationToken, logger, callMethodId, endpointInfo, paramsList, processInfo);
            }
            else
            {
                task = CreateTaskForUsualThread(cancellationToken, logger, callMethodId, endpointInfo, paramsList, processInfo);
            }
            
            processInfo.SetTask(task);

#if DEBUG
            //Log($"NEXT");
#endif

            logger.EndHostMethodActivation("3F85C6A1-3470-44E3-BCEE-A1954E723DDA", callMethodId);

            return processInfo;
        }

        private Task CreateTaskForMainThread(CancellationToken cancellationToken, IMonitorLogger logger, string callMethodId, IEndpointInfo endpointInfo, object[] paramsList, PlatformProcessInfo processInfo)
        {
            var platformListener = endpointInfo.Object;

            var task = new Task(() =>
            {
                try
                {
                    _invokingInMainThread.RunInMainThread(() => {
                        Invoke(callMethodId, endpointInfo.MethodInfo, platformListener, paramsList, logger);
                    });

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException e)
                {
                    logger.EndHostMethodExecution("C85DF3C9-3CC1-4BCF-BCFD-F200A96590B9", callMethodId);

                    if (e.InnerException.GetType() == typeof(OperationCanceledException))
                    {
                        if (processInfo.Status != ProcessStatus.Canceled && processInfo.Status != ProcessStatus.WeakCanceled)
                        {
                            processInfo.Status = ProcessStatus.Canceled;
                        }
                    }
                    else
                    {
#if DEBUG
                        logger.Info("CC60ADBD-F203-491C-8187-BD302E2B0E08", $"e = {e}");
#endif
                    }
                }
                catch (Exception e)
                {
                    logger.EndHostMethodExecution("F5A7A3BA-DCA4-4ECF-959E-D96405BD75C2", callMethodId);

                    logger.Error("B713C1AF-C89B-4430-9AA3-1751BF5D4C51", e);

                    processInfo.Status = ProcessStatus.Faulted;
                }

            }, cancellationToken);

            return task;
        }

        private Task CreateTaskForUsualThread(CancellationToken cancellationToken, IMonitorLogger logger, string callMethodId, IEndpointInfo endpointInfo, object[] paramsList, PlatformProcessInfo processInfo)
        {
            var platformListener = endpointInfo.Object;

            var task = new Task(() =>
            {
                try
                {
                    Invoke(callMethodId, endpointInfo.MethodInfo, platformListener, paramsList, logger);

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException e)
                {
                    logger.EndHostMethodExecution("85E1D8C1-7BD6-418C-AF34-AB3BD3C0A004", callMethodId);

                    if (e.InnerException.GetType() == typeof(OperationCanceledException))
                    {
                        if (processInfo.Status != ProcessStatus.Canceled && processInfo.Status != ProcessStatus.WeakCanceled)
                        {
                            processInfo.Status = ProcessStatus.Canceled;
                        }
                    }
                    else
                    {
#if DEBUG
                        logger.Info("BC5E40E6-9992-437B-AA1F-D3E0FB13B323", $"e = {e}");
#endif
                    }
                }
                catch (Exception e)
                {
                    logger.EndHostMethodExecution("65DFD0B6-17E3-4EFE-8D8A-4C3FEA3EC5ED", callMethodId);

                    logger.Error("B459B0E1-D3D8-441A-B13C-3D1145FE09C0", e);

                    processInfo.Status = ProcessStatus.Faulted;
                }

            }, cancellationToken);

            return task;
        }

        private void Invoke(string callMethodId, MethodInfo methodInfo, object platformListener, object[] paramsList, IMonitorLogger logger)
        {
#if DEBUG
            //Log($"methodInfo.Name = {methodInfo.Name}");
            //Log($"methodInfo.ReturnType?.FullName = {methodInfo.ReturnType?.FullName}");
#endif

            logger.HostMethodExecution("4F646D89-6D1A-46A1-AB72-D12B533782D2", callMethodId);

            var result = methodInfo.Invoke(platformListener, paramsList);

#if DEBUG
            //Log($"result = {result}");
#endif

            if (result == null)
            {
                logger.EndHostMethodExecution("ED84B4D9-8305-426C-A7B4-C33175EA6633", callMethodId);
            }
            else
            {
                if (methodInfo.ReturnType == typeof(Task))
                {
                    var resultTask = (Task)result;

                    resultTask.Wait();

                    logger.EndHostMethodExecution("3D8D1D61-2C7B-443F-BE35-5FCF29603DCB", callMethodId);
                }
                else
                {
                    logger.EndHostMethodExecution("F43BC09E-70DE-4E13-8475-C091AFA69499", callMethodId);

                    throw new NotSupportedException($"Return type `{methodInfo.ReturnType.FullName}` is not supported.");
                }
            }
        }

        private (object[], Dictionary<string, Value>) MapParams(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            var methodInfo = endpointInfo.MethodInfo;

            var methodInfoParamsList = methodInfo.GetParameters();

            var containsLogger = false;

            if(methodInfoParamsList.Any(p => p.ParameterType == typeof(IMonitorLogger)))
            {
                containsLogger = true;
            }

            var kindOfCommandParameters = command.KindOfCommandParameters;

            switch (kindOfCommandParameters)
            {
                case KindOfCommandParameters.NoParameters:
                    {
                        var resultList = new List<object>();
                        resultList.Add(cancellationToken);

                        if(containsLogger)
                        {
                            resultList.Add(logger);
                        }

                        if (endpointInfo.KindOfEndpoint == KindOfEndpointInfo.GenericCall)
                        {
                            resultList.Add(command.Name.NameValue);
                            resultList.Add(false);
                            resultList.Add(null);
                            resultList.Add(null);
                        }

                        return (resultList.ToArray(), new Dictionary<string, Value>());
                    }

                case KindOfCommandParameters.ParametersByDict:
                    return MapParamsByParametersByDict(cancellationToken, logger, endpointInfo, command, context, localContext, containsLogger);

                case KindOfCommandParameters.ParametersByList:
                    return MapParamsByParametersByList(cancellationToken, logger, endpointInfo, command, context, localContext, containsLogger);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommandParameters), kindOfCommandParameters, null);
            }
        }

        private (object[], Dictionary<string, Value>) MapParamsByParametersByList(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext, bool containsLogger)
        {
            if(endpointInfo.KindOfEndpoint == KindOfEndpointInfo.GenericCall)
            {
                return MapGenericCallParamsByParametersByList(cancellationToken, logger, endpointInfo, command, context, localContext, containsLogger);
            }

            var argumentsList = endpointInfo.Arguments.Where(p => !p.IsSystemDefiend);

            var resultList = new List<object>();
            resultList.Add(cancellationToken);

            if(containsLogger)
            {
                resultList.Add(logger);
            }            

            var commandParamsEnumerator = command.ParamsList.GetEnumerator();

            var paramsInfoDict = new Dictionary<string, Value>();

            foreach (var targetArgument in argumentsList)
            {
#if DEBUG
                //logger.Info("9D618C3F-ECC9-455C-B599-75B90FDCD8DF", $"targetArgument = {targetArgument}");
#endif

                if(commandParamsEnumerator.MoveNext())
                {
                    var targetCommandValue = commandParamsEnumerator.Current;

#if DEBUG
                    //logger.Info("8C8E39EC-C031-475F-8159-61DF0B6D50D4", $"targetCommandValue = {targetCommandValue}");
#endif

                    var targetValue = _platformTypesConvertorsRegistry.Convert(logger, targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType, targetCommandValue, context, localContext);

#if DEBUG
                    //logger.Info("5A1FA843-D41C-462D-99B7-D27CDBA312BC", $"targetValue = {targetValue}");
#endif

                    resultList.Add(targetValue);
                    paramsInfoDict[targetArgument.Name] = targetCommandValue;
                }
                else
                {
                    if (targetArgument.HasDefaultValue)
                    {
                        var defaultValue = targetArgument.DefaultValue;

#if DEBUG
                        //logger.Info("F02CAE5B-0DCD-4592-A0DA-F2A3B04AD43A", $"defaultValue = {defaultValue}");
#endif

                        resultList.Add(defaultValue);

                        var targetValue = _platformTypesConvertorsRegistry.ConvertToValue(logger, targetArgument.ParameterInfo.ParameterType, defaultValue, context, localContext);

#if DEBUG
                        //logger.Info("9BBB6671-EB99-4686-B64A-990ED54D52B7", $"targetValue = {targetValue}");
#endif

                        paramsInfoDict[targetArgument.Name] = targetValue;
                    }
                }
            }

            return (resultList.ToArray(), paramsInfoDict);
        }

        private (object[], Dictionary<string, Value>) MapGenericCallParamsByParametersByList(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext, bool containsLogger)
        {
            var resultList = new List<object>();
            resultList.Add(cancellationToken);

            if(containsLogger)
            {
                resultList.Add(logger);
            }

            var commandParamsList = command.ParamsList;

            resultList.Add(command.Name.NameValue);
            resultList.Add(false);
            resultList.Add(null);
            resultList.Add(commandParamsList.Select(p => (object)p).ToList());

            var paramsInfoDict = new Dictionary<string, Value>();

            var n = 0;

            foreach (var commandParam in commandParamsList)
            {
                n++;

                paramsInfoDict[n.ToString()] = commandParam;
            }

            return (resultList.ToArray(), paramsInfoDict);
        }

        private (object[], Dictionary<string, Value>) MapParamsByParametersByDict(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext, bool containsLogger)
        {
            if(endpointInfo.KindOfEndpoint == KindOfEndpointInfo.GenericCall)
            {
                return MapGenericCallParamsByParametersByDict(cancellationToken, logger, endpointInfo, command, context, localContext, containsLogger);
            }

            var commandParamsDict = command.ParamsDict.ToDictionary(p => p.Key.NameValue.ToLower(), p => p.Value);
            var argumentsDict = endpointInfo.Arguments.Where(p => !p.IsSystemDefiend).ToDictionary(p => p.Name, p => p);

            var synonymsResolver = context.DataResolversFactory.GetSynonymsResolver();

            var resultList = new List<object>();
            resultList.Add(cancellationToken);

            if(containsLogger)
            {
                resultList.Add(logger);
            }

            var paramsInfoDict = new Dictionary<string, Value>();

            foreach (var argumentItem in argumentsDict)
            {
                var argumentName = argumentItem.Key;

                var argumentInfo = argumentItem.Value;

                var isBound = false;

                if (commandParamsDict.ContainsKey(argumentName))
                {
                    isBound = true;
                }
                else
                {
                    var synonymsList = synonymsResolver?.GetSynonyms(logger, NameHelper.CreateName(argumentName), localContext).Select(p => p.NameValue).ToList();

                    if (!synonymsList.IsNullOrEmpty())
                    {
                        foreach (var synonym in synonymsList)
                        {
                            if(isBound)
                            {
                                continue;
                            }

                            if (commandParamsDict.ContainsKey(synonym))
                            {
                                isBound = true;
                                argumentName = synonym;
                            }
                        }
                    }
                }

                if (isBound)
                {
                    var targetCommandValue = commandParamsDict[argumentName];

                    var targetValue = _platformTypesConvertorsRegistry.Convert(logger, targetCommandValue.GetType(), argumentInfo.ParameterInfo.ParameterType, targetCommandValue, context, localContext);

                    resultList.Add(targetValue);

                    paramsInfoDict[argumentName] = targetCommandValue;

                    continue;
                }

                if(argumentInfo.HasDefaultValue)
                {
                    var defaultValue = argumentInfo.DefaultValue;

                    resultList.Add(defaultValue);

                    var targetValue = _platformTypesConvertorsRegistry.ConvertToValue(logger, argumentInfo.ParameterInfo.ParameterType, defaultValue, context, localContext);

#if DEBUG
                    //logger.Info("4B521F80-DD63-4CA1-896C-E820C5B35F45", $"targetValue = {targetValue}");
#endif

                    paramsInfoDict[argumentName] = targetValue;
                }
            }

            return (resultList.ToArray(), paramsInfoDict);
        }

        private (object[], Dictionary<string, Value>) MapGenericCallParamsByParametersByDict(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext, bool containsLogger)
        {
            var resultList = new List<object>();
            resultList.Add(cancellationToken);

            if(containsLogger)
            {
                resultList.Add(logger);
            }
            
            resultList.Add(command.Name.NameValue);
            resultList.Add(true);

            var commandParamsDict = command.ParamsDict;

            resultList.Add(commandParamsDict.ToDictionary(p => p.Key.NameValue, p => (object)(p.Value.ToHumanizedString())));
            resultList.Add(null);

            var paramsInfoDict = new Dictionary<string, Value>();

            foreach(var item in commandParamsDict)
            {
                paramsInfoDict[item.Key.NameValue] = item.Value;
            }

            return (resultList.ToArray(), paramsInfoDict);
        }
    }
}

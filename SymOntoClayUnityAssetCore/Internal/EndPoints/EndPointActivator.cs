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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class EndPointActivator : BaseLoggedComponent
    {
        public EndPointActivator(IMonitorLogger logger, IPlatformTypesConvertersRegistry platformTypesConvertorsRegistry, IInvokerInMainThread invokingInMainThread, ICustomThreadPool threadPool)
            : base(logger)
        {
            _platformTypesConvertorsRegistry = platformTypesConvertorsRegistry;
            _invokingInMainThread = invokingInMainThread;
            _threadPool = threadPool;
        }

        private readonly IPlatformTypesConvertersRegistry _platformTypesConvertorsRegistry;
        private readonly IInvokerInMainThread _invokingInMainThread;
        private readonly ICustomThreadPool _threadPool;

        public IProcessInfo Activate(IMonitorLogger logger, string callMethodId, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            logger.HostMethodActivation("D5CAB261-7931-433C-971F-3054EFCF9AC7", callMethodId);

#if DEBUG
            //Log($"endpointInfo = {endpointInfo}");
            //Log($"command = {command}");
#endif

            var cancellationTokenSource = new CancellationTokenSource();

            var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, context.GetCancellationToken());

            var cancellationToken = linkedCancellationTokenSource.Token;

            var mapParamsResult = MapParams(cancellationToken, logger, endpointInfo, command, context, localContext);

#if DEBUG
            //logger.Info("E2184458-2DA5-4E34-A224-BEA16B24A5F2", $"mapParamsResult.Item2 = {mapParamsResult.Item2.WriteDict_3_ToString()}");
#endif

            var paramsList = mapParamsResult.Item1;
#if DEBUG
            //Log($"paramsList?.Length = {paramsList?.Length}");
#endif

            ThreadTask task = null;
            var processInfo = new PlatformProcessInfo(cancellationTokenSource, context.GetCancellationToken(), context.AsyncEventsThreadPool, endpointInfo.Name, mapParamsResult.Item2, endpointInfo.Devices, endpointInfo.Friends, callMethodId);

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

        private ThreadTask CreateTaskForMainThread(CancellationToken cancellationToken, IMonitorLogger logger, string callMethodId, IEndpointInfo endpointInfo, object[] paramsList, PlatformProcessInfo processInfo)
        {
            var platformListener = endpointInfo.Object;

            var task = new ThreadTask(() =>
            {
                try
                {
                    _invokingInMainThread.RunInMainThread(() => {
                        Invoke(callMethodId, endpointInfo.MethodInfo, platformListener, paramsList, logger);
                    });

#if DEBUG
                    //logger.Info("1DDE78B8-7984-4221-B2B2-CEE22E00879A", $"after Invoke");
#endif

                    processInfo.SetStatus(logger, "87FE8620-1044-4645-9FBD-650E0402EB1C", ProcessStatus.Completed);
                }
                catch (TargetInvocationException e)
                {
                    logger.EndHostMethodExecution("C85DF3C9-3CC1-4BCF-BCFD-F200A96590B9", callMethodId);

                    if (e.InnerException.GetType() == typeof(OperationCanceledException))
                    {
                        if (processInfo.Status != ProcessStatus.Canceled && processInfo.Status != ProcessStatus.WeakCanceled)
                        {
                            processInfo.SetStatus(logger, "23AABB3A-2FD8-4FA0-B371-6172E41BFD26", ProcessStatus.Canceled);
                        }
                    }
                    else
                    {
#if DEBUG
                        //logger.Info("CC60ADBD-F203-491C-8187-BD302E2B0E08", $"e = {e}");
#endif
                    }
                }
                catch (Exception e)
                {
                    logger.EndHostMethodExecution("F5A7A3BA-DCA4-4ECF-959E-D96405BD75C2", callMethodId);

                    logger.Error("B713C1AF-C89B-4430-9AA3-1751BF5D4C51", e);

                    processInfo.SetStatus(logger, "4EF49830-6C8B-499E-BC46-D71104191808", ProcessStatus.Faulted);
                }

            }, _threadPool, cancellationToken);

            return task;
        }

        private ThreadTask CreateTaskForUsualThread(CancellationToken cancellationToken, IMonitorLogger logger, string callMethodId, IEndpointInfo endpointInfo, object[] paramsList, PlatformProcessInfo processInfo)
        {
            var platformListener = endpointInfo.Object;

            var task = new ThreadTask(() =>
            {
#if DEBUG
                //logger.Info("9C2487EB-BCCF-4CF2-8F06-EBE4EF8FAC93", $"Begin Run processInfo.Id = {processInfo.Id};{processInfo.ToHumanizedLabel()}");
#endif

                try
                {
                    Invoke(callMethodId, endpointInfo.MethodInfo, platformListener, paramsList, logger);

#if DEBUG
                    //logger.Info("F606E8C6-444B-4B89-8D1E-9DC34F5C1267", $"after Invoke processInfo.Id = {processInfo.Id};{processInfo.ToHumanizedLabel()}");
#endif

                    processInfo.SetStatus(logger, "37907BF0-B51E-4D54-A2C1-21C0F4938965", ProcessStatus.Completed);

#if DEBUG
                    //logger.Info("E9B7098B-06AD-4597-9FE7-D770E3C70FC5", $"after (2) Invoke processInfo.Id = {processInfo.Id};{processInfo.ToHumanizedLabel()}");
#endif
                }
                catch (TargetInvocationException e)
                {
                    logger.EndHostMethodExecution("85E1D8C1-7BD6-418C-AF34-AB3BD3C0A004", callMethodId);

                    if (e.InnerException.GetType() == typeof(OperationCanceledException))
                    {
                        if (processInfo.Status != ProcessStatus.Canceled && processInfo.Status != ProcessStatus.WeakCanceled)
                        {
                            processInfo.SetStatus(logger, "850EB9E4-49CB-4CBB-8986-9FF7475809BD", ProcessStatus.Canceled);
                        }
                    }
                    else
                    {
                        logger.Error("BC5E40E6-9992-437B-AA1F-D3E0FB13B323", e);
                    }
                }
                catch(AggregateException e)
                {
                    logger.EndHostMethodExecution("A7CBFD6F-FF4F-4CC3-86D3-3055195446A9", callMethodId);

                    if(e.InnerException.GetType() == typeof(TaskCanceledException))
                    {
                        if (processInfo.Status != ProcessStatus.Canceled && processInfo.Status != ProcessStatus.WeakCanceled)
                        {
                            processInfo.SetStatus(logger, "91A69F14-8F87-468E-8D35-825FA27A21EE", ProcessStatus.Canceled);
                        }
                    }
                    else
                    {
                        logger.Error("7B913760-AD5F-4050-B354-876A90F7C7A0", e);
                    }
                }
                catch (Exception e)
                {
                    logger.Error("B459B0E1-D3D8-441A-B13C-3D1145FE09C0", e);

                    logger.EndHostMethodExecution("65DFD0B6-17E3-4EFE-8D8A-4C3FEA3EC5ED", callMethodId);

                    processInfo.SetStatus(logger, "47C8A8EC-3B92-42F7-AF7F-E7D765919383", ProcessStatus.Faulted);
                }

#if DEBUG
                //logger.Info("A046851F-4CB5-46D5-ADB0-2A3C61984000", $"Invoke End processInfo.Id = {processInfo.Id};{processInfo.ToHumanizedLabel()}");
#endif
            }, _threadPool, cancellationToken);

            return task;
        }

        private void Invoke(string callMethodId, MethodInfo methodInfo, object platformListener, object[] paramsList, IMonitorLogger logger)
        {
#if DEBUG
            //logger.Info("B178E16A-3FB8-4394-9BC0-3FAB4C0C21B8", $"methodInfo.Name = {methodInfo.Name}");
            //logger.Info("170AEADF-AA02-4AF9-81FC-D3F3DEAF9F6E", $"methodInfo.ReturnType?.FullName = {methodInfo.ReturnType?.FullName}");
#endif

            logger.HostMethodExecution("4F646D89-6D1A-46A1-AB72-D12B533782D2", callMethodId);

            var result = methodInfo.Invoke(platformListener, paramsList);

#if DEBUG
            //logger.Info("E88720B6-2A95-4CA3-AEC4-825300868095", $"result = {result}");
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

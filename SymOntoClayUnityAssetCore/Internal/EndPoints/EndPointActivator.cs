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

        public IProcessInfo Activate(IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
#if DEBUG
            //Log($"endpointInfo = {endpointInfo}");
            //Log($"command = {command}");
#endif

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var paramsList = MapParams(cancellationToken, logger, endpointInfo, command, context, localContext);

#if DEBUG
            //Log($"paramsList?.Length = {paramsList?.Length}");
#endif

            Task task = null;
            var processInfo = new PlatformProcessInfo(cancellationTokenSource, endpointInfo.Name, endpointInfo.Devices, endpointInfo.Friends);

#if DEBUG
            //Log($"processInfo != null = {processInfo != null}");
            //Log($"endpointInfo.NeedMainThread = {endpointInfo.NeedMainThread}");
#endif

            if (endpointInfo.NeedMainThread)
            {
                task = CreateTaskForMainThread(cancellationToken, logger, endpointInfo, paramsList, processInfo);
            }
            else
            {
                task = CreateTaskForUsualThread(cancellationToken, logger, endpointInfo, paramsList, processInfo);
            }
            
            processInfo.SetTask(task);

#if DEBUG
            //Log($"NEXT");
#endif

            return processInfo;
        }

        private Task CreateTaskForMainThread(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, object[] paramsList, PlatformProcessInfo processInfo)
        {
            var platformListener = endpointInfo.Object;

            var task = new Task(() =>
            {
                try
                {
                    _invokingInMainThread.RunInMainThread(() => {
                        Invoke(endpointInfo.MethodInfo, platformListener, paramsList, logger);
                    });

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException e)
                {
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
                    logger.Error("B713C1AF-C89B-4430-9AA3-1751BF5D4C51", e);

                    processInfo.Status = ProcessStatus.Faulted;
                }

            }, cancellationToken);

            return task;
        }

        private Task CreateTaskForUsualThread(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, object[] paramsList, PlatformProcessInfo processInfo)
        {
            var platformListener = endpointInfo.Object;

            var task = new Task(() =>
            {
                try
                {
                    Invoke(endpointInfo.MethodInfo, platformListener, paramsList, logger);

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException e)
                {
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
                    logger.Error("B459B0E1-D3D8-441A-B13C-3D1145FE09C0", e);

                    processInfo.Status = ProcessStatus.Faulted;
                }

            }, cancellationToken);

            return task;
        }

        private void Invoke(MethodInfo methodInfo, object platformListener, object[] paramsList, IMonitorLogger logger)
        {
#if DEBUG
            //Log($"methodInfo.Name = {methodInfo.Name}");
            //Log($"methodInfo.ReturnType?.FullName = {methodInfo.ReturnType?.FullName}");
#endif

            var result = methodInfo.Invoke(platformListener, paramsList);

#if DEBUG
            //Log($"result = {result}");
#endif

            if (result != null)
            {
                if (methodInfo.ReturnType == typeof(Task))
                {
                    var resultTask = (Task)result;

                    resultTask.Wait();
                }
                else
                {
                    throw new NotSupportedException($"Return type `{methodInfo.ReturnType.FullName}` is not supported.");
                }
            }
        }

        private object[] MapParams(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
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

                        return resultList.ToArray();
                    }

                case KindOfCommandParameters.ParametersByDict:
                    return MapParamsByParametersByDict(cancellationToken, logger, endpointInfo, command, context, localContext, containsLogger);

                case KindOfCommandParameters.ParametersByList:
                    return MapParamsByParametersByList(cancellationToken, logger, endpointInfo, command, context, localContext, containsLogger);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommandParameters), kindOfCommandParameters, null);
            }
        }

        private object[] MapParamsByParametersByList(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext, bool containsLogger)
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

            foreach (var targetArgument in argumentsList)
            {
                if(commandParamsEnumerator.MoveNext())
                {
                    var targetCommandValue = commandParamsEnumerator.Current;

                    var targetValue = _platformTypesConvertorsRegistry.Convert(targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType, targetCommandValue, context, localContext);

                    resultList.Add(targetValue);
                }
                else
                {
                    if (targetArgument.HasDefaultValue)
                    {
                        resultList.Add(targetArgument.DefaultValue);
                    }
                }
            }

            return resultList.ToArray();
        }

        private object[] MapGenericCallParamsByParametersByList(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext, bool containsLogger)
        {
            var resultList = new List<object>();
            resultList.Add(cancellationToken);

            if(containsLogger)
            {
                resultList.Add(logger);
            }
            
            resultList.Add(command.Name.NameValue);
            resultList.Add(false);
            resultList.Add(null);
            resultList.Add(command.ParamsList.Select(p => (object)p).ToList());

            return resultList.ToArray();
        }

        private object[] MapParamsByParametersByDict(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext, bool containsLogger)
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
                    var synonymsList = synonymsResolver?.GetSynonyms(NameHelper.CreateName(argumentName), localContext).Select(p => p.NameValue).ToList();

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

                    var targetValue = _platformTypesConvertorsRegistry.Convert(targetCommandValue.GetType(), argumentInfo.ParameterInfo.ParameterType, targetCommandValue, context, localContext);

                    resultList.Add(targetValue);

                    continue;
                }

                if(argumentInfo.HasDefaultValue)
                {
                    resultList.Add(argumentInfo.DefaultValue);
                }
            }

            return resultList.ToArray();
        }

        private object[] MapGenericCallParamsByParametersByDict(CancellationToken cancellationToken, IMonitorLogger logger, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext, bool containsLogger)
        {
            var resultList = new List<object>();
            resultList.Add(cancellationToken);

            if(containsLogger)
            {
                resultList.Add(logger);
            }
            
            resultList.Add(command.Name.NameValue);
            resultList.Add(true);
            resultList.Add(command.ParamsDict.ToDictionary(p => p.Key.NameValue, p => (object)(p.Value.ToHumanizedString())));
            resultList.Add(null);

            return resultList.ToArray();
        }
    }
}

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
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
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
        public EndPointActivator(IEntityLogger logger, IPlatformTypesConvertersRegistry platformTypesConvertorsRegistry, IInvokerInMainThread invokingInMainThread)
            : base(logger)
        {
            _platformTypesConvertorsRegistry = platformTypesConvertorsRegistry;
            _invokingInMainThread = invokingInMainThread;
        }

        private readonly IPlatformTypesConvertersRegistry _platformTypesConvertorsRegistry;
        private readonly IInvokerInMainThread _invokingInMainThread;

        public IProcessInfo Activate(IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
#if DEBUG
            //Log($"endpointInfo = {endpointInfo}");
#endif

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var paramsList = MapParams(cancellationToken, endpointInfo, command, context, localContext);

            Task task = null;
            var processInfo = new PlatformProcessInfo(cancellationTokenSource, endpointInfo.Name, endpointInfo.Devices, endpointInfo.Friends);

            if (endpointInfo.NeedMainThread)
            {
                task = CreateTaskForMainThread(cancellationToken, endpointInfo, paramsList, processInfo);
            }
            else
            {
                task = CreateTaskForUsualThread(cancellationToken, endpointInfo, paramsList, processInfo);
            }
            
            processInfo.SetTask(task);

            return processInfo;
        }

        private Task CreateTaskForMainThread(CancellationToken cancellationToken, IEndpointInfo endpointInfo, object[] paramsList, PlatformProcessInfo processInfo)
        {
            var platformListener = endpointInfo.Object;

            var task = new Task(() =>
            {
                try
                {
                    _invokingInMainThread.RunInMainThread(() => {
                        Invoke(endpointInfo.MethodInfo, platformListener, paramsList);
                    });

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException e)
                {
#if DEBUG
                    Log($"e = {e}");
                    Log($"e.InnerException = {e.InnerException}");
#endif

                    if (e.InnerException.GetType() == typeof(OperationCanceledException))
                    {
                        processInfo.Status = ProcessStatus.Canceled;
                    }
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
                    //Log("Pre methodInfo.Invoke");
#endif

                    Invoke(endpointInfo.MethodInfo, platformListener, paramsList);

#if DEBUG
                    //Log("after methodInfo.Invoke");
#endif

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException e)
                {
#if DEBUG
                    Log($"e = {e}");
                    Log($"e.InnerException = {e.InnerException}");
#endif

                    if(e.InnerException.GetType() == typeof(OperationCanceledException))
                    {
                        processInfo.Status = ProcessStatus.Canceled;
                    }
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

        private void Invoke(MethodInfo methodInfo, object platformListener, object[] paramsList)
        {
#if DEBUG
            //Log($"Pre methodInfo.ReturnType.FullName = {methodInfo.ReturnType.FullName}");
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

        private object[] MapParams(CancellationToken cancellationToken, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            var kindOfCommandParameters = command.KindOfCommandParameters;

#if DEBUG
            //Log($"kindOfCommandParameters = {kindOfCommandParameters}");
#endif

            switch (kindOfCommandParameters)
            {
                case KindOfCommandParameters.NoParameters:
                    if(endpointInfo.KindOfEndpoint == KindOfEndpointInfo.GenericCall)
                    {
                        return new List<object>() { cancellationToken, command.Name.NameValue, false, null, null }.ToArray();
                    }

                    return new List<object>() { cancellationToken }.ToArray();

                case KindOfCommandParameters.ParametersByDict:
                    return MapParamsByParametersByDict(cancellationToken, endpointInfo, command, context, localContext);

                case KindOfCommandParameters.ParametersByList:
                    return MapParamsByParametersByList(cancellationToken, endpointInfo, command, context, localContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommandParameters), kindOfCommandParameters, null);
            }
        }

        private object[] MapParamsByParametersByList(CancellationToken cancellationToken, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            if(endpointInfo.KindOfEndpoint == KindOfEndpointInfo.GenericCall)
            {
                return MapGenericCallParamsByParametersByList(cancellationToken, endpointInfo, command, context, localContext);
            }

            var argumentsList = endpointInfo.Arguments.Where(p => !p.IsSystemDefiend);

            var resultList = new List<object>();
            resultList.Add(cancellationToken);

#if DEBUG
            //Log($"argumentsList.Count() = {argumentsList.Count()}");
#endif

            var commandParamsEnumerator = command.ParamsList.GetEnumerator();

            foreach (var targetArgument in argumentsList)
            {
#if DEBUG
                //Log($"targetArgument.ParameterInfo.ParameterType.FullName = {targetArgument.ParameterInfo.ParameterType.FullName}");
#endif

                if(commandParamsEnumerator.MoveNext())
                {
                    var targetCommandValue = commandParamsEnumerator.Current;

#if DEBUG
                    //Log($"targetCommandValue.GetType().FullName = {targetCommandValue.GetType().FullName}");
                    //Log($"targetCommandValue = {targetCommandValue.ToHumanizedString()}");
#endif
                    var targetValue = _platformTypesConvertorsRegistry.Convert(targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType, targetCommandValue, context, localContext);

#if DEBUG
                    //Log($"targetValue = {targetValue}");
#endif

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

        private object[] MapGenericCallParamsByParametersByList(CancellationToken cancellationToken, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            var resultList = new List<object>();
            resultList.Add(cancellationToken);
            resultList.Add(command.Name.NameValue);
            resultList.Add(false);
            resultList.Add(null);
            resultList.Add(command.ParamsList.Select(p => (object)p).ToList());

#if DEBUG
            //Log($"resultList = {resultList.WritePODListToString()}");
#endif

            return resultList.ToArray();
        }

        private object[] MapParamsByParametersByDict(CancellationToken cancellationToken, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            if(endpointInfo.KindOfEndpoint == KindOfEndpointInfo.GenericCall)
            {
                return MapGenericCallParamsByParametersByDict(cancellationToken, endpointInfo, command, context, localContext);
            }

            var commandParamsDict = command.ParamsDict.ToDictionary(p => p.Key.NameValue.ToLower(), p => p.Value);
            var argumentsDict = endpointInfo.Arguments.Where(p => !p.IsSystemDefiend).ToDictionary(p => p.Name, p => p);

            var synonymsResolver = context.DataResolversFactory.GetSynonymsResolver();

            var resultList = new List<object>();
            resultList.Add(cancellationToken);

            foreach (var argumentItem in argumentsDict)
            {
                var argumentName = argumentItem.Key;

#if DEBUG
                //Log($"argumentName = {argumentName}");
#endif

                var argumentInfo = argumentItem.Value;

                var isBound = false;

                if (commandParamsDict.ContainsKey(argumentName))
                {
#if DEBUG
                    //Log("commandParamsDict.ContainsKey(argumentName)");
#endif

                    isBound = true;
                }
                else
                {
                    var synonymsList = synonymsResolver?.GetSynonyms(NameHelper.CreateName(argumentName), localContext).Select(p => p.NameValue).ToList();

#if DEBUG
                    //Log($"synonymsList = {synonymsList.WritePODListToString()}");
#endif

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

#if DEBUG
                //Log($"isBound = {isBound}");
                //Log($"argumentName = {argumentName}");
#endif

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

        private object[] MapGenericCallParamsByParametersByDict(CancellationToken cancellationToken, IEndpointInfo endpointInfo, ICommand command, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            var resultList = new List<object>();
            resultList.Add(cancellationToken);
            resultList.Add(command.Name.NameValue);
            resultList.Add(true);
            resultList.Add(command.ParamsDict.ToDictionary(p => p.Key.NameValue, p => (object)(p.Value.ToHumanizedString())));
            resultList.Add(null);

#if DEBUG
            //Log($"resultList = {resultList.WritePODListToString()}");
#endif

            return resultList.ToArray();
        }
    }
}

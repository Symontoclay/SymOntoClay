/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var paramsList = MapParams(cancellationToken, endpointInfo, command);

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
                    var invocableInMainThreadObj = new InvocableInMainThread(() => {
                        endpointInfo.MethodInfo.Invoke(platformListener, paramsList);
                    }, _invokingInMainThread);

                    invocableInMainThreadObj.Run();

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException e)
                {
#if DEBUG
                    //Log($"e = {e}");
                    //Log($"e.InnerException = {e.InnerException}");
#endif

                    if (e.InnerException.GetType() == typeof(OperationCanceledException))
                    {
                        processInfo.Status = ProcessStatus.Canceled;
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    //Log($"e = {e}");
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
                    //Log("Pre endpointInfo.MethodInfo.Invoke");
#endif

                    endpointInfo.MethodInfo.Invoke(platformListener, paramsList);

#if DEBUG
                    //Log("after endpointInfo.MethodInfo.Invoke");
#endif

                    processInfo.Status = ProcessStatus.Completed;
                }
                catch (TargetInvocationException e)
                {
#if DEBUG
                    //Log($"e = {e}");
                    //Log($"e.InnerException = {e.InnerException}");
#endif

                    if(e.InnerException.GetType() == typeof(OperationCanceledException))
                    {
                        processInfo.Status = ProcessStatus.Canceled;
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    //Log($"e = {e}");
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

                case KindOfCommandParameters.ParametersByList:
                    return MapParamsByParametersByList(cancellationToken, endpointInfo, command);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfCommandParameters), kindOfCommandParameters, null);
            }
        }

        private object[] MapParamsByParametersByList(CancellationToken cancellationToken, IEndpointInfo endpointInfo, ICommand command)
        {
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
                    var targetValue = _platformTypesConvertorsRegistry.Convert(targetCommandValue.GetType(), targetArgument.ParameterInfo.ParameterType, targetCommandValue);

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

                if(commandParamsDict.ContainsKey(argumentName))
                {
                    var targetCommandValue = commandParamsDict[argumentName];

                    var targetValue = _platformTypesConvertorsRegistry.Convert(targetCommandValue.GetType(), argumentInfo.ParameterInfo.ParameterType, targetCommandValue);

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
    }
}

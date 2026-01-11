/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            //logger.Info("3F6C3765-C55E-4B68-8B3A-CF9265E495A6", $"endpointInfo = {endpointInfo}");
            //logger.Info("5B55107F-4196-45C3-87E4-13C6C88A9DB8", $"command = {command}");
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
            //logger.Info("A9ECBDEC-3FF8-4A8C-9EEE-AA373F917BB4", $"paramsList?.Length = {paramsList?.Length}");
#endif

            ThreadTask task = null;
            var processInfo = new PlatformProcessInfo(cancellationTokenSource, context.GetCancellationToken(), context.AsyncEventsThreadPool, context.ActiveObjectContext, endpointInfo.Name, mapParamsResult.Item2, endpointInfo.Devices, endpointInfo.Friends, callMethodId);

#if DEBUG
            //logger.Info("F296338E-6DAC-4ED9-AA1E-D0A64DFC3285", $"processInfo != null = {processInfo != null}");
            //logger.Info("BF50C2DF-0D0F-4E01-BCF1-0CA49141CA93", $"endpointInfo.NeedMainThread = {endpointInfo.NeedMainThread}");
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
            //logger.Info("88DC782A-6B37-4AF2-A050-C0793E3BCF72", $"NEXT");
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
                //logger.Info("26D88473-854F-4CCC-A4AB-604ACCA23A52", $"paramsList?.Length = {paramsList?.Length}");
                //logger.Info("FD4F489C-7DA0-44A2-B8BF-A92EC3984CF6", $"paramsList = {paramsList?.WritePODListToString()}");
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
#if DEBUG
            //Info("3F464CB3-6258-4E07-B828-9D7EBEE96BAB", $"endpointInfo.KindOfEndpoint = {endpointInfo.KindOfEndpoint}");
#endif

            if(endpointInfo.KindOfEndpoint == KindOfEndpointInfo.GenericCall)
            {
                return MapGenericCallParamsByParametersByDict(cancellationToken, logger, endpointInfo, command, context, localContext, containsLogger);
            }

            var commandParamsDict = command.ParamsDict.ToDictionary(p => p.Key.NameValue.Replace("`", string.Empty).ToLower(), p => p.Value);
            var argumentsDict = endpointInfo.Arguments.Where(p => !p.IsSystemDefiend).ToDictionary(p => p.Name, p => p);

#if DEBUG
            //Info("76AC3DC2-41E4-4EF4-8CE3-97521C78A797", $"commandParamsDict.Count = {commandParamsDict.Count}");
            //Info("B899E4D5-15B8-4F4A-86AE-032DBF9A163C", $"argumentsDict.Count = {argumentsDict.Count}");
            //foreach (var tmpCommandParamsDictItem in commandParamsDict)
            //{
            //    Info("7BDB88D1-F2CD-4FC8-B3AB-32D26547E088", $"tmpCommandParamsDictItem.Key = {tmpCommandParamsDictItem.Key}");
            //}
            //foreach (var tmpArgumentsDictItem in argumentsDict)
            //{
            //    Info("C776E8A4-9E0B-4970-96DC-62D6F3E585A5", $"tmpArgumentsDictItem.Key = {tmpArgumentsDictItem.Key}");
            //}
#endif

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

#if DEBUG
                //Info("B823AE54-A9A9-47ED-A375-2AAD326BF506", $"argumentName = {argumentName}");
#endif

                var argumentInfo = argumentItem.Value;

                var isBound = false;

                if (commandParamsDict.ContainsKey(argumentName))
                {
#if DEBUG
                    //Info("70825EC9-6B24-4008-97C4-250F4AD4F7BF", $"commandParamsDict.ContainsKey(argumentName)");
#endif

                    isBound = true;
                }
                else
                {
                    var synonymsList = synonymsResolver?.GetSynonyms(logger, NameHelper.CreateName(argumentName), localContext).Select(p => p.NameValue.Replace("`", string.Empty).ToLower()).ToList();

                    if (!synonymsList.IsNullOrEmpty())
                    {
                        foreach (var synonym in synonymsList)
                        {
#if DEBUG
                            //Info("1CA67399-A49B-4AEA-BAE6-9AEBB4D04661", $"synonym = '{synonym}'");
#endif

                            if (isBound)
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
                //Info("9C2B290C-F27E-4205-BCB5-B3307B8392B2", $"isBound = {isBound}");
#endif

                if (isBound)
                {
                    var targetCommandValue = commandParamsDict[argumentName];

#if DEBUG
                    //logger.Info("E8CCFC39-9DBF-4EBA-AB0E-BAB7FA30F49D", $"targetCommandValue = {targetCommandValue}");
                    //logger.Info("77C21D0F-8269-46E9-9CBD-8C838AA0009C", $"targetCommandValue.GetType().FullName = {targetCommandValue.GetType().FullName}");
                    //logger.Info("ED69A69F-BD34-445A-9DC3-1E5B9FFBCFBB", $"argumentInfo.ParameterInfo.ParameterType.FullName = {argumentInfo.ParameterInfo.ParameterType.FullName}");
#endif

                    var targetValue = _platformTypesConvertorsRegistry.Convert(logger, targetCommandValue.GetType(), argumentInfo.ParameterInfo.ParameterType, targetCommandValue, context, localContext);

#if DEBUG
                    //logger.Info("A308BCFB-3CE4-44DD-8D6A-D11B3281DA72", $"targetValue = {targetValue}");
#endif

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

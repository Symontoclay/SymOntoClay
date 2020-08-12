using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CoreHostListener
{
    public class CoreHostListenerHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            //var context = TstEngineContextHelper.CreateAndInitContext();

            //var dictionary = context.Dictionary;

            //var methodName = NameHelper.CreateName("go", dictionary);

            //var listener = new TstCoreHostListener();

            //var command = new Command();
            //command.Name = methodName;
            //command.ParamsDict = new Dictionary<StrongIdentifierValue, Value>();

            //var param1Name = NameHelper.CreateName("count", dictionary);
            //var param1Value = new NumberValue(12.4);

            //command.ParamsDict[param1Name] = param1Value;

            var platformEndpointsList = new List<PlatformEndpointInfo>();

            var targetAttributesList = new List<Type>(){ typeof(PlatformEndpointAttribute), typeof(BipedPlatformEndpointAttribute) };
            var targetParameterAttributeType = typeof(PlatformEndpointParameterAttribute);

            var platformListener = new TstPlatformHostListener();

            var platformListenerTypeInfo = platformListener.GetType();

            var methodsList = platformListenerTypeInfo.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(p => p.CustomAttributes.Any(x => targetAttributesList.Contains(x.AttributeType)));

            _logger.Log($"methodsList.Count() = {methodsList.Count()}");

            foreach (var method in methodsList)
            {
                _logger.Log($"method.Name = {method.Name}");
                _logger.Log($"method.CustomAttributes.Count() = {method.CustomAttributes.Count()}");

                //foreach(var customAttribute in method.CustomAttributes.Where(p => targetAttributesList.Contains(p.AttributeType)))
                //{
                //    _logger.Log($"customAttribute.AttributeType.FullName = {customAttribute.AttributeType.FullName}");
                //    _logger.Log($"customAttribute.ConstructorArguments.Count = {customAttribute.ConstructorArguments.Count}");
                //    foreach(var constructorArg in customAttribute.ConstructorArguments)
                //    {
                //        _logger.Log($"constructorArg.ArgumentType.FullName = {constructorArg.ArgumentType.FullName}");
                //        _logger.Log($"constructorArg.ArgumentType.IsArray = {constructorArg.ArgumentType.IsArray}");

                //        if (constructorArg.ArgumentType.IsArray)
                //        {
                //            _logger.Log($"constructorArg.Value = {JsonConvert.SerializeObject(constructorArg.Value, Formatting.Indented)}");
                //        }
                //        else
                //        {
                //            _logger.Log($"constructorArg.Value = {constructorArg.Value}");
                //        }                     
                //    }
                //    _logger.Log($"customAttribute.NamedArguments.Count = {customAttribute.NamedArguments.Count}");
                //    //
                //    //_logger.Log($" = {}");
                //    //_logger.Log($" = {}");
                //}
                var parametersList = method.GetParameters();
                _logger.Log($"parametersList.Length = {parametersList.Length}");

                foreach(var parameter in parametersList)
                {
                    _logger.Log($"parameter.Name = {parameter.Name}");
                    _logger.Log($"parameter.ParameterType.FullName = {parameter.ParameterType.FullName}");

                    foreach(var paramCustomAttribute in parameter.CustomAttributes)
                    {
                        _logger.Log($"paramCustomAttribute.AttributeType.FullName = {paramCustomAttribute.AttributeType.FullName}");
                        _logger.Log($"paramCustomAttribute.ConstructorArguments.Count = {paramCustomAttribute.ConstructorArguments.Count}");
                        foreach (var constructorArg in paramCustomAttribute.ConstructorArguments)
                        {
                            _logger.Log($"constructorArg.ArgumentType.FullName = {constructorArg.ArgumentType.FullName}");
                            _logger.Log($"constructorArg.ArgumentType.IsArray = {constructorArg.ArgumentType.IsArray}");

                            if (constructorArg.ArgumentType.IsArray)
                            {
                                _logger.Log($"constructorArg.Value = {JsonConvert.SerializeObject(constructorArg.Value, Formatting.Indented)}");
                            }
                            else
                            {
                                _logger.Log($"constructorArg.Value = {constructorArg.Value}");
                            }
                        }
                        _logger.Log($"paramCustomAttribute.NamedArguments.Count = {paramCustomAttribute.NamedArguments.Count}");
                    }
                }
            }

            _logger.Log("---------------------------------------------------------");

            foreach (var method in methodsList)
            {
                _logger.Log($"method.Name = {method.Name}");

                var platformEndpointInfo = new PlatformEndpointInfo();

                platformEndpointsList.Add(platformEndpointInfo);

                platformEndpointInfo.MethodInfo = method;

                var customAttribute = method.CustomAttributes.FirstOrDefault(p => targetAttributesList.Contains(p.AttributeType));

                if(customAttribute.ConstructorArguments.Any())
                {
                    var skipParams = 0;

                    var firstParam = customAttribute.ConstructorArguments[0];

                    _logger.Log($"firstParam = {JsonConvert.SerializeObject(firstParam, Formatting.Indented)}");

                    if(firstParam.ArgumentType == typeof(string))
                    {
                        skipParams++;

                        platformEndpointInfo.Name = (string)firstParam.Value;

                        if(customAttribute.ConstructorArguments.Count > 1)
                        {
                            var secondParam = customAttribute.ConstructorArguments[1];

                            _logger.Log($"secondParam = {JsonConvert.SerializeObject(secondParam, Formatting.Indented)}");

                            if(secondParam.ArgumentType == typeof(bool))
                            {
                                skipParams++;

                                platformEndpointInfo.NeedMainThread = (bool)secondParam.Value;
                            }
                        }
                    }
                    else
                    {
                        if(firstParam.ArgumentType == typeof(bool))
                        {
                            skipParams++;

                            platformEndpointInfo.Name = method.Name.ToLower();
                            platformEndpointInfo.NeedMainThread = (bool)firstParam.Value;
                        }
                    }

                    _logger.Log($"skipParams = {skipParams}");

                    var devicesList = new List<int>();

                    foreach (var constructorArg in customAttribute.ConstructorArguments.Skip(skipParams))
                    {
                        _logger.Log($"constructorArg.ArgumentType.FullName = {constructorArg.ArgumentType.FullName}");
                        _logger.Log($"constructorArg.ArgumentType.IsArray = {constructorArg.ArgumentType.IsArray}");

                        if (constructorArg.ArgumentType.IsArray)
                        {
                            devicesList.AddRange(((IEnumerable<CustomAttributeTypedArgument>)constructorArg.Value).Select(p => (int)p.Value).ToList());
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }

                    _logger.Log($"devicesList = {JsonConvert.SerializeObject(devicesList, Formatting.Indented)}");

                    platformEndpointInfo.Devices = devicesList;
                }
                else
                {
                    platformEndpointInfo.Name = method.Name.ToLower();
                }

                platformEndpointInfo.Arguments = new List<PlatformEndpointArgumentInfo>();

                var parametersList = method.GetParameters();

                foreach (var parameter in parametersList)
                {
                    var platformEndpointArgumentInfo = new PlatformEndpointArgumentInfo();

                    platformEndpointInfo.Arguments.Add(platformEndpointArgumentInfo);

                    platformEndpointArgumentInfo.ParameterInfo = parameter;
                    platformEndpointArgumentInfo.Type = parameter.ParameterType;

                    _logger.Log($"parameter.Name = {parameter.Name}");
                    _logger.Log($"parameter.ParameterType.FullName = {parameter.ParameterType.FullName}");
                    _logger.Log($"parameter.HasDefaultValue = {parameter.HasDefaultValue}");
                    _logger.Log($"parameter.DefaultValue = {parameter.DefaultValue}");

                    var parameterCustomAttribute = parameter.CustomAttributes.FirstOrDefault(p => p.AttributeType == targetParameterAttributeType);

                    if(parameterCustomAttribute == null)
                    {
                        platformEndpointArgumentInfo.Name = parameter.Name;
                    }
                    else
                    {
                        //throw new NotImplementedException();
                    }
                }
            }

            _logger.Log($"platformEndpointsList = {platformEndpointsList.WriteListToString()}");

            //var tokenSource = new CancellationTokenSource();
            //var token = tokenSource.Token;

            //var task = new Task(() => {
            //    platformListener.GoToImpl(token, new Vector3(12, 15, 0));
            //}, token);

            //task.Start();

            //Thread.Sleep(10000);

            //_logger.Log("Cancel");

            //tokenSource.Cancel();

            //Thread.Sleep(10000);

            //var process = listener.CreateProcess(command);

            //_logger.Log($"process = {process}");

            _logger.Log("End");
        }
    }
}

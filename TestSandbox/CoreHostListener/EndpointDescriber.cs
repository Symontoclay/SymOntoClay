﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace TestSandbox.CoreHostListener
{
    public static class EndpointDescriber
    {
        private static List<Type> targetAttributesList = new List<Type>() { typeof(EndpointAttribute), typeof(BipedEndpointAttribute) };
        private static Type targetParameterAttributeType = typeof(EndpointParamAttribute);

        public static List<EndpointInfo> GetEndpointsInfoList(object platformListener)
        {
            var platformEndpointsList = new List<EndpointInfo>();

            var platformListenerTypeInfo = platformListener.GetType();

            var methodsList = platformListenerTypeInfo.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(p => p.CustomAttributes.Any(x => targetAttributesList.Contains(x.AttributeType)));

            foreach (var method in methodsList)
            {
                var platformEndpointInfo = new EndpointInfo();

                platformEndpointsList.Add(platformEndpointInfo);

                platformEndpointInfo.MethodInfo = method;

                var customAttribute = method.CustomAttributes.FirstOrDefault(p => targetAttributesList.Contains(p.AttributeType));

                if (customAttribute.ConstructorArguments.Any())
                {
                    var skipParams = 0;

                    var firstParam = customAttribute.ConstructorArguments[0];

                    if (firstParam.ArgumentType == typeof(string))
                    {
                        skipParams++;

                        platformEndpointInfo.Name = (string)firstParam.Value;

                        if (customAttribute.ConstructorArguments.Count > 1)
                        {
                            var secondParam = customAttribute.ConstructorArguments[1];

                            if (secondParam.ArgumentType == typeof(bool))
                            {
                                skipParams++;

                                platformEndpointInfo.NeedMainThread = (bool)secondParam.Value;
                            }
                        }
                    }
                    else
                    {
                        if (firstParam.ArgumentType == typeof(bool))
                        {
                            skipParams++;

                            platformEndpointInfo.Name = method.Name.ToLower();
                            platformEndpointInfo.NeedMainThread = (bool)firstParam.Value;
                        }
                    }

                    var devicesList = new List<int>();

                    foreach (var constructorArg in customAttribute.ConstructorArguments.Skip(skipParams))
                    {
                        if (constructorArg.ArgumentType.IsArray)
                        {
                            devicesList.AddRange(((IEnumerable<CustomAttributeTypedArgument>)constructorArg.Value).Select(p => (int)p.Value).ToList());
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }

                    platformEndpointInfo.Devices = devicesList;
                }
                else
                {
                    platformEndpointInfo.Name = method.Name.ToLower();
                }

                platformEndpointInfo.Arguments = new List<EndpointArgumentInfo>();

                var parametersList = method.GetParameters();

                var n = 0;

                foreach (var parameter in parametersList)
                {
                    var platformEndpointArgumentInfo = new EndpointArgumentInfo();

                    platformEndpointInfo.Arguments.Add(platformEndpointArgumentInfo);

                    platformEndpointArgumentInfo.ParameterInfo = parameter;
                    platformEndpointArgumentInfo.Type = parameter.ParameterType;

                    platformEndpointArgumentInfo.HasDefaultValue = parameter.HasDefaultValue;
                    platformEndpointArgumentInfo.DefaultValue = parameter.DefaultValue;

                    platformEndpointArgumentInfo.PositionNumber = n;

                    if (n == 0 && parameter.ParameterType == typeof(CancellationToken))
                    {
                        platformEndpointArgumentInfo.IsSystemDefiend = true;
                    }

                    var parameterCustomAttribute = parameter.CustomAttributes.FirstOrDefault(p => p.AttributeType == targetParameterAttributeType);

                    if (parameterCustomAttribute == null)
                    {
                        platformEndpointArgumentInfo.Name = parameter.Name;
                    }
                    else
                    {
                        var nameArg = parameterCustomAttribute.ConstructorArguments.SingleOrDefault(p => p.ArgumentType == typeof(string));

                        if (nameArg != null)
                        {
                            platformEndpointArgumentInfo.Name = (string)nameArg.Value;
                        }

                        var kindOfParameterArg = parameterCustomAttribute.ConstructorArguments.SingleOrDefault(p => p.ArgumentType == typeof(KindOfEndpointParam));

                        if (kindOfParameterArg != null)
                        {
                            platformEndpointArgumentInfo.KindOfParameter = (KindOfEndpointParam)kindOfParameterArg.Value;
                        }
                    }

                    n++;
                }
            }

            return platformEndpointsList;
        }
    }
}

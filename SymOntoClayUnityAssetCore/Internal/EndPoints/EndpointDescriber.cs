/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public static class EndpointDescriber
    {
        private static List<Type> targetAttributesList = new List<Type>() { typeof(EndpointAttribute), typeof(BipedEndpointAttribute) };
        private static Type friendsAttributeType = typeof(FriendsEndpointsAttribute);
        private static Type targetParameterAttributeType = typeof(EndpointParamAttribute);

#if DEBUG
        //private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static IList<IEndpointInfo> GetEndpointsInfoList(object platformListener)
        {
            var platformEndpointsList = new List<IEndpointInfo>();

            var platformListenerTypeInfo = platformListener.GetType();

            var methodsList = platformListenerTypeInfo.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(p => p.CustomAttributes.Any(x => targetAttributesList.Contains(x.AttributeType)));

            foreach (var method in methodsList)
            {
                var platformEndpointInfo = new EndpointInfo();

                platformEndpointsList.Add(platformEndpointInfo);

                platformEndpointInfo.MethodInfo = method;
                platformEndpointInfo.Object = platformListener;

                var customAttribute = method.CustomAttributes.FirstOrDefault(p => targetAttributesList.Contains(p.AttributeType));

                if (customAttribute.ConstructorArguments.Any())
                {
                    var skipParams = 0;

                    var firstParam = customAttribute.ConstructorArguments[0];

                    if (firstParam.ArgumentType == typeof(string))
                    {
                        skipParams++;

                        platformEndpointInfo.Name = ((string)firstParam.Value).ToLower();

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

                customAttribute = method.CustomAttributes.FirstOrDefault(p => p.AttributeType == friendsAttributeType);

                if (customAttribute != null && customAttribute.ConstructorArguments.Any())
                {
                    var friendsList = new List<string>();

                    var firstParam = customAttribute.ConstructorArguments[0];

#if DEBUG
                    //_logger.Info($"firstParam.ArgumentType.FullName = {firstParam.ArgumentType.FullName}");
                    //_logger.Info($"firstParam.Value.GetType().FullName = {firstParam.Value.GetType().FullName}");
#endif

                    friendsList.AddRange(((IEnumerable<CustomAttributeTypedArgument>)firstParam.Value).Select(p => ((string)(p.Value)).ToLower()).Distinct().ToList());

                    platformEndpointInfo.Friends = friendsList;
                }

                platformEndpointInfo.Arguments = new List<IEndpointArgumentInfo>();

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
                        platformEndpointArgumentInfo.Name = parameter.Name.ToLower();
                    }
                    else
                    {
                        var nameArg = parameterCustomAttribute.ConstructorArguments.SingleOrDefault(p => p.ArgumentType == typeof(string));

                        if (nameArg != null)
                        {
                            platformEndpointArgumentInfo.Name = ((string)nameArg.Value).ToLower();
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

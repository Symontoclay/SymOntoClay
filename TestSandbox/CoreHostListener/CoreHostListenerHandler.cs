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

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors.DefaultConverters;
using SymOntoClay.DefaultCLIEnvironment;
using System.Collections.Generic;
using System.Numerics;
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

            var complexContext = TstEngineContextHelper.CreateAndInitContext();

            var context = complexContext.EngineContext;
            var worldContext = complexContext.WorldContext;

            //var dictionary = context.Dictionary;

            var platformTypesConvertorsRegistry = worldContext.PlatformTypesConvertorsRegistry;

            //var platformTypesConvertorsRegistry = new PlatformTypesConvertorsRegistry(context.Logger);

            //var convertor_1 = new Vector3AndWayPointValueConvertor();

            //platformTypesConvertorsRegistry.AddConvertor(convertor_1);

            //var convertor_2 = new FloatAndNumberValueConvertor();

            //platformTypesConvertorsRegistry.AddConvertor(convertor_2);

            var endpointsRegistries = new List<IEndpointsRegistry>();

            var endpointsRegistry = new EndpointsRegistry(context.Logger);
            endpointsRegistries.Add(endpointsRegistry);

            var endpointsRegistry_2 = new EndpointsRegistry(context.Logger);
            //endpointsRegistries.Add(endpointsRegistry_2);

            var endpointsProxyRegistryForDevices = new EndpointsProxyRegistryForDevices(context.Logger, endpointsRegistry_2, new List<int>() { 1, 2, 3});
            endpointsRegistries.Add(endpointsProxyRegistryForDevices);

            var endPointsResolver = new EndPointsResolver(context.Logger, platformTypesConvertorsRegistry);

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var endPointActivator = new EndPointActivator(context.Logger, platformTypesConvertorsRegistry, invokingInMainThread);

            var platformListener = new TstPlatformHostListener();

            var platformEndpointsList = EndpointDescriber.GetEndpointsInfoList(platformListener);

            endpointsRegistry.AddEndpointsRange(platformEndpointsList);

            var gunPlatformHostListener = new TstGunPlatformHostListener();

            platformEndpointsList = EndpointDescriber.GetEndpointsInfoList(gunPlatformHostListener);

            endpointsRegistry_2.AddEndpointsRange(platformEndpointsList);

            //----------------------------------

            var methodName = NameHelper.CreateName("go");

            //var listener = new TstCoreHostListener();

            var command = new Command();
            command.Name = methodName;
            command.ParamsDict = new Dictionary<StrongIdentifierValue, Value>();

            var param1Value = new WaypointValue(25, 36, context);
            var param1Name = NameHelper.CreateName("to");

            command.ParamsDict[param1Name] = param1Value;

            //var param2Value = new NumberValue(12.4);
            //var param2Name = NameHelper.CreateName("speed", dictionary);

            //command.ParamsDict[param2Name] = param2Value;

            _logger.Log($"command = {command}");

            //----------------------------------

            var endPointInfo = endPointsResolver.GetEndpointInfo(command, endpointsRegistries);

            _logger.Log($"endPointInfo = {endPointInfo}");

            if(endPointInfo != null)
            {
                var processInfo = endPointActivator.Activate(endPointInfo, command);

                _logger.Log($"processInfo = {processInfo}");

                processInfo.Start();

                Thread.Sleep(10000);

                _logger.Log("Cancel");

                processInfo.Cancel();

                Thread.Sleep(10000);
            }

            methodName = NameHelper.CreateName("shoot");

            command = new Command();
            command.Name = methodName;

            endPointInfo = endPointsResolver.GetEndpointInfo(command, endpointsRegistries);

            _logger.Log($"endPointInfo = {endPointInfo}");

            if (endPointInfo != null)
            {
                var processInfo = endPointActivator.Activate(endPointInfo, command);

                _logger.Log($"processInfo = {processInfo}");

                processInfo.Start();

                Thread.Sleep(10000);

                _logger.Log("Cancel");

                processInfo.Cancel();

                Thread.Sleep(10000);
            }

            //----------------------------------
            //var platformListener = new TstPlatformHostListener();

            //var platformEndpointsList = EndpointDescriber.GetEndpointsInfoList(platformListener);

            //_logger.Log($"platformEndpointsList = {platformEndpointsList.WriteListToString()}");

            //var goEndpoint = platformEndpointsList.FirstOrDefault(p => p.Name == "go");

            //_logger.Log($"goEndpoint = {goEndpoint}");

            //var tokenSource = new CancellationTokenSource();
            //var token = tokenSource.Token;

            //var parameterList = new List<object>() { token, new Vector3(12, 15, 0), 25 }.ToArray();

            //var task = new Task(() =>
            //{
            //    try
            //    {
            //        goEndpoint.MethodInfo.Invoke(platformListener, parameterList);
            //    }
            //    catch(TargetInvocationException)
            //    {
            //    }
            //    catch(Exception e)
            //    {
            //        _logger.Log($"e = {e}");
            //    }

            //}, token);

            //var processInfo = new PlatformProcessInfo(task, tokenSource, goEndpoint.Devices);

            //_logger.Log($"processInfo = {processInfo}");

            //processInfo.Start();

            //Thread.Sleep(10000);

            //_logger.Log("Cancel");

            //processInfo.Cancel();

            //Thread.Sleep(10000);
            //-----------
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

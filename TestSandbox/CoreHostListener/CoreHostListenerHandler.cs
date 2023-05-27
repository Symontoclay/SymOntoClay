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

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters;
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
        private static readonly IEntityLogger _logger = new LoggerNLogImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var complexContext = TstEngineContextHelper.CreateAndInitContext();

            var context = complexContext.EngineContext;
            var worldContext = complexContext.WorldContext;


            var platformTypesConvertorsRegistry = worldContext.PlatformTypesConvertorsRegistry;






            var endpointsRegistries = new List<IEndpointsRegistry>();

            var endpointsRegistry = new EndpointsRegistry(context.Logger);
            endpointsRegistries.Add(endpointsRegistry);

            var endpointsRegistry_2 = new EndpointsRegistry(context.Logger);

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


            var methodName = NameHelper.CreateName("go");


            var command = new Command();
            command.Name = methodName;
            command.ParamsDict = new Dictionary<StrongIdentifierValue, Value>();

            var param1Value = new WaypointValue(25, 36, context);
            var param1Name = NameHelper.CreateName("to");

            command.ParamsDict[param1Name] = param1Value;



            _logger.Log($"command = {command}");


            var endPointInfo = endPointsResolver.GetEndpointInfo(command, endpointsRegistries, null);

            _logger.Log($"endPointInfo = {endPointInfo}");

            if(endPointInfo != null)
            {
                var processInfo = endPointActivator.Activate(endPointInfo, command, null, null);

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

            endPointInfo = endPointsResolver.GetEndpointInfo(command, endpointsRegistries, null);

            _logger.Log($"endPointInfo = {endPointInfo}");

            if (endPointInfo != null)
            {
                var processInfo = endPointActivator.Activate(endPointInfo, command, null, null);

                _logger.Log($"processInfo = {processInfo}");

                processInfo.Start();

                Thread.Sleep(10000);

                _logger.Log("Cancel");

                processInfo.Cancel();

                Thread.Sleep(10000);
            }

























            _logger.Log("End");
        }
    }
}

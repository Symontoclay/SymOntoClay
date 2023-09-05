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
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;

namespace TestSandbox.CoreHostListener
{
    public class CoreHostListenerHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("F0E5E372-95DC-4A4D-9A69-E7F5BFEA98A5", "Begin");

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



            _logger.Info("B1C4B68E-9161-443E-8B95-F5E31AD8E7D6", $"command = {command}");


            var endPointInfo = endPointsResolver.GetEndpointInfo(_logger, command, endpointsRegistries, null);

            _logger.Info("AADBA393-119B-4566-A1CA-67BA8B3613B1", $"endPointInfo = {endPointInfo}");

            if(endPointInfo != null)
            {
                var processInfo = endPointActivator.Activate(_logger, endPointInfo, command, null, null);

                _logger.Info("6A777CA4-CCF2-4E73-BD0C-9158EDD5163A", $"processInfo = {processInfo}");

                processInfo.Start();

                Thread.Sleep(10000);

                _logger.Info("96C7DAF4-E42C-450F-BF02-425223E4250B", "Cancel");

                processInfo.Cancel();

                Thread.Sleep(10000);
            }

            methodName = NameHelper.CreateName("shoot");

            command = new Command();
            command.Name = methodName;

            endPointInfo = endPointsResolver.GetEndpointInfo(_logger, command, endpointsRegistries, null);

            _logger.Info("0385006F-F10B-4FD6-AE62-906552DCCD6A", $"endPointInfo = {endPointInfo}");

            if (endPointInfo != null)
            {
                var processInfo = endPointActivator.Activate(_logger, endPointInfo, command, null, null);

                _logger.Info("70799BDA-A7AD-406A-BEAD-9E3363CDBE83", $"processInfo = {processInfo}");

                processInfo.Start();

                Thread.Sleep(10000);

                _logger.Info("1D370D08-B6B1-40FF-A491-6074A19F778C", "Cancel");

                processInfo.Cancel();

                Thread.Sleep(10000);
            }

























            _logger.Info("C07950F1-5895-446D-8122-BA75BA272468", "End");
        }
    }
}

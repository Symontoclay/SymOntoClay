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

            var context = TstEngineContextHelper.CreateAndInitContext();

            var dictionary = context.Dictionary;

            var methodName = NameHelper.CreateName("go", dictionary);

            //var listener = new TstCoreHostListener();

            var command = new Command();
            command.Name = methodName;
            command.ParamsDict = new Dictionary<StrongIdentifierValue, Value>();

            var param1Value = new WaypointValue(new Vector2(25, 36), context);
            var param1Name = NameHelper.CreateName("to", dictionary);

            command.ParamsDict[param1Name] = param1Value;

            var param2Value = new NumberValue(12.4);
            var param2Name = NameHelper.CreateName("speed", dictionary);

            command.ParamsDict[param2Name] = param2Value;

            //var param1Name = NameHelper.CreateName("count", dictionary);
            //var param1Value = new NumberValue(12.4);

            //command.ParamsDict[param1Name] = param1Value;

            _logger.Log($"command = {command}");

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

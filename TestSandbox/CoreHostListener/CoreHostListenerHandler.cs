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

            var platformListener = new TstPlatformHostListener();

            var platformEndpointsList = EndpointDescriber.GetEndpointsInfoList(platformListener);

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

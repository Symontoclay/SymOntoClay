using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SymOntoClay.Core;
using SymOntoClay.Monitor.Common;
using TestSandbox.PlatformImplementations;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using SymOntoClay.Monitor.Internal;

namespace TestSandbox.Handlers
{
    public class MonitorHandler
    {
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();

        private struct TstStruct
        {
            public string SomeValue { get; set; }
        }

        private class TstClass
        {
            public string SomeValue { get; set; }
            public TstClass Parent { get; set; }
            public TstClass Child { get; set; }
        }

        public void Run()
        {
            _globalLogger.Info("Begin");

            Case1();

            _globalLogger.Info("End");
        }

        private void Case1()
        {
            var monitor = new SymOntoClay.Monitor.Monitor(new MonitorSettings
            {
                Enable = true,
                MessagesDir = Path.Combine(Directory.GetCurrentDirectory(), "MessagesDir"),
                OutputHandler = message => { _globalLogger.Info($"message = {message}"); },
                KindOfLogicalSearchExplain = KindOfLogicalSearchExplain.None,
                LogicalSearchExplainDumpDir = Directory.GetCurrentDirectory(),
                EnableAddingRemovingFactLoggingInStorages = false,
                PlatformLoggers = new List<IPlatformLogger>() { /*ConsoleLogger.Instance,*/ CommonNLogLogger.Instance },
                Features = new MonitorFeatures
                {
                    EnableCallMethod = true,
                    EnableParameter = true,
                    EnableOutput = true,
                    EnableTrace = true,
                    EnableDebug = true,
                    EnableInfo = true,
                    EnableWarn = true,
                    EnableError = true,
                    EnableFatal = true
                }
                //RemoteMonitor = new RemoteWCFMonitor(new RemoteWCFMonitorSettings
                //{
                //    Address = "net.pipe://localhost/MyService.svc"
                //})
        });

            _globalLogger.Info($"monitor.SessionDirectoryName = {monitor.SessionDirectoryName}");
            _globalLogger.Info($"monitor.SessionDirectoryFullName = {monitor.SessionDirectoryFullName}");

            var nonitorNode = monitor.CreateMotitorNode("9FF4FC16-06AF-4121-BD2F-F333F1BCCE95", "soldier 1");

            var threadLogger = nonitorNode.CreateThreadLogger("687E7D1B-4E52-470B-9D6F-D4CC9C08A5FD", "f5f7ed91-77e5-45f5-88f5-b7530d111bd5");

            threadLogger.Info("d14bd986-d932-4f62-b4e3-a6a38f7fb1c0", "Some message");

            Thread.Sleep(1000);

            threadLogger.Info("0F7377DC-E8F8-4AD5-885D-B04257AD4041", "Other message");

            var callMethodId = threadLogger.CallMethod("74B6789F-FA63-4732-A4F6-642B3F4B7ABA", "Method1");

            _globalLogger.Info($"callMethodId = {callMethodId}");

            threadLogger.Parameter("1F7327DD-E8F8-4AD5-885D-B04257AD4041", callMethodId, "param1", 1);
            threadLogger.Parameter("1F7777DD-E8F8-4AD5-885D-B04257AD4041", callMethodId, "param2", new List<int> { 1, 2, 3, 4, 5 });
            threadLogger.Parameter("1F7666DD-E8F8-4AD5-885D-B04257AD4041", callMethodId, "param3", new List<int> { 1, 2, 3, 4, 5 }.ToArray());
            threadLogger.Parameter("1F7666DD-E7F8-4AD5-885D-B04257AD4041", callMethodId, "param4", new TstStruct { SomeValue = "Hi!" });
            threadLogger.Parameter("1F7666DD-E7F8-4AD6-885D-B04257AD4041", callMethodId, "param5", "Hi");
            threadLogger.Parameter("1F7666DD-E7F8-5AD5-885D-B04257AD4041", callMethodId, "param6", DateTime.Now);
            threadLogger.Parameter("1F7666DD-E7F8-4BD5-885D-B04257AD4041", callMethodId, "param7", new TstClass { SomeValue = "Bazz" });
            threadLogger.Parameter("1F7666DD-E7F9-4BD5-885D-B04257AD4041", callMethodId, "param8", KindOfMessage.Info);

            threadLogger.Output("9053861C-4AD1-4C2D-ABAB-FC1CC5DB4834", "<Yes>");

            Thread.Sleep(10000);
        }
    }
}

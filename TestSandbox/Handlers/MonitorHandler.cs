/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Monitor.LogFileBuilder;
using SymOntoClay.Threading;

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

            Case2();
            //Case1();

            _globalLogger.Info("End");
        }

        public event Action SomeEvent;
        public event Action OtherEvent;

        private void Case2()
        {
            OtherEvent += () => { };

            _globalLogger.Info($"SomeEvent?.GetInvocationList().Length = {SomeEvent?.GetInvocationList().Length}");
            _globalLogger.Info($"OtherEvent?.GetInvocationList().Length = {OtherEvent?.GetInvocationList().Length}");

            using var cancellationTokenSource = new CancellationTokenSource();

            var appName = AppDomain.CurrentDomain.FriendlyName;

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);

            var monitorMessagesDir = Path.Combine(supportBasePath, "NpcMonitorMessages");

            var monitorSettings = new SymOntoClay.Monitor.MonitorSettings
            {
                CancellationToken = cancellationTokenSource.Token,
                ThreadingSettings = ConfigureThreadingSettings().AsyncEvents,
                Enable = true,
                MessagesDir = monitorMessagesDir,
                KindOfLogicalSearchExplain = KindOfLogicalSearchExplain.None,
                //LogicalSearchExplainDumpDir = Directory.GetCurrentDirectory(),
                EnableAddingRemovingFactLoggingInStorages = true,
                EnableFullCallInfo = true
            };

            var monitor = new SymOntoClay.Monitor.Monitor(monitorSettings);

            var npcId = "#020ED339-6313-459A-900D-92F809CEBDC5";
            //---------------
            var nonitorNode = monitor.CreateMotitorNode("133776E5-AAAA-4896-AB6E-02C2902A5911", npcId);

            var threadLogger = nonitorNode.CreateThreadLogger("8507C879-EE50-4E83-B72B-FDE9662D92BC", "f5f7ed91-77e5-45f5-88f5-b7530d111bd5");

            var taskId = threadLogger.StartTask("F1DEBD17-F691-4CBD-AD6F-6D845960E252");

            _globalLogger.Info($"taskId = {taskId}");

            Thread.Sleep(1000);

            cancellationTokenSource.Cancel();

            threadLogger.StopTask("B5E884FD-D8AD-414C-A6EE-BA971B248240", taskId);

            //var taskId = logger.StartTask();
            //logger.StopTask(, taskId);
            //#if DEBUG
            //            logger.Info(, $" = {?.GetInvocationList().Length}");
            //#endif

            //---------------
            var sessionDirectoryFullName = monitor.SessionDirectoryFullName;

            var sourceDirectoryName = Path.Combine(sessionDirectoryFullName, npcId);

            var logsOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            _globalLogger.Info($"logsOutputDirectory = {logsOutputDirectory}");

            var options = LogFileCreatorOptions.DefaultOptions;

            options.Write(new LogFileCreatorOptions()
            {
                SourceDirectoryName = sourceDirectoryName,
                OutputDirectory = logsOutputDirectory,
                DotAppPath = @"%USERPROFILE%\Downloads\Graphviz\bin\dot.exe",
                ToHtml = false
            });

            //_globalLogger.Info($"options = {options}");

            LogFileCreator.Run(options, _globalLogger);
        }

        private void Case1()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            var monitor = new SymOntoClay.Monitor.Monitor(new MonitorSettings
            {
                CancellationToken = cancellationTokenSource.Token,
                ThreadingSettings = ConfigureThreadingSettings().AsyncEvents,
                Enable = true,
                MessagesDir = Path.Combine(Directory.GetCurrentDirectory(), "MessagesDir"),
                OutputHandler = message => { _globalLogger.Info($"message = {message}"); },
                KindOfLogicalSearchExplain = KindOfLogicalSearchExplain.None,
                //LogicalSearchExplainDumpDir = Directory.GetCurrentDirectory(),
                EnableAddingRemovingFactLoggingInStorages = false,
                PlatformLoggers = new List<IPlatformLogger>() { /*ConsoleLogger.Instance,*/ CommonNLogLogger.Instance },
                EnableFullCallInfo = true,
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
                }//,
                //NodesSettings = new Dictionary<string, BaseMonitorSettings>()
                //{
                //    {"soldier 1", new BaseMonitorSettings
                //        {
                //            Enable = true,
                //            Features = new MonitorFeatures
                //            {
                //                EnableCallMethod = false,
                //                EnableParameter = false,
                //                EnableOutput = false,
                //                EnableTrace = false,
                //                EnableDebug = false,
                //                EnableInfo = true,
                //                EnableWarn = false,
                //                EnableError = false,
                //                EnableFatal = false
                //            }
                //        } 
                //    }
                //},
                //EnableOnlyDirectlySetUpNodes = true
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

            var callMethodId = threadLogger.CallMethod("74B6789F-FA63-4732-A4F6-642B3F4B7ABA", "Method1", true);

            _globalLogger.Info($"callMethodId = {callMethodId}");

            threadLogger.Parameter("1F7327DD-E8F8-4AD5-885D-B04257AD4041", callMethodId, "param1", 1);
            threadLogger.Parameter("0596BFC6-FBD9-4A62-8AEB-072B2067BF42", callMethodId, "param1", null);
            threadLogger.Parameter("90AE7D5A-A9EC-4890-B686-71BD34FB197D", callMethodId, "param1", new Dictionary<int, int> { { 1, 2 } });
            threadLogger.Parameter("C84B8335-347D-4510-945A-7293DDC2B4B9", callMethodId, "param2", new List<int> { 1, 2, 3, 4, 5 });
            threadLogger.Parameter("0C23C9D2-358C-449F-9602-16E600F9936D", callMethodId, "param3", new List<int> { 1, 2, 3, 4, 5 }.ToArray());
            threadLogger.Parameter("F6C17F86-4479-4723-AB55-4A70152F4414", callMethodId, "param4", new TstStruct { SomeValue = "Hi!" });
            threadLogger.Parameter("2F6C3661-8164-4ECF-AC1D-A9CC86B5B098", callMethodId, "param5", "Hi");
            threadLogger.Parameter("D110A580-2104-46C6-9059-B632C84EEB37", callMethodId, "param6", DateTime.Now);
            threadLogger.Parameter("348B4E3F-10AE-44F3-8D1A-7B631B711B24", callMethodId, "param7", new TstClass { SomeValue = "Bazz" });
            threadLogger.Parameter("6C87763C-A783-4910-8C4F-504FE012C559", callMethodId, "param8", KindOfMessage.Info);
            threadLogger.Parameter("6F4CE980-C7FC-4DD2-AA83-3F125D3069C9", callMethodId, "param9", 1.6);

            threadLogger.Output("9053861C-4AD1-4C2D-ABAB-FC1CC5DB4834", "<Yes>");

            Thread.Sleep(10000);

            cancellationTokenSource.Cancel();
        }

        private ThreadingSettings ConfigureThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 50
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 50
                }
            };
        }
    }
}

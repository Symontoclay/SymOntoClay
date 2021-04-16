/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.CLI;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Helpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.CodeExecution;
using TestSandbox.CoreHostListener;
using TestSandbox.DateTimes;
using TestSandbox.Handlers;
using TestSandbox.Helpers;
using TestSandbox.LogicalDatabase;
using TestSandbox.MonoBehaviourTesting;
using TestSandbox.Parsing;
using TestSandbox.PlatformImplementations;
using TestSandbox.Threads;

namespace TestSandbox
{
    class Program
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            EVPath.RegVar("APPDIR", Directory.GetCurrentDirectory());

            //TstDeffuzzification();
            //TstRangeValue();
            //TstFuzzyLogicNonNumericSequenceValue();
            //TstCalculateTargetAnglesForRayScanner();
            //TstCopyFilesOnBuilding();
            //TstGetRootWorldSpaceDir();
            //TstEnvironmentVariables();
            //TstCLINewHandler();
            //TstCLIRunHandler();
            //TstLogicalDatabase();//!
            //TstProcessInfoChildren();
            //TstWaitIProcessInfo();
            //TstKindOfParametersSсaffolder();
            //TstDateTimeHandler();
            //TstBaseManualControllingGameComponent();
            //TstLoadTypesPlatformTypesConvertors();
            //TstGetTypes();
            //TstMainThreadSyncThread();
            //TstCoreHostListenerHandler();
            //TstNullableArithmetic();
            //TstInheritanceItemsHandler();
            //TstDefaultSettingsOfCodeEntityHandler();
            //TstActivateMainEntity();
            //TstCompileInlineTrigger();
            //TstRegOperatorsHandler();
            //TstCreateEngineContext();
            //TstAsyncActivePeriodicObjectHandler();
            //TstSyncActivePeriodicObjectHandler();
            //TstCodeExecution();
            //TstCreateName();
            //TstExprNodeHandler();
            //TstParsing();
            //TstMonoBehaviourTestingHandler();
            TstGeneralStartHandler();//<=
            //TstGetParsedFilesInfo();

            //Thread.Sleep(10000);
        }

        private static void TstDeffuzzification()
        {
            _logger.Log("Begin");

            var veryHandler = new VeryFuzzyLogicOperatorHandler();

            //var lHandler = new LFunctionFuzzyLogicMemberFunctionHandler(0, 0.1);
            //lHandler.CheckDirty();

            //var defuzzificatedValue = lHandler.Defuzzificate();

            //_logger.Log($"defuzzificatedValue = {defuzzificatedValue}");

            var trapezoid = new TrapezoidFuzzyLogicMemberFunctionHandler(0.3, 0.4, 0.6, 0.7);
            //trapezoid = new TrapezoidFuzzyLogicMemberFunctionHandler(10, 12, 17, 20);
            trapezoid.CheckDirty();

            var defuzzificatedValue = trapezoid.Defuzzificate();

            _logger.Log($"defuzzificatedValue = {defuzzificatedValue}");

            var naiveVeryDefuzzificatedValue = veryHandler.SystemCall(defuzzificatedValue);

            _logger.Log($"naiveVeryDefuzzificatedValue = {naiveVeryDefuzzificatedValue}");

            defuzzificatedValue = trapezoid.Defuzzificate(new List<IFuzzyLogicOperatorHandler>() { veryHandler });

            _logger.Log($"defuzzificatedValue = {defuzzificatedValue}");

            _logger.Log("End");
        }

        private static void TstRangeValue()
        {
            _logger.Log("Begin");

            var leftValue = new NumberValue(0);

            var leftBoundary = new RangeBoundary();
            leftBoundary.Value = leftValue;
            leftBoundary.Includes = true;

            var rightValue = new NumberValue(12);

            var rightBoundary = new RangeBoundary();
            rightBoundary.Value = rightValue;
            rightBoundary.Includes = true;

            var range = new RangeValue();
            range.LeftBoundary = leftBoundary;
            range.RightBoundary = rightBoundary;

            range.CheckDirty();

            _logger.Log($"range = {range}");

            _logger.Log($"range.ToDbgString() = {range.ToDbgString()}");

            _logger.Log($"range.Length = {range.Length}");

            _logger.Log($"range.IsFit(-1) = {range.IsFit(-1)}");
            _logger.Log($"range.IsFit(0) = {range.IsFit(0)}");
            _logger.Log($"range.IsFit(1) = {range.IsFit(1)}");
            _logger.Log($"range.IsFit(12) = {range.IsFit(12)}");
            _logger.Log($"range.IsFit(14) = {range.IsFit(14)}");

            _logger.Log("End");
        }

        private static void TstFuzzyLogicNonNumericSequenceValue()
        {
            _logger.Log("Begin");

            var sequence = new FuzzyLogicNonNumericSequenceValue();

            _logger.Log($"sequence = {sequence}");
            _logger.Log($"sequence = {sequence.ToDbgString()}");

            var very = NameHelper.CreateName("very");

            sequence.AddIdentifier(very);

            _logger.Log($"sequence = {sequence}");
            _logger.Log($"sequence = {sequence.ToDbgString()}");

            var teenager = NameHelper.CreateName("teenager");

            sequence.AddIdentifier(teenager);

            sequence.CheckDirty();

            _logger.Log($"sequence = {sequence}");
            _logger.Log($"sequence = {sequence.ToDbgString()}");

            _logger.Log("End");
        }

        //private static float Deg2Rad = 1f;
        private static float Deg2Rad = 0.0174532924F;

        private static void TstCalculateTargetAnglesForRayScanner()
        {
            _logger.Log("Begin");

            var TotalRaysAngle = 125;
            var TotalRaysInterval = 10;
            var FocusRaysAngle = 35;
            var FocusRaysInterval = 2;

            var anglesList = new List<(float, bool)>();

            var halfOfTotalRaysAngle = TotalRaysAngle / 2;
            var halfOfFocusRaysAngle = FocusRaysAngle / 2;

            _logger.Log($"halfOfTotalRaysAngle = {halfOfTotalRaysAngle}");
            _logger.Log($"halfOfFocusRaysAngle = {halfOfFocusRaysAngle}");

            var currAngle = 0f;
            var inFocus = true;

            do
            {
                _logger.Log($"currAngle = {currAngle}; inFocus = {inFocus}");

                var radAngle = currAngle * Deg2Rad;

                _logger.Log($"radAngle = {radAngle}");

                anglesList.Add((radAngle, inFocus));

                if(currAngle != 0)
                {
                    anglesList.Add((-1 * radAngle, inFocus));
                }                

                if (currAngle >= halfOfTotalRaysAngle)
                {
                    break;
                }

                if (currAngle < halfOfFocusRaysAngle)
                {
                    currAngle += FocusRaysInterval;
                    
                    if(currAngle > halfOfFocusRaysAngle)
                    {
                        currAngle = halfOfFocusRaysAngle;
                    }
                }
                else
                {
                    currAngle += TotalRaysInterval;
                    inFocus = false;
                }

                if(currAngle > halfOfTotalRaysAngle)
                {
                    currAngle = halfOfTotalRaysAngle;
                }
            } while (true);

            _logger.Log($"anglesList = {JsonConvert.SerializeObject(anglesList, Formatting.Indented)}");

            _logger.Log("End");
        }

        private static void TstCopyFilesOnBuilding()
        {
            _logger.Log("Begin");

            var sourceDir = @"C:/Users/Sergey/Documents/GitHub/Game1/Assets";
            var outputPath = @"C:/Users/Sergey/Documents/ExampleBuild/CustomAssetExample.exe";

            BuildPipeLine.CopyFiles(sourceDir, outputPath);

            _logger.Log("End");
        }

        private static void TstGetRootWorldSpaceDir()
        {
            _logger.Log("Begin");

            var inputFile = @"C:/Users/Sergey/Documents/GitHub/Game1/Assets\HelloWorld_Example1/World/PeaceKeeper.world";

            _logger.Log($"inputFile = '{inputFile}'");

            var wspaceDir = WorldSpaceHelper.GetRootWorldSpaceDir(inputFile);

            _logger.Log($"wspaceDir = '{wspaceDir}'");

            _logger.Log("End");
        }

        private static void TstEnvironmentVariables()
        {
            _logger.Log("Begin");

            //foreach (DictionaryEntry kvpItem in Environment.GetEnvironmentVariables())
            //{
            //    _logger.Log($"kvpItem.Key = '{kvpItem.Key}';kvpItem.Value = '{kvpItem.Value}'");
            //}

            var path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            _logger.Log($"path = '{path}'");

            var pathsList = path.Split(';').Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p));

            _logger.Log($"pathsList = {JsonConvert.SerializeObject(pathsList, Formatting.Indented)}");

            var currDir = Directory.GetCurrentDirectory();

            _logger.Log($"currDir = {currDir}");

            if(!pathsList.Contains(currDir))
            {
                path = $"{path};{currDir}";

                _logger.Log($"path (after) = '{path}'");

                Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.User);
            }

            _logger.Log("End");
        }

        private static void TstCLINewHandler()
        {
            _logger.Log("Begin");

            var args = new List<string>() {
                 "new",
                 "Enemy"
            }.ToArray();

            var command = CLICommandParser.Parse(args);

            _logger.Log($"command = {command}");



            _logger.Log("End");
        }

        private static void TstCLIRunHandler()
        {
            _logger.Log("Begin");

            var args = new List<string>() {
                 "run"//,
                 //"%USERPROFILE%/source/repos/SymOntoClay/TestSandbox/Source/Npcs/PeaceKeeper/PeaceKeeper.sobj"
                 //"%USERPROFILE%/source/repos/SymOntoClay/TestSandbox/Source"
            }.ToArray();

            var targetDirectory = EVPath.Normalize("%USERPROFILE%/source/repos/SymOntoClay/TestSandbox/Source");

            _logger.Log($"targetDirectory = {targetDirectory}");

            Directory.SetCurrentDirectory(targetDirectory);

            var command = CLICommandParser.Parse(args);

            _logger.Log($"command = {command}");

            var targetFiles = RunCommandFilesSearcher.Run(command);

            _logger.Log($"targetFiles = {targetFiles}");

            //var logDir = Path.Combine(Directory.GetCurrentDirectory(), "NpcLogs");

            var invokingInMainThread = TstInvokerInMainThreadFactory.Create();

            var instance = WorldFactory.WorldInstance;

            var settings = new WorldSettings();

            settings.SharedModulesDirs = new List<string>() { targetFiles.SharedModulesDir };

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            settings.TmpDir = targetFiles.TmpDir;

            settings.HostFile = targetFiles.WorldFile;

            settings.InvokerInMainThread = invokingInMainThread;

            settings.Logging = new LoggingSettings()
            {
                //LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { new CLIPlatformLogger() },
                Enable = true,
                EnableRemoteConnection = true
            };

            _logger.Log($"settings = {settings}");

            instance.SetSettings(settings);

            var platformListener = new TstPlatformHostListener();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            //npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = targetFiles.LogicFile;
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new TstPlatformSupport();

            _logger.Log($"npcSettings = {npcSettings}");

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            Thread.Sleep(50000);

            _logger.Log("End");
        }

        private static void TstLogicalDatabase()
        {
            var handler = new LogicalDatabaseHandler();
            handler.Run();
        }

        private static void TstProcessInfoChildren()
        {
            _logger.Log("Begin");

            var parentProcessInfo = new ProcessInfo();

            var child_1 = new ProcessInfo();
            child_1.ParentProcessInfo = parentProcessInfo;

            var child_2 = new ProcessInfo();
            parentProcessInfo.AddChild(child_2);

            _logger.Log($"parentProcessInfo = {parentProcessInfo}");

            _logger.Log("End");
        }

        private static void TstWaitIProcessInfo()
        {
            _logger.Log("Begin");

            var processInfo = new ProcessInfo();

            var task = new Task(() => {
                processInfo.Status = ProcessStatus.Running;

                Thread.Sleep(10000);

                processInfo.Status = ProcessStatus.Completed;
            });

            task.Start();

            _logger.Log("task.Start()");

            ProcessInfoHelper.Wait(processInfo);

            _logger.Log("End");
        }

        private enum KindOfParameters
        {
            NoParameters,
            NamedParameters,
            PositionedParameters
        }

        private static void TstKindOfParametersSсaffolder()
        {
            _logger.Log("Begin");

            var mainParametersList = new List<KindOfParameters>() { KindOfParameters.NoParameters, KindOfParameters.NamedParameters, KindOfParameters.PositionedParameters};
            var additionalParametersList = new List<KindOfParameters>() { KindOfParameters.NoParameters, KindOfParameters.NamedParameters, KindOfParameters.PositionedParameters };

            var strList = new StringBuilder();

            foreach(var mainParameter in mainParametersList)
            {
                _logger.Log($"mainParameter = {mainParameter}");

                var mainParameterStr = string.Empty;

                switch(mainParameter)
                {
                    case KindOfParameters.NamedParameters:
                        mainParameterStr = "_MN";
                        break;

                    case KindOfParameters.PositionedParameters:
                        mainParameterStr = "_MP";
                        break;
                }

                _logger.Log($"mainParameterStr = {mainParameterStr}");

                foreach (var additionalParameter in additionalParametersList)
                {
                    _logger.Log($"additionalParameter = {additionalParameter}");

                    var additionalParameterStr = string.Empty;

                    switch(additionalParameter)
                    {
                        case KindOfParameters.NamedParameters:
                            additionalParameterStr = "_AN";
                            break;

                        case KindOfParameters.PositionedParameters:
                            additionalParameterStr = "_AP";
                            break;
                    }

                    _logger.Log($"additionalParameterStr = {additionalParameterStr}");

                    var str = $"Call{mainParameterStr}{additionalParameterStr}";

                    _logger.Log($"str = {str}");

                    strList.AppendLine(str);
                }             
            }

            _logger.Log($"strList = {strList}");

            _logger.Log("End");
        }

        private static void TstDateTimeHandler()
        {
            _logger.Log("Begin");

            var handler = new DateTimeHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstBaseManualControllingGameComponent()
        {
            _logger.Log("Begin");

            var handler = new TstBaseManualControllingGameComponentHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstLoadTypesPlatformTypesConvertors()
        {
            _logger.Log("Begin");

            var targetAttributeType = typeof(PlatformTypesConvertorAttribute);

            var typesList = AppDomainTypesEnumerator.GetTypes().Where(p => p.GetCustomAttributesData().Any(x => x.AttributeType == targetAttributeType));

            _logger.Log($"typesList.Length = {typesList.Count()}");

            foreach (var type in typesList)
            {
                _logger.Log($"type.FullName = {type.FullName}");

                var convertor = (IPlatformTypesConvertor)Activator.CreateInstance(type);

                _logger.Log($"convertor = {convertor}");
            }

            _logger.Log("End");
        }

        private static void TstGetTypes()
        {
            _logger.Log("Begin");

            var typesList = AppDomainTypesEnumerator.GetTypes();

            _logger.Log($"typesList.Length = {typesList.Count}");

            foreach (var type in typesList)
            {
                _logger.Log($"type.FullName = {type.FullName}");
            }

            _logger.Log("End");
        }

        private static void TstMainThreadSyncThread()
        {
            _logger.Log("Begin");

            var invokingInMainThread = new InvokerInMainThread();

            var task = Task.Run(() => { 
                while(true)
                {
                    invokingInMainThread.Update();

                    Thread.Sleep(1000);
                }
            });

            Thread.Sleep(5000);

            var invocableInMainThreadObj = new InvocableInMainThread(() => { _logger.Log("^)"); }, invokingInMainThread);
            invocableInMainThreadObj.Run();

            Thread.Sleep(5000);

            _logger.Log("End");
        }

        private static void TstCoreHostListenerHandler()
        {
            _logger.Log("Begin");

            var handler = new CoreHostListenerHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstNullableArithmetic()
        {
            _logger.Log("Begin");

            float? a = null;

            var b = 1 - a;

            _logger.Log($"b = {b}");
            _logger.Log($"b == null = {b == null}");

            _logger.Log("End");
        }

        private static void TstInheritanceItemsHandler()
        {
            _logger.Log("Begin");

            var handler = new InheritanceItemsHandler();
            handler.Run();

            _logger.Log("End");
        }
            
        private static void TstDefaultSettingsOfCodeEntityHandler()
        {
            _logger.Log("Begin");

            var handler = new DefaultSettingsOfCodeEntityHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstActivateMainEntity()
        {
            _logger.Log("Begin");

            var handler = new ActivateMainEntityHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstCompileInlineTrigger()
        {
            _logger.Log("Begin");

            var handler = new CompileInlineTriggerHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstRegOperatorsHandler()
        {
            _logger.Log("Begin");

            var handler = new RegOperatorsHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstCreateEngineContext()
        {
            _logger.Log("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext();

            _logger.Log("End");
        }

        private static void TstAsyncActivePeriodicObjectHandler()
        {
            _logger.Log("Begin");

            var handler = new AsyncActivePeriodicObjectHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstSyncActivePeriodicObjectHandler()
        {
            _logger.Log("Begin");

            var handler = new SyncActivePeriodicObjectHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstCodeExecution()
        {
            _logger.Log("Begin");

            var handler = new CodeExecutionHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstCreateName()
        {
            _logger.Log("Begin");

            var parserContext = new TstMainStorageContext();

            var nameVal1 = "dog";

            _logger.Log($"{nameof(nameVal1)} = {nameVal1}");

            //var result = ParseName(nameVal1);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal2 = "dog (animal)";

            //_logger.Info($"{nameof(nameVal2)} = {nameVal2}");

            //result = ParseName(nameVal2);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal3 = "dog (animal | instrument)";

            //_logger.Info($"{nameof(nameVal3)} = {nameVal3}");

            //result = ParseName(nameVal3);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal4 = "dog (animal (alive))";

            //_logger.Info($"{nameof(nameVal4)} = {nameVal4}");

            //result = ParseName(nameVal4);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal5 = "dog (alive::animal)";

            //_logger.Info($"{nameof(nameVal5)} = {nameVal5}");

            //result = ParseName(nameVal5);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal6 = "dog (alive::animal | instrument (big))";

            //_logger.Info($"{nameof(nameVal6)} = {nameVal6}");

            //result = ParseName(nameVal6);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal7 = "animal::dog";

            //result = ParseName(nameVal7);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            //var nameVal8 = "(animal | instrument)::dog";

            //result = ParseName(nameVal8);

            //_logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            var name = NameHelper.CreateName(nameVal1);

            _logger.Log($"name = {name}");

            _logger.Log("End");
        }

        private class NormalizedNameValues
        {
            public string Name { get; set; }
            public List<string> Namespaces { get; set; }
        }

        private static NormalizedNameValues ParseName(string text)
        {
#if DEBUG
            _logger.Log($"test = {text}");
#endif

            var result = new NormalizedNameValues();

            if (!text.Contains("::") && !text.Contains("("))
            {
                result.Name = text.Trim();
                return result;
            }

            throw new NotImplementedException();
        }

        private static void TstExprNodeHandler()
        {
            _logger.Log("Begin");

            var handler = new ExprNodeHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstParsing()
        {
            _logger.Log("Begin");

            var handler = new ParsingHandler();
            handler.Run();

            _logger.Log("End");
        }


        private static void TstMonoBehaviourTestingHandler()
        {
            _logger.Log("Begin");

            var handler = new MonoBehaviourTestingHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstGeneralStartHandler()
        {
            _logger.Log("Begin");

            var handler = new GeneralStartHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstGetParsedFilesInfo()
        {
            _logger.Log("Begin");

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\PixKeeper\PixKeeper.txt");

            _logger.Log($"fileName = {fileName}");

            var id = "#020ED339-6313-459A-900D-92F809CEBDC5";

            var resultList = FileHelper.GetParsedFilesInfo(fileName, id);

            _logger.Log($"resultList = {JsonConvert.SerializeObject(resultList, Formatting.Indented)}");

            _logger.Log("End");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Log($"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}

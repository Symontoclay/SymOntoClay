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

using DictionaryGenerator;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json;
using NUnit.Framework;
using SymOntoClay.BaseTestLib;
using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.CLI;
using SymOntoClay.Common;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic;
using SymOntoClay.CoreHelper;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.LogFileBuilder;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.NLP;
using SymOntoClay.ProjectFiles;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Helpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.CodeExecution;
using TestSandbox.CoreHostListener;
using TestSandbox.CreatingExamples;
using TestSandbox.DateTimes;
using TestSandbox.Handlers;
using TestSandbox.Helpers;
using TestSandbox.LogicalDatabase;
using TestSandbox.MonoBehaviorTesting;
using TestSandbox.Navigations;
using TestSandbox.Parsing;
using TestSandbox.SoundBusHandler;
using TestSandbox.Threads;

namespace TestSandbox
{
    class Program
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            EVPath.RegVar("APPDIR", Directory.GetCurrentDirectory());

            _globalLogger.Info($"args = {JsonConvert.SerializeObject(args, Formatting.Indented)}");

            //TstLogFileBuilderAppCommandLineParserHandler();
            //TstThreadTask();
            //TstThreadPoolCount();
            //TstLogFileBuilderParameterValueConverterToString();
            //TstLogFileBuilder();
            //TstMonitor();
            //TstCreateListByVarsDict();
            //TstDetectDominantItems();
            //TstSerializeValue();
            //TstWaitAsync();
            //TstWaitAsync_2();
            //TstWaitAsync_1();
            //TstWorldSpaceFilesSearcher();
            //TstSynonymsHandler();
            //TstGetFullBaseTypesListInCSharpReflection();
            //TstConvertFactToImperativeCode();
            //TstFactToHtml();
            //TstStandardFactsBuilder();
            //TstStandardFactsBuilderGetTargetVarNameHandler();
            //TstShieldString();
            //TstSampleSpeechSynthesis();
            //TstOnAddingFactEventHandler();
            //TstEventHandler();
            //TstStrCollectionCombination();
            //TstIntCollectionCombination();
            //TstModalitiesHandler();
            //TstRelationsStorageHandler();
            //TstGenerateDllDict();
            //TSTWordsFactory();
            //TstNLPConverterProvider();
            //TstNLPHandler();//<=NLP
            //TstTriggerConditionNodeHandler();
            //TstSoundBus();
            //TstNavigationHandler();
            //TstCreatorExamples();
            //TstLinguisticVariable_Tests();
            //TstManageTempProject();
            //TstAdvancedTestRunnerForMultipleInstances();//<=~
            //TstAdvancedTestRunner();//<=
            //TstAdvancedTestRunnerWithListenedFact();
            //TstTestRunnerBehaviorTestEngineInstance();//$$$
            //TstTestRunnerWithHostListener();//<=t
            //TstTestRunner();//<=
            //TstNameHelper();
            //TstDeffuzzification();
            //TstRangeValue();
            //TstFuzzyLogicNonNumericSequenceValue();
            //TstCalculateTargetAnglesForRayScanner();
            //TstCopyFilesOnBuilding();
            //TstGetRootWorldSpaceDir();
            //TstEnvironmentVariables();
            //TstCLINewHandler();
            //TstCLIRunHandler();
            //TstCLICommandParser();
            //TstLogicalDatabase();//!
            //TstProcessInfoChildren();
            //TstWaitIProcessInfo();
            //TstKindOfParametersScaffolder();
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
            //TstBattleRoyaleHandler();//<==
            //TstPlacesHandler();//<==
            //TstMonoBehaviorTestingHandler();//VT<=
            //TstSoundStartHandler();//<==
            //TstAddingFactTriggerHandler();
            //TstHtnHandler();
            TstGeneralStartHandler();//<=
            //TstGetParsedFilesInfo();

            //Thread.Sleep(10000);
        }

        private static void TstLogFileBuilderAppCommandLineParserHandler()
        {
            _globalLogger.Info("Begin");

            var handler = new LogFileBuilderAppCommandLineParserHandler();
            handler.Run();

            _globalLogger.Info("End");
        }

        private static void TstThreadTask()
        {
            _globalLogger.Info("Begin");

            using var source = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20);

            var task = new ThreadTask(() => {
                _globalLogger.Info("Run");
            }, threadPool, source.Token);

            task.OnStarted += () => { _globalLogger.Info("task.OnStarted"); };
            task.OnCanceled += () => { _globalLogger.Info("task.OnCanceled"); };
            task.OnCompleted += () => { _globalLogger.Info("task.OnCompleted"); };
            task.OnCompletedSuccessfully += () => { _globalLogger.Info("task.OnCompletedSuccessfully"); };
            task.OnFaulted += () => { _globalLogger.Info("task.OnFaulted"); };

            task.Start();

            Thread.Sleep(1000);

            _globalLogger.Info("End");
        }

        private static void TstThreadPoolCount()
        {
            _globalLogger.Info("Begin");

            ThreadPool.GetMinThreads(out var workerThreadsMin, out var completionPortThreadsMin);

            _globalLogger.Info($"workerThreadsMin = {workerThreadsMin}");
            _globalLogger.Info($"completionPortThreadsMin = {completionPortThreadsMin}");

            ThreadPool.GetMaxThreads(out var workerThreadsMax, out var completionPortThreadsMax);

            _globalLogger.Info($"workerThreadsMax = {workerThreadsMax}");
            _globalLogger.Info($"completionPortThreadsMax = {completionPortThreadsMax}");

            ThreadPool.SetMinThreads(workerThreadsMax, completionPortThreadsMax);

            _globalLogger.Info("End");
        }

        private static void TstLogFileBuilderParameterValueConverterToString()
        {
            _globalLogger.Info("Begin");

            var typeName = "System.Collections.Generic.List`1[[System.Int32, System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]";
            var base64Content = "eyIkaWQiOiIxIiwiJHZhbHVlcyI6WzEsMiwzLDQsNV19";

            _globalLogger.Info($"typeName = {typeName}");
            _globalLogger.Info($"base64Content = '{base64Content}'");

            //var base64bytes = Convert.FromBase64String(base64Content);

            //var jsonStr = Encoding.UTF8.GetString(base64bytes);

            //_globalLogger.Info($"jsonStr = {jsonStr}");

            //var type = Type.GetType(typeName);

            //_globalLogger.Info($"type.FullName = {type.FullName}");

            var result = ObjectToHumanizedStringConverter.FromBase64StringToHumanizedString(base64Content, typeName);

            _globalLogger.Info($"result = {result}");

            _globalLogger.Info("End");
        }

        private static void TstLogFileBuilder()
        {
            _globalLogger.Info("Begin");

            var handler = new LogFileBuilderHandler();
            handler.Run();

            _globalLogger.Info("End");
        }

        private static void TstMonitor()
        {
            _logger.Info("62684563-F740-4A47-B812-50CBA43FF2BF", "Begin");

            var handler = new MonitorHandler();
            handler.Run();

            _logger.Info("EDA7FA8A-840B-4E39-8CBF-92A617BDBEE0", "End");
        }

        private static void TstCreateListByVarsDict()
        {
            _logger.Info("DBD25DC2-47A1-40E2-AA21-CD3A5A2DC2E1", "Begin");

            var source = new Dictionary<string, List<string>>();

            source["$x"] = new List<string>() { "#1", "#2", "#3" };
            source["$y"] = new List<string>() { "cat", "dog", "tree" };
            source["$z"] = new List<string>() { "town", "city", "village" };

            _logger.Info("3E2E0C8D-5D61-4691-A486-6C82590293BB", $"source = {JsonConvert.SerializeObject(source, Formatting.Indented)}");

            var keysList = source.Keys.ToList();

            _logger.Info("FB5FB564-1135-47D1-8001-D66ABB67D154", $"keysList = {JsonConvert.SerializeObject(keysList, Formatting.Indented)}");

            var result = new List<List<(string, string)>>();

            TstProcessCreateListByVarsDict(0, keysList, source, new List<(string, string)>(), ref result);

            _logger.Info("808200DC-90FE-40E3-AA1E-C0F1C0BAF3BA", $"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            _logger.Info("DE146257-00B1-4B93-B6B9-DD6B883481F7", "End");
        }

        private static void TstProcessCreateListByVarsDict(int n, List<string> keysList, Dictionary<string, List<string>> source, List<(string, string)> currentValues, ref List<List<(string, string)>> result)
        {
            _logger.Info("3B552CC3-57F5-4A8A-8B76-B5A321009277", $"n = {n}");
            _logger.Info("8A703D2E-D713-4E4B-9E99-3DCC633A3070", $"keysList.Count = {keysList.Count}");
            _logger.Info("512616F8-3B37-4B23-9551-D8D55E7375FA", $"currentValues = {JsonConvert.SerializeObject(currentValues, Formatting.Indented)}");

            if (n == keysList.Count - 1)
            {
                TstProcessCreateListByVarsDictFinalNode(n, keysList, source, currentValues, ref result);
            }
            else
            {
                TstProcessCreateListByVarsDictIntermediateNode(n, keysList, source, currentValues, ref result);
            }
        }

        private static void TstProcessCreateListByVarsDictIntermediateNode(int n, List<string> keysList, Dictionary<string, List<string>> source, List<(string, string)> currentValues, ref List<List<(string, string)>> result)
        {
            _logger.Info("3A520F78-D5B9-4A75-B3FB-FA80A689B7AE", $"n = {n}");
            _logger.Info("6B149AD3-C1C3-4BCD-9FC2-E9F68C622241", $"keysList.Count = {keysList.Count}");
            _logger.Info("13EC9048-45AE-4C85-8804-934DDBDBEEC2", $"currentValues = {JsonConvert.SerializeObject(currentValues, Formatting.Indented)}");

            var key = keysList[n];

            _logger.Info("49E908B4-251B-47A3-9BC7-6E26D22425D1", $"key = {key}");

            var list = source[key];

            var nextN = n + 1;

            foreach (var item in list)
            {
                _logger.Info("802E4E3F-F1CD-4796-9288-BF3F344E53A9", $"item = {item}");

                var newCurrentValues = currentValues.ToList();
                newCurrentValues.Add((key, item));

                TstProcessCreateListByVarsDict(nextN, keysList, source, newCurrentValues, ref result);
            }
        }

        private static void TstProcessCreateListByVarsDictFinalNode(int n, List<string> keysList, Dictionary<string, List<string>> source, List<(string, string)> currentValues, ref List<List<(string, string)>> result)
        {
            _logger.Info("BCEE3A75-2913-4C06-B09A-F0F38E802213", $"n = {n}");
            _logger.Info("523CCF9A-8D42-4738-9A77-3B5221C07883", $"keysList.Count = {keysList.Count}");
            _logger.Info("2D3D6791-C8A9-4823-A73F-B0F1243CBBFE", $"currentValues = {JsonConvert.SerializeObject(currentValues, Formatting.Indented)}");

            var key = keysList[n];

            _logger.Info("88D2E766-5F6F-4C68-9171-DB899283B0CA", $"key = {key}");

            var list = source[key];

            foreach (var item in list)
            {
                _logger.Info("6F6FAD7B-543D-4021-9E55-B74BAF6E2497", $"item = {item}");

                var resultItem = new List<(string, string)>();
                result.Add(resultItem);

                foreach(var currentValue in currentValues)
                {
                    resultItem.Add(currentValue);
                }

                resultItem.Add((key, item));
            }
        }

        private static void TstDetectDominantItems()
        {
            _logger.Info("51C3F12C-FE0A-4252-8CC1-704C0303292B", "Begin");

            var initialList = new List<KindOfLogicalQueryNode>() { KindOfLogicalQueryNode.Entity, KindOfLogicalQueryNode.Concept, KindOfLogicalQueryNode.Value, KindOfLogicalQueryNode.Entity };

            _logger.Info("871009ED-8BBF-4847-A740-1FADB4E745DF", $"initialList = {JsonConvert.SerializeObject(initialList, Formatting.Indented)}");

            var orderedList = initialList.GroupBy(p => p).Select(p => new
            {
                Value = p.Key,
                Count = p.Count()
            }).OrderByDescending(p => p.Count).Take(1);

            _logger.Info("E59492F2-F862-49B9-B604-5DFF10C0DCFE", $"orderedList = {JsonConvert.SerializeObject(orderedList, Formatting.Indented)}");

            _logger.Info("FBC541AB-A68A-4854-B5B9-88BC0DD9BF3B", "End");
        }

        private static void TstSerializeValue()
        {
            _logger.Info("B32E1C96-D43F-440E-A8E8-3F5285BF611F", "Begin");

            var list = new List<object>() { new NumberValue(16) };

            _logger.Info("414395AA-7875-47E2-B70C-C93BADF1EF7D", $"list = {JsonConvert.SerializeObject(list, Formatting.Indented, new ToHumanizedStringJsonConverter())}");

            _logger.Info("8F8AB60B-F23B-469A-8DD6-29D4CBDAB78E", "End");
        }

        private static void TstWaitAsync()
        {
            _logger.Info("D0577E69-6698-493D-B025-4AB190ABB503", "Begin");

            var source1 = new CancellationTokenSource();
            var token1 = source1.Token;

            using var threadPool = new CustomThreadPool(0, 20);

            var task = ThreadTask.Run(() => {
                _logger.Info("EB024ABD-0889-4DEF-A6E9-1D36CA08F839", "Hi!");

                Thread.Sleep(10000);

                _logger.Info("F812B258-8F6B-4066-8698-55DA609BD01D", "End Hi!");
            }, threadPool, token1);

            //var taskValue = new TaskValue(task);

            //taskValue.OnComplete += () => 
            //{
            //    _logger.Info("2CD55AB4-7F0D-49BE-B17F-455D69263E48", "taskValue.OnComplete!!!!!");
            //};

            //taskValue.OnComplete += () =>
            //{
            //    _logger.Info("674D6CA3-085C-4174-9B21-6AA992F99157", "(2) taskValue.OnComplete!!!!!");
            //};

            Thread.Sleep(20000);

            _logger.Info("00DA50E0-1BCF-43C6-B37C-09F133FFFA9C", "End");
        }

        private static void TstWaitAsync_2()
        {
            _logger.Info("2532A04C-BCEC-490C-88F7-B3CD786815A3", "Begin");

            var source1 = new CancellationTokenSource();
            var token1 = source1.Token;

            var task = Task.Run(() => {
                _logger.Info("A8DE6271-F6E5-46C4-904D-A155F4565C35", "Hi!");

                Thread.Sleep(10000);

                _logger.Info("8F32F920-C5F9-471D-8128-36959249E753", "End Hi!");
            }, token1);

            Task.Run(() => {
                var waitingTask = task.WaitAsync(token1);

                waitingTask.Wait();

                _logger.Info("B38F5136-B803-452E-AFDF-2D429BA8D7A4", $"Finished!!!! task.Status = {task.Status}");
            });

            Thread.Sleep(20000);

            _logger.Info("CF3E7AE0-132B-40E7-8B39-C7887BF4EC3B", "End");
        }

        private static void TstWaitAsync_1()
        {
            _logger.Info("DB61B7C2-9E4A-420B-B58E-69346B983A85", "Begin");

            var source1 = new CancellationTokenSource();
            var token1 = source1.Token;

            var task = Task.Run(() => {
                _logger.Info("69910F34-E5D9-4C14-9B19-2A6F23E8C0DE", "Hi!");

                Thread.Sleep(10000);

                _logger.Info("FEAC882C-8F5C-475D-BBBE-801EC9C176A3", "End Hi!");
            }, token1);

            var waitingTask = task.WaitAsync(token1);

            waitingTask.Wait();

            _logger.Info("9E45461D-12C0-4F44-AF7F-0DEBB2FFA132", "End");
        }

        private static void TstWorldSpaceFilesSearcher()
        {
            _logger.Info("01DED674-A50F-4511-ADC9-EBE519F3B165", "Begin");

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = @"C:\Users\Acer\source\repos\SymOntoClayAsset\Assets\SymOntoClayScripts",
                AppName = "tst1",
                SearchMainNpcFile = false
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

#if DEBUG
            _logger.Info("ACB0323E-C2EA-488E-AC70-8FBE031549C8", $"targetFiles = {targetFiles}");
#endif 

            _logger.Info("C76B8D27-C94A-4241-B08C-3318B37EE2AC", "End");
        }

        private static void TstSynonymsHandler()
        {
            _logger.Info("ED22070B-EDBE-4559-A4C7-E5856348A332", "Begin");

            var handler = new SynonymsHandler();
            handler.Run();

            _logger.Info("3400CEE0-AAD1-4056-ACE3-9FB1AA8CA32F", "End");
        }

        private static void TstGetFullBaseTypesListInCSharpReflection()
        {
            _logger.Info("533CA9AC-7003-400F-AB4F-06EBF740BBC0", "Begin");

            var source = typeof(ConditionalEntityValue);

            _logger.Info("4024F159-B9E8-4C1B-9F34-494542F32C1C", $"source.FullName = {source.FullName}");

            var interfacesList = source.GetInterfaces();

            _logger.Info("E785E6ED-5462-4730-B204-C6E9F7AAD935", $"interfacesList.Length = {interfacesList.Length}");

            foreach(var item in interfacesList)
            {
                _logger.Info("E6F6AE7A-60C1-44C5-A61E-6E739CED8793", $"item.FullName = {item.FullName}");
            }

            PrintBaseType(source.BaseType);

            _logger.Info("6AA062AB-BADA-4431-AA32-32F544FF7238", "End");
        }

        private static void PrintBaseType(Type type)
        {
            if(type == null)
            {
                return;
            }

            _logger.Info("B90F8481-008E-426E-96CF-787586A984AD", $"type.FullName = {type.FullName}");

            PrintBaseType(type.BaseType);
        }

        private static void TstConvertFactToImperativeCode()
        {
            _logger.Info("01A26185-263A-4BDC-8531-F6E569EFF4D0", "Begin");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :}";

            _logger.Info("2AA3C13A-5F7B-4BFF-8CBE-DE9ABFC320E9", $"factStr = '{factStr}'");

            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = engineContext.Storage.GlobalStorage;
            localCodeExecutionContext.Holder = NameHelper.CreateName(engineContext.Id);

            var compiledCode = engineContext.ConvertersFactory.GetConverterFactToImperativeCode().Convert(_logger, fact, localCodeExecutionContext);

            _logger.Info("C815E21A-3AEF-4647-83E5-70C42708C2FB", $"compiledCode = {compiledCode.ToDbgString()}");

            _logger.Info("1019AE4E-43EF-430D-B1D9-B068E525A18E", "End");
        }

        private static void TstFactToHtml()
        {
            _logger.Info("A9743633-88AD-4C93-9B47-E684D52E95AD", "Begin");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";

            _logger.Info("9F220482-88E1-4002-9810-38B5058D9388", $"factStr = '{factStr}'");

            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            var targetItemForSelection = fact.PrimaryPart.Expression.Left.Left.Left.Left;

            _logger.Info("8CBF8F4B-80A4-4691-BEA3-C14CF5A0C748", $"targetItemForSelection = {targetItemForSelection.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");


            var options = new DebugHelperOptions();
            options.HumanizedOptions = HumanizedOptions.ShowOnlyMainContent;
            options.IsHtml = true;
            options.ItemsForSelection = new List<IObjectToString>() { targetItemForSelection };

            _logger.Info("16AE26A1-F184-4673-B479-477BF8659EA0", $"fact = {DebugHelperForRuleInstance.ToString(fact, options)}");

            _logger.Info("A7FF14E8-6952-4F56-8A85-783EBBB60162", "End");
        }

        private static void TstStandardFactsBuilder()
        {
            _logger.Info("99CE32B2-8653-4FEA-98E7-C98FBBEDD1F8", "Begin");

            var handler = new StandardFactsBuilderHandler();
            handler.Run();

            _logger.Info("52811802-DE88-4B02-BFAD-D7691D06608D", "End");
        }

        private static void TstStandardFactsBuilderGetTargetVarNameHandler()
        {
            _logger.Info("686B9BD2-C1EA-423B-AE69-8862FE232B93", "Begin");

            var handler = new StandardFactsBuilderGetTargetVarNameHandler();
            handler.Run();

            _logger.Info("52A89F42-0117-4FC7-BD0C-ABC5EA23DC6B", "End");
        }            

        private static void TstShieldString()
        {
            _logger.Info("AF228818-2973-4859-9F2A-573557010C19", "Begin");

            var str = "@@self";

            _logger.Info("19482D8F-CCE1-47B2-B69D-E3DE5D6DDFBB", $"str = '{str}'");

            var nameSubStr = str.Substring(2);

            _logger.Info("02CBA414-2257-4965-8DFC-569DCEF7AB07", $"nameSubStr = '{nameSubStr}'");

            var name = NameHelper.ShieldString(str);

            _logger.Info("E19DA3AB-AE64-4649-9F98-EF04C04AFB3E", $"name = '{name}'");

            _logger.Info("8382B2E2-8262-4844-BE1F-891C2D3A7B06", "End");
        }

        private static void TstSampleSpeechSynthesis()
        {
            _logger.Info("8DE15CDF-8306-497C-8564-21A074DBF119", "Begin");

            var synth = new SpeechSynthesizer();

            synth.SetOutputToDefaultAudioDevice();

            synth.Speak("This example demonstrates a basic use of Speech Synthesizer");
            synth.Speak("Go to green place!");

            _logger.Info("E1B00B6D-5432-420D-BFAF-B679A9E6CAD3", "End");
        }

        private static void TstOnAddingFactEventHandler()
        {
            _logger.Info("A42581B7-AF6A-404D-A955-FF1D7BE94579", "Begin");

            var handler = new OnAddingFactEventHandler();
            handler.Run();

            _logger.Info("3F8D1A27-A27D-4D2A-9EBD-A2470BC923DF", "End");
        }

        private static void TstEventHandler()
        {
            _logger.Info("802C193A-95BE-40B1-BB18-421668003039", "Begin");

            var handler = new Handlers.EventHandler();
            handler.Run();

            _logger.Info("0918FFF7-E3F7-467B-A52C-C1D6F9AD3C6F", "End");
        }

        private static void TstStrCollectionCombination()
        {
            _logger.Info("0E8C7739-639B-45B5-A685-20DCA62B8268", "Begin");

            var source = new List<List<string>>();

            source.Add(new List<string>() { "a", "e" });
            source.Add(new List<string>() { "h", "j", "L" });
            source.Add(new List<string>() { "b", "c", "f" });

            _logger.Info("F5CD028E-7750-4E19-B4A7-5AADE6BF8EB9", $"source = {JsonConvert.SerializeObject(source, Formatting.Indented)}");

            var result = CollectionCombinationHelper.Combine(source);

            _logger.Info("A3189D70-D4D1-45FF-8764-15EC7FB0C7C9", $"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            _logger.Info("9BF05596-9FA1-4653-8DF6-4EA4EB5CBBA8", "End");
        }

        private static void TstIntCollectionCombination()
        {
            _logger.Info("A312B861-C4F1-4FBC-90E6-2DD42EB416C0", "Begin");

            var source = new List<List<int>>();

            source.Add(new List<int>() { 1, 5 });
            source.Add(new List<int>() { 7, 9, 11 });
            source.Add(new List<int>() { 2, 3, 6 });

            _logger.Info("F3F0C842-98B8-4A11-9066-5B7E7C2FCEFD", $"source = {JsonConvert.SerializeObject(source, Formatting.Indented)}");

            var result = CollectionCombinationHelper.Combine(source);

            _logger.Info("8282395B-5136-4827-9514-A1A8C4A1D604", $"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            _logger.Info("BC71A88F-7E01-42A3-96CA-E774A33AC271", "End");
        }

        private static void TstModalitiesHandler()
        {
            _logger.Info("A354A53B-2F99-4CB9-9FA4-E65ECE61CB4C", "Begin");

            var handler = new ModalitiesHandler();
            handler.Run();

            _logger.Info("80905D58-0907-4B45-B446-4D5DD8A0F896", "End");
        }

        private static void TstRelationsStorageHandler()
        {
            _logger.Info("9491BA22-5021-4180-BACC-ACC213630CC1", "Begin");

            var handler = new RelationsStorageHandler();
            handler.Run();

            _logger.Info("AECA21EC-D4A4-400F-8C1F-AAA809AF95ED", "End");
        }

        private static void TstGenerateDllDict()
        {
            _logger.Info("36A7E4CB-918F-47C0-828B-29999B7772ED", "Begin");

            var handler = new GenerateDllDictHandler();
            handler.Run();

            _logger.Info("F3623C7E-4AC5-44AE-A455-EEA041C74367", "End");
        }

        private static void TSTWordsFactory()
        {
            _logger.Info("E5E9649D-480C-4B18-8E39-2AB5B03278DE", "Begin");

            var wordsFactory = new WordsFactory();
            wordsFactory.Run();

            _logger.Info("43313870-E3A7-4854-B0C1-62E7659DF7BF", "End");
        }

        private static void TstNLPConverterProvider()
        {
            _logger.Info("67B93130-EFEE-4076-AA45-A3E06476CA58", "Begin");

            var mainDictPath = Path.Combine(Directory.GetCurrentDirectory(), "Dicts", "BigMainDictionary.dict");

#if DEBUG
            _logger.Info("5A8EE3B2-4D86-47E6-B9A1-389230C11227", $"mainDictPath = {mainDictPath}");
#endif

            var settings = new NLPConverterProviderSettings();
            settings.DictsPaths = new List<string>() { mainDictPath };
            settings.CreationStrategy = CreationStrategy.Singleton;

#if DEBUG
            _logger.Info("30456AC0-AF91-4B0D-83EF-88F301022713", $"settings = {settings}");
#endif

            var nlpConverterProvider = new NLPConverterProvider(settings);

            var factory = nlpConverterProvider.GetFactory(_logger);

            var converter = factory.GetConverter();

            _logger.Info("EB98D2D8-19C5-481F-9867-2BC0DAF9A0F2", "End");
        }

        private static void TstNLPHandler()
        {
            _logger.Info("A27B6690-4B39-491E-83AA-332424991282", "Begin");

            var handler = new NLPHandler();
            handler.Run();

            _logger.Info("B02392AD-7091-4FB2-93D8-51EA737DD550", "End");
        }

        private static void TstTriggerConditionNodeHandler()
        {
            _logger.Info("934C5700-D080-4BF8-8F39-87C63A126D81", "Begin");

            var handler = new TriggerConditionNodeHandler();
            handler.Run();

            _logger.Info("B527FC41-1A8B-45A9-9450-ECB9E7A2D897", "End");
        }

        private static void TstSoundBus()
        {
            _logger.Info("F0914D51-3508-41F4-8185-431B857A73FC", "Begin");

            var handler = new TstSoundBusHandler();
            handler.Run();

            _logger.Info("BA631638-8254-4678-AAEC-E6CB1B3D8311", "End");
        }

        private static void TstNavigationHandler()
        {
            _logger.Info("3E8DD248-66F6-4C2C-A0C2-EC74B19DBED3", "Begin");

            var handler = new NavigationHandler();
            handler.Run();

            _logger.Info("0759EE0B-20A3-4CD4-AE10-E50228584A05", "End");
        }

        private static void TstCreatorExamples()
        {
            _logger.Info("35B0586E-17AA-4B9B-9868-172A242A22DB", "Begin");

            using var handler = new CreatorExamples_States_27_03_2022();
            handler.Run();

            _logger.Info("BBC9DC55-88A4-4787-A4ED-0F4D54875D0D", "End");
        }

        private static void TstLinguisticVariable_Tests()
        {
            _logger.Info("E0960944-B88C-4A1E-B4D8-415F307968B1", "Begin");

            var text = @"linvar logic for range [0, 1]
{
    constraints:
	    for inheritance;

	terms:
		minimal = L(0, 0.1);
		low = Trapezoid(0, 0.05, 0.3, 0.45);
		middle = Trapezoid(0.3, 0.4, 0.6, 0.7);
		high = Trapezoid(0.55, 0.7, 0.95, 1);
		maximal = S(0.9, 1);
}";

            var mainStorageContext = new UnityTestMainStorageContext();

            var codeFile = new CodeFile();

            var internalParserContext = new InternalParserContext(text, codeFile, mainStorageContext);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var result = parser.Result;

            var firstItem = result.SingleOrDefault();

            firstItem.AsLinguisticVariable.CheckDirty();

            _logger.Info("6A0E9F63-280F-4EE4-B0EF-C9507214CCA9", $"firstItem = {firstItem}");

            var term = firstItem.AsLinguisticVariable.Values[4];

            _logger.Info("4696A996-DCB7-48FF-83AC-C5D0E1B5E7AD", $"term = {term}");

            var handler = term.Handler;

            _logger.Info("9771A1B2-51CE-493B-B944-8784446682F0", $"handler = {handler}");

            _logger.Info("E765CFD3-12D6-4000-8663-A14FA68510F1", "End");
        }

        private static void TstManageTempProject()
        {
            _logger.Info("7CD26702-DE44-4FEF-AE4E-F133FAB3811F", "Begin");

            var initialDir = Directory.GetCurrentDirectory();

            _logger.Info("66BEB294-CD1F-43C5-BDF6-711CA699E669", $"initialDir = {initialDir}");

            initialDir = Path.Combine(initialDir, "TempProjects");

            _logger.Info("E70C56C9-A719-45D0-97FB-737DC6B92DEC", $"initialDir (2) = {initialDir}");

            if(!Directory.Exists(initialDir))
            {
                Directory.CreateDirectory(initialDir);
            }

            var testDir = Path.Combine(initialDir, $"TstDir{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            _logger.Info("FA8C941F-8D9D-4B54-B045-05A103E24A2E", $"testDir = {testDir}");

            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            var projectName = "Example";

            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { ProjectName = projectName };

            _logger.Info("06965C85-B17F-4360-800B-B94BBDD614F9", $"worldSpaceCreationSettings = {worldSpaceCreationSettings}");

            var wSpaceFile = WorldSpaceCreator.CreateWithOutWSpaceFile(worldSpaceCreationSettings, testDir
                    , errorMsg => _logger.Error("8726C7D4-DC3B-4484-BBD1-5C1F74675D79", errorMsg)
                    );

            _logger.Info("2EC1687B-02D6-4045-BAB4-CCDBA6FF50E8", $"wSpaceFile = {wSpaceFile}");

            var wSpaceDir = wSpaceFile.DirectoryName;

            _logger.Info("0896F326-B847-4BA2-A0BE-9B75F88C9BE6", $"wSpaceDir = {wSpaceDir}");

            var targetRelativeFileName = @"/Npcs/Example/Example.soc";

            _logger.Info("35F40A69-BEAB-4A13-B684-F76F3BBE53CD", $"targetRelativeFileName = {targetRelativeFileName}");

            if(targetRelativeFileName.StartsWith("/") || targetRelativeFileName.StartsWith("\\"))
            {
                targetRelativeFileName = targetRelativeFileName.Substring(1);
            }

            _logger.Info("F72FFE4B-F305-49BD-B977-93F6B99AC1D3", $"targetRelativeFileName (after) = {targetRelativeFileName}");

            var targetFileName = Path.Combine(wSpaceDir, targetRelativeFileName);

            _logger.Info("380D2E36-7F8E-4949-96FD-EE4E96A353BA", $"targetFileName = {targetFileName}");

            var text = @"linvar logic for range [0, 1]
{
    constraints:
	    for inheritance;

	terms:
		minimal = L(0, 0.1);
		low = Trapezoid(0, 0.05, 0.3, 0.45);
		middle = Trapezoid(0.3, 0.4, 0.6, 0.7);
		high = Trapezoid(0.55, 0.7, 0.95, 1);
		maximal = S(0.9, 1);
}

linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper is [very middle] exampleClass
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: barrel(#a) :}
	{: see(I, #a) :}
	{: dog(#b) & bird(#f) :}
	{: cat(#с) :}
	{: animal(cat) :}
	{: age(#Tom, 50) :}
	{: distance(I, #Tom, 12) :}

    on Init => {
	     'Begin from test!!!' >> @>log;


		 select {: son($x, $y) :} >> @>log;
		 select {: age(#Tom, $x) & distance(#Tom, $y) & $x is not $y :} >> @>log;




		 'End' >> @>log;

    }


    }
";

            File.WriteAllText(targetFileName, text);

            var supportBasePath = Path.Combine(testDir, "SysDirs");

            _logger.Info("3EF60072-1246-49F5-A4C6-9F30F2C81428", $"supportBasePath = {supportBasePath}");

            var monitorMessagesDir = Path.Combine(supportBasePath, "NpcMonitorMessages");

            _logger.Info("33072297-0819-4B8F-8551-B655F4FA22B0", $"monitorMessagesDir = {monitorMessagesDir}");

            using var cancellationTokenSource = new CancellationTokenSource();

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create(cancellationTokenSource.Token);

            var instance = WorldFactory.WorldInstance;

            var settings = new WorldSettings();

            settings.CancellationToken = cancellationTokenSource.Token;

            settings.EnableAutoloadingConvertors = true;

            settings.LibsDirs = new List<string>() { Path.Combine(wSpaceDir, "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(supportBasePath, "TMP");

            settings.HostFile = Path.Combine(wSpaceDir, "World/World.world");

            settings.InvokerInMainThread = invokingInMainThread;

            var callBackLogger = new CallBackLogger(
                message => { _logger.Info("29A5ECF5-4369-4759-AE03-76EF742813F5", $"message = {message}"); },
                error => { _logger.Info("68480D4E-B029-45A7-892C-D16D06CA9E79", $"error = {error}"); }
                );

            settings.Monitor = new SymOntoClay.Monitor.Monitor(new SymOntoClay.Monitor.MonitorSettings
            {
                MessagesDir = monitorMessagesDir,
                PlatformLoggers = new List<IPlatformLogger>() { callBackLogger },
                Enable = true,
                CancellationToken = cancellationTokenSource.Token,
                ThreadingSettings = ConfigureThreadingSettings().AsyncEvents
            });

            settings.ThreadingSettings = ConfigureThreadingSettings();

            _logger.Info("E17F812C-9900-4B6B-9210-A5CE8742C9DA", $"settings = {settings}");

            instance.SetSettings(settings);

            var platformListener = new object();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = Path.Combine(wSpaceDir, $"Npcs/{projectName}/{projectName}.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();
            npcSettings.ThreadingSettings = ConfigureThreadingSettings();

            _logger.Info("A471A33B-0564-49CC-AD27-2BADFB527C09", $"npcSettings = {npcSettings}");

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            Thread.Sleep(5000);

            cancellationTokenSource.Cancel();

            Directory.Delete(testDir, true);

            _logger.Info("47C7E720-4227-4738-8D52-E3270106EF66", "End");
        }

        private static ThreadingSettings ConfigureThreadingSettings()
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

        private static void TstAdvancedTestRunnerForMultipleInstances()
        {
            _logger.Info("4F0F2163-386D-4981-BC87-4900E243CB69", "Begin");

            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    _logger.Info("A0C91537-5AC2-4DCD-8199-C1479A99F083", $"n = {n}; message = {message}");
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{
    on Init
    {
        'Begin' >> @>log;
        @@host.`rotate`(#@(gun));
        'End' >> @>log;
    }
}");

                var hostListener = new HostMethods_Tests_HostListener();

                instance.CreateNPC(hostListener);

                var thingProjName = "M4A1";

                instance.WriteThingFile(thingProjName, @"app M4A1_app is gun
{
}");

                var gun = instance.CreateThing(thingProjName/*, new Vector3(100, 100, 100)*/);

                instance.StartWorld();



                Thread.Sleep(5000);
            }

            _logger.Info("8B52CEC1-4F4A-4639-ABD9-4D6F6FD50129", "End");
        }

        private static void TstAdvancedTestRunner()
        {
            _logger.Info("D7DC462F-D14E-42DF-828D-7CB3CE33D8B6", "Begin");

            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
 
        try
        {
            Go();
        }
        catch(@e)
        {
            'catch(@e)' >> @>log;
            @e >> @>log;
        }

        'End' >> @>log;
    }
}

action Go 
{
    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        break action {: attack(I, enemy) :};
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                _logger.Info("2D23322D-000C-4FFA-89B1-B43A33383A20", $"n = {n}; message = {message}");
            });

            Thread.Sleep(1000);

            npc.InsertFact(_logger, "{: see(I, #a) :}");

            Thread.Sleep(1000);

            _logger.Info("4715C87B-8AC4-4C59-AF96-37705E93BE41", "End");
        }

        private static void TstAdvancedTestRunnerWithListenedFact()
        {
            _logger.Info("D39975E6-C099-4D3D-AE51-2BA422E95729", "Begin");

            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on add fact ($_ >> @x)
	{
	    'on add fact ($_ >> @x)' >> @>log;
	    @x >> @>log;
	}

	on {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M16!!!!' >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                _logger.Info("516DBCA1-4C49-4152-960A-3FDF98A27BC8", $"n = {n}; message = {message}");
            });

            Thread.Sleep(1000);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            npc.EngineContext.Storage.InsertListenedFact(null, factStr);

            Thread.Sleep(1000);

            _logger.Info("C90E98B6-CFD1-4F25-91D3-C959BD472060", "End");
        }

        private static void TstTestRunnerBehaviorTestEngineInstance()
        {
            _logger.Info("21E465F3-B8DD-4787-AF48-50C3D0C15D07", "Begin");

            var text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

        error {: see(I, #a) :};

        'End of `a`' >> @>log;
    }

    on Enter =>
    {
        try
        {
            'Begin' >> @>log;
            a(param_1: 1);
            'End' >> @>log;        
        }
        catch
        {
            'catch' >> @>log;
        }
        catch(@e)
        {
            'catch(@e)' >> @>log;
            @e >> @>log;
        }
        else
        {
            'else' >> @>log;
        }
        ensure
        {
            'ensure' >> @>log;
        }

        'End of `Enter`' >> @>log;
    }
}";

            BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) => {
                    _logger.Info("2012B98A-BB15-41A1-A267-95D3719CF28E", $"n = {n}; message = {message}");
                    return true;
                });

            _logger.Info("EFF4EB51-E83C-4FA6-96A9-811A72B430E4", "End");
        }

        private static void TstTestRunnerWithHostListener()
        {
            _logger.Info("BEF83438-5B1E-4A1A-A729-8C5703D7D25C", "Begin");

            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.`rotate`~(30)[:on complete { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            //var hostListener = new HostMethods_Tests_HostListener();
            var hostListener = new FullGeneralized_Tests_HostListener();

            OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    _logger.Info("2012B98A-BB15-41A1-A267-95D3719CF28E", $"n = {n}; message = '{message}'");
                }, hostListener);

            _logger.Info("8E6AB0DB-9DAD-47AE-83BB-89421B778775", "End");
        }

        private static void TstTestRunner()
        {
            _logger.Info("02451C3B-2280-4CB7-8D31-6BF42E1176F9", "Begin");

            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @bx >> @>log;
        'End' >> @>log;
    }
}";

            //BehaviorTestEngineInstance.Run(fileContent: text,
            //    logChannel : (n, message) => {
            //        _logger.Info("BEF7431B-B672-473E-9ABB-76D9A2D708A1", $"n = {n}; message = {message}");

            //        if(n == 3)
            //        {
            //            return false;
            //        }

            //        return true;
            //    },
            //    htnPlanExecutionIterationsMaxCount: 2);

            var maxN = 0;

            var builder = new BehaviorTestEngineInstanceBuilder();
            builder.UseDefaultRootDirectory();
            builder.DontUsePlatformListener();
            builder.EnableHtnPlanExecution();
            builder.DontUseTimeoutToEnd();
            builder.TestedCode(text);
            builder.LogHandler((n, message) =>
            {
                _logger.Info("CEB3C0B7-4DC5-4208-88F4-9707C3F057BE", $"n = {n}; message = {message}");

                maxN = n;

                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        return true;

                    case 2:
                        Assert.AreEqual(message, "NULL");
                        return true;

                    case 3:
                        Assert.AreEqual(message, "End");
                        return false;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });
            //builder.ErrorHandler((errorMsg) => {
            //    _logger.Info("DB0176DD-B108-40DD-A527-5FD3B7C6318A", $"errorMsg = {errorMsg}");
            //});

            var testInstance = builder.Build();

            //var testInstance = builder.CreateMinimalInstance(text, (n, message) => {
            //    _logger.Info("7C09894C-BB7E-4B51-A3CF-72D7CD45E350", $"n = {n}; message = {message}");

            //    if (n == 3)
            //    {
            //        return false;
            //    }

            //    return true;
            //});

            var result = testInstance.Run();

            //var result = BehaviorTestEngineRunner.RunMinimalInstance(text, (n, message) => {
            //    _logger.Info("DC97FBD3-DB40-4A88-9C12-14030A67DE00", $"n = {n}; message = {message}");

            //    switch (n)
            //    {
            //        case 1:
            //            Assert.AreEqual(message, "Begin");
            //            return true;

            //        case 2:
            //            Assert.AreEqual(message, "1");
            //            return true;

            //        case 3:
            //            Assert.AreEqual(message, "End");
            //            return false;

            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(n), n, null);
            //    }
            //});

            _logger.Info("15E33BE2-7F30-4DD4-8822-98FBD47AEAE7", $"result = {result}");

            _logger.Info("F1323C4D-2BF6-4601-B330-58D1B9C28852", $"maxN = {maxN}");

            _logger.Info("E18A11E9-F938-49F4-8C02-A124EA88D690", "End");
        }

        private static void TstNameHelper()
        {
            _logger.Info("BAB73BFD-AD12-44A2-80FA-7523A6A1A11A", "Begin");

            var text = "$x";

            var name = NameHelper.CreateName(text);

            _logger.Info("0A3F463E-7F2E-4EFF-93C7-6C066404C9BE", $"name = {name}");

            _logger.Info("ADED3A3E-7627-42BE-92BF-912B8F2E8FC0", "End");
        }

        private static void TstDeffuzzification()
        {
            _logger.Info("F88C9B47-6BF7-497D-8E1B-ED5821A0EA74", "Begin");

            var veryHandler = new VeryFuzzyLogicOperatorHandler();

            //var lHandler = new LFunctionFuzzyLogicMemberFunctionHandler(0, 0.1);
            //lHandler.CheckDirty();

            //var defuzzificatedValue = lHandler.Defuzzificate();

            //_logger.Log($"defuzzificatedValue = {defuzzificatedValue}");

            var trapezoid = new TrapezoidFuzzyLogicMemberFunctionHandler(0.3, 0.4, 0.6, 0.7);
            //trapezoid = new TrapezoidFuzzyLogicMemberFunctionHandler(10, 12, 17, 20);
            trapezoid.CheckDirty(_logger);

            var defuzzificatedValue = trapezoid.Defuzzificate(_logger);

            _logger.Info("7DC833D8-2E97-488E-8BE7-D6659810BC35", $"defuzzificatedValue = {defuzzificatedValue}");

            var naiveVeryDefuzzificatedValue = veryHandler.SystemCall(_logger, defuzzificatedValue);

            _logger.Info("E2598E54-7922-4975-A847-76D36B317CFD", $"naiveVeryDefuzzificatedValue = {naiveVeryDefuzzificatedValue}");

            defuzzificatedValue = trapezoid.Defuzzificate(_logger, new List<IFuzzyLogicOperatorHandler>() { veryHandler });

            _logger.Info("1B0BD8F5-A296-43BD-B79B-16779B4977C3", $"defuzzificatedValue = {defuzzificatedValue}");

            _logger.Info("1F0679D3-6F2C-4483-A0F5-E716E1B4B4AE", "End");
        }

        private static void TstRangeValue()
        {
            _logger.Info("77695CB9-25A6-4FCA-B2D7-CE66EC71DBA6", "Begin");

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

            _logger.Info("AF4B7B77-7930-49F8-A27B-4E226A3404B9", $"range = {range}");

            _logger.Info("B6C432AD-C307-4DDB-BC79-376C6680BB8D", $"range.ToDbgString() = {range.ToDbgString()}");

            _logger.Info("3CC27F28-59D3-47D6-8447-F97B6DE190C7", $"range.Length = {range.Length}");

            _logger.Info("980553A3-CC98-4D1C-9232-C88FE37768C2", $"range.IsFit(-1) = {range.IsFit(-1)}");
            _logger.Info("4C12AEA1-B05B-4367-8CA1-1FF77DA6F332", $"range.IsFit(0) = {range.IsFit(0)}");
            _logger.Info("F90D93CB-28B8-4BE8-AA05-92C8AED286DA", $"range.IsFit(1) = {range.IsFit(1)}");
            _logger.Info("FD5A31EE-DB2F-47E1-A751-9B59F858D34D", $"range.IsFit(12) = {range.IsFit(12)}");
            _logger.Info("0843F1C0-297D-49E3-93C9-6185B465CC77", $"range.IsFit(14) = {range.IsFit(14)}");

            _logger.Info("0D4FE745-F35B-4E9A-955C-C804092346FC", "End");
        }

        private static void TstFuzzyLogicNonNumericSequenceValue()
        {
            _logger.Info("38899FB2-ED95-4C0E-97F2-3E977454DDB0", "Begin");

            var sequence = new FuzzyLogicNonNumericSequenceValue();

            _logger.Info("3E05BE20-F906-4051-A83B-9E6BDE933AC2", $"sequence = {sequence}");
            _logger.Info("5BBE5FEA-3931-415E-92CF-8AC902EDCCA7", $"sequence = {sequence.ToDbgString()}");

            var very = NameHelper.CreateName("very");

            sequence.AddIdentifier(very);

            _logger.Info("3A683FB9-F354-4D97-BF92-67B47FC3628D", $"sequence = {sequence}");
            _logger.Info("271A93F1-8546-42F3-A301-A17A53AB674B", $"sequence = {sequence.ToDbgString()}");

            var teenager = NameHelper.CreateName("teenager");

            sequence.AddIdentifier(teenager);

            sequence.CheckDirty();

            _logger.Info("0DBDF7F3-3C5A-4C84-AB05-159F2226459D", $"sequence = {sequence}");
            _logger.Info("E44DAC6F-A6D4-44D6-8C47-465B15EC8202", $"sequence = {sequence.ToDbgString()}");

            _logger.Info("D6727E0C-F46F-4F0C-8190-4077F5E05560", "End");
        }

        private static float Deg2Rad = 0.0174532924F;

        private static void TstCalculateTargetAnglesForRayScanner()
        {
            _logger.Info("DF0E9C07-194B-4430-B26F-0939274A8609", "Begin");

            var TotalRaysAngle = 125;
            var TotalRaysInterval = 10;
            var FocusRaysAngle = 35;
            var FocusRaysInterval = 2;

            var anglesList = new List<(float, bool)>();

            var halfOfTotalRaysAngle = TotalRaysAngle / 2;
            var halfOfFocusRaysAngle = FocusRaysAngle / 2;

            _logger.Info("14D27C16-C3AA-4CC2-8B8A-42EA275FF9BD", $"halfOfTotalRaysAngle = {halfOfTotalRaysAngle}");
            _logger.Info("F803650D-035B-4940-84A8-2207EC235E46", $"halfOfFocusRaysAngle = {halfOfFocusRaysAngle}");

            var currAngle = 0f;
            var inFocus = true;

            do
            {
                _logger.Info("CF29BBFB-7098-407A-881F-7D2DCA9EA046", $"currAngle = {currAngle}; inFocus = {inFocus}");

                var radAngle = currAngle * Deg2Rad;

                _logger.Info("B003358D-BECA-4AE8-AD42-7B297300B739", $"radAngle = {radAngle}");

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

            _logger.Info("DBD2620A-F43E-4732-A0A9-D317769B1EFB", $"anglesList = {JsonConvert.SerializeObject(anglesList, Formatting.Indented)}");

            _logger.Info("0AB0763E-0FBE-488B-8F3E-A6D3892E9952", "End");
        }

        private static void TstCopyFilesOnBuilding()
        {
            _logger.Info("24655813-6DE5-4560-84BE-FE0F6DC12A39", "Begin");

            var sourceDir = @"C:/Users/Sergey/Documents/GitHub/Game1/Assets";
            var outputPath = @"C:/Users/Sergey/Documents/ExampleBuild/CustomAssetExample.exe";

            BuildPipeLine.CopyFiles(sourceDir, outputPath);

            _logger.Info("62C48237-5EA9-4A57-9300-F84F59D1F6C3", "End");
        }

        private static void TstGetRootWorldSpaceDir()
        {
            _logger.Info("3E701AB8-E272-4B4F-8FD4-F88DD13FBAB0", "Begin");

            var inputFile = @"C:/Users/Sergey/Documents/GitHub/Game1/Assets\HelloWorld_Example1/World/PeaceKeeper.world";

            _logger.Info("FE12AD59-BDC3-422A-AB57-B71D0862633F", $"inputFile = '{inputFile}'");

            var wspaceDir = WorldSpaceHelper.GetRootWorldSpaceDir(inputFile);

            _logger.Info("AD8E2FD7-406D-42B1-AD58-3A486438CA34", $"wspaceDir = '{wspaceDir}'");

            _logger.Info("26EBCD77-698B-470E-AFDF-C9EDF78D8524", "End");
        }

        private static void TstEnvironmentVariables()
        {
            _logger.Info("B8EF5349-D77B-499C-8BBD-B3F50C0689DE", "Begin");


            var path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            _logger.Info("338359A1-B814-45FE-8CB1-CE2AC0812BAB", $"path = '{path}'");

            var pathsList = path.Split(';').Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p));

            _logger.Info("69D761ED-2382-42B8-ADDB-6CA3720AA37B", $"pathsList = {JsonConvert.SerializeObject(pathsList, Formatting.Indented)}");

            var currDir = Directory.GetCurrentDirectory();

            _logger.Info("D2F33841-465F-4C2D-BFA1-9E52B3E0C03F", $"currDir = {currDir}");

            if(!pathsList.Contains(currDir))
            {
                path = $"{path};{currDir}";

                _logger.Info("710E33F3-F748-44F5-B6E4-ADEB6450E5AF", $"path (after) = '{path}'");

                Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.User);
            }

            _logger.Info("0B669B64-4190-4380-A01E-46AC843E1432", "End");
        }

        private static void TstCLINewHandler()
        {
            _logger.Info("0D38F763-C02E-4DB5-855E-90029878AB58", "Begin");

            var args = new List<string>() {
                 "new",
                 "Enemy"
            }.ToArray();

            var command = CLICommandParser.Parse(args);

            _logger.Info("E9616FE0-2F8A-4124-9138-F8BFB4BDDE54", $"command = {command}");

            _logger.Info("4972171D-AD36-4BBF-89FF-87B331FF076F", "End");
        }

        private static void TstCLIRunHandler()
        {
            _logger.Info("5F7CF8CA-6353-47AF-847C-80CCEF9C3790", "Begin");

            var args = new List<string>() {
                 "run"//,
            }.ToArray();

            var targetDirectory = EVPath.Normalize("%USERPROFILE%/source/repos/SymOntoClay/TestSandbox/Source");

            _logger.Info("F35FAE48-F180-464A-9A6C-BDF52D270408", $"targetDirectory = {targetDirectory}");

            Directory.SetCurrentDirectory(targetDirectory);

            var command = CLICommandParser.Parse(args);

            _logger.Info("5827F255-E21E-42AF-A6EF-8DD36C93364C", $"command = {command}");

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = command.InputDir,
                InputFile = command.InputFile
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

            _logger.Info("BEDEEFAF-3DFE-4982-8441-206E5F863DA3", $"targetFiles = {targetFiles}");

            using var cancellationTokenSource = new CancellationTokenSource();

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create(cancellationTokenSource.Token);

            var instance = WorldFactory.WorldInstance;

            var settings = new WorldSettings();
            settings.CancellationToken = cancellationTokenSource.Token;
            settings.ThreadingSettings = ConfigureThreadingSettings();

            settings.LibsDirs = new List<string>() { targetFiles.SharedLibsDir };

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            settings.TmpDir = targetFiles.TmpDir;

            settings.HostFile = targetFiles.WorldFile;

            settings.InvokerInMainThread = invokingInMainThread;

            settings.Monitor = new SymOntoClay.Monitor.Monitor(new SymOntoClay.Monitor.MonitorSettings
            {
                PlatformLoggers = new List<IPlatformLogger>() { new CLIPlatformLogger() },
                Enable = true,
                CancellationToken = cancellationTokenSource.Token,
                ThreadingSettings = ConfigureThreadingSettings().AsyncEvents
            });

            _logger.Info("ECB2A35D-05A6-48C4-9C9E-27D54D70C501", $"settings = {settings}");

            instance.SetSettings(settings);

            var platformListener = new TstPlatformHostListener();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = targetFiles.LogicFile;
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();
            npcSettings.ThreadingSettings = ConfigureThreadingSettings();

            _logger.Info("7B5072FD-FC34-4F73-98CD-08D9E8815AF0", $"npcSettings = {npcSettings}");

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            Thread.Sleep(50000);

            cancellationTokenSource.Cancel();

            _logger.Info("D1D4E5FE-B5AC-41E4-AA19-A1E8A6F989BF", "End");
        }

        private static void TstCLICommandParser()
        {
            _logger.Info("2F72C161-FA6B-4166-A55F-57292C5F31FA", "Begin");

            var handler = new CLICommandParserHandler();
            handler.Run();

            _logger.Info("E3D004B7-FAE2-47C5-BDF8-9F1A8A96EACF", "End");
        }

        private static void TstLogicalDatabase()
        {
            var handler = new LogicalDatabaseHandler();
            handler.Run();
        }

        private static void TstProcessInfoChildren()
        {
            _logger.Info("290EA263-EB27-4AE1-B8C1-6AB49B2A3153", "Begin");

            var parentProcessInfo = new ProcessInfo(CancellationToken.None, null, null);

            var child_1 = new ProcessInfo(CancellationToken.None, null, null);
            child_1.ParentProcessInfo = parentProcessInfo;

            var child_2 = new ProcessInfo(CancellationToken.None, null, null);
            parentProcessInfo.AddChild(_logger, child_2);

            _logger.Info("789033F5-F42A-46E0-9E02-26E9AFBECE22", $"parentProcessInfo = {parentProcessInfo}");

            _logger.Info("E5482802-2CC1-48C8-B569-51D22A034199", "End");
        }

        private static void TstWaitIProcessInfo()
        {
            _logger.Info("F3B2D5BF-65E8-4B27-A445-805093C376A0", "Begin");

            var processInfo = new ProcessInfo(CancellationToken.None, null, null);

            var task = new Task(() => {
                processInfo.SetStatus(_logger, "7237984F-0628-432D-AE80-F01AE12D48B0", ProcessStatus.Running);

                Thread.Sleep(10000);

                processInfo.SetStatus(_logger, "B6BEFCA3-C7CB-4348-BCE4-6FED8675E5A0", ProcessStatus.Completed);
            });

            task.Start();

            _logger.Info("CF7047B1-3563-4026-ABB0-539205FD069B", "task.Start()");

            ProcessInfoHelper.Wait(_logger, processInfo);

            _logger.Info("FF2656B1-8AA9-4CF6-B0E5-8877FC49EAE6", "End");
        }

        private enum KindOfParameters
        {
            NoParameters,
            NamedParameters,
            PositionedParameters
        }

        private static void TstKindOfParametersScaffolder()
        {
            _logger.Info("5A037CCC-0FD3-49E8-B274-6AB36FA47AAC", "Begin");

            var mainParametersList = new List<KindOfParameters>() { KindOfParameters.NoParameters, KindOfParameters.NamedParameters, KindOfParameters.PositionedParameters};
            var additionalParametersList = new List<KindOfParameters>() { KindOfParameters.NoParameters, KindOfParameters.NamedParameters, KindOfParameters.PositionedParameters };

            var strList = new StringBuilder();

            foreach(var mainParameter in mainParametersList)
            {
                _logger.Info("E6CBC8CD-B5D6-4746-958A-ABF1A54E4316", $"mainParameter = {mainParameter}");

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

                _logger.Info("EA0521AB-6F7E-41CB-BC17-AF7189C7A64B", $"mainParameterStr = {mainParameterStr}");

                foreach (var additionalParameter in additionalParametersList)
                {
                    _logger.Info("C2583936-C361-487F-A980-24FD57844241", $"additionalParameter = {additionalParameter}");

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

                    _logger.Info("AA07C73A-ED5D-4545-A356-674F2E0A4E80", $"additionalParameterStr = {additionalParameterStr}");

                    var str = $"Call{mainParameterStr}{additionalParameterStr}";

                    _logger.Info("576E1C66-2563-4F67-8A0C-647CEA727BD6", $"str = {str}");

                    strList.AppendLine(str);
                }             
            }

            _logger.Info("517C404D-6E26-4465-BDDF-68AB61B91601", $"strList = {strList}");

            _logger.Info("F9594D8E-D54A-4512-8C18-B3C581276E7A", "End");
        }

        private static void TstDateTimeHandler()
        {
            _logger.Info("67B1E495-122E-4FED-A552-BFECDCE13FC8", "Begin");

            var handler = new DateTimeHandler();
            handler.Run();

            _logger.Info("E87D8845-B77F-4DFB-81C4-827CBC9C5138", "End");
        }

        private static void TstBaseManualControllingGameComponent()
        {
            _logger.Info("B44EF86A-E413-442A-A60D-E8CFE5A48807", "Begin");

            var handler = new TstBaseManualControllingGameComponentHandler();
            handler.Run();

            _logger.Info("67F323C7-2C19-43F4-AE92-268099B28F70", "End");
        }

        private static void TstLoadTypesPlatformTypesConvertors()
        {
            _logger.Info("19C092FE-75AD-4FB6-8399-91DF05BDB321", "Begin");

            var targetAttributeType = typeof(PlatformTypesConverterAttribute);

            var typesList = AppDomainTypesEnumerator.GetTypes().Where(p => p.GetCustomAttributesData().Any(x => x.AttributeType == targetAttributeType));

            _logger.Info("26DD17FF-54E5-4F42-94A3-5D084D270752", $"typesList.Length = {typesList.Count()}");

            foreach (var type in typesList)
            {
                _logger.Info("461FCB43-A9F7-495A-999F-03EBBDE6D2B6", $"type.FullName = {type.FullName}");

                var convertor = (IPlatformTypesConverter)Activator.CreateInstance(type);

                _logger.Info("59C95140-1BD1-4077-B3FB-ECB94C1ABD36", $"convertor = {convertor}");
            }

            _logger.Info("DD103299-F1EA-4A2E-AC57-D0483BEA6282", "End");
        }

        private static void TstGetTypes()
        {
            _logger.Info("DD64DB85-ACBB-4418-9E27-C7C238E7CD9E", "Begin");

            var typesList = AppDomainTypesEnumerator.GetTypes();

            _logger.Info("7C9EFB77-E8E0-4D13-99A0-6EC0DA326BD3", $"typesList.Length = {typesList.Count}");

            foreach (var type in typesList)
            {
                _logger.Info("769AB2F5-0DCE-4533-BDDE-EE931CDCD880", $"type.FullName = {type.FullName}");
            }

            _logger.Info("1927808F-717F-4163-9AE1-761122324735", "End");
        }

        private static void TstMainThreadSyncThread()
        {
            _logger.Info("23BE49E1-B545-4746-8FF1-854D6D7F3E11", "Begin");

            var invokingInMainThread = new InvokerInMainThread();

            var task = Task.Run(() => { 
                while(true)
                {
                    invokingInMainThread.Update();

                    Thread.Sleep(1000);
                }
            });

            Thread.Sleep(5000);

            var invocableInMainThreadObj = new InvocableInMainThread(() => { _logger.Info("4A81F94C-2890-40C5-887D-D86C5BFB8C4E", "^)"); }, invokingInMainThread);
            invocableInMainThreadObj.Run();

            Thread.Sleep(5000);

            _logger.Info("5903CF66-17A5-4A10-8C59-DB424E73D724", "End");
        }

        private static void TstCoreHostListenerHandler()
        {
            _logger.Info("5FE225F3-8F89-4AD5-A1C8-15E89404F484", "Begin");

            var handler = new CoreHostListenerHandler();
            handler.Run();

            _logger.Info("106115D2-C4FD-424D-982F-C859FF88DF9F", "End");
        }

        private static void TstNullableArithmetic()
        {
            _logger.Info("73A8482C-BC10-4E66-8B21-2F2789C01BED", "Begin");

            float? a = null;

            var b = 1 - a;

            _logger.Info("5259BAA8-4E89-4527-9E50-008BE5B3DA46", $"b = {b}");
            _logger.Info("1D0A41B5-258B-4F3D-8C0E-F14135B1E606", $"b == null = {b == null}");

            _logger.Info("489DC67D-FBFD-4E5D-9980-453453C458C5", "End");
        }

        private static void TstInheritanceItemsHandler()
        {
            _logger.Info("CCC34EFC-1D81-4B34-81AB-BA2C1CF87FE6", "Begin");

            var handler = new InheritanceItemsHandler();
            handler.Run();

            _logger.Info("1386C056-2C25-4D68-8C52-654254D81435", "End");
        }
            
        private static void TstDefaultSettingsOfCodeEntityHandler()
        {
            _logger.Info("D1F85C75-6980-4A1E-8D6F-6B1138434279", "Begin");

            var handler = new DefaultSettingsOfCodeEntityHandler();
            handler.Run();

            _logger.Info("DFF3BDC9-0E12-4325-AA3F-98D82833DB4B", "End");
        }

        private static void TstActivateMainEntity()
        {
            _logger.Info("DF20DB1C-9849-4808-AB10-B822A3B4B451", "Begin");

            var handler = new ActivateMainEntityHandler();
            handler.Run();

            _logger.Info("4A90A98E-6B97-4EBE-B1DB-DA88FD6C3C90", "End");
        }

        private static void TstCompileInlineTrigger()
        {
            _logger.Info("8253CD94-4983-4E53-A4A8-EB9F0F8A4926", "Begin");

            var handler = new CompileInlineTriggerHandler();
            handler.Run();

            _logger.Info("8E232D5D-F8DD-435F-A374-3B68BD9AD59A", "End");
        }

        private static void TstRegOperatorsHandler()
        {
            _logger.Info("4E96B7DC-E494-42F5-9DC0-67243292392A", "Begin");

            var handler = new RegOperatorsHandler();
            handler.Run();

            _logger.Info("54CDB7B3-C96A-42C6-836B-1ACA1B2CF8FB", "End");
        }

        private static void TstCreateEngineContext()
        {
            _logger.Info("6767B4BA-B1C3-4C47-8831-997E0465D26C", "Begin");

            var context = TstEngineContextHelper.CreateAndInitContext();

            _logger.Info("446B3B68-75E1-4DDD-84EF-B51ABFEFBAAC", "End");
        }

        private static void TstAsyncActivePeriodicObjectHandler()
        {
            _logger.Info("61BF59CB-D8C5-432E-8343-DD8A07B24BE6", "Begin");

            var handler = new AsyncActivePeriodicObjectHandler();
            handler.Run();

            _logger.Info("E96DEF49-0830-4E78-835F-3E2EBE98FDA5", "End");
        }

        private static void TstSyncActivePeriodicObjectHandler()
        {
            _logger.Info("5AFA77C9-837A-406C-896C-AB60F46E0A5D", "Begin");

            var handler = new SyncActivePeriodicObjectHandler();
            handler.Run();

            _logger.Info("A3701051-7969-4E1E-AB50-AC8770A980D7", "End");
        }

        private static void TstCodeExecution()
        {
            _logger.Info("4FB6D8B8-F157-4FC1-8926-3ACA8F6DC949", "Begin");

            var handler = new CodeExecutionHandler();
            handler.Run();

            _logger.Info("BDD0D69E-5805-4E2F-89DA-7B85012D8F1C", "End");
        }

        private static void TstCreateName()
        {
            _logger.Info("FC76D93C-5314-4852-BFAD-58ACE5999231", "Begin");

            var parserContext = new TstMainStorageContext();

            var nameVal1 = "dog";

            _logger.Info("3E73FFE5-96A2-4288-B965-D265FC6623F1", $"{nameof(nameVal1)} = {nameVal1}");





























            var name = NameHelper.CreateName(nameVal1);

            _logger.Info("2C88170A-6C28-48F1-98A1-980A34D8FCFC", $"name = {name}");

            _logger.Info("8B5DDBE3-EB99-4DAA-BEA0-09745DC23DD7", "End");
        }

        private class NormalizedNameValues
        {
            public string Name { get; set; }
            public List<string> Namespaces { get; set; }
        }

        private static NormalizedNameValues ParseName(string text)
        {
#if DEBUG
            _logger.Info("D0B9DF0B-16FE-4898-8EAD-94E5B519380E", $"test = {text}");
#endif

            var result = new NormalizedNameValues();

            if (!text.Contains("::") && !text.Contains("("))
            {
                result.Name = text.Trim();
                return result;
            }

            throw new NotImplementedException("0CB91054-9A8E-4CAC-BD64-D40882DE63C0");
        }

        private static void TstExprNodeHandler()
        {
            _logger.Info("9909DAAE-C58F-456E-9E04-A9CB6A3D198F", "Begin");

            var handler = new ExprNodeHandler();
            handler.Run();

            _logger.Info("D37A788F-086F-41DE-83BB-B3DB690D1CFA", "End");
        }

        private static void TstParsing()
        {
            _logger.Info("FEE748E2-9BA0-4561-ACA9-54A05686FA3C", "Begin");

            var handler = new ParsingHandler();
            handler.Run();

            _logger.Info("03913BD8-9341-4F74-85B0-9FD85DC1C153", "End");
        }

        private static void TstBattleRoyaleHandler()
        {
            _logger.Info("4CE05F07-108C-4535-B892-94C20E5E6E4B", "Begin");

            var handler = new BattleRoyaleHandler();
            handler.Run();

            _logger.Info("99429CC7-5AFD-42F4-97D1-830039DF67DF", "End");
        }

        private static void TstPlacesHandler()
        {
            _logger.Info("73D89609-7D39-4E57-8A92-F9E194D85945", "Begin");

            using var handler = new PlacesHandler();
            handler.Run();

            _logger.Info("2BD6E59D-1CD6-432F-9427-CDB04F69190A", "End");
        }

        private static void TstMonoBehaviorTestingHandler()
        {
            _logger.Info("E3BE2386-3885-4B2C-8A0F-A6E71AE3B7A6", "Begin");

            var handler = new MonoBehaviorTestingHandler();
            handler.Run();

            _logger.Info("5229A73C-405A-413D-AB44-DDDEBE840D31", "End");
        }

        private static void TstSoundStartHandler()
        {
            _logger.Info("CE282423-8258-4B09-8783-FF7BEFCB35F2", "Begin");

            using var handler = new SoundStartHandler();
            handler.Run();

            _logger.Info("FED25492-F87F-41A9-AA76-89EDE680CCCA", "End");
        }

        private static void TstAddingFactTriggerHandler()
        {
            _logger.Info("7B75EA54-EDEB-4761-9092-1C9D58FB6E55", "Begin");

            using var handler = new AddingFactTriggerHandler();
            handler.Run();

            _logger.Info("EABCDF37-04C2-467E-9865-0527479C5CCB", "End");
        }

        private static void TstHtnHandler()
        {
            _logger.Info("FF16064F-085D-423A-A269-3C671843A371", "Begin");

            using var handler = new HtnHandler();
            handler.Run();

            _logger.Info("40211AAF-B1A6-4A2B-B8D2-E63CD9FC0928", "End");
        }

        private static void TstGeneralStartHandler()
        {
            _logger.Info("BABAC27C-220E-4152-B16F-4D93C62631C3", "Begin");


            using var handler = new GeneralStartHandler();
            handler.Run();

            _logger.Info("82BF5322-13F3-4EB8-ABF7-6A152052EF41", "End");
        }

        private static void TstGetParsedFilesInfo()
        {
            _logger.Info("BD8086E6-A01F-4D41-88F0-749C93F88D34", "Begin");

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\PixKeeper\PixKeeper.txt");

            _logger.Info("B905A4EE-1B85-4967-9425-0976A8494256", $"fileName = {fileName}");

            var id = "#020ED339-6313-459A-900D-92F809CEBDC5";

            var resultList = FileHelper.GetParsedFilesInfo(_logger, fileName, id);

            _logger.Info("F5A00933-4051-4D62-89EF-9C8A63E49072", $"resultList = {JsonConvert.SerializeObject(resultList, Formatting.Indented)}");

            _logger.Info("18724EB5-FD43-4DC6-8D94-9B8516749CAB", "End");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Info("D90EB257-F5E2-4D7C-96AC-3D3476070498", $"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}

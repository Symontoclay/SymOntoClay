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
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.ProjectFiles;
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
using SymOntoClay.Core.Internal.Parsing.Internal;
using TestSandbox.CreatingExamples;
using TestSandbox.Navigations;
using TestSandbox.SoundBusHandler;
using SymOntoClay.UnityAsset.Core.Tests;
using System.Numerics;
using DictionaryGenerator;
using SymOntoClay.NLP.CommonDict.Implementations;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.NLP;
using System.Speech.Synthesis;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.BaseTestLib;
using NUnit.Framework;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;

namespace TestSandbox
{
    class Program
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            EVPath.RegVar("APPDIR", Directory.GetCurrentDirectory());

            TstMonitor();
            //TstCreateListByVarsDict();
            //TstDetectDoninantItems();
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
            //TstOnAddingFactEventHanler();
            //TstEventHanler();
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
            //TstBattleRoyaleHandler();//<==
            //TstPlacesHandler();//<==
            //TstMonoBehaviourTestingHandler();//VT<=
            //TstSoundStartHandler();//<==
            //TstAddingFactTriggerHandler();
            //TstGeneralStartHandler();//<=
            //TstGetParsedFilesInfo();

            //Thread.Sleep(10000);
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

        private static void TstDetectDoninantItems()
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

            var task = Task.Run(() => {
                _logger.Info("EB024ABD-0889-4DEF-A6E9-1D36CA08F839", "Hi!");

                Thread.Sleep(10000);

                _logger.Info("F812B258-8F6B-4066-8698-55DA609BD01D", "End Hi!");
            }, token1);

            var taskValue = new TaskValue(task, source1);

            taskValue.OnComplete += () => 
            {
                _logger.Info("2CD55AB4-7F0D-49BE-B17F-455D69263E48", "taskValue.OnComplete!!!!!");
            };

            taskValue.OnComplete += () =>
            {
                _logger.Info("674D6CA3-085C-4174-9B21-6AA992F99157", "(2) taskValue.OnComplete!!!!!");
            };

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

            var compiledCode = engineContext.ConvertersFactory.GetConverterFactToImperativeCode().Convert(fact, localCodeExecutionContext);

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
            _logger.Info("Begin");

            var synth = new SpeechSynthesizer();

            synth.SetOutputToDefaultAudioDevice();

            synth.Speak("This example demonstrates a basic use of Speech Synthesizer");
            synth.Speak("Go to green place!");

            _logger.Info("End");
        }

        private static void TstOnAddingFactEventHanler()
        {
            _logger.Info("Begin");

            var handler = new OnAddingFactEventHanler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstEventHanler()
        {
            _logger.Info("Begin");

            var handler = new EventHanler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstStrCollectionCombination()
        {
            _logger.Info("Begin");

            var source = new List<List<string>>();

            source.Add(new List<string>() { "a", "e" });
            source.Add(new List<string>() { "h", "j", "L" });
            source.Add(new List<string>() { "b", "c", "f" });

            _logger.Info($"source = {JsonConvert.SerializeObject(source, Formatting.Indented)}");

            var result = CollectionCombinationHelper.Combine(source);

            _logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            _logger.Info("End");
        }

        private static void TstIntCollectionCombination()
        {
            _logger.Info("Begin");

            var source = new List<List<int>>();

            source.Add(new List<int>() { 1, 5 });
            source.Add(new List<int>() { 7, 9, 11 });
            source.Add(new List<int>() { 2, 3, 6 });

            _logger.Info($"source = {JsonConvert.SerializeObject(source, Formatting.Indented)}");

            var result = CollectionCombinationHelper.Combine(source);

            _logger.Info($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            _logger.Info("End");
        }

        private static void TstModalitiesHandler()
        {
            _logger.Info("Begin");

            var handler = new ModalitiesHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstRelationsStorageHandler()
        {
            _logger.Info("Begin");

            var handler = new RelationsStorageHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstGenerateDllDict()
        {
            _logger.Info("Begin");

            var handler = new GenerateDllDictHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TSTWordsFactory()
        {
            _logger.Info("Begin");

            var wordsFactory = new WordsFactory();
            wordsFactory.Run();

            _logger.Info("End");
        }

        private static void TstNLPConverterProvider()
        {
            _logger.Info("Begin");

            var mainDictPath = Path.Combine(Directory.GetCurrentDirectory(), "Dicts", "BigMainDictionary.dict");

#if DEBUG
            _logger.Info($"mainDictPath = {mainDictPath}");
#endif

            var settings = new NLPConverterProviderSettings();
            settings.DictsPaths = new List<string>() { mainDictPath };
            settings.CreationStrategy = CreationStrategy.Singleton;

#if DEBUG
            _logger.Info($"settings = {settings}");
#endif

            var nlpConverterProvider = new NLPConverterProvider(settings);

            var factory = nlpConverterProvider.GetFactory(_logger);

            var converter = factory.GetConverter();

            _logger.Info("End");
        }

        private static void TstNLPHandler()
        {
            _logger.Info("Begin");

            var handler = new NLPHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstTriggerConditionNodeHandler()
        {
            _logger.Info("Begin");

            var handler = new TriggerConditionNodeHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstSoundBus()
        {
            _logger.Info("Begin");

            var handler = new TstSoundBusHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstNavigationHandler()
        {
            _logger.Info("Begin");

            var handler = new NavigationHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstCreatorExamples()
        {
            _logger.Info("Begin");

            using var handler = new CreatorExamples_States_27_03_2022();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstLinguisticVariable_Tests()
        {
            _logger.Info("Begin");

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

            _logger.Info($"firstItem = {firstItem}");

            var term = firstItem.AsLinguisticVariable.Values[4];

            _logger.Info($"term = {term}");

            var handler = term.Handler;

            _logger.Info($"handler = {handler}");

            _logger.Info("End");
        }

        private static void TstManageTempProject()
        {
            _logger.Info("Begin");

            var initialDir = Directory.GetCurrentDirectory();

            _logger.Info($"initialDir = {initialDir}");

            initialDir = Path.Combine(initialDir, "TempProjects");

            _logger.Info($"initialDir (2) = {initialDir}");

            if(!Directory.Exists(initialDir))
            {
                Directory.CreateDirectory(initialDir);
            }

            var testDir = Path.Combine(initialDir, $"TstDir{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            _logger.Info($"testDir = {testDir}");

            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            var projectName = "Example";

            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { ProjectName = projectName };

            _logger.Info($"worldSpaceCreationSettings = {worldSpaceCreationSettings}");

            var wSpaceFile = WorldSpaceCreator.CreateWithOutWSpaceFile(worldSpaceCreationSettings, testDir
                    , errorMsg => _logger.Error(errorMsg)
                    );

            _logger.Info($"wSpaceFile = {wSpaceFile}");

            var wSpaceDir = wSpaceFile.DirectoryName;

            _logger.Info($"wSpaceDir = {wSpaceDir}");

            var targetRelativeFileName = @"/Npcs/Example/Example.soc";

            _logger.Info($"targetRelativeFileName = {targetRelativeFileName}");

            if(targetRelativeFileName.StartsWith("/") || targetRelativeFileName.StartsWith("\\"))
            {
                targetRelativeFileName = targetRelativeFileName.Substring(1);
            }

            _logger.Info($"targetRelativeFileName (after) = {targetRelativeFileName}");

            var targetFileName = Path.Combine(wSpaceDir, targetRelativeFileName);

            _logger.Info($"targetFileName = {targetFileName}");

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

            _logger.Info($"supportBasePath = {supportBasePath}");

            var monitorMessagesDir = Path.Combine(supportBasePath, "NpcMonitorMessages");

            _logger.Info($"monitorMessagesDir = {monitorMessagesDir}");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var instance = WorldFactory.WorldInstance;

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            settings.LibsDirs = new List<string>() { Path.Combine(wSpaceDir, "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(supportBasePath, "TMP");

            settings.HostFile = Path.Combine(wSpaceDir, "World/World.world");

            settings.InvokerInMainThread = invokingInMainThread;

            var callBackLogger = new CallBackLogger(
                message => { _logger.Info($"message = {message}"); },
                error => { _logger.Info($"error = {error}"); }
                );

            settings.Monitor = new SymOntoClay.Monitor.Monitor(new SymOntoClay.Monitor.MonitorSettings
            {
                MessagesDir = monitorMessagesDir,
                PlatformLoggers = new List<IPlatformLogger>() { callBackLogger },
                Enable = true
            });

            _logger.Info($"settings = {settings}");

            instance.SetSettings(settings);

            var platformListener = new object();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = Path.Combine(wSpaceDir, $"Npcs/{projectName}/{projectName}.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            _logger.Info($"npcSettings = {npcSettings}");

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            Thread.Sleep(5000);

            Directory.Delete(testDir, true);

            _logger.Info("End");
        }

        private static void TstAdvancedTestRunnerForMultipleInstances()
        {
            _logger.Info("Begin");

            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    _logger.Info($"n = {n}; message = {message}");
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

            _logger.Info("End");
        }

        private static void TstAdvancedTestRunner()
        {
            _logger.Info("Begin");

            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        'End Go' >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                _logger.Info($"n = {n}; message = {message}");
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);

            _logger.Info("End");
        }

        private static void TstTestRunnerWithHostListener()
        {
            _logger.Info("Begin");

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        @@host.`rotate`(#@(gun));
        'End' >> @>log;
    }
}";

            var hostListener = new HostMethods_Tests_HostListener();

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    _logger.Info($"n = {n}; message = {message}");
                }, hostListener);

            _logger.Info("End");
        }

        private static void TstTestRunner()
        {
            _logger.Info("Begin");

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        
        @a = 10;

        repeat
        {
            @a >> @>log;
            @a = @a - 1;

            if(@a > 5)
            {
                continue;
            }

            'End of while iteration' >> @>log;

            break;
        }

        'End' >> @>log;
    }
}";

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    _logger.Info($"n = {n}; message = {message}");
                });

            _logger.Info("End");
        }

        private static void TstNameHelper()
        {
            _logger.Info("Begin");

            var text = "$x";

            var name = NameHelper.CreateName(text);

            _logger.Info($"name = {name}");

            _logger.Info("End");
        }

        private static void TstDeffuzzification()
        {
            _logger.Info("Begin");

            var veryHandler = new VeryFuzzyLogicOperatorHandler();




            var trapezoid = new TrapezoidFuzzyLogicMemberFunctionHandler(0.3, 0.4, 0.6, 0.7);
            trapezoid.CheckDirty();

            var defuzzificatedValue = trapezoid.Defuzzificate();

            _logger.Info($"defuzzificatedValue = {defuzzificatedValue}");

            var naiveVeryDefuzzificatedValue = veryHandler.SystemCall(defuzzificatedValue);

            _logger.Info($"naiveVeryDefuzzificatedValue = {naiveVeryDefuzzificatedValue}");

            defuzzificatedValue = trapezoid.Defuzzificate(new List<IFuzzyLogicOperatorHandler>() { veryHandler });

            _logger.Info($"defuzzificatedValue = {defuzzificatedValue}");

            _logger.Info("End");
        }

        private static void TstRangeValue()
        {
            _logger.Info("Begin");

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

            _logger.Info($"range = {range}");

            _logger.Info($"range.ToDbgString() = {range.ToDbgString()}");

            _logger.Info($"range.Length = {range.Length}");

            _logger.Info($"range.IsFit(-1) = {range.IsFit(-1)}");
            _logger.Info($"range.IsFit(0) = {range.IsFit(0)}");
            _logger.Info($"range.IsFit(1) = {range.IsFit(1)}");
            _logger.Info($"range.IsFit(12) = {range.IsFit(12)}");
            _logger.Info($"range.IsFit(14) = {range.IsFit(14)}");

            _logger.Info("End");
        }

        private static void TstFuzzyLogicNonNumericSequenceValue()
        {
            _logger.Info("Begin");

            var sequence = new FuzzyLogicNonNumericSequenceValue();

            _logger.Info($"sequence = {sequence}");
            _logger.Info($"sequence = {sequence.ToDbgString()}");

            var very = NameHelper.CreateName("very");

            sequence.AddIdentifier(very);

            _logger.Info($"sequence = {sequence}");
            _logger.Info($"sequence = {sequence.ToDbgString()}");

            var teenager = NameHelper.CreateName("teenager");

            sequence.AddIdentifier(teenager);

            sequence.CheckDirty();

            _logger.Info($"sequence = {sequence}");
            _logger.Info($"sequence = {sequence.ToDbgString()}");

            _logger.Info("End");
        }

        private static float Deg2Rad = 0.0174532924F;

        private static void TstCalculateTargetAnglesForRayScanner()
        {
            _logger.Info("Begin");

            var TotalRaysAngle = 125;
            var TotalRaysInterval = 10;
            var FocusRaysAngle = 35;
            var FocusRaysInterval = 2;

            var anglesList = new List<(float, bool)>();

            var halfOfTotalRaysAngle = TotalRaysAngle / 2;
            var halfOfFocusRaysAngle = FocusRaysAngle / 2;

            _logger.Info($"halfOfTotalRaysAngle = {halfOfTotalRaysAngle}");
            _logger.Info($"halfOfFocusRaysAngle = {halfOfFocusRaysAngle}");

            var currAngle = 0f;
            var inFocus = true;

            do
            {
                _logger.Info($"currAngle = {currAngle}; inFocus = {inFocus}");

                var radAngle = currAngle * Deg2Rad;

                _logger.Info($"radAngle = {radAngle}");

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

            _logger.Info($"anglesList = {JsonConvert.SerializeObject(anglesList, Formatting.Indented)}");

            _logger.Info("End");
        }

        private static void TstCopyFilesOnBuilding()
        {
            _logger.Info("Begin");

            var sourceDir = @"C:/Users/Sergey/Documents/GitHub/Game1/Assets";
            var outputPath = @"C:/Users/Sergey/Documents/ExampleBuild/CustomAssetExample.exe";

            BuildPipeLine.CopyFiles(sourceDir, outputPath);

            _logger.Info("End");
        }

        private static void TstGetRootWorldSpaceDir()
        {
            _logger.Info("Begin");

            var inputFile = @"C:/Users/Sergey/Documents/GitHub/Game1/Assets\HelloWorld_Example1/World/PeaceKeeper.world";

            _logger.Info($"inputFile = '{inputFile}'");

            var wspaceDir = WorldSpaceHelper.GetRootWorldSpaceDir(inputFile);

            _logger.Info($"wspaceDir = '{wspaceDir}'");

            _logger.Info("End");
        }

        private static void TstEnvironmentVariables()
        {
            _logger.Info("Begin");


            var path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            _logger.Info($"path = '{path}'");

            var pathsList = path.Split(';').Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p));

            _logger.Info($"pathsList = {JsonConvert.SerializeObject(pathsList, Formatting.Indented)}");

            var currDir = Directory.GetCurrentDirectory();

            _logger.Info($"currDir = {currDir}");

            if(!pathsList.Contains(currDir))
            {
                path = $"{path};{currDir}";

                _logger.Info($"path (after) = '{path}'");

                Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.User);
            }

            _logger.Info("End");
        }

        private static void TstCLINewHandler()
        {
            _logger.Info("Begin");

            var args = new List<string>() {
                 "new",
                 "Enemy"
            }.ToArray();

            var command = CLICommandParser.Parse(args);

            _logger.Info($"command = {command}");

            _logger.Info("End");
        }

        private static void TstCLIRunHandler()
        {
            _logger.Info("Begin");

            var args = new List<string>() {
                 "run"//,
            }.ToArray();

            var targetDirectory = EVPath.Normalize("%USERPROFILE%/source/repos/SymOntoClay/TestSandbox/Source");

            _logger.Info($"targetDirectory = {targetDirectory}");

            Directory.SetCurrentDirectory(targetDirectory);

            var command = CLICommandParser.Parse(args);

            _logger.Info($"command = {command}");

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = command.InputDir,
                InputFile = command.InputFile
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

            _logger.Info($"targetFiles = {targetFiles}");


            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var instance = WorldFactory.WorldInstance;

            var settings = new WorldSettings();

            settings.LibsDirs = new List<string>() { targetFiles.SharedLibsDir };

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            settings.TmpDir = targetFiles.TmpDir;

            settings.HostFile = targetFiles.WorldFile;

            settings.InvokerInMainThread = invokingInMainThread;

            settings.Monitor = new SymOntoClay.Monitor.Monitor(new SymOntoClay.Monitor.MonitorSettings
            {
                PlatformLoggers = new List<IPlatformLogger>() { new CLIPlatformLogger() },
                Enable = true
            });

            _logger.Info($"settings = {settings}");

            instance.SetSettings(settings);

            var platformListener = new TstPlatformHostListener();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = targetFiles.LogicFile;
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            _logger.Info($"npcSettings = {npcSettings}");

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            Thread.Sleep(50000);

            _logger.Info("End");
        }

        private static void TstCLICommandParser()
        {
            _logger.Info("Begin");

            var handler = new CLICommandParserHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstLogicalDatabase()
        {
            var handler = new LogicalDatabaseHandler();
            handler.Run();
        }

        private static void TstProcessInfoChildren()
        {
            _logger.Info("Begin");

            var parentProcessInfo = new ProcessInfo();

            var child_1 = new ProcessInfo();
            child_1.ParentProcessInfo = parentProcessInfo;

            var child_2 = new ProcessInfo();
            parentProcessInfo.AddChild(child_2);

            _logger.Info($"parentProcessInfo = {parentProcessInfo}");

            _logger.Info("End");
        }

        private static void TstWaitIProcessInfo()
        {
            _logger.Info("Begin");

            var processInfo = new ProcessInfo();

            var task = new Task(() => {
                processInfo.Status = ProcessStatus.Running;

                Thread.Sleep(10000);

                processInfo.Status = ProcessStatus.Completed;
            });

            task.Start();

            _logger.Info("task.Start()");

            ProcessInfoHelper.Wait(processInfo);

            _logger.Info("End");
        }

        private enum KindOfParameters
        {
            NoParameters,
            NamedParameters,
            PositionedParameters
        }

        private static void TstKindOfParametersSсaffolder()
        {
            _logger.Info("Begin");

            var mainParametersList = new List<KindOfParameters>() { KindOfParameters.NoParameters, KindOfParameters.NamedParameters, KindOfParameters.PositionedParameters};
            var additionalParametersList = new List<KindOfParameters>() { KindOfParameters.NoParameters, KindOfParameters.NamedParameters, KindOfParameters.PositionedParameters };

            var strList = new StringBuilder();

            foreach(var mainParameter in mainParametersList)
            {
                _logger.Info($"mainParameter = {mainParameter}");

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

                _logger.Info($"mainParameterStr = {mainParameterStr}");

                foreach (var additionalParameter in additionalParametersList)
                {
                    _logger.Info($"additionalParameter = {additionalParameter}");

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

                    _logger.Info($"additionalParameterStr = {additionalParameterStr}");

                    var str = $"Call{mainParameterStr}{additionalParameterStr}";

                    _logger.Info($"str = {str}");

                    strList.AppendLine(str);
                }             
            }

            _logger.Info($"strList = {strList}");

            _logger.Info("End");
        }

        private static void TstDateTimeHandler()
        {
            _logger.Info("Begin");

            var handler = new DateTimeHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstBaseManualControllingGameComponent()
        {
            _logger.Info("Begin");

            var handler = new TstBaseManualControllingGameComponentHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstLoadTypesPlatformTypesConvertors()
        {
            _logger.Info("Begin");

            var targetAttributeType = typeof(PlatformTypesConverterAttribute);

            var typesList = AppDomainTypesEnumerator.GetTypes().Where(p => p.GetCustomAttributesData().Any(x => x.AttributeType == targetAttributeType));

            _logger.Info($"typesList.Length = {typesList.Count()}");

            foreach (var type in typesList)
            {
                _logger.Info($"type.FullName = {type.FullName}");

                var convertor = (IPlatformTypesConverter)Activator.CreateInstance(type);

                _logger.Info($"convertor = {convertor}");
            }

            _logger.Info("End");
        }

        private static void TstGetTypes()
        {
            _logger.Info("Begin");

            var typesList = AppDomainTypesEnumerator.GetTypes();

            _logger.Info($"typesList.Length = {typesList.Count}");

            foreach (var type in typesList)
            {
                _logger.Info($"type.FullName = {type.FullName}");
            }

            _logger.Info("End");
        }

        private static void TstMainThreadSyncThread()
        {
            _logger.Info("Begin");

            var invokingInMainThread = new InvokerInMainThread();

            var task = Task.Run(() => { 
                while(true)
                {
                    invokingInMainThread.Update();

                    Thread.Sleep(1000);
                }
            });

            Thread.Sleep(5000);

            var invocableInMainThreadObj = new InvocableInMainThread(() => { _logger.Info("^)"); }, invokingInMainThread);
            invocableInMainThreadObj.Run();

            Thread.Sleep(5000);

            _logger.Info("End");
        }

        private static void TstCoreHostListenerHandler()
        {
            _logger.Info("Begin");

            var handler = new CoreHostListenerHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstNullableArithmetic()
        {
            _logger.Info("Begin");

            float? a = null;

            var b = 1 - a;

            _logger.Info($"b = {b}");
            _logger.Info($"b == null = {b == null}");

            _logger.Info("End");
        }

        private static void TstInheritanceItemsHandler()
        {
            _logger.Info("Begin");

            var handler = new InheritanceItemsHandler();
            handler.Run();

            _logger.Info("End");
        }
            
        private static void TstDefaultSettingsOfCodeEntityHandler()
        {
            _logger.Info("Begin");

            var handler = new DefaultSettingsOfCodeEntityHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstActivateMainEntity()
        {
            _logger.Info("Begin");

            var handler = new ActivateMainEntityHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstCompileInlineTrigger()
        {
            _logger.Info("Begin");

            var handler = new CompileInlineTriggerHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstRegOperatorsHandler()
        {
            _logger.Info("Begin");

            var handler = new RegOperatorsHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstCreateEngineContext()
        {
            _logger.Info("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext();

            _logger.Info("End");
        }

        private static void TstAsyncActivePeriodicObjectHandler()
        {
            _logger.Info("Begin");

            var handler = new AsyncActivePeriodicObjectHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstSyncActivePeriodicObjectHandler()
        {
            _logger.Info("Begin");

            var handler = new SyncActivePeriodicObjectHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstCodeExecution()
        {
            _logger.Info("Begin");

            var handler = new CodeExecutionHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstCreateName()
        {
            _logger.Info("Begin");

            var parserContext = new TstMainStorageContext();

            var nameVal1 = "dog";

            _logger.Info($"{nameof(nameVal1)} = {nameVal1}");





























            var name = NameHelper.CreateName(nameVal1);

            _logger.Info($"name = {name}");

            _logger.Info("End");
        }

        private class NormalizedNameValues
        {
            public string Name { get; set; }
            public List<string> Namespaces { get; set; }
        }

        private static NormalizedNameValues ParseName(string text)
        {
#if DEBUG
            _logger.Info($"test = {text}");
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
            _logger.Info("Begin");

            var handler = new ExprNodeHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstParsing()
        {
            _logger.Info("Begin");

            var handler = new ParsingHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstBattleRoyaleHandler()
        {
            _logger.Info("Begin");

            var handler = new BattleRoyaleHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstPlacesHandler()
        {
            _logger.Info("Begin");

            using var handler = new PlacesHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstMonoBehaviourTestingHandler()
        {
            _logger.Info("Begin");

            var handler = new MonoBehaviourTestingHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstSoundStartHandler()
        {
            _logger.Info("Begin");

            using var handler = new SoundStartHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstAddingFactTriggerHandler()
        {
            _logger.Info("Begin");

            using var handler = new AddingFactTriggerHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstGeneralStartHandler()
        {
            _logger.Info("Begin");


            using var handler = new GeneralStartHandler();
            handler.Run();

            _logger.Info("End");
        }

        private static void TstGetParsedFilesInfo()
        {
            _logger.Info("Begin");

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\PixKeeper\PixKeeper.txt");

            _logger.Info($"fileName = {fileName}");

            var id = "#020ED339-6313-459A-900D-92F809CEBDC5";

            var resultList = FileHelper.GetParsedFilesInfo(fileName, id);

            _logger.Info($"resultList = {JsonConvert.SerializeObject(resultList, Formatting.Indented)}");

            _logger.Info("End");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Info($"e.ExceptionObject = {e.ExceptionObject}");
        }
    }
}

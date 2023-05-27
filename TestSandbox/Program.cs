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

namespace TestSandbox
{
    class Program
    {
        private static readonly IEntityLogger _logger = new LoggerNLogImpementation();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            EVPath.RegVar("APPDIR", Directory.GetCurrentDirectory());

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
            TstBattleRoyaleHandler();//<==
            //TstPlacesHandler();//<==
            //TstMonoBehaviourTestingHandler();//VT<=
            //TstSoundStartHandler();//<==
            //TstAddingFactTriggerHandler();
            //TstGeneralStartHandler();//<=
            //TstGetParsedFilesInfo();

            //Thread.Sleep(10000);
        }

        private static void TstDetectDoninantItems()
        {
            _logger.Log("Begin");

            var initialList = new List<KindOfLogicalQueryNode>() { KindOfLogicalQueryNode.Entity, KindOfLogicalQueryNode.Concept, KindOfLogicalQueryNode.Value, KindOfLogicalQueryNode.Entity };

            _logger.Log($"initialList = {JsonConvert.SerializeObject(initialList, Formatting.Indented)}");

            var orderedList = initialList.GroupBy(p => p).Select(p => new
            {
                Value = p.Key,
                Count = p.Count()
            }).OrderByDescending(p => p.Count).Take(1);

            _logger.Log($"orderedList = {JsonConvert.SerializeObject(orderedList, Formatting.Indented)}");

            _logger.Log("End");
        }

        private static void TstSerializeValue()
        {
            _logger.Log("Begin");

            var list = new List<object>() { new NumberValue(16) };

            _logger.Log($"list = {JsonConvert.SerializeObject(list, Formatting.Indented, new ToHumanizedStringJsonConverter())}");

            _logger.Log("End");
        }

        private static void TstWaitAsync()
        {
            _logger.Log("Begin");

            var source1 = new CancellationTokenSource();
            var token1 = source1.Token;

            var task = Task.Run(() => {
                _logger.Log("Hi!");

                Thread.Sleep(10000);

                _logger.Log("End Hi!");
            }, token1);

            var taskValue = new TaskValue(task, source1);

            taskValue.OnComplete += () => 
            {
                _logger.Log("taskValue.OnComplete!!!!!");
            };

            taskValue.OnComplete += () =>
            {
                _logger.Log("(2) taskValue.OnComplete!!!!!");
            };

            Thread.Sleep(20000);

            _logger.Log("End");
        }

        private static void TstWaitAsync_2()
        {
            _logger.Log("Begin");

            var source1 = new CancellationTokenSource();
            var token1 = source1.Token;

            var task = Task.Run(() => {
                _logger.Log("Hi!");

                Thread.Sleep(10000);

                _logger.Log("End Hi!");
            }, token1);

            Task.Run(() => {
                var waitingTask = task.WaitAsync(token1);

                waitingTask.Wait();

                _logger.Log($"Finished!!!! task.Status = {task.Status}");
            });

            Thread.Sleep(20000);

            _logger.Log("End");
        }

        private static void TstWaitAsync_1()
        {
            _logger.Log("Begin");

            var source1 = new CancellationTokenSource();
            var token1 = source1.Token;

            var task = Task.Run(() => {
                _logger.Log("Hi!");

                Thread.Sleep(10000);

                _logger.Log("End Hi!");
            }, token1);

            var waitingTask = task.WaitAsync(token1);

            waitingTask.Wait();

            _logger.Log("End");
        }

        private static void TstWorldSpaceFilesSearcher()
        {
            _logger.Log("Begin");

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = @"C:\Users\Acer\source\repos\SymOntoClayAsset\Assets\SymOntoClayScripts",
                AppName = "tst1",
                SearchMainNpcFile = false
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

#if DEBUG
            _logger.Log($"targetFiles = {targetFiles}");
#endif 

            _logger.Log("End");
        }

        private static void TstSynonymsHandler()
        {
            _logger.Log("Begin");

            var handler = new SynonymsHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstGetFullBaseTypesListInCSharpReflection()
        {
            _logger.Log("Begin");

            var source = typeof(ConditionalEntityValue);

            _logger.Log($"source.FullName = {source.FullName}");

            var interfacesList = source.GetInterfaces();

            _logger.Log($"interfacesList.Length = {interfacesList.Length}");

            foreach(var item in interfacesList)
            {
                _logger.Log($"item.FullName = {item.FullName}");
            }

            PrintBaseType(source.BaseType);

            _logger.Log("End");
        }

        private static void PrintBaseType(Type type)
        {
            if(type == null)
            {
                return;
            }

            _logger.Log($"type.FullName = {type.FullName}");

            PrintBaseType(type.BaseType);
        }

        private static void TstConvertFactToImperativeCode()
        {
            _logger.Log("Begin");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :}";

            _logger.Log($"factStr = '{factStr}'");

            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = engineContext.Storage.GlobalStorage;
            localCodeExecutionContext.Holder = NameHelper.CreateName(engineContext.Id);

            var compiledCode = engineContext.ConvertersFactory.GetConverterFactToImperativeCode().Convert(fact, localCodeExecutionContext);

            _logger.Log($"compiledCode = {compiledCode.ToDbgString()}");

            _logger.Log("End");
        }

        private static void TstFactToHtml()
        {
            _logger.Log("Begin");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";

            _logger.Log($"factStr = '{factStr}'");

            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            var targetItemForSelection = fact.PrimaryPart.Expression.Left.Left.Left.Left;

            _logger.Log($"targetItemForSelection = {targetItemForSelection.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");


            var options = new DebugHelperOptions();
            options.HumanizedOptions = HumanizedOptions.ShowOnlyMainContent;
            options.IsHtml = true;
            options.ItemsForSelection = new List<IObjectToString>() { targetItemForSelection };

            _logger.Log($"fact = {DebugHelperForRuleInstance.ToString(fact, options)}");

            _logger.Log("End");
        }

        private static void TstStandardFactsBuilder()
        {
            _logger.Log("Begin");

            var handler = new StandardFactsBuilderHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstStandardFactsBuilderGetTargetVarNameHandler()
        {
            _logger.Log("Begin");

            var handler = new StandardFactsBuilderGetTargetVarNameHandler();
            handler.Run();

            _logger.Log("End");
        }            

        private static void TstShieldString()
        {
            _logger.Log("Begin");

            var str = "@@self";

            _logger.Log($"str = '{str}'");

            var nameSubStr = str.Substring(2);

            _logger.Log($"nameSubStr = '{nameSubStr}'");

            var name = NameHelper.ShieldString(str);

            _logger.Log($"name = '{name}'");

            _logger.Log("End");
        }

        private static void TstSampleSpeechSynthesis()
        {
            _logger.Log("Begin");

            var synth = new SpeechSynthesizer();

            synth.SetOutputToDefaultAudioDevice();

            synth.Speak("This example demonstrates a basic use of Speech Synthesizer");
            synth.Speak("Go to green place!");

            _logger.Log("End");
        }

        private static void TstOnAddingFactEventHanler()
        {
            _logger.Log("Begin");

            var handler = new OnAddingFactEventHanler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstEventHanler()
        {
            _logger.Log("Begin");

            var handler = new EventHanler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstStrCollectionCombination()
        {
            _logger.Log("Begin");

            var source = new List<List<string>>();

            source.Add(new List<string>() { "a", "e" });
            source.Add(new List<string>() { "h", "j", "L" });
            source.Add(new List<string>() { "b", "c", "f" });

            _logger.Log($"source = {JsonConvert.SerializeObject(source, Formatting.Indented)}");

            var result = CollectionCombinationHelper.Combine(source);

            _logger.Log($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            _logger.Log("End");
        }

        private static void TstIntCollectionCombination()
        {
            _logger.Log("Begin");

            var source = new List<List<int>>();

            source.Add(new List<int>() { 1, 5 });
            source.Add(new List<int>() { 7, 9, 11 });
            source.Add(new List<int>() { 2, 3, 6 });

            _logger.Log($"source = {JsonConvert.SerializeObject(source, Formatting.Indented)}");

            var result = CollectionCombinationHelper.Combine(source);

            _logger.Log($"result = {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            _logger.Log("End");
        }

        private static void TstModalitiesHandler()
        {
            _logger.Log("Begin");

            var handler = new ModalitiesHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstRelationsStorageHandler()
        {
            _logger.Log("Begin");

            var handler = new RelationsStorageHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstGenerateDllDict()
        {
            _logger.Log("Begin");

            var handler = new GenerateDllDictHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TSTWordsFactory()
        {
            _logger.Log("Begin");

            var wordsFactory = new WordsFactory();
            wordsFactory.Run();

            _logger.Log("End");
        }

        private static void TstNLPConverterProvider()
        {
            _logger.Log("Begin");

            var mainDictPath = Path.Combine(Directory.GetCurrentDirectory(), "Dicts", "BigMainDictionary.dict");

#if DEBUG
            _logger.Log($"mainDictPath = {mainDictPath}");
#endif

            var settings = new NLPConverterProviderSettings();
            settings.DictsPaths = new List<string>() { mainDictPath };
            settings.CreationStrategy = CreationStrategy.Singleton;

#if DEBUG
            _logger.Log($"settings = {settings}");
#endif

            var nlpConverterProvider = new NLPConverterProvider(settings);

            var factory = nlpConverterProvider.GetFactory(_logger);

            var converter = factory.GetConverter();

            _logger.Log("End");
        }

        private static void TstNLPHandler()
        {
            _logger.Log("Begin");

            var handler = new NLPHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstTriggerConditionNodeHandler()
        {
            _logger.Log("Begin");

            var handler = new TriggerConditionNodeHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstSoundBus()
        {
            _logger.Log("Begin");

            var handler = new TstSoundBusHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstNavigationHandler()
        {
            _logger.Log("Begin");

            var handler = new NavigationHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstCreatorExamples()
        {
            _logger.Log("Begin");

            using var handler = new CreatorExamples_States_27_03_2022();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstLinguisticVariable_Tests()
        {
            _logger.Log("Begin");

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

            _logger.Log($"firstItem = {firstItem}");

            var term = firstItem.AsLinguisticVariable.Values[4];

            _logger.Log($"term = {term}");

            var handler = term.Handler;

            _logger.Log($"handler = {handler}");

            _logger.Log("End");
        }

        private static void TstManageTempProject()
        {
            _logger.Log("Begin");

            var initialDir = Directory.GetCurrentDirectory();

            _logger.Log($"initialDir = {initialDir}");

            initialDir = Path.Combine(initialDir, "TempProjects");

            _logger.Log($"initialDir (2) = {initialDir}");

            if(!Directory.Exists(initialDir))
            {
                Directory.CreateDirectory(initialDir);
            }

            var testDir = Path.Combine(initialDir, $"TstDir{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            _logger.Log($"testDir = {testDir}");

            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            var projectName = "Example";

            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { ProjectName = projectName };

            _logger.Log($"worldSpaceCreationSettings = {worldSpaceCreationSettings}");

            var wSpaceFile = WorldSpaceCreator.CreateWithOutWSpaceFile(worldSpaceCreationSettings, testDir
                    , errorMsg => _logger.Error(errorMsg)
                    );

            _logger.Log($"wSpaceFile = {wSpaceFile}");

            var wSpaceDir = wSpaceFile.DirectoryName;

            _logger.Log($"wSpaceDir = {wSpaceDir}");

            var targetRelativeFileName = @"/Npcs/Example/Example.soc";

            _logger.Log($"targetRelativeFileName = {targetRelativeFileName}");

            if(targetRelativeFileName.StartsWith("/") || targetRelativeFileName.StartsWith("\\"))
            {
                targetRelativeFileName = targetRelativeFileName.Substring(1);
            }

            _logger.Log($"targetRelativeFileName (after) = {targetRelativeFileName}");

            var targetFileName = Path.Combine(wSpaceDir, targetRelativeFileName);

            _logger.Log($"targetFileName = {targetFileName}");

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

            _logger.Log($"supportBasePath = {supportBasePath}");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            _logger.Log($"logDir = {logDir}");

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
                message => { _logger.Log($"message = {message}"); },
                error => { _logger.Log($"error = {error}"); }
                );

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { callBackLogger },
                Enable = true,
                EnableRemoteConnection = true
            };

            _logger.Log($"settings = {settings}");

            instance.SetSettings(settings);

            var platformListener = new object();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = Path.Combine(wSpaceDir, $"Npcs/{projectName}/{projectName}.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            _logger.Log($"npcSettings = {npcSettings}");

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            Thread.Sleep(5000);

            Directory.Delete(testDir, true);

            _logger.Log("End");
        }

        private static void TstAdvancedTestRunnerForMultipleInstances()
        {
            _logger.Log("Begin");

            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    _logger.Log($"n = {n}; message = {message}");
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

            _logger.Log("End");
        }

        private static void TstAdvancedTestRunner()
        {
            _logger.Log("Begin");

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
                _logger.Log($"n = {n}; message = {message}");
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);

            _logger.Log("End");
        }

        private static void TstTestRunnerWithHostListener()
        {
            _logger.Log("Begin");

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
                    _logger.Log($"n = {n}; message = {message}");
                }, hostListener);

            _logger.Log("End");
        }

        private static void TstTestRunner()
        {
            _logger.Log("Begin");

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
                    _logger.Log($"n = {n}; message = {message}");
                });

            _logger.Log("End");
        }

        private static void TstNameHelper()
        {
            _logger.Log("Begin");

            var text = "$x";

            var name = NameHelper.CreateName(text);

            _logger.Log($"name = {name}");

            _logger.Log("End");
        }

        private static void TstDeffuzzification()
        {
            _logger.Log("Begin");

            var veryHandler = new VeryFuzzyLogicOperatorHandler();




            var trapezoid = new TrapezoidFuzzyLogicMemberFunctionHandler(0.3, 0.4, 0.6, 0.7);
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
            }.ToArray();

            var targetDirectory = EVPath.Normalize("%USERPROFILE%/source/repos/SymOntoClay/TestSandbox/Source");

            _logger.Log($"targetDirectory = {targetDirectory}");

            Directory.SetCurrentDirectory(targetDirectory);

            var command = CLICommandParser.Parse(args);

            _logger.Log($"command = {command}");

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = command.InputDir,
                InputFile = command.InputFile
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

            _logger.Log($"targetFiles = {targetFiles}");


            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var instance = WorldFactory.WorldInstance;

            var settings = new WorldSettings();

            settings.LibsDirs = new List<string>() { targetFiles.SharedLibsDir };

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            settings.TmpDir = targetFiles.TmpDir;

            settings.HostFile = targetFiles.WorldFile;

            settings.InvokerInMainThread = invokingInMainThread;

            settings.Logging = new LoggingSettings()
            {
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
            npcSettings.LogicFile = targetFiles.LogicFile;
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            _logger.Log($"npcSettings = {npcSettings}");

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            Thread.Sleep(50000);

            _logger.Log("End");
        }

        private static void TstCLICommandParser()
        {
            _logger.Log("Begin");

            var handler = new CLICommandParserHandler();
            handler.Run();

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

            var targetAttributeType = typeof(PlatformTypesConverterAttribute);

            var typesList = AppDomainTypesEnumerator.GetTypes().Where(p => p.GetCustomAttributesData().Any(x => x.AttributeType == targetAttributeType));

            _logger.Log($"typesList.Length = {typesList.Count()}");

            foreach (var type in typesList)
            {
                _logger.Log($"type.FullName = {type.FullName}");

                var convertor = (IPlatformTypesConverter)Activator.CreateInstance(type);

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

        private static void TstBattleRoyaleHandler()
        {
            _logger.Log("Begin");

            var handler = new BattleRoyaleHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstPlacesHandler()
        {
            _logger.Log("Begin");

            using var handler = new PlacesHandler();
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

        private static void TstSoundStartHandler()
        {
            _logger.Log("Begin");

            using var handler = new SoundStartHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstAddingFactTriggerHandler()
        {
            _logger.Log("Begin");

            using var handler = new AddingFactTriggerHandler();
            handler.Run();

            _logger.Log("End");
        }

        private static void TstGeneralStartHandler()
        {
            _logger.Log("Begin");


            using var handler = new GeneralStartHandler();
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

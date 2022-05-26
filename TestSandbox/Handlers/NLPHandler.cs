using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Dicts;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.NLP.Internal.ConvertingCGToInternal;
using SymOntoClay.NLP.Internal.ConvertingFactToInternalCG;
using SymOntoClay.NLP.Internal.ConvertingInternalCGToFact;
using SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure;
using SymOntoClay.NLP.Internal.ConvertingPhraseStructureToText;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.NLP;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class NLPHandler
    {
        public NLPHandler()
        {
            _engineContext = TstEngineContextHelper.CreateAndInitContext().EngineContext;

            var mainDictPath = Path.Combine(Directory.GetCurrentDirectory(), "Dicts", "BigMainDictionary.dict");

#if DEBUG
            _logger.Log($"mainDictPath = {mainDictPath}");
#endif

            _wordsDict = new JsonDictionary(mainDictPath);

            _converter = new NLPConverter(_logger, _wordsDict);

            //CreateCommonRelations();
        }

        private readonly EngineContext _engineContext;
        private readonly INLPConverter _converter;
        private readonly IWordsDict _wordsDict;
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();
            //Case0();

            _logger.Log("End");
        }

        private void Case5()
        {
            var nlpContext = CreateNLPConverterContext();

            var factStr = "{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}";

            var ruleInstance = Parse(factStr);

            _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");

            var converterFactToCG = new ConverterFactToInternalCG(_logger);

            var internalCG = converterFactToCG.Convert(ruleInstance, nlpContext);

            _logger.Log($"internalCG = {internalCG}");

            var dotStr = DotConverter.ConvertToString(internalCG);

            _logger.Log($"dotStr = '{dotStr}'");

            var converterInternalCGToPhraseStructure = new ConverterInternalCGToPhraseStructure(_logger, _wordsDict);

            var sentenceItem = converterInternalCGToPhraseStructure.Convert(internalCG, nlpContext);

            _logger.Log($"sentenceItem = {sentenceItem}");
            _logger.Log($"sentenceItem = {sentenceItem.ToDbgString()}");

            var converterPhraseStructureToText = new ConverterPhraseStructureToText(_logger);

            var text = converterPhraseStructureToText.Convert(sentenceItem);

            _logger.Log($"text = '{text}'");
        }

        private void Case4()
        {
            var nlpContext = CreateNLPConverterContext();

            var factStr = "{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}";

            var ruleInstance = Parse(factStr);

            _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");

            var text = _converter.Convert(ruleInstance, nlpContext);

            _logger.Log($"text = '{text}'");
        }

        private RuleInstance Parse(string text)
        {
            return _engineContext.Parser.ParseRuleInstance(text);
        }

        private void Case3()
        {
            var text = "I like my cat.";

#if DEBUG
            _logger.Log($"text = {text}");
#endif

            var ruleInstancesList = _converter.Convert(text);

            _logger.Log($"ruleInstancesList.Count = {ruleInstancesList.Count}");

            foreach (var ruleInstance in ruleInstancesList)
            {
                _logger.Log($"ruleInstance.KindOfRuleInstance = {ruleInstance.KindOfRuleInstance}");
                _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
            }
        }

        private void Case2()
        {
            //var wordsDict = new TstSimpleWordsDict();

            var text = "I like my cat.";

#if DEBUG
            _logger.Log($"text = {text}");
#endif

            //var globalContext = new GlobalParserContext(_engineContext, wordsDict, text);

            //globalContext.Run();

            var parser = new ATNParser(_engineContext.Logger, _wordsDict);

            var compactizer = new PhraseCompactizer(_engineContext.Logger, _wordsDict);

            var converterToPlainSentences = new ConverterToPlainSentences(_engineContext.Logger);

            var semanticAnalyzer = new SemanticAnalyzer(_engineContext.Logger, _wordsDict);

            var convertorCGToInternal = new ConvertorCGToInternal(_engineContext.Logger);

            var converterInternalCGToFact = new ConverterInternalCGToFact(_engineContext.Logger);

            var result = parser.Run(text);

            _logger.Log($"result.Count = {result.Count}");

            foreach (var item in result)
            {
                _logger.Log($"item = '{item.ToDbgString()}'");

                compactizer.Run(item);

                _logger.Log($"item (after) = '{item.ToDbgString()}'");

                var plainSentencesList = converterToPlainSentences.Run(item);

                _logger.Log($"plainSentencesList.Count = {plainSentencesList.Count}");

                foreach(var plainSentence in plainSentencesList)
                {
                    _logger.Log($"plainSentence = '{plainSentence.ToDbgString()}'");

                    var conceptualGraph = semanticAnalyzer.Run(plainSentence);

                    var conceptualGraphDbgStr = DotConverter.ConvertToString(conceptualGraph);

                    _logger.Log($"conceptualGraphDbgStr = '{conceptualGraphDbgStr}'");

                    //_logger.Log($"conceptualGraphDbgStr.DeepTrim() = '{conceptualGraphDbgStr.DeepTrim()}'");

                    var internalCG = convertorCGToInternal.Convert(conceptualGraph);

                    _logger.Log($"internalCG = {internalCG}");

                    var dotStr = DotConverter.ConvertToString(internalCG);

                    _logger.Log($"dotStr = '{dotStr}'");

                    var ruleInstancesList = converterInternalCGToFact.ConvertConceptualGraph(internalCG);

                    _logger.Log($"ruleInstancesList.Count = {ruleInstancesList.Count}");

                    foreach (var ruleInstance in ruleInstancesList)
                    {
                        _logger.Log($"ruleInstance.KindOfRuleInstance = {ruleInstance.KindOfRuleInstance}");
                        _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
                    }
                }
            }
        }

        private void Case1()
        {
            var result = _converter.Convert("I like my cat.()!.?,...:;-1234567890 M1$nrg, #erty3, @maror, 3% can't");

            _logger.Log($"result.Count = {result.Count}");

            foreach(var item in result)
            {
                _logger.Log($"item = {item.ToHumanizedString()}");
            }
        }

        private void Case0()
        {
            var text = "I like my cat.()!.?,...:;-1234567890 M1$nrg, #erty3, @maror, 3% can't \"";

            var lexer = new ATNStringLexer(text);
            (string, int, int) item;

            var n = 0;

            while ((item = lexer.GetItem()).Item1 != null)
            {
                n++;

                _logger.Log($"n = {n}");
                _logger.Log($"item = {item}");
            }
        }

        private void CreateCommonRelations()
        {
            CreateLikeRelation();
            CreatePossessRelation();
        }

        private void CreatePossessRelation()
        {
            var relationName = NameHelper.CreateName("possess");

            var relation = new RelationDescription();
            relation.Name = relationName;

            var argument = new RelationParameterDescription();
            argument.Name = NameHelper.CreateName("$x1");
            argument.MeaningRolesList.Add(NameHelper.CreateName("owner"));

            relation.Arguments.Add(argument);

            argument = new RelationParameterDescription();
            argument.Name = NameHelper.CreateName("$x2");
            argument.MeaningRolesList.Add(NameHelper.CreateName("possessions"));

            relation.Arguments.Add(argument);

            var inheritanceItem = new InheritanceItem();
            relation.InheritanceItems.Add(inheritanceItem);
            inheritanceItem.SubName = relation.Name;
            inheritanceItem.SuperName = NameHelper.CreateName("state");
            inheritanceItem.Rank = LogicalValue.TrueValue;

            //_logger.Log($"relation = {relation}");
            _logger.Log($"relation = {relation.ToHumanizedString()}");

            AppendRelationToStorage(relation);
        }

        private void CreateLikeRelation()
        {
            var relationName = NameHelper.CreateName("like");

            var relation = new RelationDescription();
            relation.Name = relationName;

            var argument = new RelationParameterDescription();
            argument.Name = NameHelper.CreateName("$x1");
            argument.MeaningRolesList.Add(NameHelper.CreateName("experiencer"));

            relation.Arguments.Add(argument);

            argument = new RelationParameterDescription();
            argument.Name = NameHelper.CreateName("$x2");
            argument.MeaningRolesList.Add(NameHelper.CreateName("object"));

            relation.Arguments.Add(argument);

            var inheritanceItem = new InheritanceItem();
            relation.InheritanceItems.Add(inheritanceItem);
            inheritanceItem.SubName = relation.Name;
            inheritanceItem.SuperName = NameHelper.CreateName("state");
            inheritanceItem.Rank = LogicalValue.TrueValue;

            //_logger.Log($"relation = {relation}");
            _logger.Log($"relation = {relation.ToHumanizedString()}");

            AppendRelationToStorage(relation);
        }

        private void AppendRelationToStorage(RelationDescription relation)
        {
            var globalStorage = _engineContext.Storage.GlobalStorage;

            var inheritanceStorage = globalStorage.InheritanceStorage;

            foreach (var item in relation.InheritanceItems)
            {
                inheritanceStorage.SetInheritance(item);
            }

            globalStorage.RelationsStorage.Append(relation);
        }

        private INLPConverterContext CreateNLPConverterContext()
        {
            //using var tmpContext = UnityTestEngineContextFactory.CreateTestEngineContext();

            var targetContext = _engineContext;
            //var targetContext = tmpContext.EngineContext;

            var dataResolversFactory = targetContext.DataResolversFactory;

            var relationsResolver = dataResolversFactory.GetRelationsResolver();
            var inheritanceResolver = dataResolversFactory.GetInheritanceResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = targetContext.Storage.GlobalStorage;
            localCodeExecutionContext.Holder = NameHelper.CreateName(_engineContext.Id);

            //_logger.Log($"localCodeExecutionContext = {localCodeExecutionContext}");

            var packedRelationsResolver = new PackedRelationsResolver(relationsResolver, localCodeExecutionContext);

            var packedInheritanceResolver = new PackedInheritanceResolver(inheritanceResolver, localCodeExecutionContext);

            return new NLPConverterContext(packedRelationsResolver, packedInheritanceResolver);
        }
    }
}

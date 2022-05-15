using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Dicts;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.NLP.Internal.ConvertingCGToInternal;
using SymOntoClay.NLP.Internal.ConvertingInternalCGToFact;
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

            _converter = new NLPConverter(_engineContext, _wordsDict);
        }

        private readonly EngineContext _engineContext;
        private readonly INLPConverter _converter;
        private readonly IWordsDict _wordsDict;
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            Case2();
            //Case1();
            //Case0();

            _logger.Log("End");
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
    }
}

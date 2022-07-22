/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Tests.Helpers;
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
            _engineContext = TstEngineContextHelper.CreateAndInitContextWithoutAppFiles().EngineContext;

            var mainDictPath = Path.Combine(Directory.GetCurrentDirectory(), "Dicts", "BigMainDictionary.dict");

#if DEBUG
            _logger.Log($"mainDictPath = {mainDictPath}");
#endif

            _wordsDict = new JsonDictionary(mainDictPath);

            _converter = new NLPConverter(_logger, _wordsDict);
        }

        private readonly IEngineContext _engineContext;
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
            var nlpContext = _engineContext.GetNLPConverterContext();

            //var factStr = "{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}";
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 0 :}";

            var ruleInstance = Parse(factStr);

            //ruleInstance.ObligationModality = LogicalValue.TrueValue;
            //_logger.Log($"ruleInstance = {ruleInstance}");

            _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");

            var converterFactToCG = new ConverterFactToInternalCG(_logger);

            var internalCG = converterFactToCG.Convert(ruleInstance, nlpContext);

            _logger.Log($"internalCG = {internalCG}");

            var dotStr = DotConverter.ConvertToString(internalCG);

            _logger.Log($"dotStr = '{dotStr}'");

            var converterInternalCGToPhraseStructure = new ConverterInternalCGToPhraseStructure(_logger, _wordsDict);

            var sentenceItem = converterInternalCGToPhraseStructure.Convert(internalCG, nlpContext);

            ////_logger.Log($"sentenceItem = {sentenceItem}");
            _logger.Log($"sentenceItem = {sentenceItem.ToDbgString()}");

            var converterPhraseStructureToText = new ConverterPhraseStructureToText(_logger);

            var text = converterPhraseStructureToText.Convert(sentenceItem);

            _logger.Log($"text = '{text}'");
        }

        private void Case4()
        {
            var nlpContext = _engineContext.GetNLPConverterContext();

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
            //var text = "I like my cat.";
            var text = "Go to green place!";

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

            //var text = "I like my cat.";
            var text = "Go to green place!";

#if DEBUG
            _logger.Log($"text = {text}");
#endif

            //var globalContext = new GlobalParserContext(_engineContext, wordsDict, text);

            //globalContext.Run();

            var logSessionDir = Directory.GetCurrentDirectory();

            //var logDir = Path.Combine(Directory.GetCurrentDirectory(), "ATNLogs");

            //if (!Directory.Exists(logDir))
            //{
            //    Directory.CreateDirectory(logDir);
            //}

            //var logSessionDir = Path.Combine(logDir, $"ATN_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}");

            //if (!Directory.Exists(logSessionDir))
            //{
            //    Directory.CreateDirectory(logSessionDir);
            //}

            var parser = new ATNParser(_engineContext.Logger, _wordsDict);

            var compactizer = new PhraseCompactizer(_engineContext.Logger, _wordsDict);

            var converterToPlainSentences = new ConverterToPlainSentences(_engineContext.Logger);

            var semanticAnalyzer = new SemanticAnalyzer(_engineContext.Logger, _wordsDict);

            var convertorCGToInternal = new ConvertorCGToInternal(_engineContext.Logger);

            var converterInternalCGToFact = new ConverterInternalCGToFact(_engineContext.Logger);

            var result = parser.Run(text);
            //var result = parser.Run(text, true, logSessionDir);

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

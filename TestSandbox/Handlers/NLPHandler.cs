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

using SymOntoClay.BaseTestLib;
using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.NLP;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Dicts;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.NLP.Internal.ConvertingCGToInternal;
using SymOntoClay.NLP.Internal.ConvertingFactToInternalCG;
using SymOntoClay.NLP.Internal.ConvertingInternalCGToFact;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using System.IO;

namespace TestSandbox.Handlers
{
    public class NLPHandler
    {
        public NLPHandler()
        {
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultAppFiles = false;
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseStandardLibrary = KindOfUsingStandardLibrary.BuiltIn;

            _engineContext = UnityTestEngineContextFactory.CreateAndInitTestEngineContext(factorySettings).EngineContext;

            var mainDictPath = Path.Combine(Directory.GetCurrentDirectory(), "Dicts", "BigMainDictionary.dict");

#if DEBUG
            _logger.Info("FCFA0B27-0010-48E5-B25E-25123E7C48FC", $"mainDictPath = {mainDictPath}");
#endif

            _wordsDict = new JsonDictionary(mainDictPath);

            _converter = new NLPConverter(_logger, _wordsDict);
        }

        private readonly IEngineContext _engineContext;
        private readonly INLPConverter _converter;
        private readonly IWordsDict _wordsDict;
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("4D933D64-6489-47ED-8CAA-32AABCBC4B78", "Begin");

            Case6();
            //Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();
            //Case0();

            _logger.Info("1C22A861-96A6-46FD-9187-65762CD44423", "End");
        }

        private void Case6()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :}";

            var nlpContext = _engineContext.GetNLPConverterContext();

            var ruleInstance = Parse(factStr);

            var converterFactToCG = new ConverterFactToInternalCG(_logger);

            var internalCG = converterFactToCG.Convert(_logger, ruleInstance, nlpContext);
        }

        private void Case5()
        {
            var nlpContext = _engineContext.GetNLPConverterContext();

            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 0 :}";

            var ruleInstance = Parse(factStr);


            _logger.Info("B6BEBC5B-6ADB-483E-A0FD-6BB90D9C82C8", $"ruleInstance = {ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");

            var converterFactToCG = new ConverterFactToInternalCG(_logger);

            var internalCG = converterFactToCG.Convert(_logger, ruleInstance, nlpContext);

            _logger.Info("D567EE1F-31CA-4452-A64A-D3DFF71980AF", $"internalCG = {internalCG}");

            var dotStr = DotConverter.ConvertToString(internalCG);

            _logger.Info("C46149ED-0951-485F-9147-6875F4C619B0", $"dotStr = '{dotStr}'");



            //////_logger.Info($"sentenceItem = {sentenceItem}");



        }

        private void Case4()
        {
            var nlpContext = _engineContext.GetNLPConverterContext();

            var factStr = "{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}";

            var ruleInstance = Parse(factStr);

            _logger.Info("F3B33F6C-CCDC-4B35-B788-612CCA64343C", $"ruleInstance = {ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");

            var text = _converter.Convert(_logger, ruleInstance, nlpContext);

            _logger.Info("5921CBCF-BF51-49E6-BBB6-76FB82077785", $"text = '{text}'");
        }

        private RuleInstance Parse(string text)
        {
            return _engineContext.Parser.ParseRuleInstance(text);
        }

        private void Case3()
        {
            var text = "Go to green place!";

#if DEBUG
            _logger.Info("537F688B-DAD3-4DF0-A711-AE56FA09288D", $"text = {text}");
#endif

            var ruleInstancesList = _converter.Convert(_logger, text);

            _logger.Info("10C0ECF0-DB82-44E0-A2B0-9902B9D9F829", $"ruleInstancesList.Count = {ruleInstancesList.Count}");

            foreach (var ruleInstance in ruleInstancesList)
            {
                _logger.Info("CF4AC895-BC46-4679-959C-AB3455A1C31B", $"ruleInstance.KindOfRuleInstance = {ruleInstance.KindOfRuleInstance}");
                _logger.Info("CAAE5145-15FC-4AE5-A8FD-2F30ACFCB5C5", $"ruleInstance = {ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
            }
        }

        private void Case2()
        {

            var text = "Go to green place!";

#if DEBUG
            _logger.Info("7F8B802D-1D9E-4602-BAF3-FDDB88AF0E8A", $"text = {text}");
#endif



            var logSessionDir = Directory.GetCurrentDirectory();





            var parser = new ATNParser(_engineContext.Logger, _wordsDict);

            var compactizer = new PhraseCompactizer(_engineContext.Logger, _wordsDict);

            var converterToPlainSentences = new ConverterToPlainSentences(_engineContext.Logger);

            var semanticAnalyzer = new SemanticAnalyzer(_engineContext.Logger, _wordsDict);

            var convertorCGToInternal = new ConvertorCGToInternal(_engineContext.Logger);

            var converterInternalCGToFact = new ConverterInternalCGToFact(_engineContext.Logger);

            var result = parser.Run(text);

            _logger.Info("99C82BE8-D78E-4151-A37E-F3B09675A454", $"result.Count = {result.Count}");

            foreach (var item in result)
            {
                _logger.Info("42E21121-A144-4EC9-8733-D3129AD83AE5", $"item = '{item.ToDbgString()}'");

                compactizer.Run(item);

                _logger.Info("3782D2E3-D134-427F-8E7A-C4B660E08B98", $"item (after) = '{item.ToDbgString()}'");

                var plainSentencesList = converterToPlainSentences.Run(item);

                _logger.Info("AEAB926D-6611-45FF-ADBD-FFB1FC8EC3EC", $"plainSentencesList.Count = {plainSentencesList.Count}");

                foreach(var plainSentence in plainSentencesList)
                {
                    _logger.Info("E647B6DC-716C-4CE2-AF25-5D0B928794B2", $"plainSentence = '{plainSentence.ToDbgString()}'");

                    var conceptualGraph = semanticAnalyzer.Run(plainSentence);

                    var conceptualGraphDbgStr = DotConverter.ConvertToString(conceptualGraph);

                    _logger.Info("8DB597E7-5D67-4A13-88E5-AE3AC3ED63D2", $"conceptualGraphDbgStr = '{conceptualGraphDbgStr}'");


                    var internalCG = convertorCGToInternal.Convert(conceptualGraph);

                    _logger.Info("73134F94-5A93-4524-8763-177126C87ABC", $"internalCG = {internalCG}");

                    var dotStr = DotConverter.ConvertToString(internalCG);

                    _logger.Info("699723E9-C932-44CA-811D-80574C7817F8", $"dotStr = '{dotStr}'");

                    var ruleInstancesList = converterInternalCGToFact.ConvertConceptualGraph(internalCG);

                    _logger.Info("1FCBD61F-58EC-4C1E-9324-64C4A0A61271", $"ruleInstancesList.Count = {ruleInstancesList.Count}");

                    foreach (var ruleInstance in ruleInstancesList)
                    {
                        _logger.Info("0BE537BD-E79A-420D-A551-AC81C98C00C5", $"ruleInstance.KindOfRuleInstance = {ruleInstance.KindOfRuleInstance}");
                        _logger.Info("28C0199A-DCBD-408F-9EB6-7AAC663DDA2B", $"ruleInstance = {ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
                    }
                }
            }
        }

        private void Case1()
        {
            var result = _converter.Convert(_logger, "I like my cat.()!.?,...:;-1234567890 M1$nrg, #erty3, @maror, 3% can't");

            _logger.Info("AC298F94-50AE-49C1-B91C-FB5F91840733", $"result.Count = {result.Count}");

            foreach(var item in result)
            {
                _logger.Info("7F347CCD-416F-49EC-ABB7-2AF675B6A816", $"item = {item.ToHumanizedString()}");
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

                _logger.Info("F519D2BD-D974-4406-A7B7-F5CF1C8FA0E8", $"n = {n}");
                _logger.Info("189E3FC4-F452-4EBB-9456-4866FFF58DA0", $"item = {item}");
            }
        }
    }
}

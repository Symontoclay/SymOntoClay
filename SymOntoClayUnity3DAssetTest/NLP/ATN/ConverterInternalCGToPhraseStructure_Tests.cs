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

using NUnit.Framework;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Tests.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ConvertingFactToInternalCG;
using SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure;
using SymOntoClayBaseTestLib;
using SymOntoClayBaseTestLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.NLP.ATN
{
    public class ConverterInternalCGToPhraseStructure_Tests
    {
        [SetUp]
        public void Setup()
        {
            _logger = new EmptyLogger();
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseStandardLibrary = true;
            _testEngineContext = UnityTestEngineContextFactory.CreateAndInitTestEngineContext(factorySettings);
            _engineContext = _testEngineContext.EngineContext;
            _wordsDict = DictionaryInstance.Instance;
        }

        [TearDown]
        public void Cleanup()
        {
            _testEngineContext.Dispose();
        }

        private IEntityLogger _logger;
        private ComplexTestEngineContext _testEngineContext;
        private IEngineContext _engineContext;
        private IWordsDict _wordsDict;

        private RuleInstance Parse(string text)
        {
            return _engineContext.Parser.ParseRuleInstance(text);
        }

        [Test]
        [Parallelizable]
        public void Case1()
        {
            var factStr = "{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}";

            var nlpContext = _engineContext.GetNLPConverterContext();

            var ruleInstance = Parse(factStr);

            var converterFactToCG = new ConverterFactToInternalCG(_logger);

            var internalCG = converterFactToCG.Convert(ruleInstance, nlpContext);

            var converterInternalCGToPhraseStructure = new ConverterInternalCGToPhraseStructure(_logger, _wordsDict);

            var sentenceItem = converterInternalCGToPhraseStructure.Convert(internalCG, nlpContext);

            Assert.AreEqual(@"S
    Subject:
        NP
            N:
                i
    Predicate:
        VP
            V:
                like
            Object:
                NP
                    N:
                        cat
                    D:
                        my", sentenceItem.ToDbgString().Trim());
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :}";

            var nlpContext = _engineContext.GetNLPConverterContext();

            var ruleInstance = Parse(factStr);

            var converterFactToCG = new ConverterFactToInternalCG(_logger);

            var internalCG = converterFactToCG.Convert(ruleInstance, nlpContext);

            var converterInternalCGToPhraseStructure = new ConverterInternalCGToPhraseStructure(_logger, _wordsDict);

            var sentenceItem = converterInternalCGToPhraseStructure.Convert(internalCG, nlpContext);

            Assert.AreEqual(@"S
    Predicate:
        VP
            V:
                go
            PP:
                PP
                    :P
                        to
                    :NP
                        NP
                            N:
                                place
                            AP:
                                green", sentenceItem.ToDbgString().Trim());
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 0 :}";

            var nlpContext = _engineContext.GetNLPConverterContext();

            var ruleInstance = Parse(factStr);

            var converterFactToCG = new ConverterFactToInternalCG(_logger);

            var internalCG = converterFactToCG.Convert(ruleInstance, nlpContext);

            var converterInternalCGToPhraseStructure = new ConverterInternalCGToPhraseStructure(_logger, _wordsDict);

            var sentenceItem = converterInternalCGToPhraseStructure.Convert(internalCG, nlpContext);

            Assert.AreEqual(@"S
    Subject:
        NP
            N:
                someone
    Predicate:
        VP
            V:
                goes
            Object:
                NP
                    N:
                        self
            PP:
                PP
                    :P
                        to
                    :NP
                        NP
                            N:
                                place
                            AP:
                                green", sentenceItem.ToDbgString().Trim());
        }
    }
}

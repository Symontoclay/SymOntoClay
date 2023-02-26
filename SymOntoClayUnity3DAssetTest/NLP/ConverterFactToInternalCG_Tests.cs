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

using NUnit.Framework;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Tests.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ConvertingFactToInternalCG;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.PhraseStructure;
using SymOntoClay.UnityAsset.Core.Tests.NLP.ATN;
using SymOntoClayBaseTestLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.NLP
{
    public class ConverterFactToInternalCG_Tests
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
        }

        [TearDown]
        public void Cleanup()
        {
            _testEngineContext.Dispose();
        }

        private IEntityLogger _logger;
        private ComplexTestEngineContext _testEngineContext;
        private IEngineContext _engineContext;

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

            Assert.AreEqual(false, internalCG.IsNegation);
            Assert.AreEqual(KindOfQuestion.None, internalCG.KindOfQuestion);
            Assert.AreEqual(GrammaticalTenses.Present, internalCG.Tense);
            Assert.AreEqual(GrammaticalAspect.Simple, internalCG.Aspect);
            Assert.AreEqual(GrammaticalVoice.Active, internalCG.Voice);
            Assert.AreEqual(GrammaticalMood.Indicative, internalCG.Mood);
            Assert.AreEqual(AbilityModality.None, internalCG.AbilityModality);
            Assert.AreEqual(PermissionModality.None, internalCG.PermissionModality);
            Assert.AreEqual(ObligationModality.None, internalCG.ObligationModality);
            Assert.AreEqual(ProbabilityModality.None, internalCG.ProbabilityModality);
            Assert.AreEqual(ConditionalModality.None, internalCG.ConditionalModality);

            var dotStr = DotConverter.ConvertToString(internalCG);

            Assert.AreEqual(@"digraph cluster_1{
compound=true;
n_1[shape=box,label=""like""];
n_2[shape=box,label=""i""];
n_3[shape=ellipse,label=""experiencer""];
n_4[shape=ellipse,label=""state""];
n_5[shape=ellipse,label=""object""];
subgraph cluster_2{
n_7[shape=box,label=""i""];
n_8[shape=box,label=""cat""];
n_6[shape=ellipse,label=""possess""];
n_6 -> n_8;
n_7 -> n_6;
}

n_1 -> n_3;
n_1 -> n_5;
n_2 -> n_4;
n_3 -> n_2;
n_4 -> n_1;
n_5 -> n_6[lhead=cluster_2];
}".DeepTrim(), dotStr.DeepTrim());
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

            Assert.AreEqual(false, internalCG.IsNegation);
            Assert.AreEqual(KindOfQuestion.None, internalCG.KindOfQuestion);
            Assert.AreEqual(GrammaticalTenses.Present, internalCG.Tense);
            Assert.AreEqual(GrammaticalAspect.Simple, internalCG.Aspect);
            Assert.AreEqual(GrammaticalVoice.Active, internalCG.Voice);
            Assert.AreEqual(GrammaticalMood.Imperative, internalCG.Mood);
            Assert.AreEqual(AbilityModality.None, internalCG.AbilityModality);
            Assert.AreEqual(PermissionModality.None, internalCG.PermissionModality);
            Assert.AreEqual(ObligationModality.Imperative, internalCG.ObligationModality);
            Assert.AreEqual(ProbabilityModality.None, internalCG.ProbabilityModality);
            Assert.AreEqual(ConditionalModality.None, internalCG.ConditionalModality);

            var dotStr = DotConverter.ConvertToString(internalCG);

            Assert.AreEqual(@"digraph cluster_1{
compound=true;
n_2[shape=box,label=""go""];
n_3[shape=box,label=""someone""];
n_6[shape=box,label=""self""];
n_1[shape=ellipse,label=""direction""];
n_4[shape=ellipse,label=""agent""];
n_5[shape=ellipse,label=""action""];
n_7[shape=ellipse,label=""object""];
subgraph cluster_2{
n_9[shape=box,label=""place""];
n_10[shape=box,label=""green""];
n_8[shape=ellipse,label=""color""];
n_8 -> n_10;
n_9 -> n_8;
}

n_1 -> n_8[lhead=cluster_2];
n_2 -> n_1;
n_2 -> n_4;
n_2 -> n_7;
n_3 -> n_5;
n_4 -> n_3;
n_5 -> n_2;
n_7 -> n_6;
}".DeepTrim(), dotStr.DeepTrim());
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

            Assert.AreEqual(false, internalCG.IsNegation);
            Assert.AreEqual(KindOfQuestion.None, internalCG.KindOfQuestion);
            Assert.AreEqual(GrammaticalTenses.Present, internalCG.Tense);
            Assert.AreEqual(GrammaticalAspect.Simple, internalCG.Aspect);
            Assert.AreEqual(GrammaticalVoice.Active, internalCG.Voice);
            Assert.AreEqual(GrammaticalMood.Indicative, internalCG.Mood);
            Assert.AreEqual(AbilityModality.None, internalCG.AbilityModality);
            Assert.AreEqual(PermissionModality.None, internalCG.PermissionModality);
            Assert.AreEqual(ObligationModality.None, internalCG.ObligationModality);
            Assert.AreEqual(ProbabilityModality.None, internalCG.ProbabilityModality);
            Assert.AreEqual(ConditionalModality.None, internalCG.ConditionalModality);

            var dotStr = DotConverter.ConvertToString(internalCG);

            Assert.AreEqual(@"digraph cluster_1{
compound=true;
n_2[shape=box,label=""go""];
n_3[shape=box,label=""someone""];
n_6[shape=box,label=""self""];
n_1[shape=ellipse,label=""direction""];
n_4[shape=ellipse,label=""agent""];
n_5[shape=ellipse,label=""action""];
n_7[shape=ellipse,label=""object""];
subgraph cluster_2{
n_9[shape=box,label=""place""];
n_10[shape=box,label=""green""];
n_8[shape=ellipse,label=""color""];
n_8 -> n_10;
n_9 -> n_8;
}

n_1 -> n_8[lhead=cluster_2];
n_2 -> n_1;
n_2 -> n_4;
n_2 -> n_7;
n_3 -> n_5;
n_4 -> n_3;
n_5 -> n_2;
n_7 -> n_6;
}".DeepTrim(), dotStr.DeepTrim());
        }
    }
}

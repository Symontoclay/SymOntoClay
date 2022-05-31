using NUnit.Framework;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ConvertingFactToInternalCG;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.PhraseStructure;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using SymOntoClay.UnityAsset.Core.Tests.NLP.ATN;
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
            _testEngineContext = UnityTestEngineContextFactory.CreateAndInitTestEngineContext();
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

            var nlpContext = UnityTestEngineContextFactory.CreateNLPConverterContext(_engineContext);

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
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } :}";

            var nlpContext = UnityTestEngineContextFactory.CreateNLPConverterContext(_engineContext);

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
            Assert.AreEqual(ObligationModality.None, internalCG.ObligationModality);
            Assert.AreEqual(ProbabilityModality.None, internalCG.ProbabilityModality);
            Assert.AreEqual(ConditionalModality.None, internalCG.ConditionalModality);

            var dotStr = DotConverter.ConvertToString(internalCG);

            Assert.AreEqual(@"digraph cluster_1{
compound=true;
n_2[shape=box,label=""go""];
n_1[shape=ellipse,label=""direction""];
subgraph cluster_2{
n_4[shape=box,label=""place""];
n_5[shape=box,label=""green""];
n_3[shape=ellipse,label=""color""];
n_3 -> n_5;
n_4 -> n_3;
}

n_1 -> n_3[lhead=cluster_2];
n_2 -> n_1;
}".DeepTrim(), dotStr.DeepTrim());
        }
    }
}

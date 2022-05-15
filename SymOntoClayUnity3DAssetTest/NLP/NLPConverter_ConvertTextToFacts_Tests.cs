using NUnit.Framework;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using SymOntoClay.UnityAsset.Core.Tests.NLP.ATN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.NLP
{
    public class NLPConverter_ConvertTextToFacts_Tests
    {
        [SetUp]
        public void Setup()
        {
            _logger = new EmptyLogger();
            _wordsDict = DictionaryInstance.Instance;
        }

        private IEntityLogger _logger;
        private IWordsDict _wordsDict;

        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = "I like my cat.";

            var converter = new NLPConverter(_logger, _wordsDict);

            var ruleInstancesList = converter.Convert(text);

            Assert.AreEqual(1, ruleInstancesList.Count);

            var ruleInstance = ruleInstancesList[0];

            Assert.AreEqual(KindOfRuleInstance.Fact, ruleInstance.KindOfRuleInstance);

            Assert.AreEqual(@"{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }
    }
}

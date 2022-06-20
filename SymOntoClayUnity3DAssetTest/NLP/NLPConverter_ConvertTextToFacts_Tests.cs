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

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = "Go to green place!";

            var converter = new NLPConverter(_logger, _wordsDict);

            var ruleInstancesList = converter.Convert(text);

            Assert.AreEqual(1, ruleInstancesList.Count);

            var ruleInstance = ruleInstancesList[0];

            Assert.AreEqual(KindOfRuleInstance.Fact, ruleInstance.KindOfRuleInstance);

            Assert.AreEqual(@"{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }
    }
}

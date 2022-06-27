using NUnit.Framework;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.Parsing
{
    public class LogicQueriesParsing_Tests
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
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 0.5 :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 0.5 :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            var factStr = "{: parent(#Piter, #Tom) o: 1 :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { parent(#piter,#tom) } o: 1 :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } so: 1 :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } so: 1 :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } so: 0.5 :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } so: 0.5 :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 so: 0.5 :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 so: 0.5 :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } so: 0.5 o: 1  :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 so: 0.5 :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var factStr = "{: parent(#Piter, #Tom) o: middle :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { parent(#piter,#tom) } o: middle :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
            var factStr = "{: parent(#Piter, #Tom) o: very middle :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { parent(#piter,#tom) } o: very middle :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var factStr = "{: parent(#Piter, #Tom) o: middle so: 0.5 :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { parent(#piter,#tom) } o: middle so: 0.5 :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void Case5_a()
        {
            var factStr = "{: parent(#Piter, #Tom) o: very middle so: 0.5 :}";

            var ruleInstance = Parse(factStr);

            Assert.AreEqual("{: >: { parent(#piter,#tom) } o: very middle so: 0.5 :}", ruleInstance.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }
    }
}

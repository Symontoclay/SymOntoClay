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

using NUnit.Framework;
using SymOntoClay.BaseTestLib;
using SymOntoClay.BaseTestLib.Monitoring;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
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
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            _testEngineContext = UnityTestEngineContextFactory.CreateAndInitTestEngineContext(factorySettings);
            _engineContext = _testEngineContext.EngineContext;
        }

        [TearDown]
        public void Cleanup()
        {
            _testEngineContext.Dispose();
        }

        private IMonitorLogger _logger;
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

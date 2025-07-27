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
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Helpers;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class RuleInstanceHelper_Tests
    {
        [SetUp]
        public void Setup()
        {
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

        private ComplexTestEngineContext _testEngineContext;
        private IEngineContext _engineContext;

        [Test]
        [Parallelizable]
        public void Case1()
        {
            var factStr = "{: >: { $y = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNames(factStr);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(1, varNamesList.Count);
            Assert.AreEqual("$y", varNamesList[0]);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var factStr = "{: >: { $y = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var baseVarName = "$x";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, factStr);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(0, varNamesList.Count);
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            var factStr = "{: >: { $y = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var baseVarName = "$x";
            var targetVarName = RuleInstanceHelper.GetNewUniqueVarNameWithPrefix(baseVarName, factStr);
            Assert.AreEqual("$x", targetVarName);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNames(factStr);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(2, varNamesList.Count);
            Assert.AreEqual("$x", varNamesList[0]);
            Assert.AreEqual("$y", varNamesList[1]);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var baseVarName = "$x";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, factStr);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(1, varNamesList.Count);
            Assert.AreEqual("$x", varNamesList[0]);
        }

        [Test]
        [Parallelizable]
        public void Case2_b()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var baseVarName = "$x";
            var targetVarName = RuleInstanceHelper.GetNewUniqueVarNameWithPrefix(baseVarName, factStr);
            Assert.AreEqual("$x1", targetVarName);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNames(factStr);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(3, varNamesList.Count);
            Assert.AreEqual("$x", varNamesList[0]);
            Assert.AreEqual("$x1", varNamesList[1]);
            Assert.AreEqual("$y", varNamesList[2]);
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var baseVarName = "$x";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, factStr);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(2, varNamesList.Count);
            Assert.AreEqual("$x", varNamesList[0]);
            Assert.AreEqual("$x1", varNamesList[1]);
        }

        [Test]
        [Parallelizable]
        public void Case3_b()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var baseVarName = "$x";
            var targetVarName = RuleInstanceHelper.GetNewUniqueVarNameWithPrefix(baseVarName, factStr);
            Assert.AreEqual("$x2", targetVarName);
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var factStr = "{: >: { $y = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            var varNamesList = RuleInstanceHelper.GetUniqueVarNames(fact);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(1, varNamesList.Count);
            Assert.AreEqual("$`y`", varNamesList[0]);
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
            var factStr = "{: >: { $y = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            var baseVarName = "$x";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, fact);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(0, varNamesList.Count);
        }

        [Test]
        [Parallelizable]
        public void Case4_b()
        {
            var factStr = "{: >: { $y = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            var baseVarName = "$x";
            var targetVarName = RuleInstanceHelper.GetNewUniqueVarNameWithPrefix(baseVarName, fact);
            Assert.AreEqual("$x", targetVarName);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            var varNamesList = RuleInstanceHelper.GetUniqueVarNames(fact);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(2, varNamesList.Count);
            Assert.AreEqual("$`x`", varNamesList[0]);
            Assert.AreEqual("$`y`", varNamesList[1]);
        }

        [Test]
        [Parallelizable]
        public void Case5_a()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            var baseVarName = "$x";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, fact);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(1, varNamesList.Count);
            Assert.AreEqual("$`x`", varNamesList[0]);
        }

        [Test]
        [Parallelizable]
        public void Case5_b()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            var baseVarName = "$x";
            var targetVarName = RuleInstanceHelper.GetNewUniqueVarNameWithPrefix(baseVarName, fact);
            Assert.AreEqual("$x1", targetVarName);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            var varNamesList = RuleInstanceHelper.GetUniqueVarNames(fact);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(3, varNamesList.Count);
            Assert.AreEqual("$x", varNamesList[0]);
            Assert.AreEqual("$x1", varNamesList[1]);
            Assert.AreEqual("$y", varNamesList[2]);
        }

        [Test]
        [Parallelizable]
        public void Case6_a()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            var baseVarName = "$x";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, fact);
            Assert.IsNotNull(varNamesList);
            Assert.AreEqual(2, varNamesList.Count);
            Assert.AreEqual("$x", varNamesList[0]);
            Assert.AreEqual("$x1", varNamesList[1]);
        }

        [Test]
        [Parallelizable]
        public void Case6_b()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";
            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            var baseVarName = "$x";
            var targetVarName = RuleInstanceHelper.GetNewUniqueVarNameWithPrefix(baseVarName, fact);
            Assert.AreEqual("$x2", targetVarName);
        }
    }
}

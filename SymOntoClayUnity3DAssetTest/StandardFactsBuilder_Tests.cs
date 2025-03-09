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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.StandardFacts;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class StandardFactsBuilder_Tests
    {
        [SetUp]
        public void Setup()
        {
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            _testEngineContext = UnityTestEngineContextFactory.CreateAndInitTestEngineContext(factorySettings);
            _engineContext = _testEngineContext.EngineContext;

            _standardFactsBuilder = new StandardFactsBuilder();
        }

        [TearDown]
        public void Cleanup()
        {
            _testEngineContext.Dispose();
        }

        private ComplexTestEngineContext _testEngineContext;
        private IEngineContext _engineContext;
        private IStandardFactsBuilder _standardFactsBuilder;

        [Test]
        [Parallelizable]
        public void BuildPropertyVirtualRelationInstance()
        {
            var relation = _standardFactsBuilder.BuildPropertyVirtualRelationInstance(NameHelper.CreateName("someprop"), NameHelper.CreateName("#123"), new NumberValue(16));

            Assert.AreEqual("someprop(#123,16)", relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildImplicitPropertyQueryInstance()
        {
            var fact = _standardFactsBuilder.BuildImplicitPropertyQueryInstance(NameHelper.CreateName("someprop"), NameHelper.CreateName("#123"));

            Assert.AreEqual("{: >: { someprop(#123,$_) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildSayFactString()
        {
            var factStr = _standardFactsBuilder.BuildSayFactString("#123", "{: act(M16, shoot) :}");

            Assert.AreEqual("{: say(#123, {: act(M16, shoot) :}) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildSayFactInstance()
        {
            var initialFactStr = "{: act(M16, shoot) :}";

            var initialFact = _engineContext.Parser.ParseRuleInstance(initialFactStr);

            var fact = _standardFactsBuilder.BuildSayFactInstance("#123", initialFact);

            Assert.AreEqual("{: >: { say(#123,{: >: { act(m16,shoot) } :}) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildSoundFactString()
        {
            var factStr = _standardFactsBuilder.BuildSoundFactString(15.588457107543945, 12, "{: act(M16, shoot) :}");

            Assert.AreEqual("{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildSoundFactInstance()
        {
            var initialFactStr = "{: act(M16, shoot) :}";

            var initialFact = _engineContext.Parser.ParseRuleInstance(initialFactStr);

            var fact = _standardFactsBuilder.BuildSoundFactInstance(15.588457107543945, 12, initialFact);

            Assert.AreEqual("{: >: { $x = {: >: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildAliveFactString()
        {
            var factStr = _standardFactsBuilder.BuildAliveFactString("#123");

            Assert.AreEqual("{: state(#123, alive) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildAliveFactInstance()
        {
            var fact = _standardFactsBuilder.BuildAliveFactInstance("#123");

            Assert.AreEqual("{: >: { state(#123,alive) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildDeadFactString()
        {
            var factStr = _standardFactsBuilder.BuildDeadFactString("#123");

            Assert.AreEqual("{: state(#123, dead) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildDeadFactInstance()
        {
            var fact = _standardFactsBuilder.BuildDeadFactInstance("#123");

            Assert.AreEqual("{: >: { state(#123,dead) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildStopFactString()
        {
            var factStr = _standardFactsBuilder.BuildStopFactString("#123");

            Assert.AreEqual("{: act(#123, stop) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildStopFactInstance()
        {
            var fact = _standardFactsBuilder.BuildStopFactInstance("#123");

            Assert.AreEqual("{: >: { act(#123,stop) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildWalkFactString()
        {
            var factStr = _standardFactsBuilder.BuildWalkFactString("#123");

            Assert.AreEqual("{: act(#123, walk) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildWalkFactInstance()
        {
            var fact = _standardFactsBuilder.BuildWalkFactInstance("#123");

            Assert.AreEqual("{: >: { act(#123,walk) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildWalkSoundFactString()
        {
            var factStr = _standardFactsBuilder.BuildWalkSoundFactString();

            Assert.AreEqual("{: act(someone, walk) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildWalkSoundFactInstance()
        {
            var fact = _standardFactsBuilder.BuildWalkSoundFactInstance();

            Assert.AreEqual("{: >: { act(someone,walk) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildRunFactString()
        {
            var factStr = _standardFactsBuilder.BuildRunFactString("#123");

            Assert.AreEqual("{: act(#123, run) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildRunFactInstance()
        {
            var fact = _standardFactsBuilder.BuildRunFactInstance("#123");

            Assert.AreEqual("{: >: { act(#123,run) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildRunSoundFactString()
        {
            var factStr = _standardFactsBuilder.BuildRunSoundFactString();

            Assert.AreEqual("{: act(someone, run) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildRunSoundFactInstance()
        {
            var fact = _standardFactsBuilder.BuildRunSoundFactInstance();

            Assert.AreEqual("{: >: { act(someone,run) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildHoldFactString()
        {
            var factStr = _standardFactsBuilder.BuildHoldFactString("#123", "#456");

            Assert.AreEqual("{: hold(#123, #456) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildHoldFactInstance()
        {
            var fact = _standardFactsBuilder.BuildHoldFactInstance("#123", "#456");

            Assert.AreEqual("{: >: { hold(#123,#456) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildShootFactString()
        {
            var factStr = _standardFactsBuilder.BuildShootFactString("#123");

            Assert.AreEqual("{: act(#123, shoot) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildShootFactInstance()
        {
            var fact = _standardFactsBuilder.BuildShootFactInstance("#123");

            Assert.AreEqual("{: >: { act(#123,shoot) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildShootSoundFactString()
        {
            var factStr = _standardFactsBuilder.BuildShootSoundFactString();

            Assert.AreEqual("{: act(someone, shoot) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildShootSoundFactInstance()
        {
            var fact = _standardFactsBuilder.BuildShootSoundFactInstance();

            Assert.AreEqual("{: >: { act(someone,shoot) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildReadyForShootFactString()
        {
            var factStr = _standardFactsBuilder.BuildReadyForShootFactString("#123");

            Assert.AreEqual("{: ready(#123, shoot) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildReadyForShootFactInstance()
        {
            var fact = _standardFactsBuilder.BuildReadyForShootFactInstance("#123");

            Assert.AreEqual("{: >: { ready(#123,shoot) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildSeeFactString()
        {
            var factStr = _standardFactsBuilder.BuildSeeFactString("#123");

            Assert.AreEqual("{: see(I, #123) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildSeeFactInstance()
        {
            var fact = _standardFactsBuilder.BuildSeeFactInstance("#123");

            Assert.AreEqual("{: >: { see(i,#123) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildFocusFactString()
        {
            var factStr = _standardFactsBuilder.BuildFocusFactString("#123");

            Assert.AreEqual("{: focus(I, #123) :}", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildFocusFactInstance()
        {
            var fact = _standardFactsBuilder.BuildFocusFactInstance("#123");

            Assert.AreEqual("{: >: { focus(i,#123) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildDistanceFactString_Case1()
        {
            var factStr = _standardFactsBuilder.BuildDistanceFactString("#123", 12);

            Assert.AreEqual("distance(I, #123, 12)", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildDistanceFactInstance_Case1()
        {
            var fact = _standardFactsBuilder.BuildDistanceFactInstance("#123", 12);

            Assert.AreEqual("{: >: { distance(i,#123,12) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }

        [Test]
        [Parallelizable]
        public void BuildDistanceFactString_Case2()
        {
            var factStr = _standardFactsBuilder.BuildDistanceFactString("#123", 12.6);

            Assert.AreEqual("distance(I, #123, 12.6)", factStr);
        }

        [Test]
        [Parallelizable]
        public void BuildDistanceFactInstance_Case2()
        {
            var fact = _standardFactsBuilder.BuildDistanceFactInstance("#123", 12.6);

            Assert.AreEqual("{: >: { distance(i,#123,12.6) } :}", fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent));
        }
    }
}

using NUnit.Framework;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class NameHelper_Tests
    {
        [SetUp]
        public void Setup()
        {
            _parserContext = new TstParserContext();
        }

        private IParserContext _parserContext;

        [Test]
        public void NameHelper_Tests_Case_Empty()
        {
            var name = new Name();

            Assert.AreEqual(name.IsEmpty, true);
            Assert.AreEqual(name.NameValue, string.Empty);
            Assert.AreEqual(name.Kind, KindOfName.Unknown);
            Assert.AreEqual(name.NameKey, 0);
        }

        [Test]
        public void NameHelper_Tests_Case_Concept()
        {
            var text = "dog";

            var name = NameHelper.CreateName(text, _parserContext.Dictionary);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, text);
            Assert.AreEqual(name.Kind, KindOfName.Concept);
            Assert.AreNotEqual(name.NameKey, 0);

            var key = _parserContext.Dictionary.GetKey(text);

            Assert.AreEqual(name.NameKey, key);
        }

        [Test]
        public void NameHelper_Tests_Case_Channel()
        {
            var text = "@>log";

            var name = NameHelper.CreateName(text, _parserContext.Dictionary);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, text);
            Assert.AreEqual(name.Kind, KindOfName.Channel);
            Assert.AreNotEqual(name.NameKey, 0);

            var key = _parserContext.Dictionary.GetKey(text);

            Assert.AreEqual(name.NameKey, key);
        }

        [Test]
        public void NameHelper_Tests_Case_CreateRuleOrFactName()
        {
            var name = NameHelper.CreateRuleOrFactName(_parserContext.Dictionary);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreNotEqual(name.NameValue, string.Empty);
            Assert.AreEqual(name.Kind, KindOfName.Entity);
            Assert.AreNotEqual(name.NameKey, 0);
        }

        [Test]
        public void NameHelper_Tests_Case_Entity()
        {
            var text = "#dog1";

            var name = NameHelper.CreateName(text, _parserContext.Dictionary);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, text);
            Assert.AreEqual(name.Kind, KindOfName.Entity);
            Assert.AreNotEqual(name.NameKey, 0);

            var key = _parserContext.Dictionary.GetKey(text);

            Assert.AreEqual(name.NameKey, key);
        }
    }
}

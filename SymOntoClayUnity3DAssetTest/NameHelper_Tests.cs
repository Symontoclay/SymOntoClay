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

            throw new NotImplementedException();
        }

        [Test]
        public void NameHelper_Tests_Case_Channel()
        {
            var text = "@>log";

            throw new NotImplementedException();
        }

        [Test]
        public void NameHelper_Tests_Case_CreateRuleOrFactName()
        {
            var name = NameHelper.CreateRuleOrFactName(_parserContext.Dictionary);


            throw new NotImplementedException();
        }
    }
}

using NUnit.Framework;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class Parser_Tests
    {
        [SetUp]
        public void Setup()
        {
            _parserContext = new TstParserContext();
        }

        private IParserContext _parserContext;

        [Test]
        public void Parser_Tests_Case1()
        {
            var text = @"app Enemy
{
}";

            var internalParserContext = new InternalParserContext(text, _parserContext);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var result = parser.Result;

            Assert.AreEqual(result.Count, 1);

            var firstItem = result.Single();

            var nameKey = _parserContext.Dictionary.GetKey("Enemy");

            Assert.AreNotEqual(nameKey, 0);

            Assert.AreEqual(firstItem.Kind, KindOfCodeEntity.App);
            Assert.AreEqual(firstItem.Name.NameKey, nameKey);
            Assert.AreEqual(firstItem.Name.NameValue, "Enemy");
            Assert.AreEqual(firstItem.Name.DictionaryName, _parserContext.Dictionary.Name);
            Assert.AreNotEqual(firstItem.Name.DictionaryName, string.Empty);
        }
    }
}

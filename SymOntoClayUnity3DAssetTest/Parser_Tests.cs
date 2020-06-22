﻿using NUnit.Framework;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
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
            Assert.AreEqual(firstItem.Name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(firstItem.SubItems.Count, 0);
        }

        [Test]
        public void Parser_Tests_Case2()
        {
            var text = @"app PixKeeper
{
    on Init => {
	     'Hello world!' >> @>log;
    }
}";

            var internalParserContext = new InternalParserContext(text, _parserContext);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var result = parser.Result;

            Assert.AreEqual(result.Count, 1);

            var firstItem = result.Single();

            var nameKey = _parserContext.Dictionary.GetKey("PixKeeper");

            Assert.AreNotEqual(nameKey, 0);

            Assert.AreEqual(firstItem.Kind, KindOfCodeEntity.App);
            Assert.AreEqual(firstItem.Name.NameKey, nameKey);
            Assert.AreEqual(firstItem.Name.NameValue, "PixKeeper");
            Assert.AreEqual(firstItem.Name.DictionaryName, _parserContext.Dictionary.Name);
            Assert.AreNotEqual(firstItem.Name.DictionaryName, string.Empty);
            Assert.AreEqual(firstItem.Name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(firstItem.SubItems.Count, 1);

            var subItem = firstItem.SubItems.Single();

            Assert.AreEqual(subItem.Kind, KindOfCodeEntity.InlineTrigger);

            var inlineTrigger = subItem.InlineTrigger;

            Assert.AreNotEqual(inlineTrigger, null);
            Assert.AreEqual(inlineTrigger.Kind, KindOfInlineTrigger.SystemEvents);
            Assert.AreEqual(inlineTrigger.KindOfSystemEvent, KindOfSystemEventOfInlineTrigger.Init);

            Assert.AreEqual(inlineTrigger.Statements.Count, 1);

            var statement = (AstExpressionStatement)inlineTrigger.Statements.Single();

            Assert.AreNotEqual(statement.Expression, null);

            var binOpExpr = (BinaryOperatorAstExpression)statement.Expression;

            Assert.AreEqual(binOpExpr.KindOfOperator, KindOfOperator.LeftRightStream);

            var leftNode = (ConstValueAstExpression)binOpExpr.Left;

            var strVal = leftNode.Value.AsStringValue;

            Assert.AreEqual(strVal.SystemValue, "Hello world!");

            var rightNode = (ChannelAstExpression)binOpExpr.Right;

            Assert.AreEqual(rightNode.Name.NameValue, "@>log");
            Assert.AreEqual(rightNode.Name.KindOfName, KindOfName.Channel);
        }
    }
}

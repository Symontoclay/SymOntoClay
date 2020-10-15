/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using NUnit.Framework;
using SymOntoClay.Core.Internal;
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
            _mainStorageContext = new TstMainStorageContext();
        }

        private IMainStorageContext _mainStorageContext;

        [Test]
        public void Parser_Tests_Case1()
        {
            var text = @"app Enemy
{
}";

            var codeFile = new CodeFile();

            var internalParserContext = new InternalParserContext(text, codeFile, _mainStorageContext);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var result = parser.Result;

            Assert.AreEqual(result.Count, 1);

            var firstItem = result.Single();

            var nameKey = _mainStorageContext.Dictionary.GetKey("Enemy");

            Assert.AreNotEqual(nameKey, 0);

            Assert.AreEqual(firstItem.Kind, KindOfCodeEntity.Npc);
            //Assert.AreEqual(firstItem.Name.NameKey, nameKey);
            Assert.AreEqual(firstItem.Name.NameValue, "Enemy");
            Assert.AreEqual(firstItem.Name.DictionaryName, _mainStorageContext.Dictionary.Name);
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
            var codeFile = new CodeFile();

            var internalParserContext = new InternalParserContext(text, codeFile, _mainStorageContext);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var result = parser.Result;

            Assert.AreEqual(result.Count, 1);

            var firstItem = result.Single();

            var nameKey = _mainStorageContext.Dictionary.GetKey("PixKeeper");

            Assert.AreNotEqual(nameKey, 0);

            Assert.AreEqual(firstItem.Kind, KindOfCodeEntity.Npc);
            //Assert.AreEqual(firstItem.Name.NameKey, nameKey);
            Assert.AreEqual(firstItem.Name.NameValue, "PixKeeper");
            Assert.AreEqual(firstItem.Name.DictionaryName, _mainStorageContext.Dictionary.Name);
            Assert.AreNotEqual(firstItem.Name.DictionaryName, string.Empty);
            Assert.AreEqual(firstItem.Name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(firstItem.SubItems.Count, 1);

            var subItem = firstItem.SubItems.Single();

            Assert.AreEqual(subItem.Kind, KindOfCodeEntity.InlineTrigger);

            var inlineTrigger = subItem.InlineTrigger;

            Assert.AreNotEqual(inlineTrigger, null);
            Assert.AreEqual(inlineTrigger.Kind, KindOfInlineTrigger.SystemEvent);
            Assert.AreEqual(inlineTrigger.KindOfSystemEvent, KindOfSystemEventOfInlineTrigger.Init);

            Assert.AreEqual(inlineTrigger.Statements.Count, 1);

            var statement = (AstExpressionStatement)inlineTrigger.Statements.Single();

            Assert.AreNotEqual(statement.Expression, null);

            var binOpExpr = (BinaryOperatorAstExpression)statement.Expression;

            Assert.AreEqual(binOpExpr.KindOfOperator, KindOfOperator.LeftRightStream);

            var leftNode = (ConstValueAstExpression)binOpExpr.Left;

            var strVal = leftNode.Value.AsStringValue;

            Assert.AreEqual(strVal.SystemValue, "Hello world!");

            var rightNode = (ConstValueAstExpression)binOpExpr.Right;

            Assert.AreEqual(rightNode.Value.AsStrongIdentifierValue.NameValue, "@>log");
            Assert.AreEqual(rightNode.Value.AsStrongIdentifierValue.KindOfName, KindOfName.Channel);
        }
    }
}

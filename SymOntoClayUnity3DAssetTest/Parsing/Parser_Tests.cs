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
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests.Parsing
{
    public class Parser_Tests
    {
        [Test]
        [Parallelizable]
        public void Parser_Tests_Case1()
        {
            var text = @"app Enemy
{
}";

            var firstItem = Parse(text);

            Assert.AreEqual(firstItem.Kind, KindOfCodeEntity.App);
            Assert.AreEqual(firstItem.Name.NameValue, "enemy");
            Assert.AreEqual(firstItem.Name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(firstItem.SubItems.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Parser_Tests_Case2()
        {
            var text = @"app PixKeeper
{
    on Enter => {
	     'Hello world!' >> @>log;
    }
}";

            var firstItem = Parse(text);

            Assert.AreEqual(firstItem.Kind, KindOfCodeEntity.App);
            Assert.AreEqual(firstItem.Name.NameValue, "pixkeeper");
            Assert.AreEqual(firstItem.Name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(firstItem.SubItems.Count, 1);

            var subItem = firstItem.SubItems.Single();

            Assert.AreEqual(subItem.Kind, KindOfCodeEntity.InlineTrigger);

            var inlineTrigger = subItem.AsInlineTrigger;

            Assert.AreNotEqual(inlineTrigger, null);
            Assert.AreEqual(inlineTrigger.KindOfInlineTrigger, KindOfInlineTrigger.SystemEvent);
            Assert.AreEqual(inlineTrigger.KindOfSystemEvent, KindOfSystemEventOfInlineTrigger.Enter);

            Assert.AreEqual(inlineTrigger.SetStatements.Count, 1);

            var statement = (AstExpressionStatement)inlineTrigger.SetStatements.Single();

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

        [SetUp]
        public void Setup()
        {
            _mainStorageContext = new UnityTestMainStorageContext();
        }

        private IMainStorageContext _mainStorageContext;

        private CodeItem Parse(string text)
        {
            var codeFile = new CodeFile();

            var internalParserContext = new InternalParserContext(text, codeFile, _mainStorageContext);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var result = parser.Result;

            Assert.AreEqual(result.Count, 1);

            var firstItem = result.Single();

            return firstItem;
        }
    }
}

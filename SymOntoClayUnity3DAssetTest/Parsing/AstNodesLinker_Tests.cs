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
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.UnityAsset.Core.Tests.ExprNodesHierarchy;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests.Test
{
    public class AstNodesLinker_Tests
    {
        [Test]
        [Parallelizable]
        public void AstNodesLinker_Tests_Case1()
        {
            var nodePoint = new IntermediateAstNodePoint();

            var numNode = new TstNumberAstExpressionNode();
            numNode.Value = 12;

            var intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 5;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var resultExr = (TstBaseAstExpression)nodePoint.BuildExpr();

            var str = resultExr.GetDbgString();

            Assert.AreEqual(str, "12 + 5");

            var calcResult = resultExr.Calc();

            Assert.AreEqual(calcResult, 17);
        }

        [Test]
        [Parallelizable]
        public void AstNodesLinker_Tests_Case2()
        {
            var nodePoint = new IntermediateAstNodePoint();

            var numNode = new TstNumberAstExpressionNode();
            numNode.Value = 12;

            var intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 5;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 3;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var resultExr = (TstBaseAstExpression)nodePoint.BuildExpr();

            var str = resultExr.GetDbgString();

            Assert.AreEqual(str, "12 + 5 + 3");

            var calcResult = resultExr.Calc();

            Assert.AreEqual(calcResult, 20);
        }

        [Test]
        [Parallelizable]
        public void AstNodesLinker_Tests_Case3()
        {
            var nodePoint = new IntermediateAstNodePoint();

            var numNode = new TstNumberAstExpressionNode();
            numNode.Value = 12;

            var intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 5;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Mul;

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 3;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var resultExr = (TstBaseAstExpression)nodePoint.BuildExpr();

            var str = resultExr.GetDbgString();

            Assert.AreEqual(str, "12 + 5 * 3");

            var calcResult = resultExr.Calc();

            Assert.AreEqual(calcResult, 27);
        }

        [Test]
        [Parallelizable]
        public void AstNodesLinker_Tests_Case4()
        {
            var nodePoint = new IntermediateAstNodePoint();

            var numNode = new TstNumberAstExpressionNode();
            numNode.Value = 12;

            var intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 5;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Mul;

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 3;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 2;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var resultExr = (TstBaseAstExpression)nodePoint.BuildExpr();

            var str = resultExr.GetDbgString();

            Assert.AreEqual(str, "12 + 5 * 3 + 2");

            var calcResult = resultExr.Calc();

            Assert.AreEqual(calcResult, 29);
        }

        [Test]
        [Parallelizable]
        public void AstNodesLinker_Tests_Case5()
        {
            var nodePoint = new IntermediateAstNodePoint();

            var numNode = new TstNumberAstExpressionNode();
            numNode.Value = 12;

            var intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var unaryOp = new TstUnaryAstExpression();
            unaryOp.KindOfOperator = TstKindOfOperator.Minus;

            priority = GetPriority(unaryOp.Kind, unaryOp.KindOfOperator);

            intermediateNode = new IntermediateAstNode(unaryOp, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 5;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var resultExr = (TstBaseAstExpression)nodePoint.BuildExpr();

            var str = resultExr.GetDbgString();

            Assert.AreEqual(str, "12 + - 5");

            var calcResult = resultExr.Calc();

            Assert.AreEqual(calcResult, 7);
        }

        [Test]
        [Parallelizable]
        public void AstNodesLinker_Tests_Case6()
        {
            var nodePoint = new IntermediateAstNodePoint();

            var numNode = new TstNumberAstExpressionNode();
            numNode.Value = 12;

            var intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var unaryOp = new TstUnaryAstExpression();
            unaryOp.KindOfOperator = TstKindOfOperator.Minus;

            priority = GetPriority(unaryOp.Kind, unaryOp.KindOfOperator);

            intermediateNode = new IntermediateAstNode(unaryOp, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            unaryOp = new TstUnaryAstExpression();
            unaryOp.KindOfOperator = TstKindOfOperator.Minus;

            priority = GetPriority(unaryOp.Kind, unaryOp.KindOfOperator);

            intermediateNode = new IntermediateAstNode(unaryOp, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 5;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var resultExr = (TstBaseAstExpression)nodePoint.BuildExpr();

            var str = resultExr.GetDbgString();

            Assert.AreEqual(str, "12 + - - 5");

            var calcResult = resultExr.Calc();

            Assert.AreEqual(calcResult, 17);
        }

        [Test]
        [Parallelizable]
        public void AstNodesLinker_Tests_Case7()
        {
            var nodePoint = new IntermediateAstNodePoint();

            var unaryOp = new TstUnaryAstExpression();
            unaryOp.KindOfOperator = TstKindOfOperator.Minus;

            var priority = GetPriority(unaryOp.Kind, unaryOp.KindOfOperator);

            var intermediateNode = new IntermediateAstNode(unaryOp, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var numNode = new TstNumberAstExpressionNode();
            numNode.Value = 12;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);
            var binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 5;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var resultExr = (TstBaseAstExpression)nodePoint.BuildExpr();

            var str = resultExr.GetDbgString();

            Assert.AreEqual(str, "- 12 + 5");

            var calcResult = resultExr.Calc();

            Assert.AreEqual(calcResult, -7);
        }

        [Test]
        [Parallelizable]
        public void AstNodesLinker_Tests_Case8()
        {
            var nodePoint = new IntermediateAstNodePoint();

            var varNode = new TstVarAstExpression();
            varNode.Name = "a";

            var intermediateNode = new IntermediateAstNode(varNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Assign;

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var numNode = new TstNumberAstExpressionNode();
            numNode.Value = 12;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 5;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var resultExr = (TstBaseAstExpression)nodePoint.BuildExpr();

            var str = resultExr.GetDbgString();

            Assert.AreEqual(str, "a = 12 + 5");

            var calcResult = resultExr.Calc();

            Assert.AreEqual(calcResult, 17);

            Assert.AreEqual(varNode.Value, 17);
        }

        [Test]
        [Parallelizable]
        public void AstNodesLinker_Tests_Case9()
        {
            var nodePoint = new IntermediateAstNodePoint();

            var varNode = new TstVarAstExpression();
            varNode.Name = "a";

            var intermediateNode = new IntermediateAstNode(varNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Assign;

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var varNode2 = new TstVarAstExpression();
            varNode2.Name = "b";

            intermediateNode = new IntermediateAstNode(varNode2);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Assign;

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var numNode = new TstNumberAstExpressionNode();
            numNode.Value = 12;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            binOpNode = new TstBinaryAstExpression();
            binOpNode.KindOfOperator = TstKindOfOperator.Plus;

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            numNode = new TstNumberAstExpressionNode();
            numNode.Value = 5;

            intermediateNode = new IntermediateAstNode(numNode);

            AstNodesLinker.SetNode(intermediateNode, nodePoint);

            var resultExr = (TstBaseAstExpression)nodePoint.BuildExpr();

            var str = resultExr.GetDbgString();

            Assert.AreEqual(str, "a = b = 12 + 5");

            var calcResult = resultExr.Calc();

            Assert.AreEqual(calcResult, 17);
        }

        private int UnaryPlusOrMinus = 1;
        private int MulAndDivPriotiry = 2;
        private int AddAndSubPriority = 3;
        private int AssignPriority = 4;

        private int GetPriority(TstKindOfNode kind, TstKindOfOperator kindOfOperator)
        {
            switch (kind)
            {
                case TstKindOfNode.BinaryOperator:
                    switch (kindOfOperator)
                    {
                        case TstKindOfOperator.Plus:
                        case TstKindOfOperator.Minus:
                            return AddAndSubPriority;

                        case TstKindOfOperator.Mul:
                        case TstKindOfOperator.Div:
                            return MulAndDivPriotiry;

                        case TstKindOfOperator.Assign:
                            return AssignPriority;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                    }

                case TstKindOfNode.UnaryOperator:
                    switch (kindOfOperator)
                    {
                        case TstKindOfOperator.Plus:
                        case TstKindOfOperator.Minus:
                            return UnaryPlusOrMinus;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}

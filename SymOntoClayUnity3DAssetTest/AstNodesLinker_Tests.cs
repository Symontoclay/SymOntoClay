using NUnit.Framework;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.Unity3DAsset.Test.ExprNodesHierarchy;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class AstNodesLinker_Tests
    {
        [Test]
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
        public void AstNodesLinker_Tests_Case3()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AstNodesLinker_Tests_Case4()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AstNodesLinker_Tests_Case5()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AstNodesLinker_Tests_Case6()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AstNodesLinker_Tests_Case7()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AstNodesLinker_Tests_Case8()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AstNodesLinker_Tests_Case9()
        {
            throw new NotImplementedException();
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

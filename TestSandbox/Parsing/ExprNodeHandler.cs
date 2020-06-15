using NLog;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.Unity3DAsset.Test.ExprNodesHierarchy;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.Parsing
{
    public class ExprNodeHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int UnaryPlusOrMinus = 1;
        private int MulAndDivPriotiry = 2;
        private int AddAndSubPriority = 3;
        private int AssignPriority = 4;

        private IntermediateAstNodePoint _nodePoint;

        public void Run()
        {
            _logger.Info("Begin");

            Run1();
            Run2();
            Run3();
            Run4();
            Run5();
            Run6();
            Run7();
            Run8();
            Run9();

            _logger.Info("End");
        }

        private void Run1()
        {
            _nodePoint = new IntermediateAstNodePoint();

            _logger.Info($"_nodePoint = {_nodePoint}");

            var numNode = new NumberAstExpressionNode();
            numNode.Value = 12;

            _logger.Info($"numNode = {numNode}");

            var intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 5;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var resultExr = (BaseAstExpression)_nodePoint.BuildExpr();

            _logger.Info($"resultExr = {resultExr}");

            var str = resultExr.GetDbgString();

            _logger.Info($"str = {str}");

            var calcResult = resultExr.Calc();

            _logger.Info($"calcResult = {calcResult}");
        }

        public void Run2()
        {
            _nodePoint = new IntermediateAstNodePoint();

            _logger.Info($"_nodePoint = {_nodePoint}");

            var numNode = new NumberAstExpressionNode();
            numNode.Value = 12;

            _logger.Info($"numNode = {numNode}");

            var intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 5;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 3;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var resultExr = (BaseAstExpression)_nodePoint.BuildExpr();

            _logger.Info($"resultExr = {resultExr}");

            var str = resultExr.GetDbgString();

            _logger.Info($"str = {str}");

            var calcResult = resultExr.Calc();

            _logger.Info($"calcResult = {calcResult}");
        }

        private void Run3()
        {
            _nodePoint = new IntermediateAstNodePoint();

            _logger.Info($"_nodePoint = {_nodePoint}");

            var numNode = new NumberAstExpressionNode();
            numNode.Value = 12;

            _logger.Info($"numNode = {numNode}");

            var intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 5;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Mul;

            _logger.Info($"binOpNode = {binOpNode}");

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 3;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");


            var resultExr = (BaseAstExpression)_nodePoint.BuildExpr();

            _logger.Info($"resultExr = {resultExr}");

            var str = resultExr.GetDbgString();

            _logger.Info($"str = {str}");

            var calcResult = resultExr.Calc();

            _logger.Info($"calcResult = {calcResult}");
        }

        private void Run4()
        {
            _nodePoint = new IntermediateAstNodePoint();

            _logger.Info($"_nodePoint = {_nodePoint}");

            var numNode = new NumberAstExpressionNode();
            numNode.Value = 12;

            _logger.Info($"numNode = {numNode}");

            var intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 5;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Mul;

            _logger.Info($"binOpNode = {binOpNode}");

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 3;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 2;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var resultExr = (BaseAstExpression)_nodePoint.BuildExpr();

            _logger.Info($"resultExr = {resultExr}");

            var str = resultExr.GetDbgString();

            _logger.Info($"str = {str}");

            var calcResult = resultExr.Calc();

            _logger.Info($"calcResult = {calcResult}");
        }

        private void Run5()
        {
            _nodePoint = new IntermediateAstNodePoint();

            _logger.Info($"_nodePoint = {_nodePoint}");

            var numNode = new NumberAstExpressionNode();
            numNode.Value = 12;

            _logger.Info($"numNode = {numNode}");

            var intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var unaryOp = new UnaryAstExpression();
            unaryOp.KindOfOperator = KindOfOperator.Minus;

            priority = GetPriority(unaryOp.Kind, unaryOp.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(unaryOp, KindOfIntermediateAstNode.UnaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 5;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var resultExr = (BaseAstExpression)_nodePoint.BuildExpr();

            _logger.Info($"resultExr = {resultExr}");

            var str = resultExr.GetDbgString();

            _logger.Info($"str = {str}");

            var calcResult = resultExr.Calc();

            _logger.Info($"calcResult = {calcResult}");
        }

        private void Run6()
        {
            _nodePoint = new IntermediateAstNodePoint();

            _logger.Info($"_nodePoint = {_nodePoint}");

            var numNode = new NumberAstExpressionNode();
            numNode.Value = 12;

            _logger.Info($"numNode = {numNode}");

            var intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var unaryOp = new UnaryAstExpression();
            unaryOp.KindOfOperator = KindOfOperator.Minus;

            priority = GetPriority(unaryOp.Kind, unaryOp.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(unaryOp, KindOfIntermediateAstNode.UnaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            unaryOp = new UnaryAstExpression();
            unaryOp.KindOfOperator = KindOfOperator.Minus;

            priority = GetPriority(unaryOp.Kind, unaryOp.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(unaryOp, KindOfIntermediateAstNode.UnaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 5;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var resultExr = (BaseAstExpression)_nodePoint.BuildExpr();

            _logger.Info($"resultExr = {resultExr}");

            var str = resultExr.GetDbgString();

            _logger.Info($"str = {str}");

            var calcResult = resultExr.Calc();

            _logger.Info($"calcResult = {calcResult}");
        }

        private void Run7()
        {
            _nodePoint = new IntermediateAstNodePoint();

            _logger.Info($"_nodePoint = {_nodePoint}");

            var unaryOp = new UnaryAstExpression();
            unaryOp.KindOfOperator = KindOfOperator.Minus;

            var priority = GetPriority(unaryOp.Kind, unaryOp.KindOfOperator);

            _logger.Info($"priority = {priority}");

            var intermediateNode = new IntermediateAstNode(unaryOp, KindOfIntermediateAstNode.UnaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var numNode = new NumberAstExpressionNode();
            numNode.Value = 12;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 5;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var resultExr = (BaseAstExpression)_nodePoint.BuildExpr();

            _logger.Info($"resultExr = {resultExr}");

            var str = resultExr.GetDbgString();

            _logger.Info($"str = {str}");

            var calcResult = resultExr.Calc();

            _logger.Info($"calcResult = {calcResult}");
        }

        private void Run8()
        {
            _nodePoint = new IntermediateAstNodePoint();

            _logger.Info($"_nodePoint = {_nodePoint}");

            var varNode = new VarAstExpression();
            varNode.Name = "a";

            var intermediateNode = new IntermediateAstNode(varNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Assign;

            _logger.Info($"binOpNode = {binOpNode}");

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var numNode = new NumberAstExpressionNode();
            numNode.Value = 12;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 5;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var resultExr = (BaseAstExpression)_nodePoint.BuildExpr();

            _logger.Info($"resultExr = {resultExr}");

            var str = resultExr.GetDbgString();

            _logger.Info($"str = {str}");

            var calcResult = resultExr.Calc();

            _logger.Info($"calcResult = {calcResult}");

            _logger.Info($"varNode.Value = {varNode.Value}");
        }

        private void Run9()
        {
            _nodePoint = new IntermediateAstNodePoint();

            _logger.Info($"_nodePoint = {_nodePoint}");

            var varNode = new VarAstExpression();
            varNode.Name = "a";

            var intermediateNode = new IntermediateAstNode(varNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Assign;

            _logger.Info($"binOpNode = {binOpNode}");

            var priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var varNode2 = new VarAstExpression();
            varNode2.Name = "b";

            intermediateNode = new IntermediateAstNode(varNode2);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Assign;

            _logger.Info($"binOpNode = {binOpNode}");

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var numNode = new NumberAstExpressionNode();
            numNode.Value = 12;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            binOpNode = new BinaryAstExpression();
            binOpNode.KindOfOperator = KindOfOperator.Plus;

            _logger.Info($"binOpNode = {binOpNode}");

            priority = GetPriority(binOpNode.Kind, binOpNode.KindOfOperator);

            _logger.Info($"priority = {priority}");

            intermediateNode = new IntermediateAstNode(binOpNode, KindOfIntermediateAstNode.BinaryOperator, priority);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            numNode = new NumberAstExpressionNode();
            numNode.Value = 5;

            _logger.Info($"numNode = {numNode}");

            intermediateNode = new IntermediateAstNode(numNode);

            _logger.Info($"intermediateNode = {intermediateNode}");

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _logger.Info($"_nodePoint = {_nodePoint}");

            var resultExr = (BaseAstExpression)_nodePoint.BuildExpr();

            _logger.Info($"resultExr = {resultExr}");

            var str = resultExr.GetDbgString();

            _logger.Info($"str = {str}");

            var calcResult = resultExr.Calc();

            _logger.Info($"calcResult = {calcResult}");

            _logger.Info($"varNode.Value = {varNode.Value}");
            _logger.Info($"varNode2.Value = {varNode2.Value}");
        }

        private int GetPriority(KindOfNode kind, KindOfOperator kindOfOperator)
        {
            switch (kind)
            {
                case KindOfNode.BinaryOperator:
                    switch (kindOfOperator)
                    {
                        case KindOfOperator.Plus:
                        case KindOfOperator.Minus:
                            return AddAndSubPriority;

                        case KindOfOperator.Mul:
                        case KindOfOperator.Div:
                            return MulAndDivPriotiry;

                        case KindOfOperator.Assign:
                            return AssignPriority;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                    }

                case KindOfNode.UnaryOperator:
                    switch (kindOfOperator)
                    {
                        case KindOfOperator.Plus:
                        case KindOfOperator.Minus:
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

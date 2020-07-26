using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking
{
    public static class AstNodesLinker
    {
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void SetNode(IntermediateAstNode node, IntermediateAstNodePoint point)
        {
            if (point.RootNode == null)
            {
                point.RootNode = node;
                point.CurrentNode = node;

                return;
            }

            switch (node.Kind)
            {
                case KindOfIntermediateAstNode.Leaf:
                    SetLeafNode(node, point);
                    break;

                case KindOfIntermediateAstNode.BinaryOperator:
                    SetBinaryOperatorNode(node, point);
                    break;

                case KindOfIntermediateAstNode.UnaryOperator:
                    SetUnaryOperatorNode(node, point);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(node.Kind), node.Kind, null);
            }
        }

        public static bool CanBeLeafNow(IntermediateAstNodePoint point)
        {
            var currentNode = point.CurrentNode;

            switch(currentNode.Kind)
            {
                case KindOfIntermediateAstNode.BinaryOperator:
                case KindOfIntermediateAstNode.UnaryOperator:
                    return true;

                case KindOfIntermediateAstNode.Leaf:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException(nameof(currentNode.Kind), currentNode.Kind, null);
            }
        }

        private static void SetLeafNode(IntermediateAstNode node, IntermediateAstNodePoint point)
        {
            var currentNode = point.CurrentNode;

            switch (currentNode.Kind)
            {
                case KindOfIntermediateAstNode.BinaryOperator:
                    currentNode.Right = node;
                    node.Parent = currentNode;
                    point.CurrentNode = node;
                    break;

                case KindOfIntermediateAstNode.UnaryOperator:
                    currentNode.Left = node;
                    node.Parent = currentNode;
                    point.CurrentNode = node;
                    break;

                case KindOfIntermediateAstNode.Leaf:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException(nameof(currentNode.Kind), currentNode.Kind, null);
            }
        }

        private static void SetBinaryOperatorNode(IntermediateAstNode node, IntermediateAstNodePoint point)
        {
            if (point.CurrentNode == null)
            {
                throw new NotSupportedException();
            }

            var possibleCurrentNode = GetPossibleCurrentNode(node, point);

            if (possibleCurrentNode == null)
            {
                var oldRootNode = point.RootNode;
                point.RootNode = node;
                point.CurrentNode = node;
                oldRootNode.Parent = node;
                node.Left = oldRootNode;

                return;
            }

            switch (possibleCurrentNode.Kind)
            {
                case KindOfIntermediateAstNode.BinaryOperator:
                    {
                        var oldchildNode = possibleCurrentNode.Right;
                        possibleCurrentNode.Right = node;

                        node.Parent = possibleCurrentNode;
                        point.CurrentNode = node;

                        oldchildNode.Parent = node;
                        node.Left = oldchildNode;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(possibleCurrentNode.Kind), possibleCurrentNode.Kind, null);
            }
        }

        private static void SetUnaryOperatorNode(IntermediateAstNode node, IntermediateAstNodePoint point)
        {
            if (point.CurrentNode == null)
            {
                throw new NotSupportedException();
            }

            var possibleCurrentNode = GetPossibleCurrentNode(node, point);

            if (possibleCurrentNode == null)
            {
                var oldRootNode = point.RootNode;
                point.RootNode = node;
                point.CurrentNode = node;
                oldRootNode.Parent = node;
                node.Left = oldRootNode;

                return;
            }

            switch (possibleCurrentNode.Kind)
            {
                case KindOfIntermediateAstNode.BinaryOperator:
                    {
                        var oldchildNode = possibleCurrentNode.Right;
                        possibleCurrentNode.Right = node;

                        if (oldchildNode != null)
                        {
                            throw new NotImplementedException();
                        }

                        node.Parent = possibleCurrentNode;
                        point.CurrentNode = node;
                    }
                    break;

                case KindOfIntermediateAstNode.UnaryOperator:
                    possibleCurrentNode.Left = node;
                    node.Parent = possibleCurrentNode;
                    point.CurrentNode = node;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(possibleCurrentNode.Kind), possibleCurrentNode.Kind, null);
            }
        }

        private static IntermediateAstNode GetPossibleCurrentNode(IntermediateAstNode node, IntermediateAstNodePoint point)
        {
            var currentNode = point.CurrentNode;

            while (true)
            {
                if (currentNode == null)
                {
                    return null;
                }

                var parent = currentNode.Parent;

                if (parent == null)
                {
                    if (currentNode.Priority < node.Priority)
                    {
                        return null;
                    }

                    if (currentNode.Priority == node.Priority)
                    {
                        return null;
                    }

                    return currentNode;
                }

                if (currentNode.Priority > node.Priority)
                {
                    return currentNode;
                }

                if (currentNode.Kind == KindOfIntermediateAstNode.UnaryOperator && currentNode.Priority == node.Priority)
                {
                    return currentNode;
                }

                currentNode = parent;
            }
        }
    }
}

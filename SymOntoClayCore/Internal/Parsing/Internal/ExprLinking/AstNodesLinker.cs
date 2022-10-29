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

using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking
{
    public static class AstNodesLinker
    {
#if DEBUG
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

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
#if DEBUG
                    //_logger.Info($"node = {node}");
                    //_logger.Info($"point = {point}");
#endif

                    currentNode.Left = node;
                    node.Parent = currentNode;
                    point.CurrentNode = node;

#if DEBUG
                    //_logger.Info($"point (after) = {point}");
#endif

                    break;

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
#if DEBUG
                            //_logger.Info($"oldchildNode = {oldchildNode}");
                            //_logger.Info($"node = {node}");
#endif

                            if(node.Left != null)
                            {
                                throw new NotImplementedException();
                            }

                            node.Left = oldchildNode;

#if DEBUG
                            //_logger.Info($"node (after) = {node}");
#endif
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

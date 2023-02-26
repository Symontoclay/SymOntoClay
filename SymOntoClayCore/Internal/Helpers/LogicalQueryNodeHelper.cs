/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class LogicalQueryNodeHelper
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static Value ToValue(LogicalQueryNode node)
        {
            var kindOfNode = node.Kind;

            switch (kindOfNode)
            {
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.Concept:
                    return node.Name;

                case KindOfLogicalQueryNode.Value:
                    return node.Value;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfNode), kindOfNode, null);
            }
        }

        public static void FillUpInfoAboutComplexExpression(LogicalQueryNode node, List<LogicalQueryNode> additionalKnownInfoExpressions, List<StrongIdentifierValue> varNames)
        {
#if DEBUG
            //_gbcLogger.Info($"node = {node.ToHumanizedString()}");
#endif

            var kind = node.Kind;

#if DEBUG
            //_gbcLogger.Info($"kind = {kind}");
#endif

            switch (kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    additionalKnownInfoExpressions.Add(node);
                    FillUpInfoAboutComplexExpression(node.Left, additionalKnownInfoExpressions, varNames);
                    FillUpInfoAboutComplexExpression(node.Right, additionalKnownInfoExpressions, varNames);
                    break;

                case KindOfLogicalQueryNode.Group:
                case KindOfLogicalQueryNode.UnaryOperator:
                    additionalKnownInfoExpressions.Add(node);
                    FillUpInfoAboutComplexExpression(node.Left, additionalKnownInfoExpressions, varNames);
                    break;

                case KindOfLogicalQueryNode.Relation:
                    {
                        additionalKnownInfoExpressions.Add(node);

                        foreach (var param in node.ParamsList)
                        {
                            FillUpInfoAboutComplexExpression(param, additionalKnownInfoExpressions, varNames);
                        }
                    }
                    break;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.Value:
                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                case KindOfLogicalQueryNode.Fact:
                    additionalKnownInfoExpressions.Add(node);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}

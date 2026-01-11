/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForLogicalModalityExpression
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");

        public static string ToString(LogicalModalityExpressionNode source, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToString(source, opt);
        }

        public static string ToString(LogicalModalityExpressionNode source, DebugHelperOptions options)
        {
            switch (source.Kind)
            {
                case KindOfLogicalModalityExpressionNode.BlankIdentifier:
                    return BlankIdentifierToString(source, options);

                case KindOfLogicalModalityExpressionNode.BinaryOperator:
                    return BinaryOperatorToString(source, options);

                case KindOfLogicalModalityExpressionNode.UnaryOperator:
                    return UnaryOperatorToString(source, options);

                case KindOfLogicalModalityExpressionNode.Value:
                    return ValueToString(source, options);

                case KindOfLogicalModalityExpressionNode.Group:
                    return GroupToString(source, options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
            }
        }
        
        private static string BlankIdentifierToString(LogicalModalityExpressionNode source, DebugHelperOptions options)
        {
            return "_";
        }

        private static string BinaryOperatorToString(LogicalModalityExpressionNode source, DebugHelperOptions options)
        {
            var mark = OperatorsHelper.GetSymbol(source.KindOfOperator);

            if (options.IsHtml)
            {
                mark = StringHelper.ToHtmlCode(mark);
            }

            return $"{ToString(source.Left, options)} {mark} {ToString(source.Right, options)}";
        }

        private static string UnaryOperatorToString(LogicalModalityExpressionNode source, DebugHelperOptions options)
        {
            if (source.KindOfOperator == KindOfOperator.CallFunction)
            {
                throw new NotImplementedException("DF9274E2-725A-44F3-8B07-CE8BE709E460");
            }

            var mark = OperatorsHelper.GetSymbol(source.KindOfOperator);

            if (options.IsHtml)
            {
                mark = StringHelper.ToHtmlCode(mark);
            }

            return $" {mark} {ToString(source.Left, options)}";
        }

        private static string ValueToString(LogicalModalityExpressionNode source, DebugHelperOptions options)
        {
            return DebugHelperForRuleInstance.ToString(source.Value, options);
        }

        private static string GroupToString(LogicalModalityExpressionNode source, DebugHelperOptions options)
        {
            return $"({ToString(source.Left, options)})";
        }
    }
}

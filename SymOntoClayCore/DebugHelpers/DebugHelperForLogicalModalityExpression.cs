using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForLogicalModalityExpression
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");

#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static string ToString(LogicalModalityExpressionNode source, HumanizedOptions options = HumanizedOptions.ShowAll)
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

        private static string BlankIdentifierToString(LogicalModalityExpressionNode source, HumanizedOptions options)
        {
            return "_";
        }

        private static string BinaryOperatorToString(LogicalModalityExpressionNode source, HumanizedOptions options)
        {
            return $"{ToString(source.Left, options)} {OperatorsHelper.GetSymbol(source.KindOfOperator)} {ToString(source.Right, options)}";
        }

        private static string UnaryOperatorToString(LogicalModalityExpressionNode source, HumanizedOptions options)
        {
            if (source.KindOfOperator == KindOfOperator.CallFunction)
            {
                throw new NotImplementedException();
            }

            return $" {OperatorsHelper.GetSymbol(source.KindOfOperator)} {ToString(source.Left, options)}";
        }

        private static string ValueToString(LogicalModalityExpressionNode source, HumanizedOptions options)
        {
            return DebugHelperForRuleInstance.ToString(source.Value, options);
        }

        private static string GroupToString(LogicalModalityExpressionNode source, HumanizedOptions options)
        {
            return $"({ToString(source.Left, options)})";
        }
    }
}

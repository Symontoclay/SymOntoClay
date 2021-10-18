using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class ConvertorEntityConditionExpressionToRuleInstance
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        private enum AdviceForConversionEntityConditionExpressionToRuleInstance
        {
            Root
        }

        public static RuleInstance Convert(EntityConditionExpressionNode source, CheckDirtyOptions options)
        {
            var convertingContext = new Dictionary<object, object>();

            return Convert(source, options, convertingContext);
        }

        public static RuleInstance Convert(EntityConditionExpressionNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            var result = new RuleInstance();

            var primaryRulePart = new PrimaryRulePart();

            result.PrimaryPart = primaryRulePart;

            primaryRulePart.Parent = result;
            primaryRulePart.IsActive = true;

            primaryRulePart.Expression = ConvertToLogicalQueryNode(source, AdviceForConversionEntityConditionExpressionToRuleInstance.Root, options, convertingContext);

#if DEBUG
            _gbcLogger.Info($"result = {DebugHelperForRuleInstance.ToString(result)}");
#endif

            return result;
        }

        private static LogicalQueryNode ConvertToLogicalQueryNode(EntityConditionExpressionNode source, AdviceForConversionEntityConditionExpressionToRuleInstance advice, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            switch(advice)
            {
                case AdviceForConversionEntityConditionExpressionToRuleInstance.Root:
                    switch (source.Kind)
                    {
                        case KindOfLogicalQueryNode.Concept:
                            return ConvertStandaloneConcept(source, options, convertingContext);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(source.Kind), source.Kind, null);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(advice), advice, null);
            }
        }

        private static LogicalQueryNode ConvertStandaloneConcept(EntityConditionExpressionNode source, CheckDirtyOptions options, Dictionary<object, object> convertingContext)
        {
#if DEBUG
            _gbcLogger.Info($"source = {source}");
#endif

            if (source == null)
            {
                return null;
            }

            if (convertingContext.ContainsKey(source))
            {
                return (LogicalQueryNode)convertingContext[source];
            }

            var result = new LogicalQueryNode();
            convertingContext[source] = result;

            result.Kind = KindOfLogicalQueryNode.Relation;
            result.Name = source.Name;

            var destParametersList = new List<LogicalQueryNode>();

            result.ParamsList = destParametersList;


            destParametersList.Add(CreateEntitySelfVar());
#if DEBUG
            _gbcLogger.Info($"result = {result}");
#endif

            return result;
        }

        private static LogicalQueryNode CreateEntitySelfVar()
        {
            var selfVarNode = new LogicalQueryNode();
            selfVarNode.Kind = KindOfLogicalQueryNode.LogicalVar;
            selfVarNode.Name = NameHelper.CreateName("$_");

            return selfVarNode;
        }
    }
}

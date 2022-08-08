using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    /// <summary>
    /// You can see it by http://magjac.com/graphviz-visual-editor/.
    /// </summary>
    public static class DebugHelperForLogicalSearchExplainNode
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static string ToDot(LogicalSearchExplainNode source)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            if(source.Kind != KindOfLogicalSearchExplainNode.Root)
            {
                throw new Exception($"`{source.Kind}` node can not be converted to dot. Node should be {nameof(KindOfLogicalSearchExplainNode.Root)}.");
            }

            var context = new DebugHelperForLogicalSearchExplainNodeContext();

            context.Output.AppendLine("digraph graphname {");
            context.Output.AppendLine("rankdir=\"BT\"");

            ProcessNodeContent(source, context);

            foreach(var path in context.PathsList)
            {
                context.Output.AppendLine(path);
            }

            context.Output.AppendLine("}");

            return context.Output.ToString();
        }

        private static void ProcessNodeContent(LogicalSearchExplainNode source, DebugHelperForLogicalSearchExplainNodeContext context)
        {
#if DEBUG
            //_gbcLogger.Info($"source = {source}");
#endif

            var name = context.GetNodeName(source);

#if DEBUG
            _gbcLogger.Info($"name = {name}");
#endif

            //Dot supports html https://graphviz.org/doc/info/shapes.html but the html can not contain '&' and '<>'.
            //So you should create huamnized string generation in html style.

            var nodeBuilder = new StringBuilder();
            nodeBuilder.Append($"{name} [shape=box label=<");
            nodeBuilder.Append(GetNodeLabel(source));
            nodeBuilder.AppendLine(">];");

            context.Output.AppendLine(nodeBuilder.ToString());

            if(source.Children.Any())
            {
                foreach(var item in source.Children)
                {
                    ProcessNodeContent(item, context);

                    context.PathsList.Add($"{context.GetNodeName(item)} -> {name};");
                }
            }
        }

        private static string GetNodeLabel(LogicalSearchExplainNode source)
        {
            var kind = source.Kind;

            var toHumanizedStringOptions = new DebugHelperOptions();
            toHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
            toHumanizedStringOptions.IsHtml = true;

            switch (kind)
            {
                case KindOfLogicalSearchExplainNode.Root:
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine($"<TR><TD>{source.ProcessedRuleInstance.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");
                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.Result:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");

                        sb.Append("<TR><TD>");

                        sb.Append("&#8657;");

                        if (source.IsSuccess)
                        {
                            sb.Append("&#10004;");
                        }
                        else
                        {
                            sb.Append("&#10008;");
                        }

                        sb.AppendLine("</TD></TR>");

                        var resultsOfQueryToRelationList = source.ResultsOfQueryToRelationList;

                        if (resultsOfQueryToRelationList.IsNullOrEmpty())
                        {
                            sb.AppendLine("<TR><TD>-</TD></TR>");
                        }
                        else
                        {
                            sb.AppendLine("<TR><TD>");
                            sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"1\">");

                            var varsList = resultsOfQueryToRelationList.SelectMany(p => p.ResultOfVarOfQueryToRelationList.Select(x => x.NameOfVar)).Distinct().OrderBy(p => p.NameValue).ToList();

                            sb.Append("<TH>");

                            foreach(var varItem in varsList)
                            {
                                sb.Append($"<TD>{varItem.NameValue}</TD>");
                            }

                            sb.AppendLine("</TH>");

                            foreach(var item in resultsOfQueryToRelationList)
                            {
                                sb.AppendLine("<TR>");

                                var itemVarDict = item.ResultOfVarOfQueryToRelationList.ToDictionary(p => p.NameOfVar, p => p.FoundExpression);

                                foreach (var varItem in varsList)
                                {
                                    if(itemVarDict.ContainsKey(varItem))
                                    {
                                        var expr = itemVarDict[varItem];

                                        sb.Append($"<TD>{expr.ToHumanizedString(toHumanizedStringOptions)}</TD>");

                                        continue;
                                    }

                                    sb.Append("<TD>-</TD>");
                                }

                                sb.AppendLine("</TR>");
                            }

                            sb.AppendLine("</TABLE>");
                            sb.AppendLine("</TD></TR>");
                        }

                        sb.AppendLine("<TR><TD></TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.ResultCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Results:</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.DataSourceResult:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");

                        var baseRulePartList = source.BaseRulePartList;

                        if (baseRulePartList.IsNullOrEmpty())
                        {
                            sb.AppendLine("<TR><TD>&#8648;&#10008;</TD></TR>");
                        }
                        else
                        {
                            sb.AppendLine("<TR><TD>&#8648;&#10004;</TD></TR>");

                            if (!baseRulePartList.IsNullOrEmpty())
                            {
                                sb.AppendLine("<TR><TD>");

                                sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"1\">");
                                //sb.AppendLine("<TR><TD>Rule part</TD><TD>Parent fact</TD></TR>");

                                foreach(var item in baseRulePartList)
                                {
                                    sb.AppendLine("<TR>");
                                    sb.AppendLine($"<TD>{item.ToHumanizedString(toHumanizedStringOptions)}</TD>");

                                    //var rulePartHumanizedStringOptions = new DebugHelperOptions();
                                    //rulePartHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                                    //rulePartHumanizedStringOptions.IsHtml = true;
                                    //rulePartHumanizedStringOptions.ItemsForSelection = new List<IObjectToString> { item };

                                    //sb.AppendLine($"<TD>{item.Parent.ToHumanizedString(rulePartHumanizedStringOptions)}</TD>");
                                    sb.AppendLine("</TR>");
                                }

                                sb.AppendLine("</TABLE>");

                                sb.AppendLine("</TD></TR>");
                            }
                            else
                            {
                                sb.AppendLine("<TR><TD>-</TD></TR>");
                            }
                        }

                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.DataSourceCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Data sources:</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.ConsolidatedDataSource:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Consolidated data sources</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.LogicalStorage:
                    {
                        var sb = new StringBuilder();

                        var ruleInstance = source.LogicalStorage as RuleInstance;

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine($"<TR><TD>Logical storage: {source.LogicalStorage?.Kind}</TD></TR>");
                        if(ruleInstance != null)
                        {
                            sb.AppendLine($"<TR><TD>{ruleInstance.ToHumanizedString()}</TD></TR>");
                        }
                        sb.AppendLine($"<TR><TD>Key: <b>{source.Key?.NameValue}</b></TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.LogicalStorageFilter:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Logical storage filter</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.RuleInstanceQuery:
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine($"<TR><TD>{source.ProcessedRuleInstance.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");
                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.PrimaryRulePartQuery:
                    {
                        var selectProcessedPrimaryRulePartHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedPrimaryRulePartHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedPrimaryRulePartHumanizedStringOptions.IsHtml = true;
                        selectProcessedPrimaryRulePartHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { source.ProcessedPrimaryRulePart };


                        var sb = new StringBuilder();
                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine($"<TR><TD>{source.ProcessedPrimaryRulePart.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{source.ProcessedPrimaryRulePart.Parent.ToHumanizedString(selectProcessedPrimaryRulePartHumanizedStringOptions)}</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");
                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.RelationQuery:
                    {
                        var targetProcessedItem = source.ProcessedLogicalQueryNode;

                        var selectProcessedLogicalQueryNodeHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.IsHtml = true;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { targetProcessedItem };


                        var sb = new StringBuilder();
                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Process relation:</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.RuleInstance.ToHumanizedString(selectProcessedLogicalQueryNodeHumanizedStringOptions)}</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");
                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.ProcessRelationWithDirectFactsCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Direct facts:</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.ProcessRelationWithProductionCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Production:</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.RelationWithProductionQuery:
                    {
                        var targetProcessedItem = source.ProcessedBaseRulePart;

                        var selectProcessedLogicalQueryNodeHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.IsHtml = true;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { targetProcessedItem };

                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Process production:</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.Parent.ToHumanizedString(selectProcessedLogicalQueryNodeHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>Target relation: <b>{source.TargetRelation.NameValue}</b></TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.MergedKnownInfoCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Merged KnownInfo:</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.KnownInfoResult:
                    {
                        var knownInfoList = source.KnownInfoList;
                        var varsInfoList = source.VarsInfoList;

                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>");
                        sb.AppendLine("&#8685;");

                        if (source.IsSuccess)
                        {
                            sb.Append("&#10004;");
                        }
                        else
                        {
                            sb.Append("&#10008;");
                        }

                        sb.AppendLine("</TD></TR>");

                        if(knownInfoList.IsNullOrEmpty() && varsInfoList.IsNullOrEmpty())
                        {
                            sb.AppendLine("<TR><TD>-</TD></TR>");
                        }
                        else
                        {
                            if(knownInfoList.IsNullOrEmpty())
                            {
                                sb.AppendLine("<TR><TD>");
                                sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"1\">");
                                sb.AppendLine("<TR><TD>Position</TD><TD>Var name</TD></TR>");

                                foreach(var item in varsInfoList)
                                {
                                    sb.AppendLine($"<TR><TD>{item.Position}</TD><TD>{item.NameOfVar.NameValue}</TD></TR>");
                                }

                                sb.AppendLine("</TABLE>");
                                sb.AppendLine("</TD></TR>");
                            }
                            else
                            {
                                var hasNameOfVars = knownInfoList.Any(p => p.NameOfVar != null);

                                sb.AppendLine("<TR><TD>");
                                sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"1\">");

                                sb.AppendLine("<TR>");

                                if(hasNameOfVars)
                                {
                                    sb.AppendLine("<TD>Var name</TD>");
                                }
                                sb.AppendLine("<TD>Position</TD><TD>Expression</TD>");
                                sb.AppendLine("</TR>");

                                foreach(var item in knownInfoList)
                                {
                                    sb.AppendLine("<TR>");

                                    if (hasNameOfVars)
                                    {
                                        sb.AppendLine($"<TD>{item.NameOfVar?.NameValue}</TD>");
                                    }

                                    sb.AppendLine($"<TD>{item.Position}</TD><TD>{item.Expression?.ToHumanizedString(toHumanizedStringOptions)}</TD>");

                                    sb.AppendLine("</TR>");
                                }

                                sb.AppendLine("</TABLE>");
                                sb.AppendLine("</TD></TR>");
                            }
                        }

                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.MergeKnownInfoBlock:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>&#8625;&#8593; Merge KnownInfo</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.KnownInfoDataSource:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>KnownInfo DataSource</TD></TR>");
                        sb.AppendLine($"<TR><TD>Storage name: <b>{source.StorageName}</b></TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.RelationWithProductionNextPartsCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Processing next parts:</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.RelationWithProductionNextPart:
                    {
                        var targetProcessedItem = source.ProcessedBaseRulePart;

                        var selectProcessedLogicalQueryNodeHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.IsHtml = true;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { targetProcessedItem };

                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Process next part</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.Parent.ToHumanizedString(selectProcessedLogicalQueryNodeHumanizedStringOptions)}</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");
                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.OperatorQuery:
                    {
                        var targetProcessedItem = source.ProcessedLogicalQueryNode;

                        var selectProcessedLogicalQueryNodeHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.IsHtml = true;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { targetProcessedItem };

                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine($"<TR><TD>Operator: <b>{source.KindOfOperator}</b></TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.RuleInstance.ToHumanizedString(selectProcessedLogicalQueryNodeHumanizedStringOptions)}</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                    //return string.Empty;
            }

            /*
                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD></TD></TR>");
                        sb.AppendLine("</TABLE>");
             */
        }

        private static void PrintAdditionalInformation(LogicalSearchExplainNode source, StringBuilder sb)
        {
            var additionalInformation = source.AdditionalInformation;

            if(additionalInformation.IsNullOrEmpty())
            {
                return;
            }

            throw new NotImplementedException();
        }
    }
}

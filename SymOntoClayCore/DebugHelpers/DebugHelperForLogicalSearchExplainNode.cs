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
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.ResultCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Results:</TD></TR>");
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
                            sb.AppendLine("<TR><TD>-</TD></TR>");
                        }
                        else
                        {
                            if (!baseRulePartList.IsNullOrEmpty())
                            {
                                sb.AppendLine("<TR><TD>))))))</TD></TR>");
                            }
                            else
                            {
                                sb.AppendLine("<TR><TD>-</TD></TR>");
                            }
                        }
                        
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.DataSourceCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Data sources:</TD></TR>");
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.ConsolidatedDataSource:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Consolidated data sources</TD></TR>");
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
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.LogicalStorageFilter:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Logical storage filter</TD></TR>");
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.RuleInstanceQuery:
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine($"<TR><TD>{source.ProcessedRuleInstance.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
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
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.RuleInstance.ToHumanizedString(selectProcessedLogicalQueryNodeHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine("</TABLE>");
                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.ProcessRelationWithDirectFactsCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Direct facts:</TD></TR>");
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.ProcessRelationWithProductionCollector:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Production:</TD></TR>");
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
    }
}

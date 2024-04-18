/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper;
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
            var processedNodesList = context.ProcessedNodesList;

            processedNodesList.Add(source);

            var name = context.GetNodeName(source);


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

            var commonChildren = source.CommonChildren;

            if (!commonChildren.IsNullOrEmpty())
            {
                foreach(var item in commonChildren)
                {
                    if(processedNodesList.Contains(item))
                    {
                        continue;
                    }

                    ProcessNodeContent(item, context);
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
                        sb.AppendLine("<TR><TD>&#9072;Query:</TD></TR>");
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

                        var resultsOfQueryToRelationList = source.ResultsOfQueryToRelationList?.ToList();

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

                                var itemVarDict = item.ResultOfVarOfQueryToRelationList.GroupBy(p => p.NameOfVar).ToDictionary(p => p.Key, p => p.Select(x => x.FoundExpression));

                                foreach (var varItem in varsList)
                                {
                                    if(itemVarDict.ContainsKey(varItem))
                                    {
                                        var exprList = itemVarDict[varItem];

                                        sb.Append("<TD>");

                                        if (exprList.Count() == 1)
                                        {
                                            sb.Append(exprList.Single().ToHumanizedString(toHumanizedStringOptions));
                                        }
                                        else
                                        {
                                            sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"1\">");

                                            foreach (var expr in exprList)
                                            {
                                                sb.Append($"<TR><TD>{expr.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                                            }
                                            sb.AppendLine("</TABLE>");
                                        }

                                        sb.Append("</TD>");                                        

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

                        var baseRulePartList = source.BaseRulePartList?.ToList();
                        var relationsList = source.RelationsList?.ToList();

                        if(baseRulePartList.IsNullOrEmpty() && relationsList.IsNullOrEmpty())
                        {
                            sb.AppendLine("<TR><TD>&#8648;&#10008;</TD></TR>");
                        }
                        else
                        {
                            if (baseRulePartList.IsNullOrEmpty())
                            {
                                if(relationsList.IsNullOrEmpty())
                                {
                                    sb.AppendLine("<TR><TD>&#8648;&#10008;</TD></TR>");
                                }
                                else
                                {
                                    sb.AppendLine("<TR><TD>&#8648;&#10004;</TD></TR>");

                                    sb.AppendLine("<TR><TD>");

                                    sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"1\">");

                                    foreach(var item in relationsList)
                                    {
                                        sb.AppendLine("<TR>");
                                        sb.AppendLine($"<TD>{item.ToHumanizedString(toHumanizedStringOptions)}</TD>");
                                        sb.AppendLine("</TR>");
                                    }

                                    sb.AppendLine("</TABLE>");

                                    sb.AppendLine("</TD></TR>");
                                }                                
                            }
                            else
                            {
                                if (!baseRulePartList.IsNullOrEmpty())
                                {
                                    sb.AppendLine("<TR><TD>&#8648;&#10004;</TD></TR>");

                                    sb.AppendLine("<TR><TD>");

                                    sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"1\">");

                                    foreach (var item in baseRulePartList)
                                    {
                                        sb.AppendLine("<TR>");
                                        sb.AppendLine($"<TD>{item.ToHumanizedString(toHumanizedStringOptions)}</TD>");


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

                        sb.AppendLine($"<TR><TD>Logical storage: {source.LogicalStorage.Kind} ({source.LogicalStorage.GetHashCode()})</TD></TR>");
                        if(ruleInstance != null)
                        {
                            sb.AppendLine($"<TR><TD>{ruleInstance.ToHumanizedString()}</TD></TR>");
                        }

                        var key = source.Key?.NameValue;

                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            sb.AppendLine($"<TR><TD>Key: <b>{key}</b></TD></TR>");
                        }
                        
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

                case KindOfLogicalSearchExplainNode.RelationQuestionQuery:
                    {
                        var targetProcessedItem = source.ProcessedLogicalQueryNode;

                        var selectProcessedLogicalQueryNodeHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.IsHtml = true;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { targetProcessedItem };


                        var sb = new StringBuilder();
                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Process relation question:</TD></TR>");
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
                        var knownInfoList = source.KnownInfoList?.ToList();
                        var varsInfoList = source.VarsInfoList?.ToList();

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

                case KindOfLogicalSearchExplainNode.RelationWithDirectFactQuery:
                    {
                        var targetProcessedItem = source.ProcessedBaseRulePart;

                        var selectProcessedLogicalQueryNodeHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.IsHtml = true;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { targetProcessedItem };

                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Process direct fact:</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.Parent.ToHumanizedString(selectProcessedLogicalQueryNodeHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>Target relation: <b>{source.TargetRelation.NameValue}</b></TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.RelationWithDirectFactQueryProcessTargetRelation:
                    {
                        var targetProcessedItem = source.ProcessedLogicalQueryNode;

                        var selectProcessedLogicalQueryNodeHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.IsHtml = true;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { targetProcessedItem };

                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>");

                        if (source.IsFit)
                        {
                            sb.Append("&#10004;");
                        }
                        else
                        {
                            sb.Append("&#10008;");
                        }

                        sb.AppendLine("Target relation:");
                        sb.AppendLine("</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.RuleInstance.ToHumanizedString(selectProcessedLogicalQueryNodeHumanizedStringOptions)}</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.PostFilterWithAndStrategy:
                    {
                        var targetProcessedItem = source.ProcessedLogicalQueryNode;

                        var selectProcessedLogicalQueryNodeHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.IsHtml = true;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { targetProcessedItem };

                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>PostFilterWithAndStrategy</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.RuleInstance.ToHumanizedString(selectProcessedLogicalQueryNodeHumanizedStringOptions)}</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.FetchingAllValuesForResolvingExpressionParam:
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>FetchingAllValuesForResolvingExpressionParam</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                case KindOfLogicalSearchExplainNode.GroupQuery:
                    {
                        var targetProcessedItem = source.ProcessedLogicalQueryNode;

                        var selectProcessedLogicalQueryNodeHumanizedStringOptions = new DebugHelperOptions();
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.HumanizedOptions = HumanizedOptions.ShowAll;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.IsHtml = true;
                        selectProcessedLogicalQueryNodeHumanizedStringOptions.ItemsForSelection = new List<IObjectToString>() { targetProcessedItem };

                        var sb = new StringBuilder();

                        sb.AppendLine("<TABLE border=\"0\" cellspacing=\"0\" cellborder=\"0\">");
                        sb.AppendLine("<TR><TD>Operator: <b>()</b></TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.ToHumanizedString(toHumanizedStringOptions)}</TD></TR>");
                        sb.AppendLine($"<TR><TD>{targetProcessedItem.RuleInstance.ToHumanizedString(selectProcessedLogicalQueryNodeHumanizedStringOptions)}</TD></TR>");
                        PrintAdditionalInformation(source, sb);
                        sb.AppendLine("</TABLE>");

                        return sb.ToString();
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
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

            sb.AppendLine("<TR><TD>&#9888;Additional information:</TD></TR>");

            foreach(var item in additionalInformation)
            {
                sb.AppendLine($"<TR><TD>{StringHelper.ToHtmlCode(item)}</TD></TR>");
            }
        }
    }
}

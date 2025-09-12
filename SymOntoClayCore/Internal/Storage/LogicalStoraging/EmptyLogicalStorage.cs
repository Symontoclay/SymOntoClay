using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public class EmptyLogicalStorage : BaseEmptySpecificStorage, ILogicalStorage
    {
        public EmptyLogicalStorage(IStorage storage, IMonitorLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RuleInstance ruleInstance)
        {
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, RuleInstance ruleInstance, bool isPrimary)
        {
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList)
        {
        }

        /// <inheritdoc/>
        public void Remove(IMonitorLogger logger, RuleInstance ruleInstance)
        {
        }

        /// <inheritdoc/>
        public void Remove(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList)
        {
        }

        /// <inheritdoc/>
        public void RemoveById(IMonitorLogger logger, string id)
        {
        }

        /// <inheritdoc/>
        public void AddOnChangedHandler(IOnChangedLogicalStorageHandler handler)
        {
        }

        /// <inheritdoc/>
        public void RemoveOnChangedHandler(IOnChangedLogicalStorageHandler handler)
        {
        }

        /// <inheritdoc/>
        public void AddOnChangedWithKeysHandler(IOnChangedWithKeysLogicalStorageHandler handler)
        {
        }

        /// <inheritdoc/>
        public void RemoveOnChangedWithKeysHandler(IOnChangedWithKeysLogicalStorageHandler handler)
        {
        }

        /// <inheritdoc/>
        public void AddOnAddingFactHandler(IOnAddingFactHandler handler)
        {
        }

        /// <inheritdoc/>
        public void RemoveOnAddingFactHandler(IOnAddingFactHandler handler)
        {
        }

        private static List<LogicalQueryNode> _getAllRelationsEmptyList = new List<LogicalQueryNode>();

        /// <inheritdoc/>
        public IList<LogicalQueryNode> GetAllRelations(IMonitorLogger logger, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            return _getAllRelationsEmptyList;
        }

        private static List<RuleInstance> _getAllOriginFactsEmptyList = new List<RuleInstance>();

        /// <inheritdoc/>
        public IList<RuleInstance> GetAllOriginFacts(IMonitorLogger logger)
        {
            return _getAllOriginFactsEmptyList;
        }

        private static List<BaseRulePart> _getIndexedRulePartOfFactsByKeyOfRelationEmptyList = new List<BaseRulePart>();

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            return _getIndexedRulePartOfFactsByKeyOfRelationEmptyList;
        }

        private static List<BaseRulePart> _getIndexedRulePartWithOneRelationWithVarsByKeyOfRelationEmptyList = new List<BaseRulePart>();

        /// <inheritdoc/>
        public IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode)
        {
            return _getIndexedRulePartWithOneRelationWithVarsByKeyOfRelationEmptyList;
        }

        private static List<LogicalQueryNode> _getLogicalQueryNodesEmptyList = new List<LogicalQueryNode>();

        /// <inheritdoc/>
        public IReadOnlyList<LogicalQueryNode> GetLogicalQueryNodes(IMonitorLogger logger, IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems)
        {
            return _getLogicalQueryNodesEmptyList;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            return PropertiesToDbgString(n);
        }

        private string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNSpaces = DisplayHelper.Spaces(nextN);

            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}({GetHashCode()}) Begin {Kind}");

            sb.AppendLine($"{spaces}({GetHashCode()}) End {Kind}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string FactsAndRulesToDbgString()
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        public void DbgPrintFactsAndRules(IMonitorLogger logger)
        {
        }

        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedString(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedLabel();
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedLabel(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedLabel();
        }

        private string NToHumanizedLabel()
        {
            return $"Logical storage {GetHashCode()}:{Kind}";
        }

        /// <inheritdoc/>
        public string ToHumanizedString(IMonitorLogger logger)
        {
            return ToHumanizedString();
        }

        /// <inheritdoc/>
        public MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = ToHumanizedLabel()
            };
        }
    }
}

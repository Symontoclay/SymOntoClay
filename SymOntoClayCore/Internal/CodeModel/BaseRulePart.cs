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
using NLog.Fluent;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseRulePart: AnnotatedItem, IMemberAccess, IReadOnlyMemberAccess, ILogicalSearchItem, ILogicalQueryNodeParent//, IEquatable<BaseRulePart>
    {
#if DEBUG
        private static readonly Logger _staticLogger = LogManager.GetCurrentClassLogger();
#endif

        public RuleInstance Parent { get; set; }

        /// <inheritdoc/>
        RuleInstance ILogicalSearchItem.RuleInstance => Parent;

        public bool IsActive { get; set; }
        public LogicalQueryNode Expression { get; set; }
        public Dictionary<StrongIdentifierValue, LogicalQueryNode> AliasesDict { get; set; }

        public bool HasQuestionVars { get; set; }
        public bool HasVars { get; set; }
        public bool IsParameterized { get; set; }
        public TypeOfAccess TypeOfAccess { get; set; } = TypeOfAccess.Unknown;
        public StrongIdentifierValue Holder { get; set; }

        public IDictionary<StrongIdentifierValue, IList<LogicalQueryNode>> RelationsDict { get; set; }

        public void SetHolder(StrongIdentifierValue holder)
        {
            Holder = holder;

            Expression.SetHolder(holder);
        }

        public void SetTypeOfAccess(TypeOfAccess typeOfAccess)
        {
            TypeOfAccess = typeOfAccess;

            Expression.SetTypeOfAccess(typeOfAccess);
        }

        protected void AppendBaseRulePart(BaseRulePart source, Dictionary<object, object> context)
        {
            IsActive = source.IsActive;
            Parent = source.Parent.Clone(context);
            Expression = source.Expression.Clone(context);
            AliasesDict = source.AliasesDict?.ToDictionary(p => p.Key.Clone(context), p => p.Value.Clone(context));
            HasQuestionVars = source.HasQuestionVars;
            HasVars = source.HasVars;
            IsParameterized = source.IsParameterized;
            TypeOfAccess = source.TypeOfAccess;
            Holder = source.Holder;

            RelationsDict = source.RelationsDict.ToDictionary(p => p.Key, p => (IList<LogicalQueryNode>)(p.Value.Select(x => x.Clone(context)).ToList()));

            AppendAnnotations(source, context);
        }

        /// <inheritdoc/>
        public Value ObligationModality => Parent.ObligationModality;

        /// <inheritdoc/>
        public Value SelfObligationModality => Parent.SelfObligationModality;

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            Expression?.DiscoverAllAnnotations(result);
        }

        public IList<LogicalQueryNode> GetInheritanceRelations(IMonitorLogger logger)
        {
            var result = new List<LogicalQueryNode>();
            Expression.DiscoverAllInheritanceRelations(logger, result);
            return result;
        }

        public IList<StrongIdentifierValue> GetStandaloneConcepts(IMonitorLogger logger)
        {
            var result = new List<StrongIdentifierValue>();
            Expression.DiscoverAllStandaloneConcepts(logger, result);
            return result.Distinct().ToList();
        }

        public abstract IList<BaseRulePart> GetNextPartsList(IMonitorLogger logger);

        public void PrepareDirty(RuleInstance ruleInstance)
        {
            var contextOfConvertingExpressionNode = new ContextOfConvertingExpressionNode();

            Expression.PrepareDirty(contextOfConvertingExpressionNode, ruleInstance, this);

            HasQuestionVars = contextOfConvertingExpressionNode.HasQuestionVars;
            HasVars = contextOfConvertingExpressionNode.HasVars;
            IsParameterized = contextOfConvertingExpressionNode.IsParameterized;

            RelationsDict = contextOfConvertingExpressionNode.RelationsList.GroupBy(p => p.Name).ToDictionary(p => p.Key, p => (IList<LogicalQueryNode>)p.ToList());
        }

        public void CalculateUsedKeys(List<StrongIdentifierValue> usedKeysList)
        {
            Expression.CalculateUsedKeys(usedKeysList);
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ Expression.GetLongHashCode(options);
        }

        /// <inheritdoc/>
        public void Remove(LogicalQueryNode node)
        {
            if(Expression == node)
            {
                Expression = null;
                return;
            }

            throw new NotImplementedException("9E585689-D7EA-45F6-A0F3-4F911E40BDB8");
        }

        /// <inheritdoc/>
        public void Replace(LogicalQueryNode oldNode, LogicalQueryNode newNode)
        {
            if (Expression == oldNode)
            {
                Expression = newNode;
                return;
            }

            throw new NotImplementedException("CA64272F-4452-44F3-AD0C-28D157934ED5");
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
#if DEBUG
            _staticLogger.Info($"||||||||||||||");
            _staticLogger.Info($"this = {this.ToHumanizedString()}");
#endif

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNSpace = DisplayHelper.Spaces(nextN);

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

#if DEBUG
            _staticLogger.Info(">-->-->233297CA-2BDE-4D9D-B0BC-75510AA61954");
#endif

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");
            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");
            sb.PrintObjProp(n, nameof(Holder), Holder);

#if DEBUG
            _staticLogger.Info(">-->-->0D5AB49B-4B01-4A1D-A873-1032E9FDF94A");
#endif

            sb.PrintObjProp(n, nameof(Expression), Expression);

#if DEBUG
            _staticLogger.Info(">-->-->AB6F31D6-E1AC-4354-A34B-5221728DA6BA");
#endif

            sb.PrintObjDict_1_Prop(n, nameof(AliasesDict), AliasesDict);

#if DEBUG
            _staticLogger.Info(">-->-->D2706192-6E23-428C-A612-5351EB115B02");
#endif

            if (RelationsDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(RelationsDict)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(RelationsDict)}");
                var nextNextN = nextN + DisplayHelper.IndentationStep;
                foreach (var relationsKVPItem in RelationsDict)
                {
                    sb.AppendLine($"{nextNSpace}key of relation = {relationsKVPItem.Key.NameValue}");
                    var tmpRelationsList = relationsKVPItem.Value;
                    sb.AppendLine($"{nextNSpace}count of relations = {tmpRelationsList.Count}");
                    foreach (var relation in tmpRelationsList)
                    {
                        sb.Append(relation.ToShortString(nextNextN));
                    }
                }
                sb.AppendLine($"{spaces}End {nameof(RelationsDict)}");
            }

#if DEBUG
            _staticLogger.Info(">-->-->0FD36B53-BEAB-463B-81FC-43A0D63F9913");
#endif

            sb.Append(base.PropertiesToString(n));

#if DEBUG
            _staticLogger.Info(">-->-->03B592B2-4A58-4D5A-ACF7-3CFDB2AC5F79");
#endif

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");
            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");
            sb.PrintShortObjProp(n, nameof(Holder), Holder);

            sb.PrintShortObjProp(n, nameof(Expression), Expression);

            sb.PrintShortObjDict_1_Prop(n, nameof(AliasesDict), AliasesDict);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
#if DEBUG
            _staticLogger.Info($"~~~~~~~~~~~~~~~~~~~~~~~~~");
            _staticLogger.Info($"this = {this.ToHumanizedString()}");
#endif

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

#if DEBUG
            _staticLogger.Info(">-->-->0642F358-0932-42AA-B8F0-1FFAD3331140");
#endif

            sb.PrintBriefObjDict_1_Prop(n, nameof(AliasesDict), AliasesDict);

#if DEBUG
            _staticLogger.Info(">-->-->15B442CD-705C-4371-AE8F-DEC3D5C6B650");
#endif

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");
            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");
            sb.PrintBriefObjProp(n, nameof(Holder), Holder);

#if DEBUG
            _staticLogger.Info(">-->-->CD2D4FC3-DC7E-4509-9224-110BD1AA9A81");
#endif

            sb.PrintExisting(n, nameof(Expression), Expression);

#if DEBUG
            _staticLogger.Info(">-->-->626670B1-0B3F-483B-9658-8B058BC321AF");
#endif

            sb.Append(base.PropertiesToBriefString(n));

#if DEBUG
            _staticLogger.Info(">-->-->1EAAF6FC-D468-4C9E-9E64-6E023E41F420");
#endif

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return DebugHelperForRuleInstance.BaseRulePartToString(this, options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }
    }
}

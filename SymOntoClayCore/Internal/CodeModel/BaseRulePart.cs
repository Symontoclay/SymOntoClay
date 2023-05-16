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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseRulePart: AnnotatedItem, IMemberAccess, IReadOnlyMemberAccess, ILogicalSearchItem, ILogicalQueryNodeParent//, IEquatable<BaseRulePart>
    {
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

        // <inheritdoc/>
        //public bool Equals(BaseRulePart other)
        //{
        //    if (other == null)
        //    {
        //        return false;
        //    }

        //    throw new NotImplementedException();
        //}

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

        public IList<LogicalQueryNode> GetInheritanceRelations()
        {
            var result = new List<LogicalQueryNode>();
            Expression.DiscoverAllInheritanceRelations(result);
            return result;
        }

        public IList<StrongIdentifierValue> GetStandaloneConcepts()
        {
            var result = new List<StrongIdentifierValue>();
            Expression.DiscoverAllStandaloneConcepts(result);
            return result.Distinct().ToList();
        }

        public abstract IList<BaseRulePart> GetNextPartsList();

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

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Replace(LogicalQueryNode oldNode, LogicalQueryNode newNode)
        {
            if (Expression == oldNode)
            {
                Expression = newNode;
                return;
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNSpace = DisplayHelper.Spaces(nextN);

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");
            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");
            sb.PrintObjProp(n, nameof(Holder), Holder);

            sb.PrintObjProp(n, nameof(Expression), Expression);

            sb.PrintObjDict_1_Prop(n, nameof(AliasesDict), AliasesDict);

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

            sb.Append(base.PropertiesToString(n));
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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.PrintBriefObjDict_1_Prop(n, nameof(AliasesDict), AliasesDict);

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");
            sb.AppendLine($"{spaces}{nameof(IsParameterized)} = {IsParameterized}");
            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");
            sb.PrintBriefObjProp(n, nameof(Holder), Holder);

            sb.PrintBriefObjProp(n, nameof(Expression), Expression);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return DebugHelperForRuleInstance.BaseRulePartToString(this, options);
        }
    }
}

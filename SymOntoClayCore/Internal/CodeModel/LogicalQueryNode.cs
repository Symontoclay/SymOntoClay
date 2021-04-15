/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalQueryNode: AnnotatedItem, IAstNode
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public KindOfLogicalQueryNode Kind { get; set; } = KindOfLogicalQueryNode.Unknown;
        public KindOfOperatorOfLogicalQueryNode KindOfOperator { get; set; } = KindOfOperatorOfLogicalQueryNode.Unknown;
        public StrongIdentifierValue Name { get; set; }
        public LogicalQueryNode Left { get; set; }
        public LogicalQueryNode Right { get; set; }
        public IList<LogicalQueryNode> ParamsList { get; set; }
        public Value Value { get; set; }
        public FuzzyLogicNonNumericSequenceValue FuzzyLogicNonNumericSequenceValue { get; set; }
        public bool IsQuestion { get; set; }

        public int CountParams { get; set; }
        public IList<QueryExecutingCardAboutVar> VarsInfoList { get; set; }
        public IList<QueryExecutingCardAboutKnownInfo> KnownInfoList { get; set; }

        public bool IsKeyRef => Kind == KindOfLogicalQueryNode.Concept || Kind == KindOfLogicalQueryNode.Entity || Kind == KindOfLogicalQueryNode.LogicalVar || Kind == KindOfLogicalQueryNode.QuestionVar;
        public bool IsEntityRef => Kind == KindOfLogicalQueryNode.EntityRef;

        public RuleInstance RuleInstance { get; set; }
        public BaseRulePart RulePart { get; set; }

        //public bool HasQuestionVars
        //{
        //    get
        //    {
        //        if(IsQuestion)
        //        {
        //            return true;
        //        }

        //        if(Left != null && Left.HasQuestionVars)
        //        {
        //            return true;
        //        }

        //        if(Right != null && Right.HasQuestionVars)
        //        {
        //            return true;
        //        }

        //        if (ParamsList != null && ParamsList.Any(p => p.HasQuestionVars))
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //}

        public void PrepareDirty(ContextOfConvertingExpressionNode contextOfConvertingExpressionNode, RuleInstance ruleInstance, BaseRulePart rulePart)
        {
#if DEBUG
            //_gbcLogger.Info($"this = {this}");
            //_gbcLogger.Info($"contextOfConvertingExpressionNode = {contextOfConvertingExpressionNode}");
#endif

            RuleInstance = ruleInstance;
            RulePart = rulePart;

            switch (Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch(KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.And:
                        case KindOfOperatorOfLogicalQueryNode.Or:
                        case KindOfOperatorOfLogicalQueryNode.Is:
                        case KindOfOperatorOfLogicalQueryNode.IsNot:
                        case KindOfOperatorOfLogicalQueryNode.More:
                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                        case KindOfOperatorOfLogicalQueryNode.Less:
                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            Left.PrepareDirty(contextOfConvertingExpressionNode, ruleInstance, rulePart);
                            Right.PrepareDirty(contextOfConvertingExpressionNode, ruleInstance, rulePart);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            Left.PrepareDirty(contextOfConvertingExpressionNode, ruleInstance, rulePart);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                    Name.CheckDirty();
                    break;

                case KindOfLogicalQueryNode.LogicalVar:
                    contextOfConvertingExpressionNode.HasVars = true;
                    Name.CheckDirty();
                    break;

                case KindOfLogicalQueryNode.Value:
                case KindOfLogicalQueryNode.StubParam:
                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                    break;

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    FuzzyLogicNonNumericSequenceValue.CheckDirty();
                    break;

                case KindOfLogicalQueryNode.Relation:
                    Name.CheckDirty();
                    if (Name.KindOfName == KindOfName.QuestionVar)
                    {
                        IsQuestion = true;
                    }
                    FillRelationParams(ParamsList, contextOfConvertingExpressionNode);
                    break;

                case KindOfLogicalQueryNode.Group:
                    Left.PrepareDirty(contextOfConvertingExpressionNode, ruleInstance, rulePart);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }

#if DEBUG
            //_gbcLogger.Info($"contextOfConvertingExpressionNode (after) = {contextOfConvertingExpressionNode}");
#endif
        }

        private void FillRelationParams(IList<LogicalQueryNode> sourceParamsList, ContextOfConvertingExpressionNode contextOfConvertingExpressionNode)
        {
            CountParams = sourceParamsList.Count;

            var varsInfoList = new List<QueryExecutingCardAboutVar>();
            var knownInfoList = new List<QueryExecutingCardAboutKnownInfo>();
            var i = 0;

            foreach (var param in sourceParamsList)
            {
                param.CheckDirty();

#if DEBUG
                //_gbcLogger.Info($"param = {param}");
#endif               

                var kindOfParam = param.Kind;
                switch (kindOfParam)
                {
                    case KindOfLogicalQueryNode.Concept:
                    case KindOfLogicalQueryNode.Entity:
                    case KindOfLogicalQueryNode.Value:
                    case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    case KindOfLogicalQueryNode.Relation:
                        {
                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
                            knownInfo.Kind = kindOfParam;
                            knownInfo.Expression = param;
                            knownInfo.Position = i;
                            knownInfoList.Add(knownInfo);

#if DEBUG
                            //_gbcLogger.Info($"knownInfo = {knownInfo}");
#endif
                        }
                        break;

                    case KindOfLogicalQueryNode.LogicalVar:
                        {
                            var varInfo = new QueryExecutingCardAboutVar();
                            varInfo.NameOfVar = param.Name;
                            varInfo.Position = i;
                            varsInfoList.Add(varInfo);

                            contextOfConvertingExpressionNode.HasVars = true;
                        }
                        break;

                    case KindOfLogicalQueryNode.QuestionVar:
                        {
                            var varInfo = new QueryExecutingCardAboutVar();
                            varInfo.NameOfVar = param.Name;
                            varInfo.Position = i;
                            varsInfoList.Add(varInfo);

                            contextOfConvertingExpressionNode.HasQuestionVars = true;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParam), kindOfParam, null);
                }
                i++;
            }

            VarsInfoList = varsInfoList;
            KnownInfoList = knownInfoList;

            contextOfConvertingExpressionNode.RelationsList.Add(this);
        }

        public void CalculateUsedKeys(List<StrongIdentifierValue> usedKeysList)
        {
            switch (Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch(KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.And:
                        case KindOfOperatorOfLogicalQueryNode.Or:
                        case KindOfOperatorOfLogicalQueryNode.Is:
                        case KindOfOperatorOfLogicalQueryNode.IsNot:
                        case KindOfOperatorOfLogicalQueryNode.More:
                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                        case KindOfOperatorOfLogicalQueryNode.Less:
                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            Left.CalculateUsedKeys(usedKeysList);
                            Right.CalculateUsedKeys(usedKeysList);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            Left.CalculateUsedKeys(usedKeysList);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.LogicalVar:
                    usedKeysList.Add(Name);
                    break;

                case KindOfLogicalQueryNode.Value:
                case KindOfLogicalQueryNode.StubParam:
                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                    break;

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    usedKeysList.Add(FuzzyLogicNonNumericSequenceValue.NonNumericValue);
                    break;

                case KindOfLogicalQueryNode.Relation:
                    usedKeysList.Add(Name);

                    foreach (var param in ParamsList)
                    {
                        param.CalculateUsedKeys(usedKeysList);
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    Left.CalculateUsedKeys(usedKeysList);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
#if DEBUG
            //_gbcLogger.Info($"this = {DebugHelperForRuleInstance.ToString(this)}");
#endif

            switch (Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.And:
                        case KindOfOperatorOfLogicalQueryNode.Or:
                        case KindOfOperatorOfLogicalQueryNode.Is:
                        case KindOfOperatorOfLogicalQueryNode.IsNot:
                        case KindOfOperatorOfLogicalQueryNode.More:
                        case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                        case KindOfOperatorOfLogicalQueryNode.Less:
                        case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                            return base.CalculateLongHashCode() ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode()) ^ Left.GetLongHashCode() ^ Right.GetLongHashCode();

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            return base.CalculateLongHashCode() ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode()) ^ Left.GetLongHashCode();

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.LogicalVar:
                    return base.CalculateLongHashCode() ^ Name.GetLongHashCode();

                case KindOfLogicalQueryNode.Value:
                    return base.CalculateLongHashCode() ^ Value.GetLongHashCode();

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    return base.CalculateLongHashCode() ^ FuzzyLogicNonNumericSequenceValue.GetLongHashCode();

                case KindOfLogicalQueryNode.StubParam:
                    return LongHashCodeWeights.StubWeight ^ base.CalculateLongHashCode();

                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                    break;

                case KindOfLogicalQueryNode.Relation:
                    {
                        var result = base.CalculateLongHashCode() ^ LongHashCodeWeights.BaseFunctionWeight ^ Name.GetLongHashCode();

                        foreach (var param in ParamsList)
                        {
                            result ^= LongHashCodeWeights.BaseParamWeight ^ param.GetLongHashCode();
                        }

                        return result;
                    }

                case KindOfLogicalQueryNode.Group:
                    return base.CalculateLongHashCode() ^ LongHashCodeWeights.GroupWeight ^ Left.GetLongHashCode();

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }

            throw new NotImplementedException();
        }

        IAstNode IAstNode.Left { get => Left; set => Left = (LogicalQueryNode)value; }
        IAstNode IAstNode.Right { get => Right; set => Right = (LogicalQueryNode)value; }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public LogicalQueryNode Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public LogicalQueryNode Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (LogicalQueryNode)context[this];
            }

            var result = new LogicalQueryNode();
            context[this] = result;

            result.Kind = Kind;
            result.KindOfOperator = KindOfOperator;
            result.Name = Name?.Clone(context);
            result.Left = Left?.Clone(context);
            result.Right = Right?.Clone(context);
            result.ParamsList = ParamsList?.Select(p => p.Clone(context)).ToList();
            result.Value = Value?.CloneValue(context);
            result.FuzzyLogicNonNumericSequenceValue = FuzzyLogicNonNumericSequenceValue?.Clone(context);
            result.IsQuestion = IsQuestion;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
            Left?.DiscoverAllAnnotations(result);
            Right?.DiscoverAllAnnotations(result);

            if(!ParamsList.IsNullOrEmpty())
            {
                foreach(var item in ParamsList)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            Value?.DiscoverAllAnnotations(result);
            FuzzyLogicNonNumericSequenceValue?.DiscoverAllAnnotations(result);
        }

        public void DiscoverAllInheritanceRelations(IList<LogicalQueryNode> result)
        {
            switch(Kind)
            {
                case KindOfLogicalQueryNode.Relation:
                    if(ParamsList.Count == 1)
                    {
                        var param = ParamsList.Single();

                        if(param.Kind == KindOfLogicalQueryNode.Entity || param.Kind == KindOfLogicalQueryNode.Concept)
                        {
                            result.Add(this);
                        }
                        break;
                    }

                    if(Name == NameHelper.CreateName("is"))
                    {
                        result.Add(this);
                    }
                    break;

                case KindOfLogicalQueryNode.BinaryOperator:
                    Left.DiscoverAllInheritanceRelations(result);
                    Right.DiscoverAllInheritanceRelations(result);
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    Left.DiscoverAllInheritanceRelations(result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, string.Empty);
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjProp(n, nameof(Left), Left);
            sb.PrintObjProp(n, nameof(Right), Right);
            sb.PrintObjListProp(n, nameof(ParamsList), ParamsList);
            
            sb.PrintObjProp(n, nameof(Value), Value);
            sb.PrintObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");

            sb.PrintObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.AppendLine($"{spaces}{nameof(IsKeyRef)} = {IsKeyRef}");
            sb.AppendLine($"{spaces}{nameof(IsEntityRef)} = {IsEntityRef}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.PrintShortObjProp(n, nameof(Left), Left);
            sb.PrintShortObjProp(n, nameof(Right), Right);
            sb.PrintShortObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintShortObjProp(n, nameof(Value), Value);
            sb.PrintShortObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");

            sb.PrintShortObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintShortObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.AppendLine($"{spaces}{nameof(IsKeyRef)} = {IsKeyRef}");
            sb.AppendLine($"{spaces}{nameof(IsEntityRef)} = {IsEntityRef}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.PrintExisting(n, nameof(Left), Left);
            sb.PrintExisting(n, nameof(Right), Right);
            sb.PrintExistingList(n, nameof(ParamsList), ParamsList);

            sb.PrintBriefObjProp(n, nameof(Value), Value);
            sb.PrintBriefObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");

            sb.PrintBriefObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintBriefObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.AppendLine($"{spaces}{nameof(IsKeyRef)} = {IsKeyRef}");
            sb.AppendLine($"{spaces}{nameof(IsEntityRef)} = {IsEntityRef}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public string GetHumanizeDbgString()
        {
            return DebugHelperForRuleInstance.ToString(this);
        }
    }
}

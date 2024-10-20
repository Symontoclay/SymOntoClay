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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalQueryNode: AnnotatedItem, IAstNode, IMemberAccess, IReadOnlyMemberAccess, ILogicalSearchItem, ILogicalQueryNodeParent, IEquatable<LogicalQueryNode>
    {        
        public KindOfLogicalQueryNode Kind { get; set; } = KindOfLogicalQueryNode.Unknown;

        public bool IsExpression => Kind == KindOfLogicalQueryNode.Relation || Kind == KindOfLogicalQueryNode.Group || Kind == KindOfLogicalQueryNode.BinaryOperator || Kind == KindOfLogicalQueryNode.UnaryOperator;

        public KindOfOperatorOfLogicalQueryNode KindOfOperator { get; set; } = KindOfOperatorOfLogicalQueryNode.Unknown;
        public StrongIdentifierValue Name { get; set; }
        public LogicalQueryNode Left { get; set; }
        public LogicalQueryNode Right { get; set; }
        public IList<LogicalQueryNode> ParamsList { get; set; }
        public IList<LogicalQueryNode> LinkedVars { get; set; }
        public Value Value { get; set; }
        public FuzzyLogicNonNumericSequenceValue FuzzyLogicNonNumericSequenceValue { get; set; }
        public RuleInstance Fact { get; set; }

        public bool IsQuestion { get; set; }

        public bool IsNull { get; set; }

        public int CountParams { get; set; }
        public IList<QueryExecutingCardAboutVar> VarsInfoList { get; set; }
        public IList<QueryExecutingCardAboutKnownInfo> KnownInfoList { get; set; }

        public bool IsKeyRef => Kind == KindOfLogicalQueryNode.Concept || Kind == KindOfLogicalQueryNode.Entity || Kind == KindOfLogicalQueryNode.LogicalVar || Kind == KindOfLogicalQueryNode.QuestionVar;
        public bool IsEntityRef => Kind == KindOfLogicalQueryNode.EntityRef;

        public RuleInstance RuleInstance { get; set; }
        public BaseRulePart RulePart { get; set; }

        /// <inheritdoc/>
        public TypeOfAccess TypeOfAccess { get; set; } = TypeOfAccess.Unknown;

        /// <inheritdoc/>
        public StrongIdentifierValue Holder { get; set; }

        /// <inheritdoc/>
        public Value ObligationModality => RuleInstance.ObligationModality;

        /// <inheritdoc/>
        public Value SelfObligationModality => RuleInstance.SelfObligationModality;

        public void Accept(ILogicalVisitor logicalVisitor)
        {
            logicalVisitor.VisitLogicalQueryNode(this);
        }

        /// <inheritdoc/>
        public bool Equals(LogicalQueryNode other)
        {
            if (other == null)
            {
                return false;
            }

            if(Kind != other.Kind)
            {
                return false;
            }

            switch (Kind)
            {
                case KindOfLogicalQueryNode.Relation:
                    if(Name != other.Name)
                    {
                        return false;
                    }

                    if(ParamsList.Count != other.ParamsList.Count)
                    {
                        return false;
                    }

                    var otherParamsEnumerator = other.ParamsList.GetEnumerator();

                    foreach (var thisParam in ParamsList)
                    {
                        if(!otherParamsEnumerator.MoveNext())
                        {
                            return false;
                        }

                        if (!thisParam.Equals(otherParamsEnumerator.Current))
                        {
                            return false;
                        }
                    }

                    return true;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                    return Name.Equals(other.Name);

                case KindOfLogicalQueryNode.LogicalVar:
                    return true;

                case KindOfLogicalQueryNode.Value:
                    return Value.Equals(other.Value);

                case KindOfLogicalQueryNode.Group:
                    return Left.Equals(other.Left);

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }
        }

        public void PrepareDirty(ContextOfConvertingExpressionNode contextOfConvertingExpressionNode, RuleInstance ruleInstance, BaseRulePart rulePart)
        {
            this.CheckDirty();

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
                            Left?.PrepareDirty(contextOfConvertingExpressionNode, ruleInstance, rulePart);
                            Right?.PrepareDirty(contextOfConvertingExpressionNode, ruleInstance, rulePart);
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
                    Name.CheckDirty();
                    break;

                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                    Name.CheckDirty();
                    break;

                case KindOfLogicalQueryNode.LogicalVar:
                    contextOfConvertingExpressionNode.HasVars = true;
                    Name.CheckDirty();
                    break;

                case KindOfLogicalQueryNode.Fact:
                    Fact.CheckDirty();
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
                    if (Name.KindOfName == KindOfName.LogicalVar)
                    {
                        IsQuestion = true;
                    }
                    FillRelationParams(ParamsList, contextOfConvertingExpressionNode);
                    break;

                case KindOfLogicalQueryNode.Group:
                    Left.PrepareDirty(contextOfConvertingExpressionNode, ruleInstance, rulePart);
                    break;

                case KindOfLogicalQueryNode.Var:
                    contextOfConvertingExpressionNode.IsParameterized = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }

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

                var kindOfParam = param.Kind;
                switch (kindOfParam)
                {
                    case KindOfLogicalQueryNode.Concept:
                    case KindOfLogicalQueryNode.Entity:
                    case KindOfLogicalQueryNode.Value:
                    case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                        {
                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
                            knownInfo.Kind = kindOfParam;
                            knownInfo.Expression = param;
                            knownInfo.Position = i;
                            knownInfoList.Add(knownInfo);

                            param.CheckDirty();
                        }
                        break;

                    case KindOfLogicalQueryNode.Fact:
                        {
                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
                            knownInfo.Kind = kindOfParam;
                            knownInfo.Expression = param;
                            knownInfo.Position = i;
                            knownInfoList.Add(knownInfo);

                            param.CheckDirty();
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

                    case KindOfLogicalQueryNode.Var:
                        contextOfConvertingExpressionNode.IsParameterized = true;
                        break;

                    case KindOfLogicalQueryNode.Relation:
                        {
                            var additionalKnownInfoExpressions = new List<LogicalQueryNode>();
                            var varNames = new List<StrongIdentifierValue>();

                            foreach(var node in param.ParamsList)
                            {
                                LogicalQueryNodeHelper.FillUpInfoAboutComplexExpression(node, additionalKnownInfoExpressions, varNames);
                            }

                            if (varNames.Any())
                            {
                                throw new NotImplementedException();
                            }

                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
                            knownInfo.Kind = kindOfParam;
                            knownInfo.Expression = param;
                            knownInfo.AdditionalExpressions = additionalKnownInfoExpressions;
                            knownInfo.Position = i;
                            knownInfoList.Add(knownInfo);
                        }
                        break;

                    case KindOfLogicalQueryNode.Group: 
                        {
                            var additionalKnownInfoExpressions = new List<LogicalQueryNode>();
                            var varNames = new List<StrongIdentifierValue>();

                            LogicalQueryNodeHelper.FillUpInfoAboutComplexExpression(param.Left, additionalKnownInfoExpressions, varNames);

                            if(varNames.Any())
                            {
                                throw new NotImplementedException();
                            }

                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
                            knownInfo.Kind = kindOfParam;
                            knownInfo.Expression = param;
                            knownInfo.AdditionalExpressions = additionalKnownInfoExpressions;
                            knownInfo.Position = i;
                            knownInfoList.Add(knownInfo);
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

        public void SetHolder(StrongIdentifierValue holder)
        {
            Holder = holder;

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
                            Left?.SetHolder(holder);
                            Right?.SetHolder(holder);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            Left.SetHolder(holder);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                    break;

                case KindOfLogicalQueryNode.LogicalVar:
                    break;

                case KindOfLogicalQueryNode.Value:
                case KindOfLogicalQueryNode.StubParam:
                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                    break;

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    break;

                case KindOfLogicalQueryNode.Relation:
                    foreach (var param in ParamsList)
                    {
                        var kindOfParam = param.Kind;

                        switch (kindOfParam)
                        {
                            case KindOfLogicalQueryNode.Concept:
                            case KindOfLogicalQueryNode.Entity:
                            case KindOfLogicalQueryNode.Value:
                            case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                                break;

                            case KindOfLogicalQueryNode.Relation:
                                foreach (var subParam in param.ParamsList)
                                {
                                    subParam.SetHolder(holder);
                                }
                                break;

                            case KindOfLogicalQueryNode.LogicalVar:
                                break;

                            case KindOfLogicalQueryNode.QuestionVar:
                                break;

                            case KindOfLogicalQueryNode.Var:
                                break;

                            case KindOfLogicalQueryNode.Group:
                                param.Left.SetHolder(holder);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfParam), kindOfParam, null);
                        }
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    Left.SetHolder(holder);
                    break;

                case KindOfLogicalQueryNode.Fact:
                    Fact.Holder = holder;
                    break;

                case KindOfLogicalQueryNode.Var:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }
        }

        public void SetTypeOfAccess(TypeOfAccess typeOfAccess)
        {
            TypeOfAccess = typeOfAccess; 

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
                            Left?.SetTypeOfAccess(typeOfAccess);
                            Right?.SetTypeOfAccess(typeOfAccess);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            Left.SetTypeOfAccess(typeOfAccess);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                    break;

                case KindOfLogicalQueryNode.LogicalVar:
                    break;

                case KindOfLogicalQueryNode.Value:
                case KindOfLogicalQueryNode.StubParam:
                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                    break;

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    break;

                case KindOfLogicalQueryNode.Relation:
                    foreach (var param in ParamsList)
                    {
                        var kindOfParam = param.Kind;

                        switch (kindOfParam)
                        {
                            case KindOfLogicalQueryNode.Concept:
                            case KindOfLogicalQueryNode.Entity:
                            case KindOfLogicalQueryNode.Value:
                            case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                                break;

                            case KindOfLogicalQueryNode.Relation:
                                foreach (var subParam in param.ParamsList)
                                {
                                    subParam.SetTypeOfAccess(typeOfAccess);
                                }
                                break;

                            case KindOfLogicalQueryNode.LogicalVar:
                                break;

                            case KindOfLogicalQueryNode.QuestionVar:
                                break;

                            case KindOfLogicalQueryNode.Var:
                                break;

                            case KindOfLogicalQueryNode.Group:
                                param.Left.SetTypeOfAccess(typeOfAccess);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfParam), kindOfParam, null);
                        }
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    Left.SetTypeOfAccess(typeOfAccess);
                    break;

                case KindOfLogicalQueryNode.Fact:
                    Fact.TypeOfAccess = typeOfAccess;
                    break;

                case KindOfLogicalQueryNode.Var:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }
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
                            Left?.CalculateUsedKeys(usedKeysList);
                            Right?.CalculateUsedKeys(usedKeysList);
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
                case KindOfLogicalQueryNode.Var:
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
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
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
                            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode()) ^ (Left?.GetLongHashCode(options)??0) ^ (Right?.GetLongHashCode(options)??0);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode()) ^ Left.GetLongHashCode(options);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }

                case KindOfLogicalQueryNode.Concept:
                    {
                        var nameLongHashCode = Name.GetLongHashCode(options);
                        IsNull = Name.NullValueEquals();

                        if (IsNull)
                        {
                            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.NullWeight;
                        }
                        return base.CalculateLongHashCode(options) ^ nameLongHashCode;
                    }

                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.LogicalVar:
                    return base.CalculateLongHashCode(options) ^ Name.GetLongHashCode(options);

                case KindOfLogicalQueryNode.Value:
                    {
                        var valueLongHashCode = Value.GetLongHashCode(options);

                        IsNull = Value.NullValueEquals();

                        if (IsNull)
                        {
                            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.NullWeight;
                        }
                        return base.CalculateLongHashCode(options) ^ valueLongHashCode;
                    }

                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                    return base.CalculateLongHashCode(options) ^ FuzzyLogicNonNumericSequenceValue.GetLongHashCode(options);

                case KindOfLogicalQueryNode.StubParam:
                    return LongHashCodeWeights.StubWeight ^ base.CalculateLongHashCode(options);

                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                    break;

                case KindOfLogicalQueryNode.Var:
                    return 0;

                case KindOfLogicalQueryNode.Relation:
                    {
                        var result = base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseFunctionWeight ^ Name.GetLongHashCode(options);

                        foreach (var param in ParamsList)
                        {
                            result ^= LongHashCodeWeights.BaseParamWeight ^ param.GetLongHashCode(options);
                        }

                        return result;
                    }

                case KindOfLogicalQueryNode.Group:
                    return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.GroupWeight ^ Left.GetLongHashCode(options);

                case KindOfLogicalQueryNode.Fact:
                    return base.CalculateLongHashCode(options) ^ Fact.GetLongHashCode(options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        IAstNode IAstNode.Left { get => Left; set => Left = (LogicalQueryNode)value; }

        /// <inheritdoc/>
        IAstNode IAstNode.Right { get => Right; set => Right = (LogicalQueryNode)value; }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public LogicalQueryNode Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
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
            result.LinkedVars = LinkedVars?.Select(p => p.Clone(context)).ToList();
            result.Value = Value?.CloneValue(context);
            result.FuzzyLogicNonNumericSequenceValue = FuzzyLogicNonNumericSequenceValue?.Clone(context);
            result.Fact = Fact?.Clone(context);
            result.IsQuestion = IsQuestion;
            result.IsNull = IsNull;
            result.TypeOfAccess = TypeOfAccess;
            result.Holder = Holder;

            result.CountParams = CountParams;
            result.VarsInfoList = VarsInfoList;
            result.KnownInfoList = KnownInfoList;
            result.RuleInstance = RuleInstance;
            result.RulePart = RulePart;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
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

        public void DiscoverAllInheritanceRelations(IMonitorLogger logger, IList<LogicalQueryNode> result)
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
                    Left.DiscoverAllInheritanceRelations(logger, result);
                    Right.DiscoverAllInheritanceRelations(logger, result);
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                case KindOfLogicalQueryNode.Group:
                    Left.DiscoverAllInheritanceRelations(logger, result);
                    break;

                case KindOfLogicalQueryNode.Fact:
                    {
                        var inheritanceRelations = Fact.GetInheritanceRelations(logger);

                        if(inheritanceRelations.Any())
                        {
                            foreach(var item in inheritanceRelations)
                            {
                                result.Add(item);
                            }
                        }
                    }                    
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, string.Empty);
            }
        }

        public void DiscoverAllStandaloneConcepts(IMonitorLogger logger, List<StrongIdentifierValue> result)
        {
            switch(Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    {
                        if(Left.Kind == KindOfLogicalQueryNode.Concept)
                        {
                            result.Add(Left.Name);
                        }
                        else
                        {
                            Left.DiscoverAllStandaloneConcepts(logger, result);
                        }

                        if(Right.Kind == KindOfLogicalQueryNode.Concept)
                        {
                            result.Add(Right.Name);
                        }
                        else
                        {
                            Right.DiscoverAllStandaloneConcepts(logger, result);
                        }
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                case KindOfLogicalQueryNode.UnaryOperator:
                    {
                        if (Left.Kind == KindOfLogicalQueryNode.Concept)
                        {
                            result.Add(Left.Name);
                        }
                        else
                        {
                            Left.DiscoverAllStandaloneConcepts(logger, result);
                        }
                    }
                    break;

                case KindOfLogicalQueryNode.Relation:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, string.Empty);
            }
        }

        /// <inheritdoc/>
        public void Remove(LogicalQueryNode node)
        {
            switch (Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    if(Left == node)
                    {
                        Left = null;
                    }
                    if(Right == node)
                    {
                        Right= null;
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    if (Left == node)
                    {
                        Left = null;
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    if (Left == node)
                    {
                        Left = null;
                    }
                    break;

                case KindOfLogicalQueryNode.Relation:
                    if(ParamsList.Contains(node))
                    {
                        ParamsList.Remove(node);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }
        }

        /// <inheritdoc/>
        public void Replace(LogicalQueryNode oldNode, LogicalQueryNode newNode)
        {
            switch (Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    if (Left == oldNode)
                    {
                        Left = newNode;
                    }
                    if (Right == oldNode)
                    {
                        Right = newNode;
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    if (Left == oldNode)
                    {
                        Left = newNode;
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    if (Left == oldNode)
                    {
                        Left = newNode;
                    }
                    break;

                case KindOfLogicalQueryNode.Relation:
                    if (ParamsList.Contains(oldNode))
                    {
                        var index = ParamsList.IndexOf(oldNode);

                        ParamsList[index] = newNode;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
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
            sb.PrintObjListProp(n, nameof(LinkedVars), LinkedVars);

            sb.PrintObjProp(n, nameof(Value), Value);
            sb.PrintObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);
            sb.PrintObjProp(n, nameof(Fact), Fact);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");
            sb.AppendLine($"{spaces}{nameof(IsNull)} = {IsNull}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");            

            sb.PrintObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.AppendLine($"{spaces}{nameof(IsKeyRef)} = {IsKeyRef}");
            sb.AppendLine($"{spaces}{nameof(IsEntityRef)} = {IsEntityRef}");

            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");
            sb.PrintObjProp(n, nameof(Holder), Holder);

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
            sb.PrintShortObjListProp(n, nameof(LinkedVars), LinkedVars);

            sb.PrintShortObjProp(n, nameof(Value), Value);
            sb.PrintShortObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);
            sb.PrintShortObjProp(n, nameof(Fact), Fact);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");
            sb.AppendLine($"{spaces}{nameof(IsNull)} = {IsNull}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");

            sb.PrintShortObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintShortObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.AppendLine($"{spaces}{nameof(IsKeyRef)} = {IsKeyRef}");
            sb.AppendLine($"{spaces}{nameof(IsEntityRef)} = {IsEntityRef}");

            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");
            sb.PrintShortObjProp(n, nameof(Holder), Holder);

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
            sb.PrintExisting(n, nameof(ParamsList), ParamsList);
            sb.PrintExisting(n, nameof(LinkedVars), LinkedVars);

            sb.PrintBriefObjProp(n, nameof(Value), Value);
            sb.PrintBriefObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);
            sb.PrintBriefObjProp(n, nameof(Fact), Fact);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");
            sb.AppendLine($"{spaces}{nameof(IsNull)} = {IsNull}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");

            sb.PrintBriefObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintBriefObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.AppendLine($"{spaces}{nameof(IsKeyRef)} = {IsKeyRef}");
            sb.AppendLine($"{spaces}{nameof(IsEntityRef)} = {IsEntityRef}");

            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");
            sb.PrintBriefObjProp(n, nameof(Holder), Holder);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return DebugHelperForRuleInstance.ToString(this, options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }
    }
}

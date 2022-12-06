/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalQueryNode: AnnotatedItem, IAstNode, IMemberAccess, IReadOnlyMemberAccess, ILogicalSearchItem, ILogicalQueryNodeParent
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

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

                    case KindOfLogicalQueryNode.Fact:
                        {
                            var knownInfo = new QueryExecutingCardAboutKnownInfo();
                            knownInfo.Kind = kindOfParam;
                            knownInfo.Expression = param;
                            knownInfo.Position = i;
                            knownInfoList.Add(knownInfo);

#if DEBUG
                            //_gbcLogger.Info($"knownInfo = {knownInfo}");
#endif

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
#if DEBUG
                            //_gbcLogger.Info($"param = {param.ToHumanizedString()}");
#endif

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
#if DEBUG
                            //_gbcLogger.Info($"param = {param.ToHumanizedString()}");
#endif
                            var additionalKnownInfoExpressions = new List<LogicalQueryNode>();
                            var varNames = new List<StrongIdentifierValue>();

                            LogicalQueryNodeHelper.FillUpInfoAboutComplexExpression(param.Left, additionalKnownInfoExpressions, varNames);

#if DEBUG
                            //_gbcLogger.Info($"additionalKnownInfoExpressions = {additionalKnownInfoExpressions.Select(p => p.ToHumanizedString()).WritePODListToString()}");
                            //_gbcLogger.Info($"varNames = {varNames.Select(p => p.NameValue).WritePODListToString()}");
#endif

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

        public void ResolveVariables(IPackedVarsResolver varsResolver)
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
                            Left.ResolveVariables(varsResolver);
                            Right.ResolveVariables(varsResolver);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(KindOfOperator), KindOfOperator, null);
                    }
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    switch (KindOfOperator)
                    {
                        case KindOfOperatorOfLogicalQueryNode.Not:
                            Left.ResolveVariables(varsResolver);
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
                case KindOfLogicalQueryNode.Fact:
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
                                    subParam.ResolveVariables(varsResolver);
                                }
                                break;

                            case KindOfLogicalQueryNode.LogicalVar:
                                break;

                            case KindOfLogicalQueryNode.QuestionVar:
                                break;

                            case KindOfLogicalQueryNode.Var:
                                {
#if DEBUG
                                    //_gbcLogger.Info($"param = {param}");
#endif

                                    var value = varsResolver.GetVarValue(param.Name);

#if DEBUG
                                    //_gbcLogger.Info($"value = {value}");
#endif
                                    if(value.IsStrongIdentifierValue)
                                    {
                                        var strVal = value.AsStrongIdentifierValue;

                                        var kindOfName = strVal.KindOfName;

                                        switch (kindOfName)
                                        {
                                            case KindOfName.Concept:
                                                param.Kind = KindOfLogicalQueryNode.Concept;
                                                param.Name = strVal;
                                                break;

                                            case KindOfName.Entity:
                                                param.Kind = KindOfLogicalQueryNode.Entity;
                                                param.Name = strVal;
                                                break;

                                            default:
                                                throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
                                        }
                                    }
                                    else
                                    {
                                        param.Kind = KindOfLogicalQueryNode.Value;
                                        param.Name = null;
                                        param.Value = value;
                                    }

#if DEBUG
                                    //_gbcLogger.Info($"param (after) = {param}");
#endif
                                }
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfParam), kindOfParam, null);
                        }
                    }
                    break;

                case KindOfLogicalQueryNode.Group:
                    Left.ResolveVariables(varsResolver);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }
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
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.LogicalVar:
                    return base.CalculateLongHashCode(options) ^ Name.GetLongHashCode(options);

                case KindOfLogicalQueryNode.Value:
                    return base.CalculateLongHashCode(options) ^ Value.GetLongHashCode(options);

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
            result.LinkedVars = LinkedVars?.Select(p => p.Clone(context)).ToList();
            result.Value = Value?.CloneValue(context);
            result.FuzzyLogicNonNumericSequenceValue = FuzzyLogicNonNumericSequenceValue?.Clone(context);
            result.Fact = Fact?.Clone(context);
            result.IsQuestion = IsQuestion;
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
                case KindOfLogicalQueryNode.Group:
                    Left.DiscoverAllInheritanceRelations(result);
                    break;

                case KindOfLogicalQueryNode.Fact:
                    {
                        var inheritanceRelations = Fact.GetInheritanceRelations();

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

        public void DiscoverAllStandaloneConcepts(List<StrongIdentifierValue> result)
        {
            switch(Kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    {
                        if(Left.Kind == KindOfLogicalQueryNode.Concept)
                        {
                            result.Add(Left.Name);
                        }

                        if(Right.Kind == KindOfLogicalQueryNode.Concept)
                        {
                            result.Add(Right.Name);
                        }

                        throw new NotImplementedException();
                    }

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
            sb.PrintExistingList(n, nameof(ParamsList), ParamsList);
            sb.PrintExistingList(n, nameof(LinkedVars), LinkedVars);

            sb.PrintBriefObjProp(n, nameof(Value), Value);
            sb.PrintBriefObjProp(n, nameof(FuzzyLogicNonNumericSequenceValue), FuzzyLogicNonNumericSequenceValue);
            sb.PrintBriefObjProp(n, nameof(Fact), Fact);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");
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
    }
}

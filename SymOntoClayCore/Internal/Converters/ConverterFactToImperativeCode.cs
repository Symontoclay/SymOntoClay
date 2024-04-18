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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.Converters
{
    public class ConverterFactToImperativeCode : BaseLoggedComponent
    {
        public ConverterFactToImperativeCode(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
            _compiler = context.Compiler;
            _relationsResolver = context.DataResolversFactory.GetRelationsResolver();

            _actName = NameHelper.CreateName("act");
        }

        private readonly IMainStorageContext _context;
        private readonly ICompiler _compiler;
        private readonly RelationsResolver _relationsResolver;
        private readonly StrongIdentifierValue _actName;

        public CompiledFunctionBody Convert(IMonitorLogger logger, RuleInstance ruleInstance, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var kindOfRuleInstance = ruleInstance.KindOfRuleInstance;

            List<AstStatement> statements = null;

            switch (kindOfRuleInstance)
            {
                case KindOfRuleInstance.Fact:
                    statements = ConvertFact(logger, ruleInstance, localCodeExecutionContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfRuleInstance), kindOfRuleInstance, null);
            }

            return _compiler.Compile(statements);
        }

        private List<AstStatement> ConvertFact(IMonitorLogger logger, RuleInstance fact, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var nodesList = GetSignificantNodesFromFact(logger, fact, localCodeExecutionContext);

            if(!nodesList.Any())
            {
                return new List<AstStatement>();
            }

            var result = new List<AstStatement>();

            foreach (var node in nodesList)
            {
                var kind = node.Kind;

                switch(kind)
                {
                    case KindOfLogicalQueryNode.Relation:
                        result.Add(ConvertRelation(logger, node, fact, localCodeExecutionContext));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }

            return result;
        }

        private AstStatement ConvertRelation(IMonitorLogger logger, LogicalQueryNode relation, RuleInstance fact, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var statement = new AstExpressionStatement();
            var callExpression = new CallingFunctionAstExpression();
            statement.Expression = callExpression;
            callExpression.AppendAnnotations(fact);

            var functionNameExpr = new ConstValueAstExpression();
            callExpression.Left = functionNameExpr;
            functionNameExpr.Value = relation.Name;

            var callExpressionParameters = callExpression.Parameters;

            foreach (var parameter in relation.ParamsList)
            {
                var kindOfParameter = parameter.Kind;

                switch(kindOfParameter)
                {
                    case KindOfLogicalQueryNode.Concept:
                        {
                            var conceptNameStr = parameter.Name.NormalizedNameValue;

                            if (conceptNameStr == "someone" || conceptNameStr == "self")
                            {
                                break;
                            }

                            throw new NotImplementedException();
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameter), kindOfParameter, null);
                }
            }

            var linkedVars = relation.LinkedVars;

            if(!linkedVars.IsNullOrEmpty())
            {
                foreach(var linkedVar in linkedVars)
                {
                    var linkedVarName = linkedVar.Name;

                    var relationParametersList = GetNonActRelationsWithLogicalVarInFirstParameter(logger, linkedVarName, fact, relation, localCodeExecutionContext);

                    foreach(var relationParameter in relationParametersList)
                    {
                        var secondParameter = relationParameter.ParamsList[1];

                        var kindOfSecondParameter = secondParameter.Kind;

                        switch(kindOfSecondParameter)
                        {
                            case KindOfLogicalQueryNode.Concept:
                            case KindOfLogicalQueryNode.Entity:
                                {
                                    var parameterExpression = new CallingParameter();
                                    var parameterNameExpression = new ConstValueAstExpression();

                                    parameterExpression.Name = parameterNameExpression;
                                    parameterNameExpression.Value = relationParameter.Name;

                                    var parameterValueExpression = new ConstValueAstExpression();
                                    parameterExpression.Value = parameterValueExpression;

                                    parameterValueExpression.Value = secondParameter.Name;

                                    callExpressionParameters.Add(parameterExpression);
                                }
                                break;

                            case KindOfLogicalQueryNode.Value:
                                {
                                    var parameterExpression = new CallingParameter();
                                    var parameterNameExpression = new ConstValueAstExpression();

                                    parameterExpression.Name = parameterNameExpression;
                                    parameterNameExpression.Value = relationParameter.Name;

                                    var parameterValueExpression = new ConstValueAstExpression();
                                    parameterExpression.Value = parameterValueExpression;

                                    parameterValueExpression.Value = secondParameter.Value;

                                    callExpressionParameters.Add(parameterExpression);
                                }
                                break;

                            /*
                            case KindOfLogicalQueryNode.EntityCondition:
                            case KindOfLogicalQueryNode.EntityRef:
                            case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                            case KindOfLogicalQueryNode.Fact:
                             */

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfSecondParameter), kindOfSecondParameter, null);
                        }
                    }
                }
            }

            return statement;
        }

        private List<LogicalQueryNode> GetNonActRelationsWithLogicalVarInFirstParameter(IMonitorLogger logger, StrongIdentifierValue variableName, RuleInstance fact, LogicalQueryNode processedAction, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = new List<LogicalQueryNode>();

            GetNonActRelationsWithLogicalVarInFirstParameter(logger, variableName, fact.PrimaryPart.Expression, processedAction, result, localCodeExecutionContext);

            return result;
        }

        private void GetNonActRelationsWithLogicalVarInFirstParameter(IMonitorLogger logger, StrongIdentifierValue variableName, LogicalQueryNode node, LogicalQueryNode processedAction, List<LogicalQueryNode> result, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var kind = node.Kind;

            switch(kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    GetNonActRelationsWithLogicalVarInFirstParameter(logger, variableName, node.Left, processedAction, result, localCodeExecutionContext);
                    GetNonActRelationsWithLogicalVarInFirstParameter(logger, variableName, node.Right, processedAction, result, localCodeExecutionContext);
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                case KindOfLogicalQueryNode.Group:
                    GetNonActRelationsWithLogicalVarInFirstParameter(logger, variableName, node.Left, processedAction, result, localCodeExecutionContext);
                    break;

                case KindOfLogicalQueryNode.Relation:
                    {
                        if(node == processedAction)
                        {
                            break;
                        }

                        var paramsList = node.ParamsList;

                        var paramsCount = paramsList.Count;

                        var relationInfo = _relationsResolver.GetRelation(logger, node.Name, paramsCount, localCodeExecutionContext);

                        var isAct = relationInfo.InheritanceItems.Any(p => p.SuperName == _actName);

                        if(isAct)
                        {
                            break;
                        }

                        switch (paramsCount)
                        {
                            case 1:
                                break;

                            case 2:
                                {
                                    var firstParam = paramsList[0];

                                    if(firstParam.Kind == KindOfLogicalQueryNode.LogicalVar && firstParam.Name == variableName)
                                    {
                                        result.Add(node);
                                        break;
                                    }

                                    foreach(var parameter in paramsList)
                                    {
                                        GetNonActRelationsWithLogicalVarInFirstParameter(logger, variableName, parameter, processedAction, result, localCodeExecutionContext);
                                    }
                                }
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(paramsCount), paramsCount, null);
                        }                        
                    }
                    break;

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.QuestionVar:
                case KindOfLogicalQueryNode.Value:
                case KindOfLogicalQueryNode.StubParam:
                case KindOfLogicalQueryNode.EntityCondition:
                case KindOfLogicalQueryNode.EntityRef:
                case KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence:
                case KindOfLogicalQueryNode.Fact:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private List<LogicalQueryNode> GetSignificantNodesFromFact(IMonitorLogger logger, RuleInstance fact, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = new List<LogicalQueryNode>();

            var rootRelationsList = GetRootRelationsFromFact(logger, fact);

            foreach (var relation in rootRelationsList)
            {
                var relationInfo = _relationsResolver.GetRelation(logger, relation.Name, relation.ParamsList.Count, localCodeExecutionContext);

                if(relationInfo == null)
                {
                    continue;
                }

                var isAct = relationInfo.InheritanceItems.Any(p => p.SuperName == _actName);

                if(isAct)
                {
                    result.Add(relation);
                }
            }

            return result;
        }

        private List<LogicalQueryNode> GetRootRelationsFromFact(IMonitorLogger logger, RuleInstance fact)
        {
            var result = new List<LogicalQueryNode>();

            GetRootRelationsFromFact(logger, fact.PrimaryPart.Expression, result);

            return result;
        }

        private void GetRootRelationsFromFact(IMonitorLogger logger, LogicalQueryNode node, List<LogicalQueryNode> result)
        {
            var kind = node.Kind;

            switch(kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    GetRootRelationsFromFact(logger, node.Left, result);
                    GetRootRelationsFromFact(logger, node.Right, result);
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                case KindOfLogicalQueryNode.Group:
                    GetRootRelationsFromFact(logger, node.Left, result);
                    break;

                case KindOfLogicalQueryNode.Relation:
                    result.Add(node);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}

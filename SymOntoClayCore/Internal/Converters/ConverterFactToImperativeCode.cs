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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        }

        private readonly IMainStorageContext _context;
        private readonly ICompiler _compiler;
        private readonly RelationsResolver _relationsResolver;

        public CompiledFunctionBody Convert(RuleInstance ruleInstance, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var kindOfRuleInstance = ruleInstance.KindOfRuleInstance;

            List<AstStatement> statements = null;

            switch (kindOfRuleInstance)
            {
                case KindOfRuleInstance.Fact:
                    statements = ConvertFact(ruleInstance, localCodeExecutionContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfRuleInstance), kindOfRuleInstance, null);
            }

#if DEBUG
            Log($"statements = {statements.WriteListToToHumanizedString()}");
#endif

            return _compiler.Compile(statements);
        }

        private List<AstStatement> ConvertFact(RuleInstance fact, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"fact = {fact.ToHumanizedString()}");
#endif

            var nodesList = GetSignificantNodesFromFact(fact, localCodeExecutionContext);

#if DEBUG
            Log($"nodesList = {nodesList.WriteListToToHumanizedString()}");
#endif

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
                        result.Add(ConvertRelation(node, fact, localCodeExecutionContext));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }

            return result;
        }

        private AstStatement ConvertRelation(LogicalQueryNode node, RuleInstance fact, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"node = {node.ToHumanizedString()}");
            Log($"fact = {fact.ToHumanizedString()}");
            //Log($"node = {node}");
#endif

            var statement = new AstExpressionStatement();
            var callExpression = new CallingFunctionAstExpression();
            statement.Expression = callExpression;

            var functionNameExpr = new ConstValueAstExpression();
            callExpression.Left = functionNameExpr;
            functionNameExpr.Value = node.Name;

            foreach (var parameter in node.ParamsList)
            {
#if DEBUG
                //Log($"parameter = {parameter.ToHumanizedString()}");
                //Log($"parameter = {parameter}");
#endif

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

            var linkedVars = node.LinkedVars;

            if(!linkedVars.IsNullOrEmpty())
            {
                foreach(var linkedVar in linkedVars)
                {
#if DEBUG
                    Log($"linkedVar = {linkedVar.ToHumanizedString()}");
                    //Log($"linkedVar = {linkedVar}");
#endif

                    var linkedVarName = linkedVar.Name;

#if DEBUG
                    Log($"linkedVarName = {linkedVarName}");
#endif

                    var relationsList = GetNonActRelationsWithLogicalVarInFirstParameter(linkedVarName, fact, localCodeExecutionContext);

#if DEBUG
                    Log($"relationsList = {relationsList.WriteListToToHumanizedString()}");
#endif
                }

                throw new NotImplementedException();
            }

#if DEBUG
            Log($"statement = {statement.ToHumanizedString()}");
#endif

            throw new NotImplementedException();
        }

        private List<LogicalQueryNode> GetNonActRelationsWithLogicalVarInFirstParameter(StrongIdentifierValue variableName, RuleInstance fact, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"variableName = {variableName}");
            Log($"fact = {fact.ToHumanizedString()}");
#endif

            throw new NotImplementedException();
        }

        private void GetNonActRelationsWithLogicalVarInFirstParameter()
        {
            d
        }

        private List<LogicalQueryNode> GetSignificantNodesFromFact(RuleInstance fact, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"fact = {fact.ToHumanizedString()}");
#endif

            var result = new List<LogicalQueryNode>();

            var rootRelationsList = GetRootRelationsFromFact(fact);

#if DEBUG
            //Log($"rootRelationsList = {rootRelationsList.WriteListToToHumanizedString()}");
#endif

            var actName = NameHelper.CreateName("act");

            foreach (var relation in rootRelationsList)
            {
                var relationInfo = _relationsResolver.GetRelation(relation.Name, relation.ParamsList.Count, localCodeExecutionContext);

#if DEBUG
                //Log($"relationInfo = {relationInfo}");
#endif

                var isAct = relationInfo.InheritanceItems.Any(p => p.SuperName == actName);

#if DEBUG
                //Log($"isAct = {isAct}");
#endif

                if(isAct)
                {
                    result.Add(relation);
                }
            }

            return result;
        }

        private List<LogicalQueryNode> GetRootRelationsFromFact(RuleInstance fact)
        {
#if DEBUG
            //Log($"fact = {fact.ToHumanizedString()}");
#endif

            var result = new List<LogicalQueryNode>();

            GetRootRelationsFromFact(fact.PrimaryPart.Expression, result);

            return result;
        }

        private void GetRootRelationsFromFact(LogicalQueryNode node, List<LogicalQueryNode> result)
        {
#if DEBUG
            //Log($"node = {node.ToHumanizedString()}");
#endif

            var kind = node.Kind;

            switch(kind)
            {
                case KindOfLogicalQueryNode.BinaryOperator:
                    GetRootRelationsFromFact(node.Left, result);
                    GetRootRelationsFromFact(node.Right, result);
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                case KindOfLogicalQueryNode.Group:
                    GetRootRelationsFromFact(node.Left, result);
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

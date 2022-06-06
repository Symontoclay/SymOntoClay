using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ConceptFromVarResolver
    {
        public ConceptFromVarResolver(StrongIdentifierValue name, ContextOfConverterFactToInternalCG context)
        {
            _name = name;
            _context = context;
            _logger = context.Logger;
            _visitedRelations = context.VisitedRelations;
        }

        private readonly StrongIdentifierValue _name;
        private readonly ContextOfConverterFactToInternalCG _context;
        private readonly Dictionary<LogicalQueryNode, ResultOfNode> _visitedRelations;
        private readonly IEntityLogger _logger;

        public string Resolve()
        {
#if DEBUG
            //_logger.Log($"_name = {_name}");
#endif

            var resolvingContext = new ContextOfConceptFromVarResolver();

            ProcessExpression(_context.CurrentRulePart.Expression, resolvingContext);

#if DEBUG
            //_logger.Log($"resolvingContext.FoundItems = {resolvingContext.FoundItems.WriteListToString()}");
#endif

            if(resolvingContext.FoundItems.Count == 1)
            {
                return ProcessTargetResultItem(resolvingContext.FoundItems[0]);
            }

            throw new NotImplementedException();
        }

        private string ProcessTargetResultItem(ResultItemOfConceptFromVarResolver resultItem)
        {
#if DEBUG
            //_logger.Log($"resultItem = {resultItem}");
#endif

            var relation = resultItem.Relation;

            var result = new ResultOfNode();
            result.KindOfResult = KindOfResultOfNode.ResolveVar;
            result.LogicalQueryNode = relation;

            if(relation.ParamsList.Count == 1)
            {
                _context.VisitedRelations[relation] = result;
            }            

            return resultItem.Name.NameValue;
        }

        private void ProcessExpression(LogicalQueryNode expr, ContextOfConceptFromVarResolver resolvingContext)
        {
#if DEBUG
            //_logger.Log($"expr = {expr.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var kind = expr.Kind;

            switch (kind)
            {
                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.Value:
                case KindOfLogicalQueryNode.LogicalVar:
                    break;

                case KindOfLogicalQueryNode.Relation:
                    ProcessRelation(expr, resolvingContext);
                    break;

                case KindOfLogicalQueryNode.BinaryOperator:
                    ProcessExpression(expr.Left, resolvingContext);
                    ProcessExpression(expr.Right, resolvingContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private void ProcessRelation(LogicalQueryNode relation, ContextOfConceptFromVarResolver resolvingContext)
        {
#if DEBUG
            //_logger.Log($"relation = {relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            if (_visitedRelations.ContainsKey(relation))
            {
                return;
            }

            if(!relation.LinkedVars.IsNullOrEmpty())
            {
                foreach(var linkedVar in relation.LinkedVars)
                {
#if DEBUG
                    //_logger.Log($"linkedVar = {linkedVar}");
#endif

                    if(linkedVar.Name == _name)
                    {
                        resolvingContext.FoundItems.Add(new ResultItemOfConceptFromVarResolver()
                        {
                            Name = relation.Name,
                            Relation = relation
                        });
                    }
                }
            }

            if (relation.ParamsList.Count == 1)
            {
                ProcessInheritanceRelation(relation, resolvingContext);
                return;
            }

            foreach(var param in relation.ParamsList)
            {
                ProcessExpression(param, resolvingContext);
            }
        }

        private void ProcessInheritanceRelation(LogicalQueryNode relation, ContextOfConceptFromVarResolver resolvingContext)
        {
#if DEBUG
            //_logger.Log($"relation = {relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var param = relation.ParamsList[0];

#if DEBUG
            //_logger.Log($"param = {param}");
            //_logger.Log($"param = {param.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            if (param.Kind != KindOfLogicalQueryNode.LogicalVar)
            {
                return;
            }

            if(param.Name != _name)
            {
                return;
            }

            resolvingContext.FoundItems.Add(new ResultItemOfConceptFromVarResolver()
            {
                Name = relation.Name,
                Relation = relation
            });
        }
    }
}

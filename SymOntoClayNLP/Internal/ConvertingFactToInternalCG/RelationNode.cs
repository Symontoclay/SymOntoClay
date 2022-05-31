using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class RelationNode
    {
        public RelationNode(LogicalQueryNode relation, ContextOfConverterFactToInternalCG context)
        {
            _relation = relation;
            _context = context;
            _logger = context.Logger;

            var nlpContext = context.NLPContext;

            _relationsResolver = nlpContext.RelationsResolver;
            _inheritanceResolver = nlpContext.InheritanceResolver;
        }

        private readonly LogicalQueryNode _relation;
        private readonly ContextOfConverterFactToInternalCG _context;
        private readonly IPackedRelationsResolver _relationsResolver;
        private readonly IPackedInheritanceResolver _inheritanceResolver;
        private readonly IEntityLogger _logger;

        public ResultOfNode Run()
        {
#if DEBUG
            //_logger.Log($"_relation = {_relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            if(_context.VisitedRelations.ContainsKey(_relation))
            {
                return _context.VisitedRelations[_relation];
            }

            if(_relation.ParamsList.Count == 1)
            {
                return ProcessInheritanceRelation();
            }

            var relationName = _relation.Name.NormalizedNameValue;

#if DEBUG
            //_logger.Log($"relationName = '{relationName}'");
#endif

            switch (relationName)
            {
                case SpecialNamesOfRelations.PossessName:
                    return ProcessPossessRelation();

                default:
                    return ProcessRelation();
            }
        }

        private ResultOfNode ProcessPossessRelation()
        {
            var relationName = _relation.Name;

            var relationDescription = _relationsResolver.GetRelation(relationName, _relation.ParamsList.Count);

            if (relationDescription == null)
            {
                throw new Exception($"Relation `{_relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}` is not described!");
            }

            var result = new ResultOfNode();
            _context.VisitedRelations[_relation] = result;
            result.LogicalQueryNode = _relation;
            result.KindOfResult = KindOfResultOfNode.ProcessRelation;

#if DEBUG
            //_logger.Log($"relationDescription = {relationDescription}");
            //_logger.Log($"relationDescription = {relationDescription.ToHumanizedString()}");
#endif

            var relationRelation = new InternalRelationCGNode() { Name = relationName.NameValue, Parent = _context.ConceptualGraph };

            result.CGNode = relationRelation;

            var n = 0;

            foreach (var param in _relation.ParamsList)
            {
                var paramResult = LogicalQueryNodeProcessorFactory.Run(param, _context);

#if DEBUG
                //_logger.Log($"paramResult = {paramResult}");
#endif

                var paramDescription = relationDescription.Arguments[n];
                n++;

#if DEBUG
                //_logger.Log($"paramDescription = {paramDescription}");
                //_logger.Log($"paramDescription = {paramDescription.ToHumanizedString()}");
#endif

                var meaningRolesList = paramDescription.MeaningRolesList.Select(p => p.NormalizedNameValue);

#if DEBUG
                //_logger.Log($"meaningRolesList = {meaningRolesList.WritePODListToString()}");
#endif

                if (meaningRolesList.Contains("owner"))
                {
                    var ownerConcept = paramResult.CGNode.AsGraphOrConceptNode;

                    relationRelation.AddInputNode(ownerConcept);
                }
                else
                {
                    if (meaningRolesList.Contains("possessions"))
                    {
                        var possessionsConcept = paramResult.CGNode.AsGraphOrConceptNode;

                        possessionsConcept.AddInputNode(relationRelation);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

#if DEBUG
            //var dotStr = DotConverter.ConvertToString(_context.ConceptualGraph);
            //_logger.Log($"dotStr = {dotStr}");
#endif

            return result;
        }

        private ResultOfNode ProcessRelation()
        {
            var relationName = _relation.Name;

#if DEBUG
            //_logger.Log($"relationName = {relationName}");
#endif

            var relationDescription = _relationsResolver.GetRelation(relationName, _relation.ParamsList.Count);

            if (relationDescription == null)
            {
                throw new Exception($"Relation `{_relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}` is not described!");
            }
            
#if DEBUG
            //_logger.Log($"relationDescription = {relationDescription}");
            //_logger.Log($"relationDescription = {relationDescription.ToHumanizedString()}");
#endif

            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(relationName);

#if DEBUG
            //_logger.Log($"superClassesList = {superClassesList.WriteListToString()}");
#endif

            var isState = superClassesList.Any(p => p.NormalizedNameValue == "state");
            var isAct = superClassesList.Any(p => p.NormalizedNameValue == "act");
            var isEvent = superClassesList.Any(p => p.NormalizedNameValue == "event");

#if DEBUG
            //_logger.Log($"isState = {isState}");
            //_logger.Log($"isAct = {isAct}");
            //_logger.Log($"isEvent = {isEvent}");
#endif

            if(isState || isAct || isEvent)
            {
                return ProcessStateActOrEventRelation(relationDescription, superClassesList);
            }

            var result = new ResultOfNode();
            _context.VisitedRelations[_relation] = result;
            result.LogicalQueryNode = _relation;
            result.KindOfResult = KindOfResultOfNode.ProcessRelation;

            var relationRelation = new InternalRelationCGNode { Name = relationName.NameValue, Parent = _context.ConceptualGraph };

            result.CGNode = relationRelation;

            var n = 0;

            foreach (var param in _relation.ParamsList)
            {
                var paramResult = LogicalQueryNodeProcessorFactory.Run(param, _context);

#if DEBUG
                //_logger.Log($"paramResult = {paramResult}");
#endif

                var paramDescription = relationDescription.Arguments[n];
                n++;

#if DEBUG
                //_logger.Log($"paramDescription = {paramDescription.ToHumanizedString()}");
#endif

                var meaningRolesList = paramDescription.MeaningRolesList.Select(p => p.NormalizedNameValue);

#if DEBUG
                //_logger.Log($"meaningRolesList = {meaningRolesList.WritePODListToString()}");
#endif

                if(meaningRolesList.Contains("owner"))
                {
                    var targetConcept = paramResult.CGNode.AsGraphOrConceptNode;

                    relationRelation.AddInputNode(targetConcept);
                }
                else
                {
                    var targetConcept = paramResult.CGNode.AsGraphOrConceptNode;
                    targetConcept.AddInputNode(relationRelation);
                }
            }

#if DEBUG
            //var dotStr = DotConverter.ConvertToString(_context.ConceptualGraph);
            //_logger.Log($"dotStr = {dotStr}");
#endif

            return result;
        }

        private ResultOfNode ProcessStateActOrEventRelation(RelationDescription relationDescription, IList<StrongIdentifierValue> superClassesList)
        {
            var relationName = _relation.Name;

            var isState = superClassesList.Any(p => p.NormalizedNameValue == "state");
            var isAct = superClassesList.Any(p => p.NormalizedNameValue == "act");
            var isEvent = superClassesList.Any(p => p.NormalizedNameValue == "event");

            var result = new ResultOfNode();
            _context.VisitedRelations[_relation] = result;
            result.LogicalQueryNode = _relation;
            result.KindOfResult = KindOfResultOfNode.ProcessRelation;

            var relationConcept = new InternalConceptCGNode() { Name = relationName.NameValue, Parent = _context.ConceptualGraph };

            result.CGNode = relationConcept;

            var n = 0;

            foreach (var param in _relation.ParamsList)
            {
                var paramResult = LogicalQueryNodeProcessorFactory.Run(param, _context);

#if DEBUG
                //_logger.Log($"paramResult = {paramResult}");
#endif

                var paramDescription = relationDescription.Arguments[n];
                n++;

#if DEBUG
                //_logger.Log($"paramDescription = {paramDescription.ToHumanizedString()}");
#endif

                var meaningRolesList = paramDescription.MeaningRolesList.Select(p => p.NormalizedNameValue);

#if DEBUG
                _logger.Log($"meaningRolesList = {meaningRolesList.WritePODListToString()}");
#endif

                if (isState && meaningRolesList.Contains("experiencer"))
                {
                    var targetConcept = paramResult.CGNode.AsGraphOrConceptNode;

                    var experiencerRelation = new InternalRelationCGNode() { Name = "experiencer", Parent = _context.ConceptualGraph };
                    experiencerRelation.AddInputNode(relationConcept);
                    targetConcept.AddInputNode(experiencerRelation);

                    var stateRelation = new InternalRelationCGNode() { Name = "state", Parent = _context.ConceptualGraph };
                    stateRelation.AddInputNode(targetConcept);
                    relationConcept.AddInputNode(stateRelation);
                }
                else
                {
                    if (meaningRolesList.Contains("object"))
                    {
                        var targetConcept = paramResult.CGNode.AsGraphOrConceptNode;

                        var objectRelation = new InternalRelationCGNode() { Name = "object", Parent = _context.ConceptualGraph };
                        objectRelation.AddInputNode(relationConcept);
                        targetConcept.AddInputNode(objectRelation);
                    }
                    else
                    {
#if DEBUG
                        //var dotStr_1 = DotConverter.ConvertToString(_context.ConceptualGraph);
                        //_logger.Log($"dotStr_1 = {dotStr_1}");
#endif

                        throw new NotImplementedException();
                    }
                }
            }

#if DEBUG
            //var dotStr = DotConverter.ConvertToString(_context.ConceptualGraph);
            //_logger.Log($"dotStr = {dotStr}");
#endif

            return result;
        }

        private ResultOfNode ProcessInheritanceRelation()
        {
            throw new NotImplementedException();
        }
    }
}

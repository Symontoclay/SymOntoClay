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
            _logger.Log($"_relation = {_relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            if(_context.VisitedRelations.ContainsKey(_relation))
            {
                return _context.VisitedRelations[_relation];
            }

            var result = new ResultOfNode();

            _context.VisitedRelations[_relation] = result;

            var relationName = _relation.Name;

            var relationDescription = _relationsResolver.GetRelation(relationName, _relation.ParamsList.Count);

            if(relationDescription == null)
            {
                throw new Exception($"Relation `{_relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}` is not described!");
            }

#if DEBUG
            //_logger.Log($"relationDescription = {relationDescription}");
            _logger.Log($"relationDescription = {relationDescription.ToHumanizedString()}");
#endif

            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(relationName);

#if DEBUG
            _logger.Log($"superClassesList = {superClassesList.WriteListToString()}");
#endif

            var isState = superClassesList.Any(p => p.NormalizedNameValue == "state");
            var isAct = superClassesList.Any(p => p.NormalizedNameValue == "act");
            var isEvent = superClassesList.Any(p => p.NormalizedNameValue == "event");

#if DEBUG
            _logger.Log($"isState = {isState}");
            _logger.Log($"isAct = {isAct}");
            _logger.Log($"isEvent = {isEvent}");
#endif

            result.KindOfResult = KindOfResultOfNode.ProcessRelation;

            var relationConcept = new InternalConceptCGNode() { Name = relationName.NameValue, Parent = _context.ConceptualGraph };

            var n = 0;

            foreach (var param in _relation.ParamsList)
            {
                var paramResult = LogicalQueryNodeProcessorFactory.Run(param, _context);

#if DEBUG
                _logger.Log($"paramResult = {paramResult}");
#endif

                var paramDescription = relationDescription.Arguments[n];
                n++;

#if DEBUG
                _logger.Log($"paramDescription = {paramDescription.ToHumanizedString()}");
#endif
            }

#if DEBUG
            var dotStr = DotConverter.ConvertToString(_context.ConceptualGraph);
            _logger.Log($"dotStr = {dotStr}");
#endif

            throw new NotImplementedException();
        }
    }
}

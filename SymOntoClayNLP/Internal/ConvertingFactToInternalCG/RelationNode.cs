using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
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
            _logger.Log($"_relation = {DebugHelperForRuleInstance.ToString(_relation, HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var result = new ResultOfNode();

            var relationDescription = _relationsResolver.GetRelation(_relation.Name, _relation.ParamsList.Count);

#if DEBUG
            //_logger.Log($"relationDescription = {relationDescription}");
            _logger.Log($"relationDescription = {relationDescription.ToHumanizedString()}");
#endif

            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(_relation.Name);

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

            foreach(var param in _relation.ParamsList)
            {
                var paramResult = LogicalQueryNodeProcessorFactory.Run(param, _context);

#if DEBUG
                _logger.Log($"paramResult = {paramResult}");
#endif
            }

            throw new NotImplementedException();
        }
    }
}

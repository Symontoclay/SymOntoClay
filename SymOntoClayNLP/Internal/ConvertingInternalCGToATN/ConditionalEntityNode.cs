using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToATN
{
    public class ConditionalEntityNode
    {
        public ConditionalEntityNode(InternalConceptualGraph source, IEntityLogger logger, IWordsDict wordsDict, INLPConverterContext nlpContext)
        {
            _source = source;
            _logger = logger;
            _wordsDict = wordsDict;
            _nlpContext = nlpContext;
        }

        private readonly InternalConceptualGraph _source;
        private readonly IEntityLogger _logger;
        private readonly IWordsDict _wordsDict;
        private readonly INLPConverterContext _nlpContext;

        public ResultOfNode Run()
        {
            var context = new ContextOfConditionalEntityNode();

            FindRootConcept(context);
            ProcessRelationsOfConcept(context.RootConcept, context);
            ProcessAnotherRelations(context);

            throw new NotImplementedException();
        }

        private void FindRootConcept(ContextOfConditionalEntityNode context)
        {
            var rootConcept = _source.Children.Where(p => p.IsConceptNode).Select(p => p.AsConceptNode).First(p => p.IsRootConceptOfEntitiCondition);

#if DEBUG
            _logger.Log($"rootConcept = {rootConcept}");
#endif

            context.RootConcept = rootConcept;
            context.RootWord = rootConcept.Name;
        }

        private void ProcessRelationsOfConcept(InternalConceptCGNode targetConcept, ContextOfConditionalEntityNode context)
        {
            var inputRelationsList = context.RootConcept.Inputs.Select(p => p.AsRelationNode).ToList();

#if DEBUG
            _logger.Log($"inputRelationsList = {inputRelationsList.WriteListToString()}");
#endif

            foreach(var inputRelation in inputRelationsList)
            {

            }

            throw new NotImplementedException();
        }

        private void ProcessAnotherRelations(ContextOfConditionalEntityNode context)
        {
            var anotherRelationsList = _source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).Where(p => !context.VisitedRelations.Contains(p)).ToList();

#if DEBUG
            //_logger.Log($"anotherRelationsList = {anotherRelationsList.WriteListToString()}");
#endif

            if (!anotherRelationsList.Any())
            {
                return;
            }

            throw new NotImplementedException();
        }
    }
}

using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class ConditionalEntityNode
    {
        public ConditionalEntityNode(InternalConceptualGraph source, IEntityLogger logger, IWordsDict wordsDict, INLPConverterContext nlpContext, List<InternalRelationCGNode> visitedRelations)
        {
            _source = source;
            _logger = logger;
            _wordsDict = wordsDict;
            _nlpContext = nlpContext;
            _visitedRelations = visitedRelations;
        }

        private readonly InternalConceptualGraph _source;
        private readonly IEntityLogger _logger;
        private readonly IWordsDict _wordsDict;
        private readonly INLPConverterContext _nlpContext;
        private readonly List<InternalRelationCGNode> _visitedRelations;

        public ResultOfNode Run()
        {
            var context = new ContextOfConditionalEntityNode();
            
            FindRootConcept(context);
            ProcessRelationsOfConcept(context.RootConcept, context.NounPhrase, context);
            ProcessAnotherRelations(context);

#if DEBUG
            _logger.Log($"context.NounPhrase = {context.NounPhrase}");
            _logger.Log($"context.NounPhrase = {context.NounPhrase.ToDbgString()}");
#endif

            return new ResultOfNode()
            {
                SentenceItem = context.NounPhrase
            };
        }

        private void FindRootConcept(ContextOfConditionalEntityNode context)
        {
            var rootConcept = _source.Children.Where(p => p.IsConceptNode).Select(p => p.AsConceptNode).First(p => p.IsRootConceptOfEntitiCondition);

#if DEBUG
            _logger.Log($"rootConcept = {rootConcept}");
#endif

            context.RootConcept = rootConcept;

            var convertingContext = new ContextOfConvertingInternalCGToPhraseStructure()
            {
                Logger = _logger,
                NLPContext = _nlpContext,
                WordsDict = _wordsDict
            };

            var nounWordNode = new NounWordNode(rootConcept.Name, RoleOfNoun.Subject, _logger, _wordsDict);
            var nounPhrase = nounWordNode.GetNounPhrase();

#if DEBUG
            _logger.Log($"nounPhrase = {nounPhrase}");
#endif

            context.NounPhrase = nounPhrase;
        }

        private void ProcessRelationsOfConcept(InternalConceptCGNode targetConcept, NounPhrase targetNounPhrase, ContextOfConditionalEntityNode context)
        {
            var inputRelationsList = targetConcept.Inputs.Select(p => p.AsRelationNode).ToList();

#if DEBUG
            _logger.Log($"inputRelationsList = {inputRelationsList.WriteListToString()}");
#endif

            foreach(var inputRelation in inputRelationsList)
            {
#if DEBUG
                _logger.Log($"inputRelation = {inputRelation}");
#endif

                if(_visitedRelations.Contains(inputRelation))
                {
                    continue;
                }

                _visitedRelations.Add(inputRelation);

                var relationName = inputRelation.Name;

#if DEBUG
                _logger.Log($"relationName = '{relationName}'");
#endif

                switch(relationName)
                {
                    case "possess":
                        {
                            var ownerConcept = inputRelation.Inputs.Select(p => p.AsGraphOrConceptNode).Single();

#if DEBUG
                            _logger.Log($"ownerConcept = {ownerConcept}");
#endif

                            var nounNode = new NounNode(ownerConcept, new List<string>() { "possess" }, RoleOfNoun.PossessDeterminer, _logger, _wordsDict, _nlpContext, _visitedRelations);

                            var nounNodeResult = nounNode.Run();

#if DEBUG
                            _logger.Log($"nounNodeResult = {nounNodeResult}");
#endif

                            targetNounPhrase.D = nounNodeResult.SentenceItem;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(relationName), relationName, null);
                }
            }

#if DEBUG
            _logger.Log($"targetNounPhrase = {targetNounPhrase.ToDbgString()}");
#endif
        }

        private void ProcessAnotherRelations(ContextOfConditionalEntityNode context)
        {
            var anotherRelationsList = _source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).Where(p => !_visitedRelations.Contains(p)).ToList();

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

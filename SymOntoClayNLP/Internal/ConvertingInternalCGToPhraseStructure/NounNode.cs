using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class NounNode
    {
        public NounNode(BaseInternalConceptCGNode source, RoleOfNoun roleOfNoun, BaseContextOfConvertingInternalCGToPhraseStructure baseContext)
        {
            _baseContext = baseContext;
            _wordsDict = baseContext.WordsDict;
            _logger = baseContext.Logger;
            _nlpContext = baseContext.NLPContext;
            _source = source;
            _roleOfNoun = roleOfNoun;
            _visitedRelations = baseContext.VisitedRelations;
        }

        private readonly BaseContextOfConvertingInternalCGToPhraseStructure _baseContext;
        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;
        private readonly INLPConverterContext _nlpContext;

        private readonly BaseInternalConceptCGNode _source;
        private readonly RoleOfNoun _roleOfNoun;
        private readonly List<InternalRelationCGNode> _visitedRelations;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_source = {_source}");
#endif

            var kind = _source.Kind;

#if DEBUG
            _logger.Log($"kind = {kind}");
#endif

            switch(kind)
            {
                case KindOfCGNode.Concept:
                    return ProcessConcept();

                case KindOfCGNode.Graph:
                    if(_source.AsConceptualGraph.Children.Any(p => p.IsConceptNode && p.AsConceptNode.IsRootConceptOfEntitiCondition))
                    {
                        return ProcessConditionalEntity();
                    }
                    throw new NotImplementedException();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private ResultOfNode ProcessConcept()
        {
            var conceptName = _source.Name;

#if DEBUG
            _logger.Log($"conceptName = '{conceptName}'");
#endif

            var nounWordNode = new NounWordNode(conceptName, _roleOfNoun, _logger, _wordsDict);

            if(_roleOfNoun == RoleOfNoun.PossessDeterminer)
            {
                return new ResultOfNode()
                {
                    SentenceItem = nounWordNode.GetWord()
                };
            }

            var nounPhrase = nounWordNode.GetNounPhrase();

#if DEBUG
            _logger.Log($"nounPhrase = {nounPhrase}");
#endif

            return new ResultOfNode()
            {
                SentenceItem = nounPhrase
            };
        }

        private ResultOfNode ProcessConditionalEntity()
        {
#if DEBUG
            var dotStr = DotConverter.ConvertToString(_source);
            _logger.Log($"dotStr = '{dotStr}'");
#endif

            var conditionalEntityNode = new ConditionalEntityNode(_source.AsConceptualGraph, _baseContext);
            var conditionalEntityNodeResult = conditionalEntityNode.Run();

#if DEBUG
            _logger.Log($"conditionalEntityNodeResult = {conditionalEntityNodeResult}");
#endif

            return conditionalEntityNodeResult;
        }
    }
}

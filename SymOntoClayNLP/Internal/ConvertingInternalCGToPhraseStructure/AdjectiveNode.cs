using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class AdjectiveNode
    {
        public AdjectiveNode(BaseInternalConceptCGNode source, BaseContextOfConvertingInternalCGToPhraseStructure baseContext)
        {
            _baseContext = baseContext;
            _wordsDict = baseContext.WordsDict;
            _logger = baseContext.Logger;
            _nlpContext = baseContext.NLPContext;
            _source = source;
            _visitedRelations = baseContext.VisitedRelations;
        }

        private readonly BaseContextOfConvertingInternalCGToPhraseStructure _baseContext;
        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;
        private readonly INLPConverterContext _nlpContext;

        private readonly BaseInternalConceptCGNode _source;
        private readonly List<InternalRelationCGNode> _visitedRelations;

        public ResultOfNode Run()
        {
#if DEBUG
            //_logger.Log($"_source = {_source}");
#endif

            var kind = _source.Kind;

#if DEBUG
            //_logger.Log($"kind = {kind}");
#endif

            switch (kind)
            {
                case KindOfCGNode.Concept:
                    return ProcessConcept();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private ResultOfNode ProcessConcept()
        {
            var conceptName = _source.Name;

#if DEBUG
            //_logger.Log($"conceptName = '{conceptName}'");
#endif

            var adjectiveWordNode = new AdjectiveWordNode(conceptName, _logger, _wordsDict);

#if DEBUG
            //_logger.Log($"adjectiveWordNode.GetWord() = {adjectiveWordNode.GetWord()}");
#endif

            return new ResultOfNode()
            {
                SentenceItem = adjectiveWordNode.GetWord()
            };
        }
    }
}

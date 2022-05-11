using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class SemanticAnalyzer
    {
        public SemanticAnalyzer(IEntityLogger logger, IWordsDict wordsDict)
        {
            _wordsDict = wordsDict;
            _logger = logger;
        }

        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        public ConceptualGraph Run(BaseSentenceItem sentenceItem)
        {
#if DEBUG
            _logger.Log($"sentenceItem = {sentenceItem.ToDbgString()}");
#endif

            var outerConceptualGraph = new ConceptualGraph();
            var context = new ContextOfSemanticAnalyzer();
            context.OuterConceptualGraph = outerConceptualGraph;
            context.WordsDict = _wordsDict;
            context.Logger = _logger;

            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //_logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch(kindOfSentenceItem)
            {
                case KindOfSentenceItem.Sentence:
                    ProcessSentence(sentenceItem.AsSentence, context);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }

            return outerConceptualGraph;
        }

        private void ProcessSentence(Sentence sentence, ContextOfSemanticAnalyzer context)
        {
#if DEBUG
            _logger.Log($"sentence = {sentence.ToDbgString()}");
#endif

            var sentenceNode = new SentenceNodeOfSemanticAnalyzer(context, sentence);
            sentenceNode.Run();
        }
    }
}

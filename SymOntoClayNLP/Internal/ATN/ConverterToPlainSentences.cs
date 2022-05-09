using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ConverterToPlainSentences
    {
        public ConverterToPlainSentences(IEntityLogger logger)
        {
            _logger = logger;
        }

        private readonly IEntityLogger _logger;

        public List<BaseSentenceItem> Run(BaseSentenceItem sentenceItem)
        {
#if DEBUG
            //_logger.Log($"sentenceItem = {sentenceItem.ToDbgString()}");
#endif

            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //_logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.Sentence:
                    return ProcessSentence(sentenceItem.AsSentence);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private List<BaseSentenceItem> ProcessSentence(Sentence sentence)
        {
#if DEBUG
            //_logger.Log($"sentence = {sentence.ToDbgString()}");
#endif

            if((sentence.VocativePhrase != null && (sentence.VocativePhrase.IsConjunctionPhrase || sentence.VocativePhrase.IsListPhrase))
                || (sentence.Subject != null && (sentence.Subject.IsConjunctionPhrase || sentence.Subject.IsListPhrase))
                || (sentence.Predicate != null && (sentence.Predicate.IsConjunctionPhrase || sentence.Predicate.IsListPhrase)))
            {
                throw new NotImplementedException();
            }

            return new List<BaseSentenceItem> { sentence };
        }
    }
}

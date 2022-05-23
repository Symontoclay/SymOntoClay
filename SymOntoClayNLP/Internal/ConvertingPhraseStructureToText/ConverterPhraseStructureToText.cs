using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingPhraseStructureToText
{
    public class ConverterPhraseStructureToText
    {
        public ConverterPhraseStructureToText(IEntityLogger logger)
        {
            _logger = logger;
        }

        private readonly IEntityLogger _logger;

        public string Convert(BaseSentenceItem sentenceItem)
        {
            var sb = new StringBuilder();

            RunBaseSentenceItem(sentenceItem, sb);

#if DEBUG
            //_logger.Log($"sb = '{sb}'");
#endif

            sb[0] = char.ToUpper(sb[0]);

            sb.Append(".");

#if DEBUG
            //_logger.Log($"sb (2) = '{sb}'");
#endif

            return sb.ToString();
        }

        private void RunBaseSentenceItem(BaseSentenceItem sentenceItem, StringBuilder sb)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //_logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch(kindOfSentenceItem)
            {
                case KindOfSentenceItem.Sentence:
                    RunSentence(sentenceItem.AsSentence, sb);
                    break;

                case KindOfSentenceItem.NounPhrase:
                    RunNounPhrase(sentenceItem.AsNounPhrase, sb);
                    break;

                case KindOfSentenceItem.VerbPhrase:
                    RunVerbPhrase(sentenceItem.AsVerbPhrase, sb);
                    break;

                case KindOfSentenceItem.Word:
                    RunWord(sentenceItem.AsWord, sb);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void RunSentence(Sentence sentence, StringBuilder sb)
        {
#if DEBUG
            //_logger.Log($"sentence = {sentence.ToDbgString()}");
#endif

            if(sentence.VocativePhrase != null)
            {
                throw new NotImplementedException();
            }

            if (sentence.Condition != null)
            {
                throw new NotImplementedException();
            }

            if (sentence.Subject != null)
            {
                RunBaseSentenceItem(sentence.Subject, sb);
            }

#if DEBUG
            //_logger.Log($"sb = '{sb}'");
#endif

            if (sentence.Predicate != null)
            {
                RunBaseSentenceItem(sentence.Predicate, sb);
            }

#if DEBUG
            //_logger.Log($"sb = '{sb}'");
#endif
        }

        private void RunVerbPhrase(VerbPhrase verbPhrase, StringBuilder sb)
        {
#if DEBUG
            //_logger.Log($"verbPhrase = {verbPhrase.ToDbgString()}");
#endif

            if(verbPhrase.Intrusion != null)
            {
                throw new NotImplementedException();
            }

            if (verbPhrase.V != null)
            {
                RunBaseSentenceItem(verbPhrase.V, sb);
            }

#if DEBUG
            //_logger.Log($"sb = '{sb}'");
#endif

            if (verbPhrase.VP != null)
            {
                throw new NotImplementedException();
            }

            if (verbPhrase.Negation != null)
            {
                throw new NotImplementedException();
            }

            if (verbPhrase.Object != null)
            {
                RunBaseSentenceItem(verbPhrase.Object, sb);
            }

            if (verbPhrase.PP != null)
            {
                throw new NotImplementedException();
            }

            if (verbPhrase.CP != null)
            {
                throw new NotImplementedException();
            }

#if DEBUG
            //_logger.Log($"sb = '{sb}'");
#endif
        }

        private void RunNounPhrase(NounPhrase nounPhrase, StringBuilder sb)
        {
#if DEBUG
            //_logger.Log($"nounPhrase = {nounPhrase.ToDbgString()}");
#endif

            if (nounPhrase.D != null)
            {
                RunBaseSentenceItem(nounPhrase.D, sb);
            }

#if DEBUG
            //_logger.Log($"sb = '{sb}'");
#endif

            if (nounPhrase.N != null)
            {
                RunBaseSentenceItem(nounPhrase.N, sb);
            }

#if DEBUG
            //_logger.Log($"sb = '{sb}'");
#endif

            if (nounPhrase.QP != null)
            {
                throw new NotImplementedException();
            }

            if (nounPhrase.AP != null)
            {
                throw new NotImplementedException();
            }

            if (nounPhrase.NounAdjunct != null)
            {
                throw new NotImplementedException();
            }

            if (nounPhrase.Negation != null)
            {
                throw new NotImplementedException();
            }

            if(nounPhrase.HasPossesiveMark)
            {
                throw new NotImplementedException();
            }

            if (nounPhrase.PP != null)
            {
                throw new NotImplementedException();
            }

            if (nounPhrase.AdvP != null)
            {
                throw new NotImplementedException();
            }

            if (nounPhrase.RelativeClauses != null)
            {
                throw new NotImplementedException();
            }

            if (nounPhrase.OtherClauses != null)
            {
                throw new NotImplementedException();
            }

            if (nounPhrase.InfinitivePhrases != null)
            {
                throw new NotImplementedException();
            }

#if DEBUG
            //_logger.Log($"sb = '{sb}'");
#endif
        }

        private void RunWord(Word word, StringBuilder sb)
        {
#if DEBUG
            //_logger.Log($"word = {word.ToDbgString()}");
            //_logger.Log($"sb.Length = {sb.Length}");
#endif

            if (sb.Length > 0)
            {
                sb.Append(" ");
            }

            sb.Append(word.Content);

#if DEBUG
            //_logger.Log($"sb = '{sb}'");
#endif
        }
    }
}

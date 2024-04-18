/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingPhraseStructureToText
{
    public class ConverterPhraseStructureToText
    {
        public ConverterPhraseStructureToText(IMonitorLogger logger)
        {
            _logger = logger;
        }

        private readonly IMonitorLogger _logger;

        public string Convert(BaseSentenceItem sentenceItem)
        {
            var sb = new StringBuilder();

            RunBaseSentenceItem(sentenceItem, sb);

            sb[0] = char.ToUpper(sb[0]);

            sb.Append(".");

            return sb.ToString();
        }

        private void RunBaseSentenceItem(BaseSentenceItem sentenceItem, StringBuilder sb)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

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

                case KindOfSentenceItem.PreOrPostpositionalPhrase:
                    RunPreOrPostpositionalPhrase(sentenceItem.AsPreOrPostpositionalPhrase, sb);
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

            if (sentence.Predicate != null)
            {
                RunBaseSentenceItem(sentence.Predicate, sb);
            }

        }

        private void RunVerbPhrase(VerbPhrase verbPhrase, StringBuilder sb)
        {
            if(verbPhrase.Intrusion != null)
            {
                throw new NotImplementedException();
            }

            if (verbPhrase.V != null)
            {
                RunBaseSentenceItem(verbPhrase.V, sb);
            }

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
                RunBaseSentenceItem(verbPhrase.PP, sb);
            }

            if (verbPhrase.CP != null)
            {
                throw new NotImplementedException();
            }

        }

        private void RunNounPhrase(NounPhrase nounPhrase, StringBuilder sb)
        {
            if (nounPhrase.D != null)
            {
                RunBaseSentenceItem(nounPhrase.D, sb);
            }

            if (nounPhrase.AP != null)
            {
                RunBaseSentenceItem(nounPhrase.AP, sb);
            }

            if (nounPhrase.N != null)
            {
                RunBaseSentenceItem(nounPhrase.N, sb);
            }

            if (nounPhrase.QP != null)
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

        }

        private void RunPreOrPostpositionalPhrase(PreOrPostpositionalPhrase preOrPostpositionalPhrase, StringBuilder sb)
        {
            if(preOrPostpositionalPhrase.P != null)
            {
                RunBaseSentenceItem(preOrPostpositionalPhrase.P, sb);
            }

            if (preOrPostpositionalPhrase.P2 != null)
            {
                throw new NotImplementedException();
            }

            if (preOrPostpositionalPhrase.Adv != null)
            {
                throw new NotImplementedException();
            }

            if (preOrPostpositionalPhrase.NP != null)
            {
                RunBaseSentenceItem(preOrPostpositionalPhrase.NP, sb);
            }

        }

        private void RunWord(Word word, StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                sb.Append(" ");
            }

            sb.Append(word.Content);

        }
    }
}

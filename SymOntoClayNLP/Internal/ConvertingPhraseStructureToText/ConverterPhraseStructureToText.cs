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
                throw new NotImplementedException("B001E2C2-2949-4D72-9839-9FDEC4A782D3");
            }

            if (sentence.Condition != null)
            {
                throw new NotImplementedException("B7384C04-8EBE-4FCF-8CE1-BACDABB31289");
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
                throw new NotImplementedException("423EC793-9D65-497C-8050-E0A78411C912");
            }

            if (verbPhrase.V != null)
            {
                RunBaseSentenceItem(verbPhrase.V, sb);
            }

            if (verbPhrase.VP != null)
            {
                throw new NotImplementedException("B344EBB9-1980-4BE1-AF04-5B2500BED07C");
            }

            if (verbPhrase.Negation != null)
            {
                throw new NotImplementedException("3D304CFA-FB33-45A2-A175-C1C19DD15024");
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
                throw new NotImplementedException("DE06DDC6-50CB-4586-B8E6-3E33B06325CC");
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
                throw new NotImplementedException("ED641052-335F-4369-BD46-7355FCABB8E9");
            }



            if (nounPhrase.NounAdjunct != null)
            {
                throw new NotImplementedException("755917C8-BD19-460D-8B10-776D83F1510A");
            }

            if (nounPhrase.Negation != null)
            {
                throw new NotImplementedException("EC22F510-5812-4951-89D2-83E617AF9B33");
            }

            if(nounPhrase.HasPossesiveMark)
            {
                throw new NotImplementedException("C0E56E96-FCF8-4717-84B4-370689D50510");
            }

            if (nounPhrase.PP != null)
            {
                throw new NotImplementedException("54B77216-BC09-4BE3-8D42-8A78A34E2A50");
            }

            if (nounPhrase.AdvP != null)
            {
                throw new NotImplementedException("88FD569A-4ED3-4B9D-9198-1B5231D2AB9E");
            }

            if (nounPhrase.RelativeClauses != null)
            {
                throw new NotImplementedException("E2EFB0F5-685B-413E-BF49-9517779CE430");
            }

            if (nounPhrase.OtherClauses != null)
            {
                throw new NotImplementedException("861C599B-89FF-4409-831A-B00F0D785462");
            }

            if (nounPhrase.InfinitivePhrases != null)
            {
                throw new NotImplementedException("869810D1-E2EE-4A2D-8561-A0B38BCEDDE0");
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
                throw new NotImplementedException("F6BBA939-ED0F-422D-85F4-C5C84F78C62B");
            }

            if (preOrPostpositionalPhrase.Adv != null)
            {
                throw new NotImplementedException("5DC569C6-9EE8-494A-A10E-33D9CCFA5404");
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

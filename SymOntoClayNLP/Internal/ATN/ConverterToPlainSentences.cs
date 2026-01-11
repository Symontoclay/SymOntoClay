/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ConverterToPlainSentences
    {
        public ConverterToPlainSentences(IMonitorLogger logger)
        {
            _logger = logger;
        }

        private readonly IMonitorLogger _logger;

        public List<BaseSentenceItem> Run(BaseSentenceItem sentenceItem)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

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
            if((sentence.VocativePhrase != null && (sentence.VocativePhrase.IsConjunctionPhrase || sentence.VocativePhrase.IsListPhrase))
                || (sentence.Subject != null && (sentence.Subject.IsConjunctionPhrase || sentence.Subject.IsListPhrase))
                || (sentence.Predicate != null && (sentence.Predicate.IsConjunctionPhrase || sentence.Predicate.IsListPhrase)))
            {
                throw new NotImplementedException("7F19A153-3C38-4B54-AAFA-35BB66A8D2BB");
            }

            return new List<BaseSentenceItem> { sentence };
        }
    }
}

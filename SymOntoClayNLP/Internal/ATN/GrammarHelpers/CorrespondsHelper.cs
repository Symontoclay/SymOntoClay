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

using NLog;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN.GrammarHelpers
{
    public static class CorrespondsHelper
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static bool SubjectAndVerb(BaseGrammaticalWordFrame subject, VerbGrammaticalWordFrame verb)
        {
            switch(subject.PartOfSpeech)
            {
                case GrammaticalPartOfSpeech.Pronoun:
                    return PronounSubjectAndVerb(subject.AsPronoun, verb);

                case GrammaticalPartOfSpeech.Noun:
                    return NounSubjectAndVerb(subject.AsNoun, verb);

                default:
                    throw new ArgumentOutOfRangeException(nameof(subject.PartOfSpeech), subject.PartOfSpeech, null);
            }
        }

        private static bool PronounSubjectAndVerb(PronounGrammaticalWordFrame subject, VerbGrammaticalWordFrame verb)
        {
            if(subject.Person == GrammaticalPerson.First)
            {
                if(subject.Number == GrammaticalNumberOfWord.Singular && verb.Person == GrammaticalPerson.Neuter && (verb.Number == GrammaticalNumberOfWord.Singular || verb.Number == GrammaticalNumberOfWord.Neuter))
                {
                    return true;
                }
            }

            throw new NotImplementedException("5D36E04F-6927-4AF3-9D97-A43ADA738215");
        }

        private static bool NounSubjectAndVerb(NounGrammaticalWordFrame subject, VerbGrammaticalWordFrame verb)
        {
            throw new NotImplementedException("BE40BCA2-7368-4EE9-8E55-049841CEAA85");
        }
    }
}

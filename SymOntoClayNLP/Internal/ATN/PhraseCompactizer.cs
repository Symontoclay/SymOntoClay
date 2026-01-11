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
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class PhraseCompactizer
    {
        public PhraseCompactizer(IMonitorLogger logger, IWordsDict wordsDict)
        {
            _wordsDict = wordsDict;
            _logger = logger;
        }

        private readonly IWordsDict _wordsDict;
        private readonly IMonitorLogger _logger;

        public void Run(BaseSentenceItem sentenceItem)
        {
            NRun(sentenceItem, null);
        }

        private void NRun(BaseSentenceItem sentenceItem, PhraseCompactizerVerbChain verbChain)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

            switch(kindOfSentenceItem)
            {
                case KindOfSentenceItem.Sentence:
                    ProcessSentence(sentenceItem.AsSentence);
                    break;

                case KindOfSentenceItem.VerbPhrase:
                    ProcessVerbPhrase(sentenceItem.AsVerbPhrase, verbChain);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessSentence(Sentence sentence)
        {
            if (sentence.Condition != null)
            {
                throw new NotImplementedException("A214C50A-A3ED-4812-9EEB-EB2763433552");
            }

            var verbChain = new PhraseCompactizerVerbChain();

            if (sentence.Predicate == null)
            {
                throw new NotImplementedException("BB244D33-BD61-4F78-A2DD-E23FD5675108");
            }
            else
            {
                NRun(sentence.Predicate, verbChain);
            }

            var verbChainState = verbChain.State;

            VerbPhrase targetVerbPhrase = null;

            switch (verbChainState)
            {
                case PhraseCompactizerVerbChain.StateOfChain.V:
                    {
                        var verbPhrase = verbChain.V;
                        targetVerbPhrase = verbPhrase;

                        var verb = verbPhrase.V.WordFrame.AsVerb;

                        if(verb.Number != GrammaticalNumberOfWord.Neuter)
                        {
                            throw new NotImplementedException("C286D22B-8EBD-481D-83D7-8B340531ED86");
                        }

                        verbPhrase.Aspect = GrammaticalAspect.Simple;

                        if(verb.Tense == GrammaticalTenses.All)
                        {
                            verbPhrase.Tense = GrammaticalTenses.Present;
                        }
                        else
                        {
                            throw new NotImplementedException("3685F152-704B-4D6C-A5DA-E2177857D735");
                        }

                        verbPhrase.Voice = GrammaticalVoice.Active;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(verbChainState), verbChainState, null);
            }

            sentence.IsNegation = targetVerbPhrase.Negation != null;

            sentence.KindOfQuestion = KindOfQuestion.None;//tmp

            sentence.Aspect = targetVerbPhrase.Aspect;
            sentence.Tense = targetVerbPhrase.Tense;
            sentence.Voice = targetVerbPhrase.Voice;
            sentence.AbilityModality = targetVerbPhrase.AbilityModality;

            if(sentence.AbilityModality == AbilityModality.Undefined)
            {
                sentence.AbilityModality = AbilityModality.None;
            }

            sentence.PermissionModality = targetVerbPhrase.PermissionModality;

            if(sentence.PermissionModality == PermissionModality.Undefined)
            {
                sentence.PermissionModality = PermissionModality.None;
            }

            sentence.ObligationModality = targetVerbPhrase.ObligationModality;

            if(sentence.ObligationModality == ObligationModality.Undefined)
            {
                sentence.ObligationModality = ObligationModality.None;
            }

            sentence.ProbabilityModality = targetVerbPhrase.ProbabilityModality;

            if(sentence.ProbabilityModality == ProbabilityModality.Undefined)
            {
                sentence.ProbabilityModality = ProbabilityModality.None;
            }

            sentence.ConditionalModality = targetVerbPhrase.ConditionalModality;

            if(sentence.ConditionalModality == ConditionalModality.Undefined)
            {
                sentence.ConditionalModality = ConditionalModality.None;
            }

            if (sentence.Subject == null)
            {
                sentence.Mood = GrammaticalMood.Imperative;
            }
            else
            {
                sentence.Mood = GrammaticalMood.Indicative;
            }

        }

        private void ProcessVerbPhrase(VerbPhrase verbPhrase, PhraseCompactizerVerbChain verbChain)
        {
            NProcessVerbPhrase(verbPhrase, verbChain);

            if(verbPhrase.CP != null)
            {
                throw new NotImplementedException("67188286-D623-4E25-870D-81D34151FE1B");
            }
        }

        private void NProcessVerbPhrase(VerbPhrase verbPhrase, PhraseCompactizerVerbChain verbChain)
        {
            var verb = verbPhrase.V.WordFrame.AsVerb;

            if (verb.IsModal)
            {
                throw new NotImplementedException("669B42B1-F08B-4B8D-B097-DB10A371CD6F");
            }

            if (verb.IsFormOfToBe)
            {
                throw new NotImplementedException("1AC16894-5A3E-44F3-B56E-976FCD28C74A");
            }

            if (verb.IsFormOfToHave)
            {
                throw new NotImplementedException("C2DB1BDA-6A70-4162-A4B2-292AC5808B34");
            }

            if (verb.IsFormOfToDo)
            {
                throw new NotImplementedException("B317955A-C6AF-46B2-AB3C-0DA9598A5636");
            }

            if (verb.RootWord == "need")
            {
                throw new NotImplementedException("241F53BC-895C-43E9-B987-E1A0DA785AFA");
            }

            verbChain.V = verbPhrase;
            verbChain.State = PhraseCompactizerVerbChain.StateOfChain.V;
        }
    }
}

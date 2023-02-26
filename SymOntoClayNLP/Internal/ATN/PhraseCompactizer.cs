/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class PhraseCompactizer
    {
        public PhraseCompactizer(IEntityLogger logger, IWordsDict wordsDict)
        {
            _wordsDict = wordsDict;
            _logger = logger;
        }

        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        public void Run(BaseSentenceItem sentenceItem)
        {
            NRun(sentenceItem, null);
        }

        private void NRun(BaseSentenceItem sentenceItem, PhraseCompactizerVerbChain verbChain)
        {
#if DEBUG
            //_logger.Log($"sentenceItem = {sentenceItem.ToDbgString()}");
            //_logger.Log($"verbChain = {verbChain}");
#endif

            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //_logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

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
#if DEBUG
            //_logger.Log($"sentence = {sentence.ToDbgString()}");
#endif

            if (sentence.Condition != null)
            {
                throw new NotImplementedException();
            }

            var verbChain = new PhraseCompactizerVerbChain();

            if (sentence.Predicate == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                NRun(sentence.Predicate, verbChain);
            }

#if DEBUG
            //_logger.Log($"verbChain = {verbChain}");
#endif

            var verbChainState = verbChain.State;

            VerbPhrase targetVerbPhrase = null;

            switch (verbChainState)
            {
                case PhraseCompactizerVerbChain.StateOfChain.V:
                    {
                        var verbPhrase = verbChain.V;
                        targetVerbPhrase = verbPhrase;

                        var verb = verbPhrase.V.WordFrame.AsVerb;

#if DEBUG
                        //_logger.Log($"verb = {verb}");
#endif

                        if(verb.Number != GrammaticalNumberOfWord.Neuter)
                        {
                            throw new NotImplementedException();
                        }

                        verbPhrase.Aspect = GrammaticalAspect.Simple;

                        if(verb.Tense == GrammaticalTenses.All)
                        {
                            verbPhrase.Tense = GrammaticalTenses.Present;
                        }
                        else
                        {
                            throw new NotImplementedException();
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

#if DEBUG
            //_logger.Log($"sentence = {sentence}");
#endif
        }

        private void ProcessVerbPhrase(VerbPhrase verbPhrase, PhraseCompactizerVerbChain verbChain)
        {
            NProcessVerbPhrase(verbPhrase, verbChain);

            if(verbPhrase.CP != null)
            {
                throw new NotImplementedException();
            }
        }

        private void NProcessVerbPhrase(VerbPhrase verbPhrase, PhraseCompactizerVerbChain verbChain)
        {
#if DEBUG
            //_logger.Log($"sentenceItem = {verbPhrase.ToDbgString()}");
            //_logger.Log($"verbChain = {verbChain}");
            //_logger.Log($"verbPhrase.V = {verbPhrase.V}");
#endif

            var verb = verbPhrase.V.WordFrame.AsVerb;

            if (verb.IsModal)
            {
                throw new NotImplementedException();
            }

            if (verb.IsFormOfToBe)
            {
                throw new NotImplementedException();
            }

            if (verb.IsFormOfToHave)
            {
                throw new NotImplementedException();
            }

            if (verb.IsFormOfToDo)
            {
                throw new NotImplementedException();
            }

            if (verb.RootWord == "need")
            {
                throw new NotImplementedException();
            }

            verbChain.V = verbPhrase;
            verbChain.State = PhraseCompactizerVerbChain.StateOfChain.V;
        }
    }
}

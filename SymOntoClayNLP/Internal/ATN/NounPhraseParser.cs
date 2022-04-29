using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN.GrammarHelpers;
using SymOntoClay.NLP.Internal.ATN.ParsingDirectives;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class NounPhraseParser : BaseParser
    {
        public enum State
        {
            Init,
            ProcessN,
            GotN
        }

        public override void SetStateAsInt32(int state)
        {
            _state = (State)state;
        }

        private State _state;

        private NounPhrase _nounPhrase;

        /// <inheritdoc/>
        public override void OnEnter()
        {
#if DEBUG
            Log($"Begin");
#endif

            _nounPhrase = new NounPhrase();

#if DEBUG
            Log($"End");
#endif
        }

        /// <inheritdoc/>
        public override void OnRun(ATNToken token)
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"token = {token}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch(token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                var wasProcessed = false;

                                var wordFramesList = token.WordFrames;

                                var subjectsPronounsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun).Select(p => p.AsPronoun).Where(p => p.TypeOfPronoun == TypeOfPronoun.Personal && p.Case == CaseOfPersonalPronoun.Subject);

                                if(subjectsPronounsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var item in subjectsPronounsList)
                                    {
#if DEBUG
                                        Log($"item = {item}");
#endif

                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.ProcessN, ConvertToConcreteATNToken(token, item)));
                                    }
                                }                                

                                if (!wasProcessed)
                                {
                                    throw new UnExpectedTokenException(token);
                                }
                            }
                            break;

                        default:
                            throw new UnExpectedTokenException(token);
                    }
                    break;

                case State.GotN:
                    {
                        var wasProcessed = false;

                        var wordFramesList = token.WordFrames;

                        var subject = _nounPhrase.N.AsWord.WordFrame;

                        var verbsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Verb).Select(p => p.AsVerb).Where(p => CorrespondsHelper.SubjectAndVerb(subject, p));

                        if(verbsList.Any())
                        {
                            wasProcessed = true;

                            foreach(var item in verbsList)
                            {
#if DEBUG
                                Log($"item = {item}");
#endif

                                SetParser(new ReturnToParentDirective(_nounPhrase, ConvertToConcreteATNToken(token, item)));
                            }
                        }                        

                        if (!wasProcessed)
                        {
                            throw new UnExpectedTokenException(token);
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        /// <inheritdoc/>
        public override void OnVariant(ConcreteATNToken token)
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"token = {token}");
#endif

            switch (_state)
            {
                case State.ProcessN:
                    {
                        _nounPhrase.N = ConvertToWord(token);

                        _state = State.GotN;

                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        /// <inheritdoc/>

        public override void OnReceiveReturn(BaseSentenceItem phrase)
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"phrase = {phrase}");
#endif

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void OnFinish()
        {
#if DEBUG
            Log($"Begin");
#endif

            throw new NotImplementedException();

#if DEBUG
            Log($"End");
#endif
        }
    }
}

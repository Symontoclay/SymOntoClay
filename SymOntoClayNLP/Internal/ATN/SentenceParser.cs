using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN.ParsingDirectives;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class SentenceParser: BaseParser
    {
        public enum State
        {
            Init,
            WaitForSubject,
            GotSubject,
            WaitForPredicate,
            GotPredicate
        }

        /// <inheritdoc/>
        public override void SetStateAsInt32(int state)
        {
            _state = (State)state;
        }

        private State _state;

        private Sentence _sentence;

        /// <inheritdoc/>
        public override void OnEnter()
        {
#if DEBUG
            Log($"Begin");
#endif

            _sentence = new Sentence();

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

                                var nounWordFramesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Noun);

#if DEBUG
                                Log($"nounWordFramesList = {nounWordFramesList.WriteListToString()}");
#endif

                                if(nounWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var nounWordFrame in nounWordFramesList)
                                    {
#if DEBUG
                                        Log($"nounWordFrame = {nounWordFrame}");
#endif

                                        SetParser(new RunVariantDirective<SentenceParser>(State.WaitForSubject, ConvertToConcreteATNToken(token, nounWordFrame)));
                                    }
                                }

                                var pronounWordFramesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun);

#if DEBUG
                                Log($"pronounWordFramesList = {pronounWordFramesList.WriteListToString()}");
#endif

                                if(pronounWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var pronounWordFrame in pronounWordFramesList)
                                    {
#if DEBUG
                                        Log($"pronounWordFrame = {pronounWordFrame}");
#endif

                                        SetParser(new RunVariantDirective<SentenceParser>(State.WaitForSubject, ConvertToConcreteATNToken(token, pronounWordFrame)));
                                    }
                                }

                                if(!wasProcessed)
                                {
                                    throw new UnExpectedTokenException(token);
                                }
                            }
                            break;

                        default:
                            throw new UnExpectedTokenException(token);
                    }
                    break;

                case State.GotSubject:
                    switch (token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                var wasProcessed = false;

                                var wordFramesList = token.WordFrames;

                                var verbsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Verb);

                                if(verbsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var item in verbsList)
                                    {
#if DEBUG
                                        Log($"item = {item}");
#endif

                                        SetParser(new RunVariantDirective<SentenceParser>(State.WaitForPredicate, ConvertToConcreteATNToken(token, item)));
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
                case State.WaitForSubject:
                    switch(token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                SetParser(new RunChildDirective<NounPhraseParser>(NounPhraseParser.State.Init, State.GotSubject, token));
                            }
                            break;

                        default:
                            throw new UnExpectedTokenException(token);
                    }
                    break;

                case State.WaitForPredicate:
                    switch (token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                SetParser(new RunChildDirective<VerbPhraseParser>(VerbPhraseParser.State.Init, State.GotPredicate, token));
                            }
                            break;

                        default:
                            throw new UnExpectedTokenException(token);
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
            Log($"phrase = {phrase.ToDbgString()}");
#endif

            switch (_state)
            {
                case State.GotSubject:
                    {
                        _sentence.Subject = phrase;

#if DEBUG
                        Log($"_sentence = {_sentence.ToDbgString()}");
                        Log($"ExpectedBehavior = {ExpectedBehavior}");
#endif
                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                case State.GotPredicate:
                    {
                        _sentence.Predicate = phrase;

#if DEBUG
                        Log($"_sentence = {_sentence.ToDbgString()}");
                        Log($"ExpectedBehavior = {ExpectedBehavior}");
#endif

                        SetParser(new ReturnToParentDirective(_sentence));
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}

﻿using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN.ParsingDirectives;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class VerbPhraseParser : BaseParser
    {
        public enum State
        {
            Init,
            WaitForV,
            GotV,
            WaitForSubject,
            GotSubject,
            WaitForPP,
            GotPP
        }

        /// <inheritdoc/>
        public override BaseParser Fork(ParserContext newParserContext)
        {
            var result = new VerbPhraseParser();
            result.FillUpBaseParser(this, newParserContext);
            result._state = _state;
            result._verbPhrase = _verbPhrase.Clone();

            return result;
        }

        public override void SetStateAsInt32(int state)
        {
            _state = (State)state;
        }

        /// <inheritdoc/>
        public override string GetStateAsString()
        {
            return _state.ToString();
        }

        private State _state;

        private VerbPhrase _verbPhrase;

        /// <inheritdoc/>
        public override string GetPhraseAsString()
        {
            return _verbPhrase.ToDbgString();
        }

        /// <inheritdoc/>
        public override void OnEnter()
        {
#if DEBUG
            //Log($"Begin");
#endif

            _verbPhrase = new VerbPhrase();

#if DEBUG
            //Log($"End");
#endif
        }

        /// <inheritdoc/>
        public override void OnRun(ATNToken token)
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"token = {token}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                var wasProcessed = false;

                                var wordFramesList = token.WordFrames;

                                var verbsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Verb);

                                if (verbsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var item in verbsList)
                                    {
#if DEBUG
                                        //Log($"item = {item}");
#endif

                                        SetParser(new RunVariantDirective<VerbPhraseParser>(State.WaitForV, ConvertToConcreteATNToken(token, item)));
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

                case State.GotV:
                    switch(token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                var wasProcessed = false;

                                var wordFramesList = token.WordFrames;

                                var articlesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Article);

                                if(articlesList.Any())
                                {
                                    wasProcessed = true;


                                    foreach (var item in articlesList)
                                    {
#if DEBUG
                                        //Log($"item = {item}");
#endif

                                        SetParser(new RunVariantDirective<VerbPhraseParser>(State.WaitForSubject, ConvertToConcreteATNToken(token, item)));
                                    }
                                }                                

                                var prepositionsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Preposition);

                                if(prepositionsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var item in prepositionsList)
                                    {
#if DEBUG
                                        //Log($"item = {item}");
#endif

                                        SetParser(new RunVariantDirective<VerbPhraseParser>(State.WaitForPP, ConvertToConcreteATNToken(token, item)));
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
            //Log($"_state = {_state}");
            //Log($"token = {token}");
#endif

            switch (_state)
            {
                case State.WaitForV:
                    {
                        _verbPhrase.V = ConvertToWord(token);

#if DEBUG
                        //Log($"_verbPhrase = {_verbPhrase.ToDbgString()}");
#endif

                        _state = State.GotV;

                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                case State.WaitForSubject:
                    {
                        SetParser(new RunChildDirective<NounPhraseParser>(NounPhraseParser.State.Init, State.GotSubject, token));
                    }
                    break;

                case State.WaitForPP:
                    {
                        SetParser(new RunChildDirective<PreOrPostpositionalPhraseParser>(PreOrPostpositionalPhraseParser.State.Init, State.GotPP, token));
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
            //Log($"_state = {_state}");
            //Log($"phrase = {phrase.ToDbgString()}");
#endif

            switch (_state)
            {
                case State.GotSubject:
                    {
                        _verbPhrase.Object = phrase;

#if DEBUG
                        //Log($"_verbPhrase = {_verbPhrase.ToDbgString()}");
                        //Log($"ExpectedBehavior = {ExpectedBehavior}");
#endif
                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                case State.GotPP:
                    {
                        throw new NotImplementedException();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        /// <inheritdoc/>
        public override void OnEmptyLexer()
        {
#if DEBUG
            //Log($"Begin");
#endif

            SetParser(new ReturnToParentDirective(_verbPhrase));

#if DEBUG
            //Log($"End");
#endif
        }
    }
}

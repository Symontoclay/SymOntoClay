﻿using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN.ParsingDirectives;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class PreOrPostpositionalPhraseParser : BaseParser
    {
        public enum State
        {
            Init,
            WaitForP,
            GotP,
            WaitForNP,
            GotNP
        }

        /// <inheritdoc/>
        public override BaseParser Fork(ParserContext newParserContext)
        {
            var result = new PreOrPostpositionalPhraseParser();
            result.FillUpBaseParser(this, newParserContext);
            result._state = _state;
            result._pp = _pp.Clone();

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

        private PreOrPostpositionalPhrase _pp;

        /// <inheritdoc/>
        public override string GetPhraseAsString()
        {
            return _pp.ToDbgString();
        }

        /// <inheritdoc/>
        public override void OnEnter()
        {
#if DEBUG
            //Log($"Begin");
#endif

            _pp = new PreOrPostpositionalPhrase();

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

                                var prepositionsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Preposition);

                                if (prepositionsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var item in prepositionsList)
                                    {
#if DEBUG
                                        //Log($"item = {item}");
#endif

                                        SetParser(new RunVariantDirective<PreOrPostpositionalPhraseParser>(State.WaitForP, ConvertToConcreteATNToken(token, item)));
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

                case State.GotP:
                    switch (token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                var wasProcessed = false;

                                var wordFramesList = token.WordFrames;

                                var nounWordFramesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Noun);

#if DEBUG
                                //Log($"nounWordFramesList = {nounWordFramesList.WriteListToString()}");
#endif

                                if(nounWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var item in nounWordFramesList)
                                    {
                                        SetParser(new RunVariantDirective<PreOrPostpositionalPhraseParser>(State.WaitForNP, ConvertToConcreteATNToken(token, item)));
                                    }
                                }

                                var pronounWordFramesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun);

#if DEBUG
                                //Log($"pronounWordFramesList = {pronounWordFramesList.WriteListToString()}");
#endif

                                if (pronounWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var item in pronounWordFramesList)
                                    {
                                        SetParser(new RunVariantDirective<PreOrPostpositionalPhraseParser>(State.WaitForNP, ConvertToConcreteATNToken(token, item)));
                                    }
                                }

                                var adjectiveWordFramesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Adjective);

                                if (adjectiveWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var item in adjectiveWordFramesList)
                                    {
                                        SetParser(new RunVariantDirective<PreOrPostpositionalPhraseParser>(State.WaitForNP, ConvertToConcreteATNToken(token, item)));
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
                case State.WaitForP:
                    {
                        _pp.P = ConvertToWord(token);

#if DEBUG
                        //Log($"_pp = {_pp.ToDbgString()}");
#endif

                        _state = State.GotP;

                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                case State.WaitForNP:
                    switch (token.Kind)
                    {
                        case KindOfATNToken.Word:
                            SetParser(new RunChildDirective<NounPhraseParser>(NounPhraseParser.State.Init, State.GotNP, token));
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
            //Log($"_state = {_state}");
            //Log($"phrase = {phrase.ToDbgString()}");
#endif

            switch (_state)
            {
                case State.GotNP:
                    {
                        _pp.NP = phrase;

                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        /// <inheritdoc/>
        public override void OnEmptyLexer()
        {
            SetParser(new ReturnToParentDirective(_pp));
        }
    }
}

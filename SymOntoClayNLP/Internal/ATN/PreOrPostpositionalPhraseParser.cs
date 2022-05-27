using SymOntoClay.NLP.CommonDict;
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
            GotP
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}

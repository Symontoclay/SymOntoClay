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
            _pp = new PreOrPostpositionalPhrase();

        }

        /// <inheritdoc/>
        public override void OnRun(ATNToken token)
        {
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

                                if(nounWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var item in nounWordFramesList)
                                    {
                                        SetParser(new RunVariantDirective<PreOrPostpositionalPhraseParser>(State.WaitForNP, ConvertToConcreteATNToken(token, item)));
                                    }
                                }

                                var pronounWordFramesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun);

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
            switch (_state)
            {
                case State.WaitForP:
                    {
                        _pp.P = ConvertToWord(token);

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

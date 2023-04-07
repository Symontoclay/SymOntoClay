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
        public override BaseParser Fork(ParserContext newParserContext)
        {
            var result = new SentenceParser();
            result.FillUpBaseParser(this, newParserContext);
            result._state = _state;
            result._sentence = _sentence.Clone();

            return result;
        }

        /// <inheritdoc/>
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

        private Sentence _sentence;

        /// <inheritdoc/>
        public override string GetPhraseAsString()
        {
            return _sentence.ToDbgString();
        }

        /// <inheritdoc/>
        public override void OnEnter()
        {
            _sentence = new Sentence();

        }

        /// <inheritdoc/>
        public override void OnRun(ATNToken token)
        {
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

                                if(nounWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var nounWordFrame in nounWordFramesList)
                                    {
                                        SetParser(new RunVariantDirective<SentenceParser>(State.WaitForSubject, ConvertToConcreteATNToken(token, nounWordFrame)));
                                    }
                                }

                                var pronounWordFramesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun);

                                if(pronounWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var pronounWordFrame in pronounWordFramesList)
                                    {
                                        SetParser(new RunVariantDirective<SentenceParser>(State.WaitForSubject, ConvertToConcreteATNToken(token, pronounWordFrame)));
                                    }
                                }

                                var verbWordFramesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Verb);

                                if (verbWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var verbWordFrame in verbWordFramesList)
                                    {
                                        SetParser(new RunVariantDirective<SentenceParser>(State.WaitForPredicate, ConvertToConcreteATNToken(token, verbWordFrame)));
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
            switch (_state)
            {
                case State.WaitForSubject:
                    switch(token.Kind)
                    {
                        case KindOfATNToken.Word:
                            SetParser(new RunChildDirective<NounPhraseParser>(NounPhraseParser.State.Init, State.GotSubject, token, RoleOfNounPhrase.Subject));
                            break;

                        default:
                            throw new UnExpectedTokenException(token);
                    }
                    break;

                case State.WaitForPredicate:
                    switch (token.Kind)
                    {
                        case KindOfATNToken.Word:
                            SetParser(new RunChildDirective<VerbPhraseParser>(VerbPhraseParser.State.Init, State.GotPredicate, token));
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
                case State.GotSubject:
                    {
                        _sentence.Subject = phrase;

                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                case State.GotPredicate:
                    {
                        _sentence.Predicate = phrase;

                        SetParser(new ReturnToParentDirective(_sentence));
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}

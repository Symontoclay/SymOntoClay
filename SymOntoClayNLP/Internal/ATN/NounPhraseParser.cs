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
            WaitForD,
            GotD,
            WaitForN,
            GotN,
            WaitForAP,
            GotAP
        }

        /// <inheritdoc/>
        public override BaseParser Fork(ParserContext newParserContext)
        {
            var result = new NounPhraseParser();
            result.FillUpBaseParser(this, newParserContext);
            result._state = _state;
            result._nounPhrase = _nounPhrase.Clone();

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

        private NounPhrase _nounPhrase;

        /// <inheritdoc/>
        public override string GetPhraseAsString()
        {
            return _nounPhrase.ToDbgString();
        }

        public override void SetRole(object role)
        {
            _roleOfNounPhrase = (RoleOfNounPhrase)role;
        }

        private RoleOfNounPhrase _roleOfNounPhrase = RoleOfNounPhrase.Undefined;

        public override string GetRoleAsString()
        {
            return _roleOfNounPhrase.ToString();
        }

        /// <inheritdoc/>
        public override void OnEnter()
        {
            _nounPhrase = new NounPhrase();

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

                                var nounsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Noun);

                                if(nounsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var item in nounsList)
                                    {
                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.WaitForN, ConvertToConcreteATNToken(token, item)));
                                    }
                                }

                                var subjectsPronounsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun).Select(p => p.AsPronoun).Where(p => p.TypeOfPronoun == TypeOfPronoun.Personal && p.Case == CaseOfPersonalPronoun.Subject);

                                if(subjectsPronounsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var item in subjectsPronounsList)
                                    {
                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.WaitForN, ConvertToConcreteATNToken(token, item)));
                                    }
                                }

                                var articlesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Article);

                                if (articlesList.Any())
                                {
                                    wasProcessed = true;


                                    foreach (var item in articlesList)
                                    {
                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.WaitForD, ConvertToConcreteATNToken(token, item))); ;
                                    }
                                }

                                var adjectiveWordFramesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Adjective);

                                if (adjectiveWordFramesList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var item in adjectiveWordFramesList)
                                    {
                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.WaitForAP, ConvertToConcreteATNToken(token, item)));
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

                case State.GotD:
                    switch (token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                var wasProcessed = false;

                                var wordFramesList = token.WordFrames;

                                var nounsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Noun);

                                if(nounsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var item in nounsList)
                                    {
                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.WaitForN, ConvertToConcreteATNToken(token, item)));
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

                case State.GotAP:
                    switch (token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                var wasProcessed = false;

                                var wordFramesList = token.WordFrames;

                                var nounsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Noun);

                                if (nounsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var item in nounsList)
                                    {
                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.WaitForN, ConvertToConcreteATNToken(token, item)));
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
                    switch (token.Kind)
                    {
                        case KindOfATNToken.Word:
                            {
                                var wasProcessed = false;

                                var wordFramesList = token.WordFrames;

                                if (_roleOfNounPhrase == RoleOfNounPhrase.Subject)
                                {
                                    var subject = _nounPhrase.N.AsWord.WordFrame;

                                    var verbsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Verb).Select(p => p.AsVerb).Where(p => CorrespondsHelper.SubjectAndVerb(subject, p));

                                    if (verbsList.Any())
                                    {
                                        wasProcessed = true;

                                        foreach (var item in verbsList)
                                        {
                                            SetParser(new ReturnToParentDirective(_nounPhrase, ConvertToConcreteATNToken(token, item)));
                                        }
                                    }
                                }

                                if (!wasProcessed)
                                {
                                    throw new UnExpectedTokenException(token);
                                }
                            }
                            break;

                        case KindOfATNToken.Point:
                        case KindOfATNToken.ExclamationMark:
                            ReturnToParent(token);
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
                case State.WaitForD:
                    {
                        _nounPhrase.D = ConvertToWord(token);

                        _state = State.GotD;

                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                case State.WaitForN:
                    {
                        _nounPhrase.N = ConvertToWord(token);

                        _state = State.GotN;

                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                case State.WaitForAP:
                    {
                        _nounPhrase.AP = ConvertToWord(token);

                        _state = State.GotAP;

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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void OnEmptyLexer()
        {
            ReturnToParent(null);
        }

        private void ReturnToParent(ATNToken token)
        {
            if(_nounPhrase.N == null)
            {
                throw new UnExpectedTokenException(token);
            }

            SetParser(new ReturnToParentDirective(_nounPhrase));
        }
    }
}

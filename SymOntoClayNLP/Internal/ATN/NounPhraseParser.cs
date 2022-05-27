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
            GotN
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

        /// <inheritdoc/>
        public override void OnEnter()
        {
#if DEBUG
            //Log($"Begin");
#endif

            _nounPhrase = new NounPhrase();

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
#if DEBUG
                                        //Log($"item = {item}");
#endif

                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.WaitForN, ConvertToConcreteATNToken(token, item)));
                                    }
                                }

                                var subjectsPronounsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun).Select(p => p.AsPronoun).Where(p => p.TypeOfPronoun == TypeOfPronoun.Personal && p.Case == CaseOfPersonalPronoun.Subject);

                                if(subjectsPronounsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach(var item in subjectsPronounsList)
                                    {
#if DEBUG
                                        //Log($"item = {item}");
#endif

                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.WaitForN, ConvertToConcreteATNToken(token, item)));
                                    }
                                }

                                var articlesList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Article);

                                if (articlesList.Any())
                                {
                                    wasProcessed = true;


                                    foreach (var item in articlesList)
                                    {
#if DEBUG
                                        //Log($"item = {item}");
#endif

                                        SetParser(new RunVariantDirective<NounPhraseParser>(State.WaitForD, ConvertToConcreteATNToken(token, item))); ;
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
#if DEBUG
                                        //Log($"item = {item}");
#endif

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

                                var subject = _nounPhrase.N.AsWord.WordFrame;

                                var verbsList = wordFramesList.Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Verb).Select(p => p.AsVerb).Where(p => CorrespondsHelper.SubjectAndVerb(subject, p));

                                if (verbsList.Any())
                                {
                                    wasProcessed = true;

                                    foreach (var item in verbsList)
                                    {
#if DEBUG
                                        //Log($"item = {item}");
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

                        case KindOfATNToken.Point:
                            SetParser(new ReturnToParentDirective(_nounPhrase));
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
                case State.WaitForD:
                    {
                        _nounPhrase.D = ConvertToWord(token);

#if DEBUG
                        //Log($"_nounPhrase.ToDbgString() = {_nounPhrase.ToDbgString()}");
#endif

                        _state = State.GotD;

                        ExpectedBehavior = ExpectedBehaviorOfParser.WaitForCurrToken;
                    }
                    break;

                case State.WaitForN:
                    {
                        _nounPhrase.N = ConvertToWord(token);

#if DEBUG
                        //Log($"_nounPhrase.ToDbgString() = {_nounPhrase.ToDbgString()}");
#endif

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
            //Log($"_state = {_state}");
            //Log($"phrase = {phrase}");
#endif

            throw new NotImplementedException();
        }
    }
}

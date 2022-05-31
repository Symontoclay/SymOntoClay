using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class AnnotationParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForItem,
            GotItem
        }

        public AnnotationParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public Annotation Result { get; private set; } = new Annotation();

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result.CheckDirty();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenAnnotationBracket:
                            _state = State.WaitForItem;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            {
                                Result.MeaningRolesList.Add(ParseName(_currToken.Content));

                                _state = State.GotItem;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseAnnotationBracket:
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _state = State.WaitForItem;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}

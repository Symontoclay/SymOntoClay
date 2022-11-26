using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LibPaser : BaseObjectParser
    {
        private enum State
        {
            Init,
            GotLib,
            GotName,
            GotInheritance,
            ContentStarted
        }

        public LibPaser(InternalParserContext context)
            : base(context, KindOfCodeEntity.Lib)
        {
        }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch(_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Lib:
                            _state = State.GotLib;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotLib:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            Result.Name = ParseName(_currToken.Content);
                            _state = State.GotName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.ContentStarted;
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Is:
                                    ProcessInheritance();
                                    _state = State.GotInheritance;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotInheritance:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.ContentStarted;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.ContentStarted:
                    ProcessGeneralContent();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}

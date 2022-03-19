using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class StatePaser : BaseObjectParser
    {
        private enum State
        {
            Init,
            GotStateMark,
            GotName,
            GotInheritance,
            ContentStarted
        }

        public StatePaser(InternalParserContext context)
            : base(context, KindOfCodeEntity.State)
        {
        }

        private State _state = State.Init;

        private StateDef _stateDef;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            base.OnEnter();

            _stateDef = Result.AsState;
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.State:
                            _state = State.GotStateMark;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotStateMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            var name = ParseName(_currToken.Content);

                            _stateDef.Name = name;

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

        private void ProcessInheritance()
        {
            _context.Recovery(_currToken);
            var parser = new InheritanceParser(_context, Result.Name);
            parser.Run();
            Result.InheritanceItems.AddRange(parser.Result);

            _state = State.GotInheritance;
        }
    }
}

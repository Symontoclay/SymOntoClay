using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class AnnotationSystemEventParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForKindOfEvent,
            GotKindOfEvent,
            GotAsyncMark,
            WaitForAction,
            GotAction
        }

        public AnnotationSystemEventParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public AnnotationSystemEvent Result { get; private set; } = new AnnotationSystemEvent();

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
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.On:
                                    _state = State.WaitForKindOfEvent;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForKindOfEvent:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Complete:
                                    Result.Kind = KindOfAnnotationSystemEvent.Complete;
                                    _state = State.GotKindOfEvent;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotKindOfEvent:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAsyncMark:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForAction:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAction:
                    _context.Recovery(_currToken);
                    Exit();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessFunctionBody()
        {
            _context.Recovery(_currToken);
            var parser = new FunctionBodyParser(_context);
            parser.Run();

            var statementsList = parser.Result;

            Result.Statements = statementsList;
            Result.CompiledFunctionBody = _context.Compiler.Compile(statementsList);

            _state = State.GotAction;
        }
    }
}

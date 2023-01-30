using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class ConstructorDeclParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotConstructorMark,
            GotParameters,
            WaitForAction,
            GotAction
        }

        public ConstructorDeclParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public Constructor Result { get;private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = CreateConstructorAndSetAsCurrentCodeItem();
            Result.TypeOfAccess = _context.CurrentDefaultSetings.TypeOfAccess;
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            RemoveCurrentCodeEntity();
        }

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
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Constructor:
                                    _state = State.GotConstructorMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotConstructorMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            ProcessParameters();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotParameters:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            _context.Recovery(_currToken);
                            _state = State.WaitForAction;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);
                                var parser = new FunctionBodyParser(_context);
                                parser.Run();
                                var statementsList = parser.Result;

                                Result.Statements = statementsList;
                                Result.CompiledFunctionBody = _context.Compiler.Compile(statementsList);
                                _state = State.GotAction;
                            }
                            break;

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

        private void ProcessParameters()
        {
            _context.Recovery(_currToken);

            var parser = new FunctionParametersParser(_context);
            parser.Run();

            Result.Arguments = parser.Result;

            _state = State.GotParameters;
        }
    }
}

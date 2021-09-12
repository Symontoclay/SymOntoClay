using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class OperatorParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotOperatorMark,
            GotParameters,
            WaitForAction,
            GotAction
        }

        public OperatorParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public CodeEntity Result { get; private set; }
        private Operator _operator;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = CreateCodeEntity();
            Result.Kind = KindOfCodeEntity.Operator;

            _operator = CreateOperator();
            _operator.CodeEntity = Result;

            Result.Operator = _operator;
            Result.CodeFile = _context.CodeFile;
            Result.ParentCodeEntity = CurrentCodeEntity;
            SetCurrentCodeEntity(Result);

            if (Result.ParentCodeEntity != null)
            {
                _operator.Holder = Result.ParentCodeEntity.Name;
            }
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            if(_operator.KindOfOperator == KindOfOperator.Unknown)
            {
                _operator.KindOfOperator = KindOfOperator.CallFunction;
            }

            RemoveCurrentCodeEntity();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
            Log($"_operator = {_operator}");
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
                                case KeyWordTokenKind.Operator:
                                    _state = State.GotOperatorMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotOperatorMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new FunctionParametersParser(_context);
                                parser.Run();

                                _operator.Arguments = parser.Result;

                                _state = State.GotParameters;
                            }
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

                                _operator.Statements = statementsList;
                                _operator.CompiledFunctionBody = _context.Compiler.Compile(statementsList);
                                _state = State.GotAction;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
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

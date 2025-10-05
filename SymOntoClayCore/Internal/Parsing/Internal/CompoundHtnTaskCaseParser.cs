using SymOntoClay.Core.Internal.CodeModel;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CompoundHtnTaskCaseParser : BaseCompoundHtnTaskItemsSectionParser
    {
        private enum State
        {
            Init,
            GotCaseMark,
            WaitForCondition,
            GotCondition,
            ContentStarted
        }

        public CompoundHtnTaskCaseParser(InternalParserContext context)
            : base(context)
        {
        }

        public CompoundHtnTaskCase Result { get; private set; }

        private State _state = State.Init;
        
        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new CompoundHtnTaskCase();

            RegisterResult(Result);

            SetCurrentCodeItem(Result);
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
            //Info("DE93DB33-2003-45CD-BBFA-A1466C4FCB7B", $"_state = {_state}");
            //Info("11A79601-440A-4241-AEA1-ABE664DADA99", $"_currToken = {_currToken}");
            //Info("1C1F3464-5B88-4571-807F-715241E9FFF7", $"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Case:
                                    _state = State.GotCaseMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotCaseMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.ContentStarted;
                            break;

                        case TokenKind.Word:
                        case TokenKind.OpenFactBracket:
                            _context.Recovery(_currToken);
                            _state = State.WaitForCondition;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForCondition:
                    {
                        _context.Recovery(_currToken);
                        var parser = new AstExpressionParser(_context, TokenKind.OpenFigureBracket);
                        parser.Run();

                        var conditionExpr = parser.Result;

#if DEBUG
                        //Info("B0F268ED-E78B-4D08-AF64-FC932A334429", $"conditionExpr = {conditionExpr}");
#endif

                        var compiledFunctionBody = _context.Compiler.CompileLambda(conditionExpr);

                        Result.Condition = new LogicalExecutableExpression(conditionExpr, compiledFunctionBody);
                        Result.ConditionExpression = conditionExpr;

                        _state = State.GotCondition;
                    }
                    break;

                case State.GotCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.ContentStarted;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.ContentStarted:
                    ParseCompoundHtnTaskItemsSectionContent();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }
    }
}

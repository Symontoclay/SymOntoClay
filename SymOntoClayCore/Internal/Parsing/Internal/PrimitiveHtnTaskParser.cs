using SymOntoClay.Core.Internal.CodeModel;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class PrimitiveHtnTaskParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotPrimitive,
            GotPrimitiveTask,
            GotName,
            ContentStarted
        }

        public PrimitiveHtnTaskParser(InternalParserContext context)
            : base(context)
        {
        }

        public PrimitiveHtnTask Result { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new PrimitiveHtnTask();

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
            Info("9AF1E157-908F-424F-8923-DF7D92660000", $"_state = {_state}");
            Info("6E3464D7-0CAB-4149-A196-267E7D6CA8F0", $"_currToken = {_currToken}");
            //Info(, $"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Primitive:
                                    _state = State.GotPrimitive;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotPrimitive:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Task:
                                    _state = State.GotPrimitiveTask;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotPrimitiveTask:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            Result.Name = ParseName(_currToken.Content);
                            _state = State.GotName;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotName:
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
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseFigureBracket:
                            Exit();
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Operator:
                                    {
                                        _context.Recovery(_currToken);

                                        var parser = new PrimitiveHtnTaskOperatorParser(_context);
                                        parser.Run();

                                        Result.Operator = parser.Result;
                                    }
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }
    }
}

using SymOntoClay.Core.Internal.CodeModel;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class StrategicTaskParser : BaseCompoundTaskParser
    {
        private enum State
        {
            Init,
            GotStrategic,
            GotStrategicTask,
            GotName,
            ContentStarted
        }

        public StrategicTaskParser(InternalParserContext context)
            : base(context)
        {
        }

        private StrategicTask _strategicTask;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _strategicTask = new StrategicTask();
            Result = _strategicTask;

            SetCurrentCodeItem(Result);
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            RemoveCurrentCodeEntity();
        }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info(, $"_state = {_state}");
            //Info(, $"_currToken = {_currToken}");
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
                                case KeyWordTokenKind.Strategic:
                                    _state = State.GotStrategic;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotStrategic:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Task:
                                    _state = State.GotStrategicTask;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotStrategicTask:
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

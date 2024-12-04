using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CompoundTaskParser: BaseCompoundTaskParser
    {
        private enum State
        {
            Init,
            GotCompound,
            GotCompoundTask,
            GotName,
            ContentStarted
        }

        public CompoundTaskParser(InternalParserContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            throw new NotImplementedException("9E053861-AF64-4264-8201-85FE6883DD3F");
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            throw new NotImplementedException("DB857224-277F-4F0A-9B0D-B92F2B7A46D2");
        }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Info("AA2DDA77-8684-48D7-9A83-0693221247CD", $"_state = {_state}");
            Info("AA99F952-32B4-4483-A58E-83370DC30CD3", $"_currToken = {_currToken}");
            //Info(, $"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Compound:
                                    _state = State.GotCompound;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotCompound:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Task:
                                    _state = State.GotCompoundTask;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotCompoundTask:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.ContentStarted:
                    switch (_currToken.TokenKind)
                    {
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

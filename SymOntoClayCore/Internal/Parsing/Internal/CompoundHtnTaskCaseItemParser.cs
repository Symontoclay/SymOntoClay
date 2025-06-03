using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CompoundHtnTaskCaseItemParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotName
        }

        public CompoundHtnTaskCaseItemParser(InternalParserContext context)
            : base(context)
        {
        }

        public CompoundHtnTaskCaseItem Result { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new CompoundHtnTaskCaseItem();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info("91E8B19B-8E64-48F9-AB1C-05A3FBA67455", $"_state = {_state}");
            //Info("7A6F5A6D-9E0F-4323-BB11-7ADDDC1614C0", $"_currToken = {_currToken}");
            //Info(, $"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            Result.Name = NameHelper.CreateName(_currToken.Content);
                            _state = State.GotName;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            Exit();
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

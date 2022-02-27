using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class AccessibilityAreasParser: BaseObjectParser
    {
        private enum State
        {
            Init,
            GotAccessibilityAreasMark
        }

        public AccessibilityAreasParser(InternalParserContext context, CodeItem codeItem)
            : base(context, codeItem)
        {
            _codeItem = codeItem;
        }

        private State _state = State.Init;

        private CodeItem _codeItem;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {


                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAccessibilityAreasMark:
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

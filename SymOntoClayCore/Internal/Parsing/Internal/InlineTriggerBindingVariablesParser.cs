using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class InlineTriggerBindingVariablesParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForLeftVariable,
            WaitForKindOfBinding,
            WaitForRightVariable
        }

        public InlineTriggerBindingVariablesParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public List<BindingVariableItem> Result { get; private set; } = new List<BindingVariableItem>();
        private BindingVariableItem _currentItem;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            Log($"Result = {Result.WriteListToString()}");
            Log($"_currentItem = {_currentItem}");
            Log($"_state = {_state}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            InitItem();
                            _state = State.WaitForLeftVariable;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;


                case State.WaitForLeftVariable:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.LogicalVar:
                            throw new NotImplementedException();

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForKindOfBinding:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForRightVariable:
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

        private void InitItem()
        {
            if(_currentItem != null)
            {
                Result.Add(_currentItem);
            }

            _currentItem = new BindingVariableItem();
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            if (_currentItem != null)
            {
                Result.Add(_currentItem);
            }
        }
    }
}

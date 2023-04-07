/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
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
            WaitForRightVariable,
            GotRightVariable
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
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:                            
                            _state = State.WaitForLeftVariable;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;


                case State.WaitForLeftVariable:
                    InitItem();

                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.LogicalVar:                            
                            _currentItem.LeftVariable = NameHelper.CreateName(_currToken.Content);
                            _state = State.WaitForKindOfBinding;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForKindOfBinding:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.LeftRightStream:
                            _currentItem.Kind = KindOfBindingVariable.LeftToRignt;
                            _state = State.WaitForRightVariable;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForRightVariable:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                            _currentItem.RightVariable = NameHelper.CreateName(_currToken.Content);
                            _state = State.GotRightVariable;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotRightVariable:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _state = State.WaitForLeftVariable;
                            break;

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

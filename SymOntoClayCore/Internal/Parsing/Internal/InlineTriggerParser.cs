/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using NLog.LayoutRenderers.Wrappers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class InlineTriggerParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForCondition,
            GotCondition,
            GotBindingVariables,
            WaitForAction,
            GotAction
        }

        public InlineTriggerParser(InternalParserContext context)
            : base(context)
        {
        }
        
        private State _state = State.Init;
        public InlineTrigger Result => _inlineTrigger;
        private InlineTrigger _inlineTrigger;

        /// <inheritdoc/>
        protected override void OnEnter()
        {      
            _inlineTrigger = CreateInlineTriggerAndSetAsCurrentCodeItem();
            _inlineTrigger.Name = NameHelper.CreateRuleOrFactName();
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
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.On:
                                    _state = State.WaitForCondition;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Enter:
                                    _inlineTrigger.KindOfInlineTrigger = KindOfInlineTrigger.SystemEvent;
                                    _inlineTrigger.KindOfSystemEvent = KindOfSystemEventOfInlineTrigger.Enter;
                                    _state = State.GotCondition;
                                    break;

                                case KeyWordTokenKind.Leave:
                                    _inlineTrigger.KindOfInlineTrigger = KindOfInlineTrigger.SystemEvent;
                                    _inlineTrigger.KindOfSystemEvent = KindOfSystemEventOfInlineTrigger.Leave;
                                    _state = State.GotCondition;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.OpenFactBracket:
                            {
                                _inlineTrigger.KindOfInlineTrigger = KindOfInlineTrigger.LogicConditional;

                                _context.Recovery(_currToken);

                                var parser = new LogicalQueryParser(_context);
                                parser.Run();

                                _inlineTrigger.Condition = parser.Result;

                                _state = State.GotCondition;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        case TokenKind.OpenRoundBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new InlineTriggerBindingVariablesParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result.WriteListToString()}");
#endif

                                _inlineTrigger.BindingVariables = new BindingVariables(parser.Result);

                                _state = State.GotBindingVariables;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotBindingVariables:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.On:
                                case KeyWordTokenKind.Fun:
                                case KeyWordTokenKind.Operator:
                                case KeyWordTokenKind.Public:
                                case KeyWordTokenKind.Protected:
                                case KeyWordTokenKind.Private:
                                    _context.Recovery(_currToken);
                                    Exit();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessFunctionBody()
        {
            _context.Recovery(_currToken);
            var parser = new FunctionBodyParser(_context);
            parser.Run();
            var statementsList = parser.Result;
            _inlineTrigger.Statements = statementsList;
            _inlineTrigger.CompiledFunctionBody = _context.Compiler.Compile(statementsList);
            _state = State.GotAction;
        }
    }
}

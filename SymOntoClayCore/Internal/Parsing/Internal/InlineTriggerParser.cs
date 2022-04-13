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
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
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
            WaitForSetCondition,
            WaitForCloseRoundBracketOfSetCondition,
            GotSetCondition,
            GotSetBindingVariables,
            GotDown,
            WaitForResetCondition,
            WaitForCloseRoundBracketOfResetCondition,
            GotResetCondition,
            WaitForDoubleConditionsStrategy,
            WaitForFinishingDoubleConditionsStrategy,
            GotDoubleConditionsStrategy,
            WaitForName,
            GotName,
            GotAlias,
            WaitForSetAction,
            GotSetAction,
            WaitForResetAction,
            GotResetAction
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
                                    _state = State.WaitForSetCondition;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForSetCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Enter:
                                    _inlineTrigger.KindOfInlineTrigger = KindOfInlineTrigger.SystemEvent;
                                    _inlineTrigger.KindOfSystemEvent = KindOfSystemEventOfInlineTrigger.Enter;
                                    _state = State.GotSetCondition;
                                    break;

                                case KeyWordTokenKind.Leave:
                                    _inlineTrigger.KindOfInlineTrigger = KindOfInlineTrigger.SystemEvent;
                                    _inlineTrigger.KindOfSystemEvent = KindOfSystemEventOfInlineTrigger.Leave;
                                    _state = State.GotSetCondition;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.OpenFactBracket:                        
                            {
                                _inlineTrigger.KindOfInlineTrigger = KindOfInlineTrigger.LogicConditional;

                                _context.Recovery(_currToken);

                                var parser = new TriggerConditionParser(_context, new TerminationToken(TokenKind.Word, KeyWordTokenKind.Down));
                                parser.Run();

                                _inlineTrigger.SetCondition = parser.Result;

                                _state = State.GotSetCondition;
                            }
                            break;

                        case TokenKind.OpenRoundBracket:
                            {
                                _inlineTrigger.KindOfInlineTrigger = KindOfInlineTrigger.LogicConditional;

                                var parser = new TriggerConditionParser(_context, TokenKind.CloseRoundBracket);
                                parser.Run();

                                _inlineTrigger.SetCondition = parser.Result;

                                _state = State.WaitForCloseRoundBracketOfSetCondition;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForCloseRoundBracketOfSetCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            _state = State.GotSetCondition;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSetCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForSetAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            ProcessSetFunctionBody();
                            break;

                        case TokenKind.OpenRoundBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new InlineTriggerBindingVariablesParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result.WriteListToString()}");
#endif

                                _inlineTrigger.SetBindingVariables = new BindingVariables(parser.Result);

                                _state = State.GotSetBindingVariables;
                            }
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Down:
                                    if(_inlineTrigger.KindOfInlineTrigger == KindOfInlineTrigger.LogicConditional)
                                    {
                                        _state = State.GotDown;
                                        break;
                                    }
                                    throw new UnexpectedTokenException(_currToken);

                                case KeyWordTokenKind.Duration:
                                    {
                                        _context.Recovery(_currToken);

                                        var parser = new TriggerConditionParser(_context);
                                        parser.Run();

                                        _inlineTrigger.ResetCondition = parser.Result;

                                        _state = State.GotResetCondition;
                                    }
                                    break;

                                case KeyWordTokenKind.As:
                                    _state = State.WaitForName;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSetBindingVariables:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForSetAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            ProcessSetFunctionBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotDown:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.On:
                            _state = State.WaitForResetCondition;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForResetCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:                        
                            {
                                _context.Recovery(_currToken);

                                var parser = new TriggerConditionParser(_context);
                                parser.Run();

                                _inlineTrigger.ResetCondition = parser.Result;

                                _state = State.GotResetCondition;
                            }
                            break;

                        case TokenKind.OpenRoundBracket:
                            {
                                var parser = new TriggerConditionParser(_context, TokenKind.CloseRoundBracket);
                                parser.Run();

                                _inlineTrigger.ResetCondition = parser.Result;

                                _state = State.WaitForCloseRoundBracketOfResetCondition;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForCloseRoundBracketOfResetCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            _state = State.GotResetCondition;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotResetCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForSetAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            ProcessSetFunctionBody();
                            break;

                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForDoubleConditionsStrategy;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForDoubleConditionsStrategy:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Set:
                                    _inlineTrigger.DoubleConditionsStrategy = DoubleConditionsStrategy.PriorSet;
                                    _state = State.WaitForFinishingDoubleConditionsStrategy;
                                    break;

                                case KeyWordTokenKind.Down:
                                    _inlineTrigger.DoubleConditionsStrategy = DoubleConditionsStrategy.PriorReset;
                                    _state = State.WaitForFinishingDoubleConditionsStrategy;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.Assign:
                            _inlineTrigger.DoubleConditionsStrategy = DoubleConditionsStrategy.Equal;
                            _state = State.WaitForFinishingDoubleConditionsStrategy;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForFinishingDoubleConditionsStrategy:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            _state = State.GotDoubleConditionsStrategy;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotDoubleConditionsStrategy:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForSetAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            ProcessSetFunctionBody();
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.As:
                                    _state = State.WaitForName;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            Result.Name = NameHelper.CreateName(_currToken.Content);
                            _state = State.GotName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForSetAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            ProcessSetFunctionBody();
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Alias:
                                    {
                                        _context.Recovery(_currToken);

                                        var parser = new AliasPaser(_context, TokenKind.Lambda, TokenKind.OpenFigureBracket);
                                        parser.Run();

                                        Result.AddAliasRange(parser.Result);

                                        _state = State.GotAlias;
                                    }
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAlias:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForSetAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            ProcessSetFunctionBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForSetAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessSetFunctionBody();
                            break;

                        case TokenKind.OpenFactBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new InlineTriggerActiveRuleContentParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result.WriteListToString()}");
#endif

                                _inlineTrigger.RuleInstancesList = parser.Result;

                                Exit();
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSetAction:
                    if (IsTerminator())
                    {
                        _context.Recovery(_currToken);
                        Exit();
                        break;
                    }

                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Else:
                                    _state = State.WaitForResetAction;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForResetAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessResetFunctionBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotResetAction:
                    if(IsTerminator())
                    {
                        _context.Recovery(_currToken);
                        Exit();
                        break;
                    }

                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private bool IsTerminator()
        {
            switch (_currToken.TokenKind)
            {
                case TokenKind.CloseFigureBracket:
                    return true;

                case TokenKind.Word:
                    switch(_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.On:
                        case KeyWordTokenKind.Fun:
                        case KeyWordTokenKind.Operator:
                        case KeyWordTokenKind.Public:
                        case KeyWordTokenKind.Protected:
                        case KeyWordTokenKind.Private:
                        case KeyWordTokenKind.Var:
                            return true;

                        default:
                            return false;
                    }

                default:
                    return false;
            }
        }

        private void ProcessSetFunctionBody()
        {
            var resultOfParsing = ParseFunctionBody();

            _inlineTrigger.SetStatements = resultOfParsing.Item1;
            _inlineTrigger.SetCompiledFunctionBody = resultOfParsing.Item2;
            _state = State.GotSetAction;
        }

        private void ProcessResetFunctionBody()
        {
            var resultOfParsing = ParseFunctionBody();

            _inlineTrigger.ResetStatements = resultOfParsing.Item1;
            _inlineTrigger.ResetCompiledFunctionBody = resultOfParsing.Item2;
            _state = State.GotResetAction;
        }

        private (List<AstStatement>, CompiledFunctionBody) ParseFunctionBody()
        {
            _context.Recovery(_currToken);
            var parser = new FunctionBodyParser(_context);
            parser.Run();
            var statementsList = parser.Result;

            return (statementsList, _context.Compiler.Compile(statementsList));
        }
    }
}

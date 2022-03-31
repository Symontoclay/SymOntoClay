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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class BaseObjectParser : BaseInternalParser
    {
        protected BaseObjectParser(InternalParserContext context, KindOfCodeEntity kindOfCodeEntity)
            : base(context)
        {
            _kindOfCodeEntity = kindOfCodeEntity;
        }

        protected BaseObjectParser(InternalParserContext context, CodeItem codeItem)
            : base(context)
        {
            _inAccessibilityAreas = true;
            Result = codeItem;
        }

        private readonly KindOfCodeEntity _kindOfCodeEntity;
        private readonly bool _inAccessibilityAreas;

        public CodeItem Result { get; protected set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
#if DEBUG
            //Log("Begin");
#endif

            if(!_inAccessibilityAreas)
            {
                Result = ObjectFactory(_kindOfCodeEntity);

                SetCurrentCodeItem(Result);
            }

#if DEBUG
            //Log("End");
#endif
        }

        private CodeItem ObjectFactory(KindOfCodeEntity kind)
        {
            switch(kind)
            {
                case KindOfCodeEntity.Action:
                    return CreateAction();

                case KindOfCodeEntity.State:
                    return CreateState();

                case KindOfCodeEntity.App:
                    return CreateApp();

                case KindOfCodeEntity.Class:
                    return CreateClass();

                case KindOfCodeEntity.World:
                    return CreateWorld();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
#if DEBUG
            //Log("Begin");
#endif

            RemoveCurrentCodeEntity();

#if DEBUG
            //Log("End");
#endif
        }

        protected virtual void OnAddInlineTrigger(InlineTrigger inlineTrigger)
        {
        }

        protected virtual void OnAddNamedFunction(NamedFunction namedFunction)
        {
        }

        protected virtual void OnAddOperator(Operator op)
        {
        }

        protected virtual void OnAddRuleInstance(RuleInstance ruleInstance)
        {
        }

        protected void ProcessGeneralContent()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"(_context.CurrentDefaultSetings != null) = {_context.CurrentDefaultSetings != null}");
#endif

            switch (_currToken.TokenKind)
            {
                case TokenKind.CloseFigureBracket:
                    Exit();
                    break;

                case TokenKind.Word:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.On:
                            {
                                _context.Recovery(_currToken);
                                var parser = new InlineTriggerParser(_context);
                                parser.Run();

                                var inlineTrigger = parser.Result;

                                Result.SubItems.Add(inlineTrigger);

                                OnAddInlineTrigger(inlineTrigger);
                            }
                            break;

                        case KeyWordTokenKind.Fun:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NamedFunctionParser(_context);
                                parser.Run();

                                var namedFunction = parser.Result;

                                Result.SubItems.Add(namedFunction);

                                OnAddNamedFunction(namedFunction);
                            }
                            break;

                        case KeyWordTokenKind.Operator:
                            {
                                _context.Recovery(_currToken);
                                var parser = new OperatorParser(_context);
                                parser.Run();

                                var op = parser.Result;

                                Result.SubItems.Add(op);

                                OnAddOperator(op);
                            }
                            break;

                        case KeyWordTokenKind.Var:
                            ProcessVar();
                            break;

                        case KeyWordTokenKind.Public:
                        case KeyWordTokenKind.Protected:
                        case KeyWordTokenKind.Private:
                            {
                                _context.Recovery(_currToken);
                                var parser = new AccessibilityAreasParser(_context, Result);
                                parser.Run();

#if DEBUG
                                //Log($"Result = {Result}");
#endif

                                Exit();
                            }
                            break;

                        case KeyWordTokenKind.Set:
                            {
                                _context.Recovery(_currToken);
                                var parser = new SetDirectiveParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result.WriteListToString()}");
#endif

                                Result.Directives.AddRange(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Enter:
                            {
                                var nextToken = _context.GetToken();

#if DEBUG
                                //Log($"nextToken = {nextToken}");
#endif

                                switch (nextToken.TokenKind)
                                {
                                    case TokenKind.Word:
                                        switch(nextToken.KeyWordTokenKind)
                                        {
                                            case KeyWordTokenKind.On:
                                                switch(Result.Kind)
                                                {
                                                    case KindOfCodeEntity.State:
                                                        {
                                                            var parser = new LogicalClausesSectionParser(_context);
                                                            parser.Run();

#if DEBUG
                                                            //Log($"parser.Result = {parser.Result.WriteListToString()}");
#endif

                                                            Result.ActivatingConditions.AddRange(parser.Result);
                                                        }
                                                        break;

                                                    default:
                                                        throw new ArgumentOutOfRangeException(nameof(Result.Kind), Result.Kind, null);
                                                }
                                                break;

                                            default:
                                                throw new UnexpectedTokenException(_currToken);
                                        }
                                        break;

                                    default:
                                        throw new UnexpectedTokenException(_currToken);
                                }
                            }
                            break;

                        case KeyWordTokenKind.Leave:
                            {
                                var nextToken = _context.GetToken();

#if DEBUG
                                //Log($"nextToken = {nextToken}");
#endif

                                switch (nextToken.TokenKind)
                                {
                                    case TokenKind.Word:
                                        switch(nextToken.KeyWordTokenKind)
                                        {
                                            case KeyWordTokenKind.On:
                                                switch(Result.Kind)
                                                {
                                                    case KindOfCodeEntity.State:
                                                        {
                                                            var parser = new LogicalClausesSectionParser(_context);
                                                            parser.Run();

#if DEBUG
                                                            //Log($"parser.Result = {parser.Result.WriteListToString()}");
#endif

                                                            Result.DeactivatingConditions.AddRange(parser.Result.Select(p => p.Condition));
                                                        }
                                                        break;

                                                    default:
                                                        throw new ArgumentOutOfRangeException(nameof(Result.Kind), Result.Kind, null);
                                                }
                                                break;

                                            default:
                                                throw new UnexpectedTokenException(_currToken);
                                        }
                                        break;

                                    default:
                                        throw new UnexpectedTokenException(_currToken);
                                }
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case TokenKind.OpenFactBracket:
                    {
                        _context.Recovery(_currToken);
                        var parser = new LogicalQueryAsCodeEntityParser(_context);
                        parser.Run();

                        var ruleInstance = parser.Result;

                        Result.SubItems.Add(ruleInstance);

                        OnAddRuleInstance(ruleInstance);
                    }
                    break;

                case TokenKind.Var:
                    ProcessVar();
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void ProcessVar()
        {
            _context.Recovery(_currToken);

            var parser = new FieldParser(_context);
            parser.Run();

            Result.SubItems.Add(parser.Result);
        }
    }
}

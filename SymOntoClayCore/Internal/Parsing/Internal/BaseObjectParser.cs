/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
            : this(context, kindOfCodeEntity, null)
        {
        }

        protected BaseObjectParser(InternalParserContext context, KindOfCodeEntity kindOfCodeEntity, TerminationToken[] terminationTokens)
            : base(context, terminationTokens)
        {
            _kindOfCodeEntity = kindOfCodeEntity;
        }

        protected BaseObjectParser(InternalParserContext context, CodeItem codeItem)
            : this(context, codeItem, null)
        {
        }

        protected BaseObjectParser(InternalParserContext context, CodeItem codeItem, TerminationToken[] terminationTokens)
            : base(context, terminationTokens)
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
            if(!_inAccessibilityAreas)
            {
                Result = ObjectFactory(_kindOfCodeEntity);

                SetCurrentCodeItem(Result);
            }
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

                case KindOfCodeEntity.AnonymousObject:
                    return CreateAnonymousObject();

                case KindOfCodeEntity.World:
                    return CreateWorld();

                case KindOfCodeEntity.Lib:
                    return CreateLib();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            RemoveCurrentCodeEntity();
        }

        protected virtual void OnAddInlineTrigger(InlineTrigger inlineTrigger)
        {
        }

        protected virtual void OnAddNamedFunction(NamedFunction namedFunction)
        {
        }

        protected virtual void OnAddConstructor(Constructor constructor)
        {
        }

        protected virtual void OnAddOperator(Operator op)
        {
        }

        protected virtual void OnAddRuleInstance(RuleInstance ruleInstance)
        {
        }

        protected void ProcessInheritance()
        {
            _context.Recovery(_currToken);
            var parser = new InheritanceParser(_context, Result.Name, TokenKind.OpenFigureBracket);
            parser.Run();
            Result.InheritanceItems.AddRange(parser.Result);
        }

        /// <summary>
        /// Processes general content of object
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="UnexpectedTokenException"></exception>
        protected void ProcessGeneralContent()
        {
#if DEBUG
            Info("62DA50CF-1985-4CD1-BDF4-5E654AD4C6BD", $"62DA50CF-1985-4CD1-BDF4-5E654AD4C6BD _currToken = {_currToken}");
            //sInfo("AF5D18E8-D25F-4E9B-8D1B-351532AB581C", $"AF5D18E8-D25F-4E9B-8D1B-351532AB581C Result = {Result}");
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
                                var parser = new FunctionDeclParser(_context);
                                parser.Run();

                                var namedFunction = parser.Result;

                                Result.SubItems.Add(namedFunction);

                                OnAddNamedFunction(namedFunction);
                            }
                            break;

                        case KeyWordTokenKind.Constructor:
                            {
                                _context.Recovery(_currToken);
                                var parser = new ConstructorDeclParser(_context);

                                parser.Run();

                                var constructor = parser.Result;

                                Result.SubItems.Add(constructor);

                                OnAddConstructor(constructor);
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

                                Exit();
                            }
                            break;

                        case KeyWordTokenKind.Set:
                            {
                                _context.Recovery(_currToken);
                                var parser = new SetDirectiveParser(_context);
                                parser.Run();

                                Result.Directives.AddRange(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Enter:
                            {
                                var nextToken = _context.GetToken();

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

                                                            Result.DeactivatingConditions.AddRange(parser.Result.Select(p => p.Condition));
                                                        }
                                                        break;

                                                    default:
                                                        throw new ArgumentOutOfRangeException(nameof(Result.Kind), Result.Kind, null);
                                                }
                                                break;

                                            default:
                                                throw new UnexpectedTokenException(nextToken);
                                        }
                                        break;

                                    default:
                                        throw new UnexpectedTokenException(nextToken);
                                }
                            }
                            break;

                        case KeyWordTokenKind.Relation:
                            {
                                _context.Recovery(_currToken);
                                var parser = new RelationDescriptionParser(_context);
                                parser.Run();
                                Result.SubItems.Add(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Idle:
                            {
                                _context.Recovery(_currToken);
                                var parser = new IdleActionsParser(_context);
                                parser.Run();
                                Result.IdleActionItems.AddRange(parser.Result);
                            }
                            break;

                        case KeyWordTokenKind.Import:
                            {
                                _context.Recovery(_currToken);
                                var parser = new ImportParser(_context);
                                parser.Run();

                                Result.ImportsList.Add(parser.Result);
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

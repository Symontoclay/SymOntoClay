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
using System;
using System.Collections.Generic;
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

        private readonly KindOfCodeEntity _kindOfCodeEntity;

        public CodeItem Result { get; protected set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
#if DEBUG
            //Log("Begin");
#endif
            Result = CreateCodeEntityAndSetAsCurrent(_kindOfCodeEntity);

#if DEBUG
            //Log("End");
#endif
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

                                var codeEntity = parser.Result;

                                Result.SubItems.Add(codeEntity);

                                OnAddInlineTrigger(codeEntity.InlineTrigger, codeEntity);
                            }
                            break;

                        case KeyWordTokenKind.Fun:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NamedFunctionParser(_context);
                                parser.Run();

                                var codeEntity = parser.Result;

                                Result.SubItems.Add(codeEntity);

                                OnAddNamedFunction(codeEntity.NamedFunction, codeEntity);
                            }
                            break;

                        case KeyWordTokenKind.Operator:
                            {
                                _context.Recovery(_currToken);
                                var parser = new OperatorParser(_context);
                                parser.Run();

                                var codeEntity = parser.Result;

                                Result.SubItems.Add(codeEntity);

                                OnAddOperator(codeEntity.Operator, codeEntity);
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

                        var codeEntity = parser.Result;

                        Result.SubItems.Add(codeEntity);

                        OnAddRuleInstance(codeEntity.RuleInstance, codeEntity);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }
    }
}

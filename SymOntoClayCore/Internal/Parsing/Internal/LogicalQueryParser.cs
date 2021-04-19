/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalQueryParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForContent,
            WaitForPrimaryRulePart,
            GotPrimaryRulePart,
            WaitForSecondaryRulePart,
            GotSecondaryRulePart
        }

        public LogicalQueryParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public RuleInstance Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new RuleInstance();
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            if (Result.Name == null || Result.Name.IsEmpty)
            {
                Result.Name = NameHelper.CreateRuleOrFactName();
            }

            var secondaryPartsList = Result.SecondaryParts;

            if (secondaryPartsList.IsNullOrEmpty())
            {
                var primaryPart = Result.PrimaryPart;

                if(primaryPart.HasQuestionVars)
                {
                    Result.Kind = KindOfRuleInstance.Question;
                }
                else
                {
                    Result.Kind = KindOfRuleInstance.Fact;
                }
            }
            else
            {
                Result.Kind = KindOfRuleInstance.Rule;

                var primaryPart = Result.PrimaryPart;

                primaryPart.SecondaryParts = secondaryPartsList.ToList();

                foreach(var secondaryPart in secondaryPartsList)
                {
                    secondaryPart.PrimaryPart = primaryPart;
                }
            }

            Result.CheckDirty();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            switch(_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            _state = State.WaitForContent;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForContent:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.PrimaryLogicalPartMark:
                            _state = State.WaitForPrimaryRulePart;
                            break;

                        case TokenKind.Word:
                        case TokenKind.LogicalVar:
                        {
                                _context.Recovery(_currToken);

                                var paser = new PrimaryRulePartParser(_context, TokenKind.CloseFactBracket);
                                paser.Run();

#if DEBUG
                                //Log($"paser.Result = {paser.Result}");
#endif
                                paser.Result.Parent = Result;
                                Result.PrimaryPart = paser.Result;

                                _state = State.GotPrimaryRulePart;
                            }
                            break;

                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);

                                var paser = new PrimaryRulePartParser(_context, TokenKind.CloseFigureBracket);
                                paser.Run();

#if DEBUG
                                //Log($"paser.Result = {paser.Result}");
#endif
                                paser.Result.Parent = Result;
                                Result.PrimaryPart = paser.Result;

                                _state = State.GotPrimaryRulePart;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForPrimaryRulePart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);

                                var paser = new PrimaryRulePartParser(_context, TokenKind.CloseFigureBracket);
                                paser.Run();

#if DEBUG
                                //Log($"paser.Result = {paser.Result}");
#endif
                                paser.Result.Parent = Result;
                                Result.PrimaryPart = paser.Result;

                                _state = State.GotPrimaryRulePart;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotPrimaryRulePart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseFactBracket:
                            Exit();
                            break;

                        case TokenKind.LeftRightArrow:
                            Result.PrimaryPart.IsActive = true;
                            _state = State.WaitForSecondaryRulePart;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForSecondaryRulePart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);

                                var paser = new SecondaryRulePartParser(_context, TokenKind.CloseFigureBracket);
                                paser.Run();

#if DEBUG
                                //Log($"paser.Result = {paser.Result}");
#endif

                                if(Result.SecondaryParts == null)
                                {
                                    Result.SecondaryParts = new List<SecondaryRulePart>();
                                }

                                paser.Result.Parent = Result;

                                Result.SecondaryParts.Add(paser.Result);

                                _state = State.GotSecondaryRulePart;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSecondaryRulePart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseFactBracket:
                            Exit();
                            break;

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

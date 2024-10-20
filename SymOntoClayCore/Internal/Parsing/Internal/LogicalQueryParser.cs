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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

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
            GotSecondaryRulePart,
            GotObligationModality,
            GotSelfObligationModality
        }

        public LogicalQueryParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public RuleInstance Result { get; private set; }

        private bool _nameHasBeenParsed;
        private bool _primaryPartHasBeenParsed;
        private bool _secondaryPartHasBeenParsed;
        private bool _obligationModalityHasBeenParsed;
        private bool _selfObligationModalityHasBeenParsed;

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
                    Result.KindOfRuleInstance = KindOfRuleInstance.Question;
                }
                else
                {
                    Result.KindOfRuleInstance = KindOfRuleInstance.Fact;
                }
            }
            else
            {
                Result.KindOfRuleInstance = KindOfRuleInstance.Rule;

                var primaryPart = Result.PrimaryPart;

                primaryPart.SecondaryParts = secondaryPartsList.ToList();

                foreach(var secondaryPart in secondaryPartsList)
                {
                    secondaryPart.PrimaryPart = primaryPart;
                }
            }

            if(_context.NeedCheckDirty)
            {
                Result.CheckDirty();
            }
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
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
                        case TokenKind.Not:
                            ProcessPrimaryRulePart(TokenKind.CloseFactBracket);
                            break;

                        case TokenKind.OpenFigureBracket:
                            ProcessPrimaryRulePart(TokenKind.CloseFigureBracket);
                            break;

                        case TokenKind.Entity:
                            {
                                if(_nameHasBeenParsed)
                                {
                                    throw new NotImplementedException();
                                }

                                Result.Name = NameHelper.CreateName(_currToken.Content);

                                _nameHasBeenParsed = true;
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
                            ProcessPrimaryRulePart(TokenKind.CloseFigureBracket);
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

                        case TokenKind.Word:
                            ProcessModalities();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForSecondaryRulePart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessSecondaryRulePart(TokenKind.CloseFigureBracket);
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

                        case TokenKind.Word:
                            ProcessModalities();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotObligationModality:                    
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseFactBracket:
                            Exit();
                            break;

                        case TokenKind.Word:
                            ProcessModalities();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSelfObligationModality:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseFactBracket:
                            Exit();
                            break;

                        case TokenKind.Word:
                            ProcessModalities();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessPrimaryRulePart(TokenKind terminatingTokenKind)
        {
            if(_primaryPartHasBeenParsed)
            {
                throw new UnexpectedTokenException(_currToken);
            }

            _context.Recovery(_currToken);

            var parser = new PrimaryRulePartParser(_context, terminatingTokenKind);
            parser.Run();

            parser.Result.Parent = Result;
            Result.PrimaryPart = parser.Result;

            _primaryPartHasBeenParsed = true;

            _state = State.GotPrimaryRulePart;
        }

        private void ProcessSecondaryRulePart(TokenKind terminatingTokenKind)
        {
            if(_secondaryPartHasBeenParsed)
            {
                throw new UnexpectedTokenException(_currToken);
            }

            _context.Recovery(_currToken);

            var parser = new SecondaryRulePartParser(_context, terminatingTokenKind);
            parser.Run();

            if (Result.SecondaryParts == null)
            {
                Result.SecondaryParts = new List<SecondaryRulePart>();
            }

            parser.Result.Parent = Result;

            Result.SecondaryParts.Add(parser.Result);

            _secondaryPartHasBeenParsed = true;

            _state = State.GotSecondaryRulePart;
        }

        private void ProcessModalities()
        {
            var kindOfRuleInstanceSectionMark = GetKindOfRuleInstanceSectionMark();

            switch (kindOfRuleInstanceSectionMark)
            {
                case KindOfRuleInstanceSectionMark.ObligationModality:
                    {
                        if(_obligationModalityHasBeenParsed)
                        {
                            throw new UnexpectedTokenException(_currToken);
                        }

                        var parser = new LogicalModalityValueParser(_context);
                        parser.Run();

                        Result.ObligationModality = parser.Result;

                        _obligationModalityHasBeenParsed = true;

                        _state = State.GotObligationModality;
                    }
                    break;

                case KindOfRuleInstanceSectionMark.SelfObligationModality:
                    {
                        if(_selfObligationModalityHasBeenParsed)
                        {
                            throw new UnexpectedTokenException(_currToken);
                        }

                        var parser = new LogicalModalityValueParser(_context);
                        parser.Run();

                        Result.SelfObligationModality = parser.Result;

                        _selfObligationModalityHasBeenParsed = true;

                        _state = State.GotSelfObligationModality;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfRuleInstanceSectionMark), kindOfRuleInstanceSectionMark, null);
            }
        }
    }
}

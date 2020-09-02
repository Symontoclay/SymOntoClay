﻿using SymOntoClay.Core.Internal.CodeModel;
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
            Result.DictionaryName = _context.Dictionary.Name;

            if (Result.Name == null || Result.Name.IsEmpty)
            {
                Result.Name = NameHelper.CreateRuleOrFactName(_context.Dictionary);
            }

            var secondaryPartsList = Result.SecondaryParts;

            if (!secondaryPartsList.IsNullOrEmpty())
            {
                var primaryPart = Result.PrimaryPart;

                primaryPart.SecondaryParts = secondaryPartsList.ToList();

                foreach(var secondaryPart in secondaryPartsList)
                {
                    secondaryPart.PrimaryPart = primaryPart;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            Log($"_state = {_state}");
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
                            {
                                _context.Recovery(_currToken);

                                var paser = new PrimaryRulePartParser(_context, TokenKind.CloseFactBracket);
                                paser.Run();

#if DEBUG
                                Log($"paser.Result = {paser.Result}");
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
                                Log($"paser.Result = {paser.Result}");
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
                                Log($"paser.Result = {paser.Result}");
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
                                Log($"paser.Result = {paser.Result}");
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

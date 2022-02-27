﻿using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class FieldParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotName,
            WaitForType,
            GotType,
            WaitForValue,
            GotValue
        }

        public FieldParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public Field Result => _field;
        private Field _field;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _field = CreateField();
            _field.TypeOfAccess = _context.CurrentDefaultSetings.TypeOfAccess;
#if DEBUG
            //Log($"_field = {_field}");
            //Log($"_context.CurrentDefaultSetings.TypeOfAccess = {_context.CurrentDefaultSetings.TypeOfAccess}");
#endif
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"_field = {_field}");
            //Log($"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                            _field.Name = ParseName(_currToken.Content);
                            _state = State.GotName;
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Var:
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            Exit();
                            break;

                        case TokenKind.Colon:
                            _state = State.WaitForType;
                            break;

                        case TokenKind.Assign:
                            _state = State.WaitForValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForType:
                    _field.TypesList = ParseTypesOfParameterOrVar();
                    _state = State.GotType;
                    break;

                case State.GotType:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            Exit();
                            break;

                        case TokenKind.Assign:
                            _state = State.WaitForValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForValue:
                    {
                        var parsingResult = ParseValueOnObjDefLevel();

#if DEBUG
                        //Log($"parsingResult = {parsingResult}");
#endif

                        var kindOfValueOnObjDefLevel = parsingResult.Kind;

                        switch (kindOfValueOnObjDefLevel)
                        {
                            case KindOfValueOnObjDefLevel.ConstLiteral:
                                _field.Value = parsingResult.Value;
                                _state = State.GotValue;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfValueOnObjDefLevel), kindOfValueOnObjDefLevel, null);
                        }
                    }
                    break;

                case State.GotValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
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

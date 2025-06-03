using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class PropertyParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotPropMark,
            GotName,
            WaitForType,
            GotType,
            WaitForLambdaGetCode,
            WaitForValue,
            GotValue
        }

        public PropertyParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public Property Result => _property;
        private Property _property;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _property = CreateProperty();
            _property.TypeOfAccess = _context.CurrentDefaultSettings.TypeOfAccess;
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info("9A4B06EC-8658-4B3B-9D65-A97DAD9C59A1", $"_state = {_state}");
            //Info("584F3684-A578-4E83-AADE-38FF794F372F", $"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Prop:
                                    _state = State.GotPropMark;
                                    break;

                                default:
                                    ParsePropertyName();
                                    break;
                            }
                            break;

                        case TokenKind.Identifier:
                            ParsePropertyName();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotPropMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            ParsePropertyName();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
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

                        case TokenKind.Lambda:
                            _state = State.WaitForLambdaGetCode;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForType:
                    _property.TypesList = ParseTypesOfParameterOrVar();
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

                        case TokenKind.Lambda:
                            _state = State.WaitForLambdaGetCode;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForLambdaGetCode:
                    {
                        _context.Recovery(_currToken);
                        var parser = new CodeExpressionStatementParser(_context);
                        parser.Run();

                        _property.KindOfProperty = KindOfProperty.Readonly;

#if DEBUG
                        //Info("CFC03BA9-895D-44C7-A6FD-3B127CDCA5E9", $"parser.Result = {parser.Result}");
#endif

                        _property.GetStatements.Add(parser.Result);

                        _property.GetCompiledFunctionBody = _context.Compiler.CompileLambda(parser.Result);

                        Exit();
                    }
                    break;

                case State.WaitForValue:
                    {
                        _context.Recovery(_currToken);

                        var parser = new AstExpressionParser(_context, TokenKind.Semicolon);
                        parser.Run();

                        _property.DefaultValue = parser.Result;
                        _state = State.GotValue;
                    }
                    break;

                case State.GotValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }

        private void ParsePropertyName()
        {
            var name = ParseName(_currToken.Content);

            _property.Name = name;

            _state = State.GotName;
        }
    }
}

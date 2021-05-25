using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class FunctionParametersParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForParameter,
            GotParameterName,
            WaitForParameterType,
            GotOneParameterType,
            WaitForDefaultValue,
            GotDefaultValue,
            GotComma
        }

        public FunctionParametersParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public List<FunctionArgumentInfo> Result { get; set; } = new List<FunctionArgumentInfo>();
        private FunctionArgumentInfo _curentFunctionArgumentInfo;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        case TokenKind.Var:
                            {
                                _curentFunctionArgumentInfo = new FunctionArgumentInfo();
                                Result.Add(_curentFunctionArgumentInfo);

                                _curentFunctionArgumentInfo.Name = ParseName(_currToken.Content);

                                _state = State.GotParameterName;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotParameterName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _state = State.GotComma;
                            break;

                        case TokenKind.Assign:
                            _state = State.WaitForDefaultValue;
                            break;

                        case TokenKind.Colon:
                            _state = State.WaitForParameterType;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForParameterType:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                _curentFunctionArgumentInfo.TypesList.Add(ParseName(_currToken.Content));

                                _state = State.GotOneParameterType;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotOneParameterType:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        case TokenKind.Assign:
                            _state = State.WaitForDefaultValue;
                            break;

                        case TokenKind.Comma:
                            _state = State.GotComma;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForDefaultValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.String:
                            {
                                _curentFunctionArgumentInfo.DefaultValue = new StringValue(_currToken.Content);
                                _curentFunctionArgumentInfo.HasDefaultValue = true;
                                _state = State.GotDefaultValue;
                            }
                            break;

                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);

                                var parser = new NumberParser(_context);
                                parser.Run();

                                _curentFunctionArgumentInfo.DefaultValue = parser.Result;

                                _curentFunctionArgumentInfo.HasDefaultValue = true;
                                _state = State.GotDefaultValue;
                            }
                            break;

                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                _curentFunctionArgumentInfo.DefaultValue = ParseName(_currToken.Content);

                                _curentFunctionArgumentInfo.HasDefaultValue = true;
                                _state = State.GotDefaultValue;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotDefaultValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _state = State.GotComma;
                            break;

                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                throw new NotImplementedException();
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotComma:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                            _context.Recovery(_currToken);
                            _state = State.WaitForParameter;
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

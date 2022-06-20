/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
            GotParameterType,
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
        private FunctionArgumentInfo _curentArgumentInfo;

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
                                _curentArgumentInfo = new FunctionArgumentInfo();
                                Result.Add(_curentArgumentInfo);

                                _curentArgumentInfo.Name = ParseName(_currToken.Content);

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
                    _curentArgumentInfo.TypesList = ParseTypesOfParameterOrVar();
                    _state = State.GotParameterType;
                    break;

                case State.GotParameterType:
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
                    {
                        var parsingResult = ParseValueOnObjDefLevel();

#if DEBUG
                        //Log($"parsingResult = {parsingResult}");
#endif

                        var kindOfValueOnObjDefLevel = parsingResult.Kind;

                        switch (kindOfValueOnObjDefLevel)
                        {
                            case KindOfValueOnObjDefLevel.ConstLiteral:
                                _curentArgumentInfo.DefaultValue = parsingResult.Value;
                                _curentArgumentInfo.HasDefaultValue = true;
                                _state = State.GotDefaultValue;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfValueOnObjDefLevel), kindOfValueOnObjDefLevel, null);
                        }
                    }
                    break;

                    //switch (_currToken.TokenKind)
                    //{
                    //    case TokenKind.String:
                    //        {
                    //            _curentFunctionArgumentInfo.DefaultValue = new StringValue(_currToken.Content);
                    //            _curentFunctionArgumentInfo.HasDefaultValue = true;
                    //            _state = State.GotDefaultValue;
                    //        }
                    //        break;

                    //    case TokenKind.Number:
                    //        {
                    //            _context.Recovery(_currToken);

                    //            var parser = new NumberParser(_context);
                    //            parser.Run();

                    //            _curentFunctionArgumentInfo.DefaultValue = parser.Result;

                    //            _curentFunctionArgumentInfo.HasDefaultValue = true;
                    //            _state = State.GotDefaultValue;
                    //        }
                    //        break;

                    //    case TokenKind.Identifier:
                    //    case TokenKind.Word:
                    //        {
                    //            _curentFunctionArgumentInfo.DefaultValue = ParseName(_currToken.Content);

                    //            _curentFunctionArgumentInfo.HasDefaultValue = true;
                    //            _state = State.GotDefaultValue;
                    //        }
                    //        break;

                    //    default:
                    //        throw new UnexpectedTokenException(_currToken);
                    //}
                    //break;

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
                            throw new NotImplementedException();

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

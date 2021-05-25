using SymOntoClay.Core.Internal.CodeModel;
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
            GotParameterName
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
            //Log($"Result = {Result}");            
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

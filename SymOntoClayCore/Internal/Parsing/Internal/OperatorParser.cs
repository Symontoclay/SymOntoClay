using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class OperatorParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotOperatorMark
        }

        public OperatorParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public CodeEntity Result { get; private set; }
        private Operator _operator;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = CreateCodeEntity();
            Result.Kind = KindOfCodeEntity.Operator;

            _operator = CreateOperator();
            _operator.CodeEntity = Result;

            Result.Operator = _operator;
            Result.CodeFile = _context.CodeFile;
            Result.ParentCodeEntity = CurrentCodeEntity;
            SetCurrentCodeEntity(Result);

            if (Result.ParentCodeEntity != null)
            {
                _operator.Holder = Result.ParentCodeEntity.Name;
            }
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            RemoveCurrentCodeEntity();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
            Log($"_operator = {_operator}");
            //Log($"Result = {Result}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Operator:
                                    _state = State.GotOperatorMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotOperatorMark:
                    switch (_currToken.TokenKind)
                    {
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

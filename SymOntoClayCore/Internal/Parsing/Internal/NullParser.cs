using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class NullParser : BaseInternalParser
    {
        public NullParser(InternalParserContext context)
            : base(context)
        {
        }

        public NullValue Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");
            //Log($"_state = {_state}");
#endif

            switch(_currToken.TokenKind)
            {
                case TokenKind.Word:
                    switch(_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Null:
                            Result = new NullValue();
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }
    }
}

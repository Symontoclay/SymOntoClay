using SymOntoClay.Core.Internal.Parsing.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing
{
    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(Token token)
            : base($"Unexpected token {token.ToDebugString()}.")
        {
        }
    }
}

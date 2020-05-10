using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing
{
    public class UnexpectedSymbolException : Exception
    {
        public UnexpectedSymbolException(char symbol, int line, int pos)
            : base($"Unexpected symbol `{symbol}` at line {line} and pos {pos}.")
        {
        }
    }
}

/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.Parsing.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing
{
    public class UnexpectedTokenException : Exception
    {
        private static string BuildMessage(string fullText, Token token, string message)
        {
            var sb = new StringBuilder();

            if(!string.IsNullOrWhiteSpace(message))
            {
                sb.Append(message);
            }

            sb.Append($" Unexpected token {token.ToDebugString()}.");

            if(!string.IsNullOrWhiteSpace(fullText))
            {
                sb.AppendLine();
                sb.Append($"in `{fullText}`.");
            }

            return sb.ToString().Trim();
        }

        public UnexpectedTokenException(Token token)
            : base(BuildMessage(string.Empty, token, string.Empty))
        {
        }

        public UnexpectedTokenException(string fullText, Token token)
            : base(BuildMessage(fullText, token, string.Empty))
        {
        }

        public UnexpectedTokenException(Token token, string message)
            : base(BuildMessage(string.Empty, token, message))
        {
        }

        public UnexpectedTokenException(string fullText, Token token, string message)
            : base(BuildMessage(fullText, token, message))
        {
        }
    }
}

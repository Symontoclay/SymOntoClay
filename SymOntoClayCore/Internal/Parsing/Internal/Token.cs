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

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class Token: IObjectToString
    {
        public TokenKind TokenKind { get; set; } = TokenKind.Unknown;
        public KeyWordTokenKind KeyWordTokenKind { get; set; } = KeyWordTokenKind.Unknown;
        public string Content { get; set; } = string.Empty;

        public int Pos { get; set; }
        public int Line { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TokenKind)} = {TokenKind}");
            sb.AppendLine($"{spaces}{nameof(KeyWordTokenKind)} = {KeyWordTokenKind}");
            sb.AppendLine($"{spaces}{nameof(Content)} = {Content}");
            sb.AppendLine($"{spaces}{nameof(Pos)} = {Pos}");
            sb.AppendLine($"{spaces}{nameof(Line)} = {Line}");
            return sb.ToString();
        }
        
        public string ToDebugString()
        {
            var sb = new StringBuilder($"`{TokenKind}`");

            if(KeyWordTokenKind != KeyWordTokenKind.Unknown)
            {
                sb.Append($"[`{KeyWordTokenKind}`]");
            }

            sb.Append($"({Line}, {Pos})");

            if (!string.IsNullOrWhiteSpace(Content))
            {
                sb.Append($": `{Content}`");
            }

            return sb.ToString();
        }
    }
}

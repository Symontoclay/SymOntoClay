using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class Token: IObjectToString
    {
        public TokenKind TokenKind { get; set; } = TokenKind.Unknown;
        public TokenKind KeyWordTokenKind { get; set; } = TokenKind.Unknown;
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
            var tmpSb = new StringBuilder($"`{TokenKind}`({Line}, {Pos})");

            if (!string.IsNullOrWhiteSpace(Content))
            {
                tmpSb.Append($": `{Content}`");
            }

            return tmpSb.ToString();
        }
    }
}

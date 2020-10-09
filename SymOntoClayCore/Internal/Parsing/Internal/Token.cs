/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
            var tmpSb = new StringBuilder($"`{TokenKind}`({Line}, {Pos})");

            if (!string.IsNullOrWhiteSpace(Content))
            {
                tmpSb.Append($": `{Content}`");
            }

            return tmpSb.ToString();
        }
    }
}

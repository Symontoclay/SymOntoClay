using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class TerminationToken : IEquatable<TerminationToken>, IEquatable<Token>, IObjectToString
    {
        public TerminationToken(TokenKind tokenKind)
            : this(tokenKind, KeyWordTokenKind.Unknown, DefaultNeedRecovery)
        {
        }

        public TerminationToken(TokenKind tokenKind, KeyWordTokenKind keyWordTokenKind)
            : this(tokenKind, keyWordTokenKind, DefaultNeedRecovery)
        {
        }

        public TerminationToken(TokenKind tokenKind, KeyWordTokenKind keyWordTokenKind, bool needRecovery)
        {
            TokenKind = tokenKind;
            KeyWordTokenKind = keyWordTokenKind;
            NeedRecovery = needRecovery;
        }

        public const bool DefaultNeedRecovery = true;

        public TokenKind TokenKind { get; set; } = TokenKind.Unknown;
        public KeyWordTokenKind KeyWordTokenKind { get; set; } = KeyWordTokenKind.Unknown;
        public bool NeedRecovery { get; set; } = DefaultNeedRecovery;

        public static implicit operator TerminationToken(TokenKind tokenKind) => new TerminationToken(tokenKind);

        /// <inheritdoc/>
        public bool Equals(TerminationToken other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return TokenKind == other.TokenKind && KeyWordTokenKind == other.KeyWordTokenKind;
        }

        /// <inheritdoc/>
        public bool Equals(Token other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return TokenKind == other.TokenKind && KeyWordTokenKind == other.KeyWordTokenKind;
        }

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
            sb.AppendLine($"{spaces}{nameof(NeedRecovery)} = {NeedRecovery}");
            return sb.ToString();
        }

        public string ToDebugString()
        {
            var sb = new StringBuilder($"`{TokenKind}`");

            if (KeyWordTokenKind != KeyWordTokenKind.Unknown)
            {
                sb.Append($"[`{KeyWordTokenKind}`]");
            }

            return sb.ToString();
        }
    }
}

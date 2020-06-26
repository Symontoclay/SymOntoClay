using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Name: Value
    {
        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.IdentifierValue;

        public string DictionaryName { get; set; }

        public bool IsEmpty { get; set; } = true;

        public KindOfName KindOfName { get; set; } = KindOfName.Unknown;
        public string NameValue { get; set; } = string.Empty;
        public ulong NameKey { get; set; }

        /// <inheritdoc/>
        public override bool IsIdentifierValue => true;

        /// <inheritdoc/>
        public override Name AsIdentifierValue => this;

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            var result = new Name();

            result.DictionaryName = DictionaryName;

            result.IsEmpty = IsEmpty;

            result.KindOfName = KindOfName;
            result.NameValue = NameValue;
            result.NameKey = NameKey;

            result.AppendAnnotations(this, cloneContext);
            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(KindOfName)} = {KindOfName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(KindOfName)} = {KindOfName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(KindOfName)} = {KindOfName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}`{NameValue}`";
        }
    }
}

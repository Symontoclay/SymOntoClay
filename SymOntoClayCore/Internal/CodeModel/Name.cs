﻿using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Name: Value, IEquatable<Name>
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
        public override object GetSystemValue()
        {
            return NameValue;
        }

        /// <inheritdoc/>
        public bool Equals(Name other)
        {
            if (other == null)
            {
                return false;
            }

            return NameKey == other.NameKey;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var personObj = obj as Name;
            if (personObj == null)
            {
                return false;
            }

            return Equals(personObj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return NameKey.GetHashCode();
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public Name Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public Name Clone(Dictionary<object, object> context)
        {
            if(context.ContainsKey(this))
            {
                return (Name)context[this];
            }

            var result = new Name();
            context[this] = result;

            result.DictionaryName = DictionaryName;

            result.IsEmpty = IsEmpty;

            result.KindOfName = KindOfName;
            result.NameValue = NameValue;
            result.NameKey = NameKey;

            result.AppendAnnotations(this, context);
            return result;
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
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

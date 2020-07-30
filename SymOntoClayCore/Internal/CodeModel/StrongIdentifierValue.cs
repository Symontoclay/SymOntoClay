using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class StrongIdentifierValue: Value, IEquatable<StrongIdentifierValue>
    {
        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.StrongIdentifierValue;

        public string DictionaryName { get; set; }

        public bool IsEmpty { get; set; } = true;

        public KindOfName KindOfName { get; set; } = KindOfName.Unknown;
        public string NameValue { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override bool IsStrongIdentifierValue => true;

        /// <inheritdoc/>
        public override StrongIdentifierValue AsStrongIdentifierValue => this;

        public IndexedStrongIdentifierValue Indexed { get; set; }

        public IndexedStrongIdentifierValue GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertStrongIdentifierValue(this, mainStorageContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override IndexedValue GetIndexedValue(IMainStorageContext mainStorageContext)
        {
            return GetIndexed(mainStorageContext);
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem IndexedAnnotatedItem => Indexed;

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext)
        {
            return GetIndexed(mainStorageContext);
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertStrongIdentifierValue(this, mainStorageContext, convertingContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return NameValue;
        }

        /// <inheritdoc/>
        public bool Equals(StrongIdentifierValue other)
        {
            if (other == null)
            {
                return false;
            }

            return NameValue == other.NameValue;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var personObj = obj as StrongIdentifierValue;
            if (personObj == null)
            {
                return false;
            }

            return Equals(personObj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return NameValue.GetHashCode();
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
        public StrongIdentifierValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public StrongIdentifierValue Clone(Dictionary<object, object> context)
        {
            if(context.ContainsKey(this))
            {
                return (StrongIdentifierValue)context[this];
            }

            var result = new StrongIdentifierValue();
            context[this] = result;

            result.DictionaryName = DictionaryName;

            result.IsEmpty = IsEmpty;

            result.KindOfName = KindOfName;
            result.NameValue = NameValue;

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

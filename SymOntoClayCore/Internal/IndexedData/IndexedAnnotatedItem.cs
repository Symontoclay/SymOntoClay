using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public abstract class IndexedAnnotatedItem: IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        [ResolveToType(typeof(IndexedLogicalValue))]
        public virtual IList<IndexedValue> QuantityQualityModalities { get; set; } = new List<IndexedValue>();

        /// <summary>
        /// It is 'Clauses section' in the documentation.
        /// </summary>
        [ResolveToType(typeof(IndexedLogicalValue))]
        public virtual IList<IndexedValue> WhereSection { get; set; } = new List<IndexedValue>();

        public virtual IndexedStrongIdentifierValue Holder { get; set; }

        /// <summary>
        /// Returns <c>true</c> if the instance has modalities or additional sections, otherwise returns <c>false</c>.
        /// </summary>
        public bool HasModalitiesOrSections => !QuantityQualityModalities.IsNullOrEmpty() || !WhereSection.IsNullOrEmpty();

        public virtual IList<IndexedLogicalAnnotation> Annotations { get; set; }

        public bool HasConditionalSections => !WhereSection.IsNullOrEmpty();

        private long? _longConditionalHashCode;

        public virtual long GetLongConditionalHashCode()
        {
#if DEBUG
            var i = this;
#endif

            return _longConditionalHashCode.Value;
        }

        public virtual void CalculateLongConditionalHashCode()
        {
            if (HasConditionalSections)
            {
                throw new NotImplementedException();
            }

            _longConditionalHashCode = 0;
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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HasModalitiesOrSections)} = {HasModalitiesOrSections}");
            sb.AppendLine($"{spaces}{nameof(HasConditionalSections)} = {HasConditionalSections}");

            sb.AppendLine($"{spaces}{nameof(_longConditionalHashCode)} = {_longConditionalHashCode}");

            sb.PrintObjListProp(n, nameof(QuantityQualityModalities), QuantityQualityModalities);
            sb.PrintObjListProp(n, nameof(WhereSection), WhereSection);
            sb.PrintObjProp(n, nameof(Holder), Holder);
            sb.PrintObjListProp(n, nameof(Annotations), Annotations);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HasModalitiesOrSections)} = {HasModalitiesOrSections}");
            sb.AppendLine($"{spaces}{nameof(HasConditionalSections)} = {HasConditionalSections}");

            sb.AppendLine($"{spaces}{nameof(_longConditionalHashCode)} = {_longConditionalHashCode}");

            sb.PrintShortObjListProp(n, nameof(QuantityQualityModalities), QuantityQualityModalities);
            sb.PrintShortObjListProp(n, nameof(WhereSection), WhereSection);
            sb.PrintShortObjProp(n, nameof(Holder), Holder);
            sb.PrintShortObjListProp(n, nameof(Annotations), Annotations);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HasModalitiesOrSections)} = {HasModalitiesOrSections}");
            sb.AppendLine($"{spaces}{nameof(HasConditionalSections)} = {HasConditionalSections}");

            sb.AppendLine($"{spaces}{nameof(_longConditionalHashCode)} = {_longConditionalHashCode}");

            sb.PrintExistingList(n, nameof(QuantityQualityModalities), QuantityQualityModalities);
            sb.PrintExistingList(n, nameof(WhereSection), WhereSection);
            sb.PrintBriefObjProp(n, nameof(Holder), Holder);
            sb.PrintExistingList(n, nameof(Annotations), Annotations);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            return PropertiesToDbgString(n);
        }

        protected virtual string PropertiesToDbgString(uint n)
        {
#if DEBUG
            _gbcLogger.Info(this);
#endif

            throw new NotImplementedException();
        }
    }
}

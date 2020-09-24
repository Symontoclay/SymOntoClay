using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedAnnotationValue : IndexedValue
    {
        public AnnotationValue OriginalAnnotationValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalAnnotationValue;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.AnnotationValue;

        /// <inheritdoc/>
        public override bool IsAnnotationValue => true;

        /// <inheritdoc/>
        public override IndexedAnnotationValue AsAnnotationValue => this;

        public IndexedAnnotatedItem AnnotatedItem { get; set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return AnnotatedItem;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return base.CalculateLongHashCode() ^ (AnnotatedItem?.GetLongHashCode() ?? 0);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(AnnotatedItem), AnnotatedItem);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(AnnotatedItem), AnnotatedItem);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(AnnotatedItem), AnnotatedItem);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}[:-- Annotation --:]";
        }
    }
}

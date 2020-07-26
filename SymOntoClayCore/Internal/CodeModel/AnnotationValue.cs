using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class AnnotationValue: Value
    {
        public AnnotatedItem AnnotatedItem { get; set; }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.AnnotationValue;

        /// <inheritdoc/>
        public override bool IsAnnotationValue => true;

        /// <inheritdoc/>
        public override AnnotationValue AsAnnotationValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return null;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            if (cloneContext.ContainsKey(this))
            {
                return (Value)cloneContext[this];
            }

            var result = new AnnotationValue();
            cloneContext[this] = result;

            result.AnnotatedItem = AnnotatedItem?.CloneAnnotatedItem(cloneContext);

            result.AppendAnnotations(this, cloneContext);
            return result;
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

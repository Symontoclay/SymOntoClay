using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalQueryOperationValue : Value
    {
        public KindOfLogicalQueryOperation KindOfLogicalQueryOperation { get; set; } = KindOfLogicalQueryOperation.Unknown;

        /// <summary>
        /// Inserted or searched value.
        /// </summary>
        public Value Target { get; set; }

        public Value Source { get; set; }
        public Value Dest { get; set; }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.LogicalQueryOperationValue;

        /// <inheritdoc/>
        public override bool IsLogicalQueryOperationValue => true;

        /// <inheritdoc/>
        public override LogicalQueryOperationValue AsLogicalQueryOperationValue => this;

        public IndexedLogicalQueryOperationValue Indexed { get; set; }

        public IndexedLogicalQueryOperationValue GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertLogicalQueryOperationValue(this, mainStorageContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem IndexedAnnotatedItem => Indexed;

        /// <inheritdoc/>
        public override IndexedValue GetIndexedValue(IMainStorageContext mainStorageContext)
        {
            return GetIndexed(mainStorageContext);
        }

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
                return ConvertorToIndexed.ConvertLogicalQueryOperationValue(this, mainStorageContext, convertingContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return this;
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

            var result = new LogicalQueryOperationValue();
            cloneContext[this] = result;

            result.KindOfLogicalQueryOperation = KindOfLogicalQueryOperation;
            result.Target = Target?.CloneValue(cloneContext);
            result.Source = Source?.CloneValue(cloneContext);
            result.Dest = Dest?.CloneValue(cloneContext);

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfLogicalQueryOperation)} = {KindOfLogicalQueryOperation}");

            sb.PrintObjProp(n, nameof(Target), Target);
            sb.PrintObjProp(n, nameof(Source), Source);
            sb.PrintObjProp(n, nameof(Dest), Dest);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfLogicalQueryOperation)} = {KindOfLogicalQueryOperation}");

            sb.PrintShortObjProp(n, nameof(Target), Target);
            sb.PrintShortObjProp(n, nameof(Source), Source);
            sb.PrintShortObjProp(n, nameof(Dest), Dest);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfLogicalQueryOperation)} = {KindOfLogicalQueryOperation}");

            sb.PrintBriefObjProp(n, nameof(Target), Target);
            sb.PrintBriefObjProp(n, nameof(Source), Source);
            sb.PrintBriefObjProp(n, nameof(Dest), Dest);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

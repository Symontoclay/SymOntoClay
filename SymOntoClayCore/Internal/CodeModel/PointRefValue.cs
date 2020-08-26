using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PointRefValue : Value
    {
        public PointRefValue(Value leftOperand, Value rightOperand)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.PointRefValue;

        /// <inheritdoc/>
        public override bool IsPointRefValue => true;

        /// <inheritdoc/>
        public override PointRefValue AsPointRefValue => this;

        public Value LeftOperand { get; private set; }
        public Value RightOperand { get; private set; }

        public IndexedPointRefValue Indexed { get; set; }

        public IndexedPointRefValue GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertPointRefValue(this, mainStorageContext);
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
                return ConvertorToIndexed.ConvertPointRefValue(this, mainStorageContext, convertingContext);
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

            var result = new PointRefValue(LeftOperand, RightOperand);
            cloneContext[this] = result;

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(LeftOperand), LeftOperand);
            sb.PrintObjProp(n, nameof(RightOperand), RightOperand);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            
            sb.PrintShortObjProp(n, nameof(LeftOperand), LeftOperand);
            sb.PrintShortObjProp(n, nameof(RightOperand), RightOperand);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            
            sb.PrintBriefObjProp(n, nameof(LeftOperand), LeftOperand);
            sb.PrintBriefObjProp(n, nameof(RightOperand), RightOperand);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

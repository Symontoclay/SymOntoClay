using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RuleInstanceValue : Value
    {
        public RuleInstanceValue(RuleInstance ruleInstance)
        {
            RuleInstance = ruleInstance;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.RuleInstanceValue;

        /// <inheritdoc/>
        public override bool IsRuleInstanceValue => true;

        /// <inheritdoc/>
        public override RuleInstanceValue AsRuleInstanceValue => this;

        public RuleInstance RuleInstance { get; private set; }

        /// <inheritdoc/>
        [ResolveToType(typeof(LogicalValue))]
        public override IList<Value> QuantityQualityModalities { get => RuleInstance.QuantityQualityModalities; set => RuleInstance.QuantityQualityModalities = value; }

        /// <inheritdoc/>
        [ResolveToType(typeof(LogicalValue))]
        public override IList<Value> WhereSection { get => RuleInstance.WhereSection; set => RuleInstance.WhereSection = value; }

        /// <inheritdoc/>
        public override StrongIdentifierValue Holder { get => RuleInstance.Holder; set => RuleInstance.Holder = value; }

        /// <inheritdoc/>
        public override IList<RuleInstance> Annotations { get => RuleInstance.Annotations; set => RuleInstance.Annotations = value; }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            RuleInstance.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        public override Value GetAnnotationValue()
        {
            return RuleInstance.GetAnnotationValue();
        }

        public IndexedRuleInstanceValue Indexed { get; set; }

        public IndexedRuleInstanceValue GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertRuleInstanceValue(this, mainStorageContext);
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
                return ConvertorToIndexed.ConvertRuleInstanceValue(this, mainStorageContext, convertingContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return RuleInstance;
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

            var result = new RuleInstanceValue(RuleInstance.Clone(cloneContext));
            cloneContext[this] = result;

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(RuleInstance), RuleInstance);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

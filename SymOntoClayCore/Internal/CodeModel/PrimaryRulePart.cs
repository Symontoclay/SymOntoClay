using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PrimaryRulePart: BaseRulePart
    {
        public List<SecondaryRulePart> SecondaryParts { get; set; } = new List<SecondaryRulePart>();

        public IndexedPrimaryRulePart Indexed { get; set; }

        public IndexedPrimaryRulePart GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                ConvertorToIndexed.ConvertRuleInstance(Parent, mainStorageContext);
            }

            return Indexed;
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
                ConvertorToIndexed.ConvertRuleInstance(Parent, mainStorageContext, convertingContext);
            }

            return Indexed;
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
        public PrimaryRulePart Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public PrimaryRulePart Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PrimaryRulePart)context[this];
            }

            var result = new PrimaryRulePart();
            context[this] = result;

            result.SecondaryParts = SecondaryParts.Select(p => p.Clone(context)).ToList();

            result.AppendBaseRulePart(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.PrintExisting(n, nameof(Indexed), Indexed);
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.PrintExisting(n, nameof(Indexed), Indexed);
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.PrintExisting(n, nameof(Indexed), Indexed);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class CompoundHtnTaskItemsSection : CodeItem
    {
        public List<CompoundHtnTaskCaseItem> Items { get; set; } = new List<CompoundHtnTaskCaseItem>();

        protected void FillUpCompoundHtnTaskItemsSection(CompoundHtnTaskItemsSection dest, Dictionary<object, object> context)
        {
            dest.Items = Items?.Select(p => p.Clone(context))?.ToList();

            dest.AppendCodeItem(this, context);
        }

        /// <inheritdoc/>
        public override ulong GetLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options);

            if (!Items.IsNullOrEmpty())
            {
                foreach (var item in Items)
                {
                    result ^= item.GetLongHashCode(options);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            if (!Items.IsNullOrEmpty())
            {
                foreach (var item in Items)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(Items), Items);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(Items), Items);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(Items), Items);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        protected string ContentToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            sb.AppendLine("{");

            if (!Items.IsNullOrEmpty())
            {
                foreach (var item in Items)
                {
                    sb.AppendLine(item.ToHumanizedString(options));
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}

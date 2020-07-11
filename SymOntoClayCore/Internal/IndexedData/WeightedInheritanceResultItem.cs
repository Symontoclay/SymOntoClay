using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class WeightedInheritanceResultItem<T>: WeightedInheritanceItem
        where T: IndexedAnnotatedItem
    {
        public WeightedInheritanceResultItem(T resultItem)
        {
            ResultItem = resultItem;
        }

        public WeightedInheritanceResultItem(T resultItem, WeightedInheritanceItem source)
            : base(source)
        {
            ResultItem = resultItem;
        }

        public T ResultItem { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(ResultItem), ResultItem);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(ResultItem), ResultItem);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(ResultItem), ResultItem);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

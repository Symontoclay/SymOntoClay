using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class WeightedInheritanceResultItemWithStorageInfo<T>: WeightedInheritanceResultItem<T>
        where T : IndexedAnnotatedItem
    {
        public WeightedInheritanceResultItemWithStorageInfo(WeightedInheritanceResultItem<T> source, int storageDistance, IStorage storage)
            : base(source.ResultItem, source)
        {
            StorageDistance = storageDistance;
            Storage = storage;
        }

        public int StorageDistance { get; set; }
        public IStorage Storage { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(StorageDistance)} = {StorageDistance}");
            sb.AppendLine($"{spaces}{nameof(Storage)} = {Storage?.Kind}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(StorageDistance)} = {StorageDistance}");
            sb.AppendLine($"{spaces}{nameof(Storage)} = {Storage?.Kind}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(StorageDistance)} = {StorageDistance}");
            sb.AppendLine($"{spaces}{nameof(Storage)} = {Storage?.Kind}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

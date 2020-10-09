/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

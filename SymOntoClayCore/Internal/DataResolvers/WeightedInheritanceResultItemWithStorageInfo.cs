/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class WeightedInheritanceResultItemWithStorageInfo<T>: WeightedInheritanceResultItem<T>
        where T : AnnotatedItem
    {
        public WeightedInheritanceResultItemWithStorageInfo(WeightedInheritanceResultItem<T> source, int storageDistance, IStorage storage)
            : base(source.ResultItem, source)
        {
            StorageDistance = storageDistance;
            Storage = storage;
        }

        public int StorageDistance { get; set; }
        public IStorage Storage { get; set; }
        public List<uint> ParametersRankMatrix { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(StorageDistance)} = {StorageDistance}");
            sb.AppendLine($"{spaces}{nameof(Storage)} = {Storage?.Kind}");
            sb.PrintPODList(n, nameof(ParametersRankMatrix), ParametersRankMatrix);

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
            sb.PrintPODList(n, nameof(ParametersRankMatrix), ParametersRankMatrix);

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
            sb.PrintPODList(n, nameof(ParametersRankMatrix), ParametersRankMatrix);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

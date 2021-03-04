/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    [Obsolete("IndexedData must be removed!", true)]
    public abstract class IndexedAnnotatedItem: IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString, ISymOntoClayDisposable
    {
        public abstract AnnotatedItem OriginalAnnotatedItem { get; }

        [ResolveToType(typeof(IndexedLogicalValue))]
        public virtual IList<IndexedValue> QuantityQualityModalities { get; set; } = new List<IndexedValue>();

        /// <summary>
        /// It is 'Clauses section' in the documentation.
        /// </summary>
        [ResolveToType(typeof(IndexedLogicalValue))]
        public virtual IList<IndexedValue> WhereSection { get; set; } = new List<IndexedValue>();

        public virtual IndexedStrongIdentifierValue Holder { get; set; }

        /// <summary>
        /// Returns <c>true</c> if the instance has modalities or additional sections, otherwise returns <c>false</c>.
        /// </summary>
        public bool HasModalitiesOrSections => !QuantityQualityModalities.IsNullOrEmpty() || !WhereSection.IsNullOrEmpty();

        public virtual IList<IndexedLogicalAnnotation> Annotations { get; set; }

        public bool HasConditionalSections => !WhereSection.IsNullOrEmpty();

        private ulong? _longConditionalHashCode;

        public virtual ulong GetLongConditionalHashCode()
        {
            return _longConditionalHashCode.Value;
        }

        private ulong? _longHashCode;

        public virtual ulong GetLongHashCode()
        {
            return _longHashCode.Value;
        }

        public void CalculateLongHashCodes()
        {
            _longHashCode = CalculateLongHashCode();
            CalculateLongConditionalHashCode();
        }

        protected virtual void CalculateLongConditionalHashCode()
        {
            ulong result = 0;

            if (!WhereSection.IsNullOrEmpty())
            {
                foreach(var whereItem in WhereSection)
                {
                    result ^= whereItem.GetLongHashCode();
                }
            }

            _longConditionalHashCode = result;
        }

        protected virtual ulong CalculateLongHashCode()
        {
            ulong result = 0;

            if(!QuantityQualityModalities.IsNullOrEmpty())
            {
                foreach(var item in QuantityQualityModalities)
                {
                    result ^= LongHashCodeWeights.BaseModalityWeight ^ item.GetLongHashCode();
                }
            }

            if (!WhereSection.IsNullOrEmpty())
            {
                foreach (var item in WhereSection)
                {
                    result ^= LongHashCodeWeights.BaseModalityWeight ^ item.GetLongHashCode();
                }
            }

            if(Holder != null)
            {
                result ^= LongHashCodeWeights.BaseModalityWeight ^ Holder.GetLongHashCode();
            }

            if (!Annotations.IsNullOrEmpty())
            {
                foreach (var item in Annotations)
                {
                    result ^= LongHashCodeWeights.BaseModalityWeight ^ item.GetLongHashCode();
                }
            }

            return result;
        }

        private readonly object _disposingLockObj = new object();
        private bool _isDisposed;

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get
            {
                lock(_disposingLockObj)
                {
                    return _isDisposed;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_disposingLockObj)
            {
                if(_isDisposed)
                {
                    return;
                }

                _isDisposed = true;

                OnDisposed();

                OriginalAnnotatedItem.Dispose();
            }
        }

        protected virtual void OnDisposed()
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HasModalitiesOrSections)} = {HasModalitiesOrSections}");
            sb.AppendLine($"{spaces}{nameof(HasConditionalSections)} = {HasConditionalSections}");

            sb.AppendLine($"{spaces}{nameof(_longConditionalHashCode)} = {_longConditionalHashCode}");
            sb.AppendLine($"{spaces}{nameof(_longHashCode)} = {_longHashCode}");

            sb.PrintObjListProp(n, nameof(QuantityQualityModalities), QuantityQualityModalities);
            sb.PrintObjListProp(n, nameof(WhereSection), WhereSection);
            sb.PrintObjProp(n, nameof(Holder), Holder);
            sb.PrintObjListProp(n, nameof(Annotations), Annotations);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HasModalitiesOrSections)} = {HasModalitiesOrSections}");
            sb.AppendLine($"{spaces}{nameof(HasConditionalSections)} = {HasConditionalSections}");

            sb.AppendLine($"{spaces}{nameof(_longConditionalHashCode)} = {_longConditionalHashCode}");
            sb.AppendLine($"{spaces}{nameof(_longHashCode)} = {_longHashCode}");

            sb.PrintShortObjListProp(n, nameof(QuantityQualityModalities), QuantityQualityModalities);
            sb.PrintShortObjListProp(n, nameof(WhereSection), WhereSection);
            sb.PrintShortObjProp(n, nameof(Holder), Holder);
            sb.PrintShortObjListProp(n, nameof(Annotations), Annotations);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HasModalitiesOrSections)} = {HasModalitiesOrSections}");
            sb.AppendLine($"{spaces}{nameof(HasConditionalSections)} = {HasConditionalSections}");

            sb.AppendLine($"{spaces}{nameof(_longConditionalHashCode)} = {_longConditionalHashCode}");
            sb.AppendLine($"{spaces}{nameof(_longHashCode)} = {_longHashCode}");

            sb.PrintExistingList(n, nameof(QuantityQualityModalities), QuantityQualityModalities);
            sb.PrintExistingList(n, nameof(WhereSection), WhereSection);
            sb.PrintBriefObjProp(n, nameof(Holder), Holder);
            sb.PrintExistingList(n, nameof(Annotations), Annotations);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            return PropertiesToDbgString(n);
        }

        protected virtual string PropertiesToDbgString(uint n)
        {
#if DEBUG
            DebugLogger.Instance.Info(this);
#endif

            throw new NotImplementedException();
        }
    }
}

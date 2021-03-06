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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class AnnotatedItem : IAnnotatedItem, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString, ISymOntoClayDisposable
    {
        [ResolveToType(typeof(LogicalValue))]
        public virtual IList<Value> QuantityQualityModalities { get; set; } = new List<Value>();

        /// <summary>
        /// It is 'Clauses section' in the documentation.
        /// </summary>
        [ResolveToType(typeof(LogicalValue))]
        public virtual IList<Value> WhereSection { get; set; } = new List<Value>();

        public virtual StrongIdentifierValue Holder { get; set; }

        /// <summary>
        /// Returns <c>true</c> if the instance has modalities or additional sections, otherwise returns <c>false</c>.
        /// </summary>
        public bool HasModalitiesOrSections => !QuantityQualityModalities.IsNullOrEmpty() || !WhereSection.IsNullOrEmpty();

        public bool HasConditionalSections => !WhereSection.IsNullOrEmpty();

        private bool _isDirty = true;

        public void CheckDirty()
        {
            if (_isDirty)
            {
                CalculateLongHashCodes();
                _isDirty = false;
            }
        }

        private ulong? _longConditionalHashCode;

        public virtual ulong GetLongConditionalHashCode()
        {
            if(!_longConditionalHashCode.HasValue)
            {
                CalculateLongHashCodes();
            }

            return _longConditionalHashCode.Value;
        }

        private ulong? _longHashCode;

        public virtual ulong GetLongHashCode()
        {
            if(!_longHashCode.HasValue)
            {
                CalculateLongHashCodes();
            }

            return _longHashCode.Value;
        }

        public void CalculateLongHashCodes()
        {
            _longHashCode = CalculateLongHashCode();
            CalculateLongConditionalHashCode();
            _isDirty = false;
        }

        protected virtual void CalculateLongConditionalHashCode()
        {
            ulong result = 0;

            if (!WhereSection.IsNullOrEmpty())
            {
                foreach (var whereItem in WhereSection)
                {
                    result ^= whereItem.GetLongHashCode();
                }
            }

            _longConditionalHashCode = result;
        }

        protected virtual ulong CalculateLongHashCode()
        {
            ulong result = 0;

            if (!QuantityQualityModalities.IsNullOrEmpty())
            {
                foreach (var item in QuantityQualityModalities)
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

            if (Holder != null)
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

        public virtual IList<RuleInstance> Annotations { get; set; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public AnnotatedItem CloneAnnotatedItem()
        {
            var context = new Dictionary<object, object>();
            return CloneAnnotatedItem(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public abstract AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context);

        public void AppendAnnotations(AnnotatedItem source)
        {
            var cloneContext = new Dictionary<object, object>();
            AppendAnnotations(source, cloneContext);
        }

        public void AppendAnnotations(AnnotatedItem source, Dictionary<object, object> cloneContext)
        {
            if (source.QuantityQualityModalities == null)
            {
                QuantityQualityModalities = null;
            }
            else
            {
                QuantityQualityModalities = new List<Value>();

                foreach(var item in source.QuantityQualityModalities)
                {
                    QuantityQualityModalities.Add(item.CloneValue(cloneContext));
                }
            }

            if(source.WhereSection == null)
            {
                WhereSection = null;
            }
            else
            {
                WhereSection = new List<Value>();

                foreach (var item in source.WhereSection)
                {
                    WhereSection.Add(item.CloneValue(cloneContext));
                }
            }

            if(source.Holder == null)
            {
                Holder = null;
            }
            else
            {
                Holder = source.Holder;
            }

            if(source.Annotations == null)
            {
                Annotations = null;
            }
            else
            {
                Annotations = new List<RuleInstance>();

                foreach(var annotation in source.Annotations)
                {
                    Annotations.Add(annotation.Clone());
                }
            }
        }

        public IList<RuleInstance> GetAllAnnotations()
        {
            var result = new List<RuleInstance>();
            DiscoverAllAnnotations(result);
            return result.Distinct().ToList();
        }

        public virtual void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            if (!Annotations.IsNullOrEmpty())
            {
                foreach (var annotation in Annotations)
                {
                    result.Add(annotation);

                    annotation.DiscoverAllAnnotations(result);
                }
            }

            if (!QuantityQualityModalities.IsNullOrEmpty())
            {
                foreach (var item in QuantityQualityModalities)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if (!WhereSection.IsNullOrEmpty())
            {
                foreach (var item in WhereSection)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if (Holder != null)
            {
                Holder.DiscoverAllAnnotations(result);
            }
        }

        private Value _annotationValue;
        private readonly object _annotationValueLockObj = new object();

        public virtual Value GetAnnotationValue()
        {
            lock(_annotationValueLockObj)
            {
                if(_annotationValue == null)
                {
                    _annotationValue = new AnnotationValue() { AnnotatedItem = this};
                }

                return _annotationValue;
            }            
        }

        private readonly object _disposingLockObj = new object();
        private bool _isDisposed;

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get
            {
                lock (_disposingLockObj)
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
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;

                OnDisposed();
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

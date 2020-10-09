/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class AnnotatedItem : IAnnotatedItem, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
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

        public virtual IList<RuleInstance> Annotations { get; set; }

        public abstract IndexedAnnotatedItem IndexedAnnotatedItem { get; }

        public abstract IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext);
        public abstract IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext);

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
            throw new NotImplementedException();
        }
    }
}

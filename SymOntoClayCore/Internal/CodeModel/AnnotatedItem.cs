using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class AnnotatedItem : IAnnotatedItem, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        [ResolveToType(typeof(LogicalValue))]
        public IList<Value> QuantityQualityModalities { get; set; }

        public void AppendAnnotations(AnnotatedItem source)
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
                    QuantityQualityModalities.Add(item.CloneValue());
                }
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

            sb.PrintObjListProp(n, nameof(QuantityQualityModalities), QuantityQualityModalities);

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

            sb.PrintShortObjListProp(n, nameof(QuantityQualityModalities), QuantityQualityModalities);

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

            sb.PrintExistingList(n, nameof(QuantityQualityModalities), QuantityQualityModalities);

            return sb.ToString();
        }
    }
}

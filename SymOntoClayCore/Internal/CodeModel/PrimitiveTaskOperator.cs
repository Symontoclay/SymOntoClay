using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PrimitiveTaskOperator : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public StrongIdentifierValue Name { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public PrimitiveTaskOperator Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public PrimitiveTaskOperator Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PrimitiveTaskOperator)context[this];
            }

            var result = new PrimitiveTaskOperator();
            context[this] = result;

            return result;
        }

        public ulong GetLongHashCode(CheckDirtyOptions options)
        {
            Name.CheckDirty(options);

            var result = Name.GetLongHashCode(options);

            return result;
        }

        public void DiscoverAllAnnotations(IList<Annotation> result)
        {
            Name?.DiscoverAllAnnotations(result);
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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(Name), Name);

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Name), Name);

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Name), Name);

            return sb.ToString();
        }

        public string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder(Name?.ToHumanizedString(options));

            //sb.Append(";");

            return sb.ToString();
        }
    }
}

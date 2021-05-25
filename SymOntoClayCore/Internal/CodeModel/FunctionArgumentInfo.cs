using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FunctionArgumentInfo : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public StrongIdentifierValue Name { get; set; }
        public bool HasDefaultValue { get; set; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public FunctionArgumentInfo Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public FunctionArgumentInfo Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (FunctionArgumentInfo)context[this];
            }

            var result = new FunctionArgumentInfo();
            context[this] = result;

            result.Name = Name.Clone(context);
            result.HasDefaultValue = HasDefaultValue;

            return result;
        }

        public ulong GetLongConditionalHashCode()
        {
            Name.CheckDirty();

            return Name.GetLongConditionalHashCode();
        }

        public void DiscoverAllAnnotations(IList<RuleInstance> result)
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
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");

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
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");

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
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");

            return sb.ToString();
        }
    }
}

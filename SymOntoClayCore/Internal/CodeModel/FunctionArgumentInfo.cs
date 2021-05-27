using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FunctionArgumentInfo : IFunctionArgument
    {
        public StrongIdentifierValue Name { get; set; }
        public List<StrongIdentifierValue> TypesList { get; set; } = new List<StrongIdentifierValue>();
        public bool HasDefaultValue { get; set; }
        public Value DefaultValue { get; set; }

        /// <inheritdoc/>
        IList<StrongIdentifierValue> IFunctionArgument.TypesList => TypesList;

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
            result.TypesList = TypesList?.Select(p => p.Clone(context)).ToList();
            result.HasDefaultValue = HasDefaultValue;
            result.DefaultValue = DefaultValue.CloneValue(context);

            return result;
        }

        public ulong GetLongConditionalHashCode()
        {
            Name.CheckDirty();

            if (!TypesList.IsNullOrEmpty())
            {
                foreach(var item in TypesList)
                {
                    item.CheckDirty();
                }
            }                

            DefaultValue?.CheckDirty();

            var result = Name.GetLongConditionalHashCode();

            if (!TypesList.IsNullOrEmpty())
            {
                foreach (var item in TypesList)
                {
                    result ^= item.GetLongConditionalHashCode();
                }
            }

            return result;
        }

        public void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            Name?.DiscoverAllAnnotations(result);

            if (!TypesList.IsNullOrEmpty())
            {
                foreach (var item in TypesList)
                {
                    item.DiscoverAllAnnotations(result);
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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjListProp(n, nameof(TypesList), TypesList);

            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            sb.PrintObjProp(n, nameof(DefaultValue), DefaultValue);

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
            sb.PrintShortObjListProp(n, nameof(TypesList), TypesList);
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            sb.PrintShortObjProp(n, nameof(DefaultValue), DefaultValue);

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
            sb.PrintBriefObjListProp(n, nameof(TypesList), TypesList);
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            sb.PrintBriefObjProp(n, nameof(DefaultValue), DefaultValue);

            return sb.ToString();
        }
    }
}

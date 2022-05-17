using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RelationParameterDescription: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public StrongIdentifierValue Name { get; set; }
        public List<StrongIdentifierValue> TypesList { get; set; } = new List<StrongIdentifierValue>();
        public List<StrongIdentifierValue> MeaningRolesList { get; set; } = new List<StrongIdentifierValue>();

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public RelationParameterDescription Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public RelationParameterDescription Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RelationParameterDescription)context[this];
            }

            var result = new RelationParameterDescription();
            context[this] = result;

            result.Name = Name.Clone(context);
            result.TypesList = TypesList?.Select(p => p.Clone(context)).ToList();
            result.MeaningRolesList = MeaningRolesList?.Select(p => p.Clone(context)).ToList();
            //result.HasDefaultValue = HasDefaultValue;
            //result.DefaultValue = DefaultValue.CloneValue(context);

            return result;
        }

        public ulong GetLongConditionalHashCode(CheckDirtyOptions options)
        {
            Name.CheckDirty(options);

            if (!TypesList.IsNullOrEmpty())
            {
                foreach (var item in TypesList)
                {
                    item.CheckDirty(options);
                }
            }

            if (!MeaningRolesList.IsNullOrEmpty())
            {
                foreach (var item in MeaningRolesList)
                {
                    item.CheckDirty(options);
                }
            }

            //DefaultValue?.CheckDirty(options);

            var result = Name.GetLongConditionalHashCode(options);

            if (!TypesList.IsNullOrEmpty())
            {
                foreach (var item in TypesList)
                {
                    result ^= item.GetLongConditionalHashCode(options);
                }
            }

            if (!MeaningRolesList.IsNullOrEmpty())
            {
                foreach (var item in MeaningRolesList)
                {
                    result ^= item.GetLongConditionalHashCode(options);
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

            if (!MeaningRolesList.IsNullOrEmpty())
            {
                foreach (var item in MeaningRolesList)
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
            sb.PrintObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);

            //sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            //sb.PrintObjProp(n, nameof(DefaultValue), DefaultValue);

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
            sb.PrintShortObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);

            //sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            //sb.PrintShortObjProp(n, nameof(DefaultValue), DefaultValue);

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
            sb.PrintBriefObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);
            //sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            //sb.PrintBriefObjProp(n, nameof(DefaultValue), DefaultValue);

            return sb.ToString();
        }

        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var sb = new StringBuilder(Name.NameValue);

            if(TypesList.IsNullOrEmpty())
            {
                sb.Append(": any");
            }
            else
            {
                sb.Append(": (");
                sb.Append(string.Join("|", TypesList.Select(p => p.NameValue)));
                sb.Append(")");
            }

            if(!MeaningRolesList.IsNullOrEmpty())
            {
                sb.Append("[:");
                sb.Append(string.Join(",", MeaningRolesList.Select(p => p.NameValue)));
                sb.Append(":]");
            }

            return sb.ToString();
        }
    }
}

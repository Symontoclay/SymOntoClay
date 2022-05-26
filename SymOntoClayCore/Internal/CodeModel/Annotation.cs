﻿using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Annotation: ItemWithLongHashCodes, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public List<RuleInstance> Facts { get; set; } = new List<RuleInstance>();
        public List<StrongIdentifierValue> MeaningRolesList { get; set; } = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            ulong result = 0;

            if(!Facts.IsNullOrEmpty())
            {
                foreach(var fact in Facts)
                {
                    result ^= fact.GetLongHashCode();
                }
            }

            return 0u;
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public Annotation Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public Annotation Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Annotation)context[this];
            }

            var result = new Annotation();
            context[this] = result;

            result.Facts = Facts?.Select(p => p.Clone(context)).ToList();
            result.MeaningRolesList = MeaningRolesList?.Select(p => p.Clone(context)).ToList();

            //result.IsSource = IsSource;
            //result.KindOfRuleInstance = KindOfRuleInstance;
            //result.PrimaryPart = PrimaryPart.Clone(context);
            //result.SecondaryParts = SecondaryParts?.Select(p => p.Clone(context)).ToList();
            //result.IsParameterized = IsParameterized;
            //result.UsedKeysList = UsedKeysList?.ToList();

            //result.AppendCodeItem(this, context);

            return result;
        }

        public void DiscoverAllAnnotations(IList<Annotation> result)
        {
            throw new NotImplementedException();
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

            sb.PrintObjListProp(n, nameof(Facts), Facts);
            sb.PrintObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);

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

            sb.PrintShortObjListProp(n, nameof(Facts), Facts);
            sb.PrintShortObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);

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

            sb.PrintBriefObjListProp(n, nameof(Facts), Facts);
            sb.PrintBriefObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);

            return sb.ToString();
        }
    }
}
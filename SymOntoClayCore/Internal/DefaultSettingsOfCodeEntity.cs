/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public class DefaultSettingsOfCodeEntity: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public IList<Value> QuantityQualityModalities { get; set; }

        public IList<Value> WhereSection { get; set; }

        public StrongIdentifierValue Holder { get; set; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public DefaultSettingsOfCodeEntity Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public DefaultSettingsOfCodeEntity Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (DefaultSettingsOfCodeEntity)context[this];
            }

            var result = new DefaultSettingsOfCodeEntity();
            context[this] = result;

            result.QuantityQualityModalities = QuantityQualityModalities?.Select(p => p.CloneValue(context)).ToList();
            result.WhereSection = WhereSection?.Select(p => p.CloneValue(context)).ToList();
            result.Holder = Holder?.Clone(context);

            return result;
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

            sb.PrintObjListProp(n, nameof(QuantityQualityModalities), QuantityQualityModalities);
            sb.PrintObjListProp(n, nameof(WhereSection), WhereSection);
            sb.PrintObjProp(n, nameof(Holder), Holder);

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

            sb.PrintShortObjListProp(n, nameof(QuantityQualityModalities), QuantityQualityModalities);
            sb.PrintShortObjListProp(n, nameof(WhereSection), WhereSection);
            sb.PrintShortObjProp(n, nameof(Holder), Holder);

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

            sb.PrintExistingList(n, nameof(QuantityQualityModalities), QuantityQualityModalities);
            sb.PrintExistingList(n, nameof(WhereSection), WhereSection);

            sb.PrintBriefObjProp(n, nameof(Holder), Holder);

            return sb.ToString();
        }
    }
}

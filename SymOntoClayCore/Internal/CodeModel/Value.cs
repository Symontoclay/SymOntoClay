/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class Value: AnnotatedItem
    {
        public abstract KindOfValue KindOfValue { get; }

        public virtual bool IsNullValue => false;
        public virtual NullValue AsNullValue => null;

        public virtual bool IsLogicalValue => false;
        public virtual LogicalValue AsLogicalValue => null;

        public virtual bool IsNumberValue => false;
        public virtual NumberValue AsNumberValue => null;

        public virtual bool IsStringValue => false;
        public virtual StringValue AsStringValue => null;

        public virtual bool IsStrongIdentifierValue => false;
        public virtual StrongIdentifierValue AsStrongIdentifierValue => null;

        public virtual bool IsTaskValue => false;
        public virtual TaskValue AsTaskValue => null;

        public virtual bool IsAnnotationValue => false;
        public virtual AnnotationValue AsAnnotationValue => null;

        public virtual bool IsWaypointValue => false;
        public virtual WaypointValue AsWaypointValue => null;

        public virtual bool IsInstanceValue => false;
        public virtual InstanceValue AsInstanceValue => null;

        public virtual bool IsHostValue => false;
        public virtual HostValue AsHostValue => null;

        public virtual bool IsPointRefValue => false;
        public virtual PointRefValue AsPointRefValue => null;

        public virtual bool IsRuleInstanceValue => false;
        public virtual RuleInstanceValue AsRuleInstanceValue => null;

        public virtual bool IsLogicalSearchResultValue => false;
        public virtual LogicalSearchResultValue AsLogicalSearchResultValue => null;

        public virtual bool IsLogicalQueryOperationValue => false;
        public virtual LogicalQueryOperationValue AsLogicalQueryOperationValue => null;

        public abstract object GetSystemValue();

        public abstract IndexedValue GetIndexedValue(IMainStorageContext mainStorageContext);

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public Value CloneValue()
        {
            var cloneContext = new Dictionary<object, object>();
            return CloneValue(cloneContext);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="cloneContext">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public abstract Value CloneValue(Dictionary<object, object> cloneContext);

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfValue)} = {KindOfValue}");
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfValue)} = {KindOfValue}");
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfValue)} = {KindOfValue}");
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

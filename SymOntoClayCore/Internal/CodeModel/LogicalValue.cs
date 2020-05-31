﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalValue: Value
    {
        public LogicalValue(float systemValue)
        {
            if(systemValue > 1F || systemValue < 0F)
            {
                throw new ArgumentOutOfRangeException(nameof(systemValue), systemValue, "The system (C#) value which represents SymOntoCklay's logical value must be between 0 and 1.");
            }

            SystemValue = systemValue;
        }

        /// <inheritdoc/>
        public override KindOfValue Kind => KindOfValue.LogicalValue;

        /// <inheritdoc/>
        public override bool IsLogicalValue => true;

        /// <inheritdoc/>
        public override LogicalValue AsLogicalValue => this;

        public float? SystemValue { get; private set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class NumberValue: Value
    {
        public NumberValue(double systemValue, ICodeModelContext context)
            : base(context)
        {
            SystemValue = systemValue;
        }

        /// <inheritdoc/>
        public override KindOfValue Kind => KindOfValue.NumberValue;

        /// <inheritdoc/>
        public override string TypeName => "number";

        /// <inheritdoc/>
        public override bool IsNumberValue => true;

        /// <inheritdoc/>
        public override NumberValue AsNumberValue => this;

        public double SystemValue { get; private set; }

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

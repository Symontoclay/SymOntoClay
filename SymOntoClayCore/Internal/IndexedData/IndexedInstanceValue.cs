﻿using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedInstanceValue : IndexedValue
    {
        public InstanceValue OriginalInstanceValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalInstanceValue;

        public InstanceInfo InstanceInfo { get; set; }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.InstanceValue;

        /// <inheritdoc/>
        public override bool IsInstanceValue => true;

        /// <inheritdoc/>
        public override IndexedInstanceValue AsInstanceValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return InstanceInfo;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}

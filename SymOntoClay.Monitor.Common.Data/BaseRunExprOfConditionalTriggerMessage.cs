using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseRunExprOfConditionalTriggerMessage: BaseConditionalTriggerMessage
    {
        public MonitoredHumanizedLabel ExprLabel { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(ExprLabel), ExprLabel);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}

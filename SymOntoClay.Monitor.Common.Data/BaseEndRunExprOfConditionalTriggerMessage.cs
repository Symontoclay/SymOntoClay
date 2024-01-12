using Newtonsoft.Json;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseEndRunExprOfConditionalTriggerMessage: BaseRunExprOfConditionalTriggerMessage
    {
        public bool IsSuccess { get; set; }
        public bool IsPeriodic { get; set; }
        public List<List<MonitoredHumanizedLabel>> FetchedResults { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.AppendLine($"{spaces}{nameof(IsPeriodic)} = {IsPeriodic}");
            sb.AppendLine($"{spaces}{nameof(FetchedResults)} = {JsonConvert.SerializeObject(FetchedResults, Formatting.Indented)}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}

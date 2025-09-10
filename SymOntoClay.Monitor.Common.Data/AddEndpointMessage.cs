using SymOntoClay.Common.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class AddEndpointMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.AddEndpoint;

        public string EndpointName { get; set; }
        public IReadOnlyList<int> ParamsCountList { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(EndpointName)} = {EndpointName}");
            sb.PrintPODListProp(n, nameof(ParamsCountList), ParamsCountList);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}

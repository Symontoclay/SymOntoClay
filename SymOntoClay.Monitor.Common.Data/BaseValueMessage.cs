using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common.Data
{
    public abstract class BaseValueMessage : BaseMessage
    {
        public string TypeName { get; set; }
        public string Base64Content { get; set; }
        public string HumanizedString { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(TypeName)} = {TypeName}");
            sb.AppendLine($"{spaces}{nameof(Base64Content)} = {Base64Content}");
            sb.AppendLine($"{spaces}{nameof(HumanizedString)} = {HumanizedString}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}

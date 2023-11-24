using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.MonitorSerializableObjects
{
    public class BaseEntityValueMonitorSerializableObject: IObjectToString
    {
        public string EntityId {  get; set; }
        public string Id { get; set; }
        public string IdForFacts { get; set; }

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(EntityId)} = {EntityId}");
            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(IdForFacts)} = {IdForFacts}");
            sb.AppendLine($"{spaces}{nameof()} = {}");
            sb.AppendLine($"{spaces}{nameof()} = {}");
            sb.AppendLine($"{spaces}{nameof()} = {}");
            return sb.ToString();
        }
    }
}

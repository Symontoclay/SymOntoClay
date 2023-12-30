using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common
{
    public class Changer: IObjectToString
    {
        public Changer(KindOfChanger kindOfChanger, string id)
        {
            KindOfChanger = kindOfChanger;
            Id = id;
        }

        public KindOfChanger KindOfChanger { get; }
        public string Id { get; }

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

            sb.AppendLine($"{spaces}{nameof(KindOfChanger)} = {KindOfChanger}");
            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");

            return sb.ToString();
        }
    }
}

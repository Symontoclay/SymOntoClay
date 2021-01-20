using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public struct RelativeCoordinate: IObjectToString
    {
        public float Distance { get; set; }
        public float HorizontalAngle { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");
            sb.AppendLine($"{spaces}{nameof(HorizontalAngle)} = {HorizontalAngle}");
            return sb.ToString();
        }
    }
}

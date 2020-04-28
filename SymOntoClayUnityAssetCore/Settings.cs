using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public class Settings: IObjectToString
    {
        public LoggingSettings Logging { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();

            if(Logging == null)
            {
                sb.AppendLine($"{spaces}{nameof(Logging)} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(Logging)}");
                sb.Append(Logging.ToString(nextN));
                sb.AppendLine($"{spaces}End {nameof(Logging)}");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }
    }
}

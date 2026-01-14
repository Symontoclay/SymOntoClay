using SymOntoClay.Common.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.MessagePacking
{
    public class TstLogMessage: TstBaseLogMessage
    {
        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(ActivatedAction), ActivatedAction);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}

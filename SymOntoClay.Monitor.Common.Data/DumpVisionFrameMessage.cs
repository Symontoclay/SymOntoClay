using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class DumpVisionFrameMessage: BaseVisionFrameMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.DumpVisionFrame;

        public List<MonitoredVisibleItem> VisibleItems { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(VisibleItems), VisibleItems);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}

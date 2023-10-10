using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public class NodeIdFileNameTemplateOptionItem: BaseFileNameTemplateOptionItem
    {
        /// <inheritdoc/>
        public override string GetText(string nodeId, string threadId)
        {
            if(string.IsNullOrWhiteSpace(nodeId))
            {
                return string.Empty;
            }

            if (IfThreadIdExists && string.IsNullOrWhiteSpace(threadId))
            {
                return string.Empty;
            }

            return nodeId;
        }
    }
}

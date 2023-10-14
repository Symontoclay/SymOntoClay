using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public class ThreadIdFileNameTemplateOptionItem : BaseFileNameTemplateOptionItem
    {
        /// <inheritdoc/>
        public override string ItemName { get => "ThreadId"; set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public override string GetText(string nodeId, string threadId)
        {
            if(string.IsNullOrWhiteSpace(threadId))
            {
                return string.Empty;
            }

            if (IfNodeIdExists && string.IsNullOrWhiteSpace(nodeId))
            {
                return string.Empty;
            }

            return threadId;
        }
    }
}

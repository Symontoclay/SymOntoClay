using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public class ExtensionFileNameTemplateOptionItem: TextFileNameTemplateOptionItem
    {
        public ExtensionFileNameTemplateOptionItem()
        {
        }

        public ExtensionFileNameTemplateOptionItem(string text)
            : base(text)
        {
        }

        /// <inheritdoc/>
        public override bool IsExtension => true;
    }
}

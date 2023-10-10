using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public class ShortDateTimeFileNameTemplateOptionItem: DateTimeStampFileNameTemplateOptionItem
    {
        public ShortDateTimeFileNameTemplateOptionItem()
            : base(LogFileBuilderConstants.ShortDateTimeFormat)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public class LongDateTimeFileNameTemplateOptionItem: DateTimeStampFileNameTemplateOptionItem
    {
        public LongDateTimeFileNameTemplateOptionItem()
            : base(LogFileBuilderConstants.LongDateTimeFormat)
        {
        }
    }
}

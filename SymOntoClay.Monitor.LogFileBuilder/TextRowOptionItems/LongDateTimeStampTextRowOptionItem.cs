using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public class LongDateTimeStampTextRowOptionItem : DateTimeStampTextRowOptionItem
    {
        public LongDateTimeStampTextRowOptionItem()
            : base("yyyy-MM-dd HH:mm:ss.ffff")
        {
        }
    }
}

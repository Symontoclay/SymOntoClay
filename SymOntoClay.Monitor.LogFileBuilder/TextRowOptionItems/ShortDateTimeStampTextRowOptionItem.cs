﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public class ShortDateTimeStampTextRowOptionItem : DateTimeStampTextRowOptionItem
    {
        public ShortDateTimeStampTextRowOptionItem()
            : base(LogFileBuilderConstants.ShortDateTimeFormat)
        {
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common.Data
{
    public enum KindOfMessage
    {
        Unknown,
        CreateMotitorNode,
        CreateThreadLogger,
        CallMethod,
        Parameter,
        Output,
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}
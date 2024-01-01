﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class CancelInstanceExecutionMessage : BaseCancelMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.CancelInstanceExecution;
    }
}
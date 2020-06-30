using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IChannelHandler
    {
        Value Read();
        Value Write(Value value);
    }
}

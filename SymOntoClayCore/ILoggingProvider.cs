using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ILoggingProvider
    {
        KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; }
        string DumpToFile(LogicalSearchExplainNode explainNode);
        bool EnableAddingRemovingFactLoggingInStorages { get; }
    }
}

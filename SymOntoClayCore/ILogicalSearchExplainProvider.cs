using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ILogicalSearchExplainProvider
    {
        KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; }
        string DumpToFile(LogicalSearchExplainNode explainNode);
    }
}

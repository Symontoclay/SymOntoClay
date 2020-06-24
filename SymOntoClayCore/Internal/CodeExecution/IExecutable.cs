using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IExecutable
    {
        bool IsSystemDefined { get; }
        CompiledFunctionBody CompiledFunctionBody { get; }
        ISystemHandler SystemHandler { get; }
    }
}

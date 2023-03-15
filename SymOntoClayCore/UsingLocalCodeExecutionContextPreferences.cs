using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public enum UsingLocalCodeExecutionContextPreferences
    {
        Default,
        UseCallerAsParent,
        UseOwnAsParent,
        UseBothOwnAndCallerAsParent
    }
}

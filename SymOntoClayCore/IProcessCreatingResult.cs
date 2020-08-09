using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IProcessCreatingResult : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        IProcessInfo Process { get; }
    }
}

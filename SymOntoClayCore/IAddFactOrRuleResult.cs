using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IAddFactOrRuleResult: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        KindOfAddFactOrRuleResult KindOfResult { get; }
        IItemWithModalities MutablePart { get; }
    }
}

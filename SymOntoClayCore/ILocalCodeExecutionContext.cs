using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ILocalCodeExecutionContext : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        ILocalCodeExecutionContext Parent { get; }

        bool UseParentInResolving { get; }

        StrongIdentifierValue Holder { get;}
        IStorage Storage { get;}

        StrongIdentifierValue Owner { get;}
        IStorage OwnerStorage { get; }

        KindOfLocalCodeExecutionContext Kind { get; }
        KindOfAddFactOrRuleResult KindOfAddFactResult { get; set; }
        MutablePartOfRuleInstance MutablePart { get; }
        RuleInstance AddedRuleInstance { get; }
    }
}

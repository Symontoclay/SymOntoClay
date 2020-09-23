using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public enum KindOfCodeEntity
    {
        Unknown,
        World,
        Host,
        App,
        Class,
        Instance,
        InlineTrigger,
        Operator,
        Channel,
        RuleOrFact
    }
}

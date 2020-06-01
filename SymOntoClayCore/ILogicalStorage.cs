using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ILogicalStorage
    {
        KindOfStorage Kind { get; }
        void Append(RuleInstance ruleInstance);
        void Append(RuleInstance ruleInstance, bool isPrimary);
    }
}

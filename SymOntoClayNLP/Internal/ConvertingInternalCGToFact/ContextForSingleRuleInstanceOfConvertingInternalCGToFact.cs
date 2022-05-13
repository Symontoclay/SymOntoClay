using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToFact
{
    public class ContextForSingleRuleInstanceOfConvertingInternalCGToFact
    {
        public RuleInstance CurrentRuleInstance { get; set; }
        public Dictionary<string, string> EntityConditionsDict { get; set; } = new Dictionary<string, string>();
    }
}

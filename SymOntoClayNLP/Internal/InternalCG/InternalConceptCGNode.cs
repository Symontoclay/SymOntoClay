using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.InternalCG
{
    public class InternalConceptCGNode : BaseInternalConceptCGNode
    {
        public override KindOfCGNode Kind => KindOfCGNode.Concept;
        public override bool IsConceptNode => true;
        public override InternalConceptCGNode AsConceptNode => this;

        public bool IsRootConceptOfEntitiCondition { get; set; }

        public override void Destroy()
        {
            Parent = null;
            foreach (var node in Inputs)
            {
                NSRemoveInputNode(node);
            }

            foreach (var node in Outputs)
            {
                NSRemoveOutputNode(node);
            }
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(IsRootConceptOfEntitiCondition)} = {IsRootConceptOfEntitiCondition}");
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        public override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(IsRootConceptOfEntitiCondition)} = {IsRootConceptOfEntitiCondition}");
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }
    }
}

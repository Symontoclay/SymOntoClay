using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.CG
{
    public class ConceptCGNode : BaseConceptCGNode
    {
        /// <inheritdoc/>
        public override KindOfCGNode Kind => KindOfCGNode.Concept;
    }
}

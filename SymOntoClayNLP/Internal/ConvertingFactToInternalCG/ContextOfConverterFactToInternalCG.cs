using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ContextOfConverterFactToInternalCG
    {
        public InternalConceptualGraph ConceptualGraph { get; set; }
        public IEntityLogger Logger { get; set; }
        public INLPConverterContext NLPContext { get; set; }
    }
}

using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToCG
{
    public class ContextOfConverterFactToCG
    {
        public ConceptualGraph OuterConceptualGraph { get; set; }
        public ConceptualGraph ConceptualGraph { get; set; }
        public IEntityLogger Logger { get; set; }
        public IStorage Storage { get; set; }
    }
}

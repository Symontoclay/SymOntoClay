using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToCG
{
    public class ConverterFactToCG
    {
        public ConverterFactToCG(IEntityLogger logger)
        {
            _logger = logger;
        }

        private readonly IEntityLogger _logger;

        public ConceptualGraph Convert(RuleInstance fact, INLPConverterContext nlpContext)
        {
            var outerConceptualGraph = new ConceptualGraph();
            var context = new ContextOfConverterFactToCG();
            context.ConceptualGraph = outerConceptualGraph;
            context.Logger = _logger;
            context.NLPContext = nlpContext;

            var factNode = new RuleInstanceNode(fact, context);
            var factNodeResult = factNode.Run();

#if DEBUG
            _logger.Log($"factNodeResult = {factNodeResult}");
#endif

            throw new NotImplementedException();

            return outerConceptualGraph;
        }
    }
}

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

        public ConceptualGraph Convert(RuleInstance fact, IStorage storage)
        {
            var outerConceptualGraph = new ConceptualGraph();
            var context = new ContextOfConverterFactToCG();
            context.OuterConceptualGraph = outerConceptualGraph;
            context.Logger = _logger;
            context.Storage = storage;

            var factNode = new RuleInstanceNode(fact, context);
            factNode.Run();

            return outerConceptualGraph;
        }

        private ConceptualGraph Convert(PrimaryRulePart primaryPart, ContextOfConverterFactToCG context)
        {
#if DEBUG
            _logger.Log($"primaryPart = {DebugHelperForRuleInstance.BaseRulePartToString(primaryPart, HumanizedOptions.ShowOnlyMainContent)}");
#endif

            throw new NotImplementedException();
        }
    }
}

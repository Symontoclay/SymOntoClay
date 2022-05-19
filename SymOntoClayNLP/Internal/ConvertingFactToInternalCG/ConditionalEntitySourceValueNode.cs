using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ConditionalEntitySourceValueNode
    {
        public ConditionalEntitySourceValueNode(ConditionalEntitySourceValue value, ContextOfConverterFactToInternalCG context)
        {
            _value = value;
            _context = context;
            _logger = context.Logger;
        }

        private readonly ConditionalEntitySourceValue _value;
        private readonly ContextOfConverterFactToInternalCG _context;
        private readonly IEntityLogger _logger;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_value = {_value.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var outerConceptualGraph = new InternalConceptualGraph();
            var context = new ContextOfConverterFactToInternalCG();
            context.ConceptualGraph = outerConceptualGraph;
            context.Logger = _logger;
            context.NLPContext = _context.NLPContext;

            if(_value.LogicalQuery == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                var factNode = new RuleInstanceNode(_value.LogicalQuery, context);
                var factNodeResult = factNode.Run();

#if DEBUG
                _logger.Log($"factNodeResult = {factNodeResult}");
#endif

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}

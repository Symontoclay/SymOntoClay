using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.Dot;
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
            //_logger.Log($"_value = {_value.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var outerConceptualGraph = new InternalConceptualGraph();

            if (_context.ConceptualGraph != null)
            {
                outerConceptualGraph.Parent = _context.ConceptualGraph;
            }

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
                //_logger.Log($"factNodeResult = {factNodeResult}");
#endif
            }

#if DEBUG
            //var dotStr = DotConverter.ConvertToString(context.ConceptualGraph);
            //_logger.Log($"dotStr = {dotStr}");
#endif

            var result = new ResultOfNode() { KindOfResult = KindOfResultOfNode.ProcessConditionalEntity };
            result.CGNode = outerConceptualGraph;

            return result;
        }
    }
}

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ConceptNode
    {
        public ConceptNode(LogicalQueryNode concept, ContextOfConverterFactToInternalCG context)
        {
            _concept = concept;
            _context = context;
            _logger = context.Logger;
        }

        private readonly LogicalQueryNode _concept;
        private readonly ContextOfConverterFactToInternalCG _context;
        private readonly IEntityLogger _logger;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_concept = {_concept.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var result = new ResultOfNode();
            result.KindOfResult = KindOfResultOfNode.ProcessConcept;
            result.LogicalQueryNode = _concept;
            result.ConceptCGNode = new InternalConceptCGNode() { Name = _concept.Name.NameValue, Parent = _context.ConceptualGraph };

            return result;
        }
    }
}

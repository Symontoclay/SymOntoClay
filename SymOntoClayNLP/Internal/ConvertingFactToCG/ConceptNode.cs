using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToCG
{
    public class ConceptNode
    {
        public ConceptNode(LogicalQueryNode concept, ContextOfConverterFactToCG context)
        {
            _concept = concept;
            _context = context;
            _logger = context.Logger;
        }

        private readonly LogicalQueryNode _concept;
        private readonly ContextOfConverterFactToCG _context;
        private readonly IEntityLogger _logger;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_concept = {DebugHelperForRuleInstance.ToString(_concept, HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var result = new ResultOfNode();
            result.LogicalQueryNode = _concept;
            result.ConceptCGNode = new ConceptCGNode() { Name = _concept.Name.NameValue, Parent = _context.ConceptualGraph };

            return result;
        }
    }
}

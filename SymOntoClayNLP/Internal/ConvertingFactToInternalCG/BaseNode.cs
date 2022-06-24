using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public abstract class BaseNode
    {
        protected BaseNode(ContextOfConverterFactToInternalCG context)
        {
            _context = context;
            _logger = context.Logger;
        }

        protected readonly ContextOfConverterFactToInternalCG _context;
        protected readonly IEntityLogger _logger;

        protected InternalConceptCGNode CreateOrGetExistingInternalConceptCGNode(string name)
        {
#if DEBUG
            //_logger.Log($"name = {name}");
#endif

            var result = _context.ConceptualGraph.Children.SingleOrDefault(p => p.Kind == KindOfCGNode.Concept && p.Name == name);

#if DEBUG
            //_logger.Log($"result = {result}");
#endif

            if (result != null)
            {
                return result.AsConceptNode;
            }

            return new InternalConceptCGNode() { Name = name, Parent = _context.ConceptualGraph };
        }
    }
}

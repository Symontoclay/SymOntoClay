using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToText
{
    public class NounNode
    {
        public NounNode(BaseInternalConceptCGNode source, List<string> disabledRelations, ContextOfConvertingInternalCGToText context)
        {
            _context = context;
            _logger = context.Logger;
            _source = source;
            _disabledRelations = disabledRelations;
        }

        private readonly ContextOfConvertingInternalCGToText _context;
        private readonly IEntityLogger _logger;

        private readonly BaseInternalConceptCGNode _source;
        private readonly List<string> _disabledRelations;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_source = {_source}");
#endif

            var kind = _source.Kind;

#if DEBUG
            _logger.Log($"kind = {kind}");
#endif

            switch(kind)
            {
                case KindOfCGNode.Concept:
                    return ProcessConcept();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private ResultOfNode ProcessConcept()
        {
            var conceptName = _source.Name;

#if DEBUG
            _logger.Log($"conceptName = '{conceptName}'");
#endif

            return new ResultOfNode() 
            { 
                MainText = conceptName, 
                RootWord = conceptName 
            };
        }
    }
}

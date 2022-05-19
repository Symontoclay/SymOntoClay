using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class LogicalVarNode
    {
        public LogicalVarNode(LogicalQueryNode expression, ContextOfConverterFactToInternalCG context)
        {
            _var = expression;
            _context = context;
            _logger = context.Logger;
        }

        private readonly LogicalQueryNode _var;
        private readonly ContextOfConverterFactToInternalCG _context;
        private readonly IEntityLogger _logger;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_var = {_var.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var varName = _var.Name.NameValue;

#if DEBUG
            _logger.Log($"varName = '{varName}'");
#endif

            if (_context.VarConceptDict.ContainsKey(varName))
            {
                throw new NotImplementedException();
            }

            var conceptName = ResolveConceptName();

#if DEBUG
            _logger.Log($"conceptName = '{conceptName}'");
#endif

            var result = new ResultOfNode();
            result.LogicalQueryNode = _var;

            var concept = new InternalConceptCGNode() { Name = conceptName, Parent = _context.ConceptualGraph };

            if(varName == "$_")
            {
                concept.IsRootConceptOfEntitiCondition = true;
            }

            result.ConceptCGNode = concept;

            return result;
        }

        private string ResolveConceptName()
        {
            var conceptFromVarResolver = new ConceptFromVarResolver(_var.Name, _context);
            return conceptFromVarResolver.Resolve();
        }
    }
}

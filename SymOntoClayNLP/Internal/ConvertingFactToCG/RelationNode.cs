using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToCG
{
    public class RelationNode
    {
        public RelationNode(LogicalQueryNode relation, ContextOfConverterFactToCG context)
        {
            _relation = relation;
            _context = context;
            _logger = context.Logger;
        }

        private readonly LogicalQueryNode _relation;
        private readonly ContextOfConverterFactToCG _context;
        private readonly IEntityLogger _logger;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_relation = {DebugHelperForRuleInstance.ToString(_relation, HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var result = new ResultOfNode();

            throw new NotImplementedException();
        }
    }
}

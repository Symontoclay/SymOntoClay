using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalQueryAsCodeEntityParser : BaseInternalParser
    {
        public LogicalQueryAsCodeEntityParser(InternalParserContext context)
            : base(context)
        {
        }

        public CodeEntity Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = CreateCodeEntity();
            Result.Kind = KindOfCodeEntity.RuleOrFact;

            Result.CodeFile = _context.CodeFile;
            Result.ParentCodeEntity = CurrentCodeEntity;
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
#endif

            _context.Recovery(_currToken);
            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var ruleInstanceItem = parser.Result;

#if DEBUG
            //Log($"ruleInstanceItem = {ruleInstanceItem}");
#endif

            Result.Name = ruleInstanceItem.Name;
            Result.RuleInstance = ruleInstanceItem;

            Exit();            
        }
    }
}

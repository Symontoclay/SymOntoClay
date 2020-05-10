using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class TopLevelParser: BaseInternalParser
    {
        public TopLevelParser(InternalParserContext context)
            : base(context)
        {
        }

        public List<CodeEntity> Result => throw new NotImplementedException();

        protected override void OnRun()
        {
            Log($"_currToken = {_currToken}");
        }
    }
}

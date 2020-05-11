using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.Helpers
{
    public class TstParserContext : IParserContext
    {
        private readonly ILogger _logger = new EmptyLogger();

        public ILogger Logger => _logger;
    }
}

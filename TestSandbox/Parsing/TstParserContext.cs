using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Parsing
{
    public class TstParserContext : IParserContext
    {
        private readonly ILogger _logger = new LoggerImpementation();

        public ILogger Logger => _logger;
    }
}

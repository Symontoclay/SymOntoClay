using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Dict;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.Helpers
{
    public class TstParserContext : IParserContext
    {
        public TstParserContext()
        {
            _dictionary = new EntityDictionary(_logger);
            _dictionary.LoadFromSourceCode();
        }

        private readonly IEntityLogger _logger = new EmptyLogger();

        public IEntityLogger Logger => _logger;

        private readonly EntityDictionary _dictionary;

        public IEntityDictionary Dictionary => _dictionary;
    }
}

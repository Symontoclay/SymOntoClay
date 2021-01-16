using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache
{
    public class LogicQueryParseAndCache: BaseWorldCoreComponent, ILogicQueryParseAndCache
    {
        public LogicQueryParseAndCache(WorldSettings settings, IWorldCoreContext coreContext)
            : base(coreContext)
        {
        }

        private readonly Dictionary<string, RuleInstance> _cache = new Dictionary<string, RuleInstance>();

        /// <inheritdoc/>
        public RuleInstance GetLogicRuleOrFact(string text)
        {
            var textKey = text.Replace(" ", string.Empty).Trim();

            if(_cache.ContainsKey(textKey))
            {
                return _cache[textKey];
            }

            throw new NotImplementedException();
        }
    }
}

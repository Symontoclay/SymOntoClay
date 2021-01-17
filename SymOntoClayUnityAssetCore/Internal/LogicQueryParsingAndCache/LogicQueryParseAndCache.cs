using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Parsing;
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
            var settingsOfContext = new BaseCoreSettings();
            settingsOfContext.Logger = coreContext.Logger;
            settingsOfContext.Dictionary = settingsOfContext.Dictionary;
            settingsOfContext.DateTimeProvider = settingsOfContext.DateTimeProvider;

            _context = EngineContextHelper.CreateAndInitBaseCoreContext(settingsOfContext);

            _parser = new Parser(_context);
        }

        private readonly IBaseCoreContext _context;
        private readonly IParser _parser;
        private readonly Dictionary<string, RuleInstance> _cache = new Dictionary<string, RuleInstance>();

        /// <inheritdoc/>
        public RuleInstance GetLogicRuleOrFact(string text)
        {
            var textKey = text.Replace(" ", string.Empty).Trim();

            if(_cache.ContainsKey(textKey))
            {
                return _cache[textKey];
            }

            var codeEntity = _parser.Parse(text);

            if(codeEntity.Kind == KindOfCodeEntity.RuleOrFact)
            {
                return codeEntity.RuleInstance;
            }

            throw new NotSupportedException($"There can only be rule or fact here!");
        }
    }
}

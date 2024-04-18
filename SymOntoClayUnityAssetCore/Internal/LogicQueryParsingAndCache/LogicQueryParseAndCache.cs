/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache
{
    public class LogicQueryParseAndCache: BaseWorldCoreComponent, ILogicQueryParseAndCache
    {
        public LogicQueryParseAndCache(WorldSettings settings, IWorldCoreContext coreContext)
            : base(coreContext)
        {
            var settingsOfContext = new BaseCoreSettings();
            settingsOfContext.MonitorNode = coreContext.MonitorNode;
            
            settingsOfContext.DateTimeProvider = coreContext.DateTimeProvider;

            _context = EngineContextHelper.CreateAndInitBaseCoreContext(settingsOfContext);

            _parser = new Parser(_context);
        }

        private readonly IBaseCoreContext _context;
        private readonly IParser _parser;
        private readonly Dictionary<string, RuleInstance> _cache = new Dictionary<string, RuleInstance>();

        /// <inheritdoc/>
        public RuleInstance GetLogicRuleOrFact(IMonitorLogger logger, string text)
        {
            var textKey = text.Replace(" ", string.Empty).Trim();

            if(_cache.ContainsKey(textKey))
            {
                return _cache[textKey];
            }

            var result = _parser.ParseRuleInstance(text);
            _cache[textKey] = result;
            return result;            
        }
    }
}

using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ATNParser
    {
        public ATNParser(IEntityLogger logger, IWordsDict wordsDict)
        {
            _wordsDict = wordsDict;
            _logger = logger;
        }

        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        public List<BaseSentenceItem> Run(string text)
        {
            return Run(text, false, string.Empty);
        }

        public List<BaseSentenceItem> Run(string text, bool dumpToLogDirOnExit, string logDir)
        {
#if DEBUG
            //_logger.Log($"text = {text}");
#endif

            var globalContext = new GlobalParserContext(_logger, _wordsDict, text, dumpToLogDirOnExit, logDir);

            globalContext.Run();

            return globalContext.GetPhrases();
        }
    }
}

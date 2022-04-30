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
        public ATNParser(INLPContext context, IWordsDict wordsDict)
        {
            _context = context;
            _wordsDict = wordsDict;
            _logger = context.Logger;
        }

        private readonly INLPContext _context;
        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        public List<BaseSentenceItem> Run(string text)
        {
#if DEBUG
            //_logger.Log($"text = {text}");
#endif

            var globalContext = new GlobalParserContext(_context, _wordsDict, text);

            globalContext.Run();

            return globalContext.GetPhrases();
        }
    }
}

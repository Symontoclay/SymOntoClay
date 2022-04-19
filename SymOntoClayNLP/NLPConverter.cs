using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP
{
    public class NLPConverter: INLPConverter
    {
        public NLPConverter(INLPContext context, IWordsDict wordsDict)
        {
            _context = context;
            _wordsDict = wordsDict;
            _logger = context.Logger;
        }

        private readonly INLPContext _context;
        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        public IList<RuleInstance> Convert(string text)
        {
#if DEBUG
            _logger.Log($"text = {text}");
            var lexer = new ATNStringLexer(text);
            (string, int, int) item;

            while((item = lexer.GetItem()).Item1 != null)
            {
                _logger.Log($"item = {item}");
            }

            ATNToken token = null;

            var lexer_2 = new ATNLexer(text, _wordsDict);
            while((token = lexer_2.GetToken()) != null)
            {
                _logger.Log($"token = {token}");
            }
#endif

            throw new NotImplementedException();
        }
    }
}

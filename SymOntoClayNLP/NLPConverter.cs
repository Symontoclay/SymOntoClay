using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP
{
    public class NLPConverter: INLPConverter
    {
        public NLPConverter(INLPContext context)
        {
            _context = context;
            _logger = context.Logger;
        }

        private readonly INLPContext _context;
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
#endif

            throw new NotImplementedException();
        }
    }
}

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class InternalParserContext
    {
        public InternalParserContext()
        {
        }

        public InternalParserContext(string text, IParserContext context)
        {
            _context = context;
            _lexer = new Lexer(text, context.Logger);
        }

        public ILogger Logger => _context.Logger;

        private IParserContext _context;
        private Lexer _lexer;
        private Queue<Token> _recoveriesTokens = new Queue<Token>();

        public Token GetToken()
        {
            if (_recoveriesTokens.Count == 0)
            {
                return _lexer.GetToken();
            }

            return _recoveriesTokens.Dequeue();
        }

        public void Recovery(Token token)
        {
            _recoveriesTokens.Enqueue(token);
        }

        public bool IsEmpty()
        {
            var tmpToken = GetToken();

            if (tmpToken == null)
            {
                return true;
            }

            Recovery(tmpToken);

            return false;
        }

        /// <summary>
        /// Number of remaining characters.
        /// </summary>
        public int Count
        {
            get
            {
                return _recoveriesTokens.Count + _lexer.Count;
            }
        }

        public InternalParserContext Fork()
        {
            var result = new InternalParserContext();
            result._context = _context;
            result._lexer = _lexer.Fork();
            result._recoveriesTokens = new Queue<Token>(_recoveriesTokens);
            return result;
        }

        public void Assing(InternalParserContext context)
        {
            _lexer.Assing(context._lexer);
            _recoveriesTokens = new Queue<Token>(context._recoveriesTokens);
        }
    }
}

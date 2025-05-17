using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class InternalParserCoreContext
    {
        protected InternalParserCoreContext()
        {
        }

        public InternalParserCoreContext(InternalParserCoreContext source, IMonitorLogger logger)
        {
            throw new NotImplementedException();
        }

        public InternalParserCoreContext(string text, IMonitorLogger logger, LexerMode mode = LexerMode.Code)
        {
            _logger = logger;
            _lexer = new Lexer(text, logger, mode);
        }

        private readonly IMonitorLogger _logger;

        public IMonitorLogger Logger => _logger;

        private Lexer _lexer;
        private Stack<Token> _recoveriesTokens = new Stack<Token>();

        public Token GetToken()
        {
            if (_recoveriesTokens.Count == 0)
            {
                return _lexer.GetToken();
            }

            return _recoveriesTokens.Pop();
        }

        public void Recovery(Token token)
        {
            _recoveriesTokens.Push(token);

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

        public InternalParserCoreContext Fork()
        {
            var result = new InternalParserCoreContext();

            Append(result);

            return result;
        }

        protected void Append(InternalParserCoreContext dest)
        {
            dest._lexer = _lexer.Fork();
            dest._recoveriesTokens = new Stack<Token>(_recoveriesTokens.Reverse().ToList());
        }

        public void Assign(InternalParserCoreContext context)
        {
            _lexer.Assign(context._lexer);
            _recoveriesTokens = new Stack<Token>(context._recoveriesTokens.Reverse());
        }
    }
}

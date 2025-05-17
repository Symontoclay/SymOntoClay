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
            _logger = logger;
            _source = source;
        }

        public InternalParserCoreContext(string text, IMonitorLogger logger, LexerMode mode = LexerMode.Code)
        {
            _logger = logger;
            _lexer = new Lexer(text, logger, mode);
        }

        private InternalParserCoreContext _source;

        private readonly IMonitorLogger _logger;

        public IMonitorLogger Logger => _logger;

        private Lexer _lexer;
        private Stack<Token> _recoveriesTokens = new Stack<Token>();

        public Token GetToken()
        {
            if(_source != null)
            {
                return _source.GetToken();
            }

            if (_recoveriesTokens.Count == 0)
            {
                return _lexer.GetToken();
            }

            return _recoveriesTokens.Pop();
        }

        public void Recovery(Token token)
        {
            if (_source != null)
            {
                _source.Recovery(token);
            }

            _recoveriesTokens.Push(token);
        }

        public bool IsEmpty()
        {
            if (_source != null)
            {
                return _source.IsEmpty();
            }

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
                if (_source != null)
                {
                    return _source.Count;
                }

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
            dest._source = _source;
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

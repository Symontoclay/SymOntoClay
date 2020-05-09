using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class Lexer
    {
        private enum State
        {
            Init
        }

        public Lexer(string text, ILogger logger)
        {
            _logger = logger;
            _items = new Queue<char>(text.ToList());
        }

        private readonly ILogger _logger;
        private Queue<char> _items;
        private State _state = State.Init;

        private Queue<Token> _recoveriesTokens = new Queue<Token>();

        private int _currentPos;
        private int _currentLine = 1;

        public Token GetToken()
        {
            if (_recoveriesTokens.Count > 0)
            {
                return _recoveriesTokens.Dequeue();
            }

            StringBuilder tmpBuffer = null;

            while (_items.Count > 0)
            {
                var tmpChar = _items.Dequeue();

                _currentPos++;

#if DEBUG
                _logger.Log($"tmpChar = {tmpChar}");
                _logger.Log($"_currentPos = {_currentPos}");
#endif
            }

            throw new NotImplementedException();
        }
    }
}

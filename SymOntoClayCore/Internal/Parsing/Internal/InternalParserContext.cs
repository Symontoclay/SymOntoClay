/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class InternalParserContext
    {
        private InternalParserContext()
        {
        }

        public InternalParserContext(string text, CodeFile codeFile, IBaseCoreContext context)
        {
            _context = context;
            CodeFile = codeFile;
            _lexer = new Lexer(text, context.Logger);
        }

        public IEntityLogger Logger => _context.Logger;
        public ICompiler Compiler => _context.Compiler;

        public CodeFile CodeFile { get; private set; }

        private IBaseCoreContext _context;
        private Lexer _lexer;
        private Queue<Token> _recoveriesTokens = new Queue<Token>();

        private Stack<CodeEntity> _codeEntities = new Stack<CodeEntity>();

        public void SetCurrentCodeEntity(CodeEntity codeEntity)
        {
            _codeEntities.Push(codeEntity);
        }

        public void RemoveCurrentCodeEntity()
        {
            _codeEntities.Pop();
        }

        public CodeEntity CurrentCodeEntity
        {
            get
            {
                if(!_codeEntities.Any())
                {
                    return null;
                }

                return _codeEntities.Peek();
            }
        }

        private Stack<DefaultSettingsOfCodeEntity> _defaultSettingsOfCodeEntity = new Stack<DefaultSettingsOfCodeEntity>();
        private DefaultSettingsOfCodeEntity _currentDefaultSetings;

        public DefaultSettingsOfCodeEntity CurrentDefaultSetings => _currentDefaultSetings;

        public void SetCurrentDefaultSetings(DefaultSettingsOfCodeEntity defaultSettings)
        {
            if (!_defaultSettingsOfCodeEntity.Any())
            {
                _defaultSettingsOfCodeEntity.Push(defaultSettings);
                _currentDefaultSetings = defaultSettings;
                return;
            }

            DefaultSettingsOfCodeEntityHelper.Mix(_currentDefaultSetings, defaultSettings);
            _defaultSettingsOfCodeEntity.Push(defaultSettings);
            _currentDefaultSetings = defaultSettings;
        }

        public void RemoveCurrentDefaultSetings()
        {
            if (!_defaultSettingsOfCodeEntity.Any())
            {
                return;
            }

            _defaultSettingsOfCodeEntity.Pop();

            if(_defaultSettingsOfCodeEntity.Any())
            {
                _currentDefaultSetings = _defaultSettingsOfCodeEntity.Peek();
            }
            else
            {
                _currentDefaultSetings = null;
            }
        }

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
#if DEBUG
            //Logger.Log($"token = {token}");
#endif

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
            result._recoveriesTokens = new Queue<Token>(_recoveriesTokens.ToList());
            result.CodeFile = CodeFile;
            result._codeEntities = new Stack<CodeEntity>(_codeEntities.Select(p => p.Clone()).ToList());

            result._defaultSettingsOfCodeEntity = new Stack<DefaultSettingsOfCodeEntity>(_defaultSettingsOfCodeEntity.Select(p => p.Clone()).ToList());
            result._currentDefaultSetings = result._defaultSettingsOfCodeEntity.Peek();

            return result;
        }

        public void Assing(InternalParserContext context)
        {
            _lexer.Assing(context._lexer);
            _recoveriesTokens = new Queue<Token>(context._recoveriesTokens);
        }
    }
}

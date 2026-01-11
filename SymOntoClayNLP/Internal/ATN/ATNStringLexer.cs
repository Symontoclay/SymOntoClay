/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ATNStringLexer
    {
        private enum State
        {
            Init,
            InWord
        }

        public ATNStringLexer(string text)
        {
            _itemsList = new List<char>(text.Replace("’", "'").Replace("`", "'").ToList());

            _items = new Queue<char>(_itemsList);
        }

        private ATNStringLexer()
        {
        }

        private Queue<char> _items;
        private List<char> _itemsList;
        private int _currentPos;
        private int _currentLine = 1;

        private State _state = State.Init;

        public ATNStringLexer Fork()
        {
            var newLexer = new ATNStringLexer();
            newLexer._itemsList = _itemsList.ToList();
            newLexer._items = new Queue<char>(_items);
            newLexer._currentPos = _currentPos;
            newLexer._currentLine = _currentLine;
            newLexer._state = _state;

            return newLexer;
        }

        public (string, int, int) GetItem()
        {
            StringBuilder buffer = null;
            var bufferPos = 0;

            while (_items.Count > 0)
            {
                var ch = _items.Dequeue();
                var nextChar = PeekNextChar();

                _currentPos++;

                switch (_state)
                {
                    case State.Init:
                        {
                            if(IsRawLetter(ch))
                            {
                                if(IsRawLetter(nextChar))
                                {
                                    buffer = new StringBuilder();
                                    buffer.Append(ch);
                                    bufferPos = _currentPos;
                                    _state = State.InWord;
                                    break;
                                }
                                else
                                {
                                    return (ch.ToString(), _currentLine, _currentPos);
                                }
                            }                            

                            switch(ch)
                            {
                                case ' ':
                                    break;
                                
                                case '(':
                                case ')':
                                case ',':
                                case ':':
                                case ';':
                                case '-':                                
                                case '!':
                                case '?':
                                case '"':
                                    return (ch.ToString(), _currentLine, _currentPos);

                                case '.':
                                    if(nextChar != '.')
                                    {
                                        return (ch.ToString(), _currentLine, _currentPos);
                                    }

                                    {
                                        var pos = _currentPos - 1;

                                        var item1 = Forecast(pos);
                                        var item2 = Forecast(pos + 1);
                                        var item3 = Forecast(pos + 2);

                                        if (item1 == '.' && item2 == '.' && item3 == '.')
                                        {
                                            var targetPos = _currentPos;

                                            _currentPos += 2;
                                            _items.Dequeue();
                                            _items.Dequeue();

                                            return ("...", _currentLine, targetPos);
                                        }

                                        return (ch.ToString(), _currentLine, _currentPos);
                                    }

                                default:
                                    {
                                        var intChVal = (int)ch;

                                        switch(intChVal)
                                        {
                                            case 9:
                                                break;

                                            case 10:
                                                _currentPos = 0;
                                                _currentLine++;
                                                break;

                                            case 13:
                                                break;

                                            default:
                                                throw new ArgumentOutOfRangeException(nameof(ch), ch, null);
                                        }
                                    }
                                    break;                                    
                            }
                        }
                        break;

                        case State.InWord:
                        {
                            buffer.Append(ch);

                            if (IsRawLetter(nextChar))
                            {                                
                                break;
                            }

                            _state = State.Init;

                            return (buffer.ToString(), _currentLine, bufferPos);
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
                }
            }

            return (null, 0, 0);
        }

        private char? Forecast(int pos)
        {
            if(pos >= _itemsList.Count)
            {
                return null;
            }

            return _itemsList[pos];
        }

        private bool IsRawLetter(char? ch)
        {
            if(!ch.HasValue)
            {
                return false;
            }

            var chVal = ch.Value;

            if (char.IsLetterOrDigit(chVal))
            {
                return true;
            }

            switch(chVal)
            {
                case '@':
                case '$':
                case '%':
                case '#':
                case '\'':
                    return true;

                default: return false;
            }
        }

        private char? PeekNextChar()
        {
            if(_items.Count == 0)
            {
                return null;
            }

            return _items.Peek();
        }
    }
}

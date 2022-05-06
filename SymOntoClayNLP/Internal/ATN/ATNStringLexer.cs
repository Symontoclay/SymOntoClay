﻿using NLog;
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

        private Queue<char> _items;
        private List<char> _itemsList;
        private int _currentPos;
        private int _currentLine = 1;

        private State _state = State.Init;

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

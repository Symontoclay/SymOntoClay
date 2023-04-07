/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ATNLexer
    {
        public ATNLexer(string text, IWordsDict wordsDict)
        {
            _wordsDict = wordsDict;
            _lexer = new ATNStringLexer(text);
            InitTransformsDict();
        }

        private ATNLexer(IWordsDict wordsDict)
        {
            _wordsDict = wordsDict;

            InitTransformsDict();
        }

        private ATNStringLexer _lexer;
        private IWordsDict _wordsDict;
        private Queue<(string, int, int)> _recoveredSourceItems = new Queue<(string, int, int)> ();
        private Dictionary<string, List<string>> _transformsDict = new Dictionary<string, List<string>>();

        private Queue<ATNToken> _recoveriesTokens = new Queue<ATNToken>();

        public ATNLexer Fork()
        {
            var newATNLexer = new ATNLexer(_wordsDict);
            newATNLexer._lexer = _lexer.Fork();
            newATNLexer._recoveredSourceItems = new Queue<(string, int, int)>(_recoveredSourceItems);
            newATNLexer._recoveriesTokens = new Queue<ATNToken>(_recoveriesTokens);

            return newATNLexer;
        }

        public bool HasToken()
        {
            if(_recoveriesTokens.Any())
            {
                return true;
            }

            var currToken = GetToken();

            if(currToken == null)
            {
                return false;
            }

            Recovery(currToken);

            return true;
        }

        public ATNToken GetToken()
        {
            if (_recoveriesTokens.Count > 0)
            {
                return _recoveriesTokens.Dequeue();
            }

            var item = GetSourceItem();

            var strItem = item.Item1;

            if(strItem == null)
            {
                return null;
            }

            var result = new ATNToken()
            {
                Content = strItem,
                Line = item.Item2,
                Pos = item.Item3
            };

            switch (strItem)
            {
                case "(":
                    result.Kind = KindOfATNToken.OpenRoundBracket;
                    break;

                case ")":
                    result.Kind = KindOfATNToken.CloseRoundBracket;
                    break;

                case ",":
                    result.Kind = KindOfATNToken.Comma;
                    break;

                case ":":
                    result.Kind = KindOfATNToken.Colon;
                    break;

                case ";":
                    result.Kind = KindOfATNToken.Semicolon;
                    break;

                case "-":
                    result.Kind = KindOfATNToken.Dash;
                    break;

                case ".":
                    result.Kind = KindOfATNToken.Point;
                    break;

                case "!":
                    result.Kind = KindOfATNToken.ExclamationMark;
                    break;

                case "?":
                    result.Kind = KindOfATNToken.QuestionMark;
                    break;

                case "\"":
                    result.Kind = KindOfATNToken.DoubleQuotationMark;
                    break;

                default:
                    {
                        if(strItem.All(p => char.IsLetter(p)))
                        {
                            result.Kind = KindOfATNToken.Word;

                            result.WordFrames = _wordsDict.GetWordFramesByWord(strItem.ToLower());

                            return result;
                        }

                        if(strItem.All(p => char.IsDigit(p)))
                        {
                            result.Kind = KindOfATNToken.Number;

                            return result;
                        }

                        result.Kind = KindOfATNToken.Word;

                        return result;
                    }
            }

            return result;
        }

        private (string, int, int) GetSourceItem()
        {
            if(_recoveredSourceItems.Count > 0)
            {
                return _recoveredSourceItems.Dequeue();
            }

            var item = _lexer.GetItem();

            var itemStrVal = item.Item1;

            if (itemStrVal == null)
            {
                return (null, 0, 0);
            }

            if(_transformsDict.ContainsKey(itemStrVal))
            {
                var targetList = _transformsDict[itemStrVal].ToList();

                var firstItem = targetList.First();

                targetList.Remove(firstItem);

                targetList.Reverse();

                foreach (var targetWord in targetList)
                {
                    _recoveredSourceItems.Enqueue((targetWord, item.Item2, item.Item3));
                }

                return (firstItem, item.Item2, item.Item3);
            }

            return item;
        }

        public void Recovery(ATNToken token)
        {
            _recoveriesTokens.Enqueue(token);
        }

        private void InitTransformsDict()
        {
            _transformsDict.Add("can't", new List<string>() { "can", "not" });
            _transformsDict.Add("cannot", new List<string>() { "can", "not" });
            _transformsDict.Add("couldn't", new List<string>() { "could", "not" });
            _transformsDict.Add("mayn't", new List<string>() { "may", "not" });
            _transformsDict.Add("mightn't", new List<string>() { "might", "not" });
            _transformsDict.Add("mustn't", new List<string>() { "must", "not" });
            _transformsDict.Add("haven't", new List<string>() { "have", "not" });
            _transformsDict.Add("don't", new List<string>() { "do", "not" });
            _transformsDict.Add("doesn't", new List<string>() { "does", "not" });
            _transformsDict.Add("didn't", new List<string>() { "did", "not" });
            _transformsDict.Add("isn't", new List<string>() { "is", "not" });
            _transformsDict.Add("shouldn't", new List<string>() { "should", "not" });
            _transformsDict.Add("wouldn't", new List<string>() { "would", "not" });
            _transformsDict.Add("oughtn't", new List<string>() { "ought", "not" });
            _transformsDict.Add("there's", new List<string>() { "there", "is" });
        }
    }
}

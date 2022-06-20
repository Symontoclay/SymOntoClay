/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public static class ReaderOfRootNounSourceWordItem
    {
        public static RootNounSourceWordItem Read(string source)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"Read source = {source}");
#endif

            var n = 0;

            var result = new RootNounSourceWordItem();

            var sb = new StringBuilder();

            var lastCharNum = 0;
            var hasAtsign = false;

            foreach (var ch in source)
            {
                var charNum = (int)ch;

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read ch = {ch} charNum = {charNum}");
#endif

                if(charNum == 32)
                {
                    if(lastCharNum != charNum)
                    {
                        n++;
                    }

                    lastCharNum = charNum;

                    var strValue = sb.ToString();

                    switch(n)
                    {
                        case 1:
                            {
                                var numOfWord = int.Parse(strValue);
                                result.WordNum = numOfWord;
                            }
                            break;

                        case 5:
                            result.Word = strValue.Replace("(a)", string.Empty).Replace("(p)", string.Empty).Trim();
                            break;

                        default:
                            if(n > 5)
                            {
                                if(hasAtsign)
                                {
                                    var numOfWord = int.Parse(strValue);
                                    result.ParentWordNumsList.Add(numOfWord);
                                    hasAtsign = false;
                                }
                                else
                                {
                                    if (strValue == "@")
                                    {
                                        hasAtsign = true;
                                    }
                                }
                            }
#if DEBUG
                            //NLog.LogManager.GetCurrentClassLogger().Info($"Read strValue = '{strValue}' n = {n}");
#endif
                            break;
                    }

                    sb = new StringBuilder();

                    continue;
                }

                lastCharNum = charNum;
                sb.Append(ch);
            }

            ReaderOfRootSourceWordItemHelper.FillMetrixOfSourceWordNetItem(result);
            return result;
        }
    }
}

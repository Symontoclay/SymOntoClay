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
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public static class ReaderOfRootSourceWordItemHelper
    {
        public static void FillMetrixOfSourceWordNetItem(IRootWordBasedMetrixOfSourceWordNet item)
        {
            var word = item.Word;

            if (IsStartFromDigit(word))
            {
                item.IsDigit = true;
            }

            if(IsName(word))
            {
                item.IsName = true;
            }

            if(IsComplex(word))
            {
                item.IsComplex = true;
            }
        }

        public static bool IsStartFromDigit(string word)
        {
            var firstChar = word[0];

            if (char.IsDigit(firstChar))
            {
                return true;
            }

            return false;
        }

        public static bool IsName(string word)
        {
            var firstChar = word[0];

            if (char.IsUpper(firstChar))
            {
                return true;
            }

            return false;
        }

        public static bool IsComplex(string word)
        {
            if (word.Contains("_") || word.Contains("-"))
            {
                return true;
            }

            return false;
        }

        public static void SeparateWords<T>(List<T> source, ref List<string> usualWordsList, ref List<string> namesWordsList, ref List<string> digitsWordsList, ref List<string> complexWordsList) where T: IRootWordBasedMetrixOfSourceWordNet
        {
            foreach (var item in source)
            {
#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"SeparateWords item = {item}");
#endif

                if(item.IsDigit)
                {
#if DEBUG
                    //NLog.LogManager.GetCurrentClassLogger().Info("SeparateWords IsDigit");
#endif
                    digitsWordsList.Add(item.Word);
                    continue;
                }

                if (item.IsName)
                {
#if DEBUG
                    //NLog.LogManager.GetCurrentClassLogger().Info("SeparateWords IsName");
#endif
                    namesWordsList.Add(item.Word);
                    continue;
                }

                if (item.IsComplex)
                {
#if DEBUG
                    //NLog.LogManager.GetCurrentClassLogger().Info("SeparateWords IsComplex");
#endif
                    complexWordsList.Add(item.Word);
                    continue;
                }

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info("SeparateWords Usual");
#endif
                usualWordsList.Add(item.Word);
            }

            usualWordsList = usualWordsList.Distinct().OrderBy(p => p).ToList();
            namesWordsList = namesWordsList.Distinct().OrderBy(p => p).ToList();
            digitsWordsList = digitsWordsList.Distinct().OrderBy(p => p).ToList();
            complexWordsList = complexWordsList.Distinct().OrderBy(p => p).ToList();
        }
    }
}

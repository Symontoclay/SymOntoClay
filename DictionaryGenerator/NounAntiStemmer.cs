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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public class NounAntiStemmer : BaseAntiStemmer
    {
        public NounAntiStemmer()
            : base(new NounExceptionCasesWordNetSource())
        {
        }

        public string GetPluralForm(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm baseForm = {baseForm}");
#endif

            var lastChar = baseForm.Last();

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm lastChar = {lastChar}");
#endif

            var multipleForm = string.Empty;

            switch(lastChar)
            {
                case 'h':
                case 'j':
                case 's':
                case 'x':
                case 'z':
                    multipleForm = $"{baseForm}es";
                    break;

                default:
                    multipleForm = $"{baseForm}s";
                    break;
            }

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm multipleForm = {multipleForm}");
#endif

            var listOfExeptWords = GetExceptionsList(baseForm);

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm listOfExeptWords.Count = {listOfExeptWords.Count}");
            //foreach (var exceptWord in listOfExeptWords)
            //{
            //    NLog.LogManager.GetCurrentClassLogger().Info($"GetPluralForm exceptWord = {exceptWord}");
            //}
#endif
            if (listOfExeptWords.Count == 0)
            {
                return multipleForm;
            }

            var edEndsWord = listOfExeptWords.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(edEndsWord))
            {
                return edEndsWord;
            }

            return multipleForm;
        }

        public string GetPossesiveForSingular(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPossesiveForSingular baseForm = {baseForm}");
#endif

            var lastChar = baseForm.Last();

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPossesiveForSingular lastChar = {lastChar}");
#endif

            if(lastChar == '\'')
            {
                return $"{baseForm}s";
            }

            return $"{baseForm}'s";
        }

        public string GetPossesiveForPlural(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPossesiveForPlural baseForm = {baseForm}");
#endif

            var lastChar = baseForm.Last();

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetPossesiveForPlural lastChar = {lastChar}");
#endif

            if (lastChar == '\'')
            {
                return baseForm;
            }

            return $"{baseForm}'";
        }
    }
}

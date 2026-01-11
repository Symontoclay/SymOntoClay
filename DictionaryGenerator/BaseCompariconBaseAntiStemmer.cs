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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public abstract class BaseCompariconBaseAntiStemmer : BaseAntiStemmer
    {
        protected BaseCompariconBaseAntiStemmer(BaseExceptionCasesWordNetSource exceptionCasesSource, BaseIrregularComparisonSource irregularComparisonSource)
            : base(exceptionCasesSource)
        {
            var irregularComparisonList = irregularComparisonSource.ReadAll();

            foreach(var irregularComparison in irregularComparisonList)
            {
                var rootWord = irregularComparison.BaseWord;

                mIrregularComparativeDict[rootWord] = irregularComparison.ComparativeForm;
                mIrregularSuperlativeDict[rootWord] = irregularComparison.SuperlativeForm;
            }
        }

        private Dictionary<string, string> mIrregularComparativeDict = new Dictionary<string, string>();
        private Dictionary<string, string> mIrregularSuperlativeDict = new Dictionary<string, string>();

        protected string GetIrregularComparativeForm(string baseForm)
        {
            if(mIrregularComparativeDict.ContainsKey(baseForm))
            {
                return mIrregularComparativeDict[baseForm];
            }

            return string.Empty;
        }

        protected string GetIrregularSuperlativeForm(string baseForm)
        {
            if(mIrregularSuperlativeDict.ContainsKey(baseForm))
            {
                return mIrregularSuperlativeDict[baseForm];
            }

            return string.Empty;
        }

        protected string GetStandardComparisonForm(string baseForm)
        {
            var modifiedBaseForm = baseForm;

            var lastChar = baseForm.Last();

            if (lastChar == 'y')
            {
                modifiedBaseForm = $"{baseForm.Remove(baseForm.Length - 1)}i";
            }

            var comparativeForm = $"{modifiedBaseForm}er";

            var listOfExeptWords = GetExceptionsList(baseForm);
            var exceptWord = listOfExeptWords.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(exceptWord))
            {
                return exceptWord;
            }

            return comparativeForm;
        }

        protected string GetStandardSuperlativeForm(string baseForm)
        {
            var modifiedBaseForm = baseForm;

            var lastChar = baseForm.Last();

            if (lastChar == 'y')
            {
                modifiedBaseForm = $"{baseForm.Remove(baseForm.Length - 1)}i";
            }

            var superlativeForm = $"{modifiedBaseForm}est";

            var listOfExeptWords = GetExceptionsList(baseForm);
            var exceptWord = listOfExeptWords.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(exceptWord))
            {
                return exceptWord;
            }

            return superlativeForm;
        }
    }
}

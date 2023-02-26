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
    public class AdjAntiStemmer : BaseCompariconBaseAntiStemmer
    {
        public AdjAntiStemmer()
            : base(new AdjExceptionCasesWordNetSource(), new AdjIrregularComparisonSource())
        {
        }

        public string GetComparativeForm(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetComparativeForm baseForm = {baseForm}");
#endif

            var comparativeForm = GetIrregularComparativeForm(baseForm);

            if(!string.IsNullOrWhiteSpace(comparativeForm))
            {
                return comparativeForm;
            }

            return GetStandardComparisonForm(baseForm);
        }

        public string GetSuperlativeForm(string baseForm)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"GetSuperlativeForm baseForm = {baseForm}");
#endif
            var superlativeForm = GetIrregularSuperlativeForm(baseForm);

            if(!string.IsNullOrWhiteSpace(superlativeForm))
            {
                return superlativeForm;
            }

            return GetStandardSuperlativeForm(baseForm);
        }
    }
}

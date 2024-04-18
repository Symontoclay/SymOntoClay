/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
    public abstract class BaseAntiStemmer
    {
        protected BaseAntiStemmer(BaseExceptionCasesWordNetSource exceptionCasesSource)
        {
            var exceptionCaseItemsList = exceptionCasesSource.ReadAll();

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"VerbAntiStemmer exceptionCaseItemsList.Count = {exceptionCaseItemsList.Count}");
#endif

            mExceptionsDict = exceptionCaseItemsList.GroupBy(p => p.RootWord).ToDictionary(p => p.Key, p => p.Select(x => x.ExceptWord).Distinct().ToList());
        }

        private Dictionary<string, List<string>> mExceptionsDict;

        protected List<string> GetExceptionsList(string rootWord)
        {
            if(mExceptionsDict.ContainsKey(rootWord))
            {
                return mExceptionsDict[rootWord];
            }

            return new List<string>();
        }
    }
}

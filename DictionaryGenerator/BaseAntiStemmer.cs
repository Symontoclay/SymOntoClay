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

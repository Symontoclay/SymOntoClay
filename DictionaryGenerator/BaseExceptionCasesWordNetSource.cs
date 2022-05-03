using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public abstract class BaseExceptionCasesWordNetSource : BaseRootWordNetSource
    {
        protected BaseExceptionCasesWordNetSource(string localPath)
            : base(localPath, 0)
        {
        }

        public List<ExceptionCaseItem> ReadAll()
        {
            var resultList = new List<ExceptionCaseItem>();

            Read((string currentLine) => {
#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read currentLine = {currentLine}");
#endif

                var itemsList = ReaderOfExceptionCaseItem.Read(currentLine);

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read itemsList.Count = {itemsList.Count}");
#endif

                resultList.AddRange(itemsList);
            });

            return resultList;
        }
    }
}

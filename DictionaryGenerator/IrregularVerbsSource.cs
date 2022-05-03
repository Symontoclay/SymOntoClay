using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class IrregularVerbsSource : BaseRootWordNetSource
    {
        public IrregularVerbsSource()
            : base(@"Resources\MyDicts\irverbs.txt", 0)
        {
        }

        public List<IrregularVerbItem> ReadAll()
        {
            var resultList = new List<IrregularVerbItem>();

            Read((string currentLine) => {
#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read currentLine = {currentLine}");
#endif

                var item = ReaderOfIrregularVerbItem.Read(currentLine);

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read item = {item}");
#endif

                resultList.Add(item);
            });

            return resultList;
        }
    }
}

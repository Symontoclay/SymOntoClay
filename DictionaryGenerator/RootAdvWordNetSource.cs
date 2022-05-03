using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public class RootAdvWordNetSource : BaseRootWordNetSource
    {
        public RootAdvWordNetSource()
            : base(@"Resources\WordNet\dict\data.adv", 30)
        {
        }

        public List<RootAdvSourceWordItem> ReadAll()
        {
            var resultList = new List<RootAdvSourceWordItem>();

            Read((string currentLine) => {
#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read currentLine = {currentLine}");
#endif

                var item = ReaderOfRootAdvSourceWordItem.Read(currentLine);

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read item = {item}");
#endif

                resultList.Add(item);
            });

            return resultList;
        }
    }
}

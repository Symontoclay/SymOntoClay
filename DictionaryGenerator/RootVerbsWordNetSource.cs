using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public class RootVerbsWordNetSource: BaseRootWordNetSource
    {
        public RootVerbsWordNetSource()
            : base(@"Resources\WordNet\dict\data.verb", 30)
        {
        }

        public List<RootVerbSourceWordItem> ReadAll()
        {
            var resultList = new List<RootVerbSourceWordItem>();

            Read((string currentLine) => {
#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read currentLine = {currentLine}");
#endif

                var item = ReaderOfRootVerbSourceWordItem.Read(currentLine);

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read item = {item}");
#endif

                resultList.Add(item);
            });

            return resultList;
        }
    }
}

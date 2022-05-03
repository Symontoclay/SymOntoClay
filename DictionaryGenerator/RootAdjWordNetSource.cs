using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public class RootAdjWordNetSource : BaseRootWordNetSource
    {
        public RootAdjWordNetSource()
            : base(@"Resources\WordNet\dict\data.adj", 30)
        {
        }

        public List<RootAdjSourceWordItem> ReadAll()
        {
            var resultList = new List<RootAdjSourceWordItem>();

            Read((string currentLine) => {
#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read currentLine = {currentLine}");
#endif

                var item = ReaderOfRootAdjSourceWordItem.Read(currentLine);

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read item = {item}");
#endif

                resultList.Add(item);
            });

            return resultList;
        }
    }
}

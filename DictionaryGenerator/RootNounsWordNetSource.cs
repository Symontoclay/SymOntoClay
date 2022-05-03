using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DictionaryGenerator
{
    public class RootNounsWordNetSource: BaseRootWordNetSource
    {
        public RootNounsWordNetSource()
            : base(@"Resources\WordNet\dict\data.noun", 30)
        {
        }

        public List<RootNounSourceWordItem> ReadAll()
        {
            var resultList = new List<RootNounSourceWordItem>();

            Read((string currentLine) => {
                var item = ReaderOfRootNounSourceWordItem.Read(currentLine);

#if DEBUG
                //if(item.Word == "elephant")
                //{
                //    NLog.LogManager.GetCurrentClassLogger().Info($"Read item = {item}");
                //}
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read item = {item}");
                //if(item.ParentWordNumsList.Count > 1)
                //{
                //    throw new NotImplementedException();
                //}
#endif

                resultList.Add(item);
            });

            return resultList;
        }
    }
}

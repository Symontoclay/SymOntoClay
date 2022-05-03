using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public abstract class BaseIrregularComparisonSource: BaseRootWordNetSource
    {
        public BaseIrregularComparisonSource(string localPath)
            : base(localPath, 0)
        {
        }

        public List<IrregularComparisonItem> ReadAll()
        {
            var resultList = new List<IrregularComparisonItem>();

            Read((string currentLine) => {
#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read currentLine = {currentLine}");
#endif

                var item = ReaderOfIrregularVerbItem.Read(currentLine);

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read item = {item}");
#endif

                resultList.Add(new IrregularComparisonItem() {
                    BaseWord = item.RootWord,
                    ComparativeForm = item.PastForm,
                    SuperlativeForm = item.ParticleForm
                });
            });

            return resultList;
        }
    }
}

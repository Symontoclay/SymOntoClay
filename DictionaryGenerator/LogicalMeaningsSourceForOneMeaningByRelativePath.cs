using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DictionaryGenerator
{
    public class LogicalMeaningsSourceForOneMeaningByRelativePath
    {
        public LogicalMeaningsSourceForOneMeaningByRelativePath(string localPath)
        {
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;

            var absolutePath = Path.Combine(rootPath, localPath);

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSourceForOneMeaningByRelativePath absolutePath = {absolutePath}");
#endif

            mInternalData = new LogicalMeaningsSourceForOneMeaning(absolutePath);
        }

        private LogicalMeaningsSourceForOneMeaning mInternalData;

        public bool ContainsWord(string word)
        {
            return mInternalData.ContainsWord(word);
        }
    }
}

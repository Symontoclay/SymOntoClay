using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DictionaryGenerator
{
    public class LogicalMeaningsSource
    {
        public LogicalMeaningsSource(string localPath)
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSource localPath = {localPath}");
#endif

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;

            var absolutePath = Path.Combine(rootPath, localPath);

#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSource absolutePath = {absolutePath}");
#endif

            if(Directory.Exists(absolutePath))
            {
                var filesList = Directory.GetFiles(absolutePath);

#if DEBUG
                NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSource filesList.Length = {filesList.Length}");
#endif

                foreach(var fileName in filesList)
                {
#if DEBUG
                    NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSource fileName = {fileName}");
#endif

                    var item = new LogicalMeaningsSourceForOneMeaning(fileName);
                    mSourcesList.Add(item);
                }
            }
        }

        private List<LogicalMeaningsSourceForOneMeaning> mSourcesList = new List<LogicalMeaningsSourceForOneMeaning>();

        public List<string> GetLogicalMeanings(string word)
        {
            var result = new List<string>();

            foreach(var item in mSourcesList)
            {
                if(item.ContainsWord(word))
                {
                    result.Add(item.LogicalMeaning);
                }
            }
            return result;
        }
    }
}

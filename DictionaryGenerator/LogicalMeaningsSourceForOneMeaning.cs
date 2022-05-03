using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DictionaryGenerator
{
    public class LogicalMeaningsSourceForOneMeaning
    {
        public LogicalMeaningsSourceForOneMeaning(string path)
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSourceForOneMeaning path = {path}");
#endif

            var fileInfo = new FileInfo(path);
            var fileName = fileInfo.Name;
            var extension = fileInfo.Extension;

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSourceForOneMeaning fileName = {fileName}");
#endif

            fileName = fileName.Replace(extension, string.Empty);

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSourceForOneMeaning NEXT fileName = {fileName}");
#endif

            LogicalMeaning = fileName;

            using (var fs = File.OpenRead(path))
            {
                using (var sr = new StreamReader(fs))
                {
                    var currentLine = string.Empty;

                    while ((currentLine = sr.ReadLine()) != null)
                    {
#if DEBUG
                        NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSourceForOneMeaning currentLine = {currentLine}");
#endif

                        if(string.IsNullOrWhiteSpace(currentLine))
                        {
                            continue;
                        }

                        mWordsList.Add(currentLine.Trim());
                    }
                }
            }
        }

        public string LogicalMeaning { get; private set; }
        private List<string> mWordsList = new List<string>();

        public bool ContainsWord(string word)
        {
            return mWordsList.Contains(word);
        }
    }
}

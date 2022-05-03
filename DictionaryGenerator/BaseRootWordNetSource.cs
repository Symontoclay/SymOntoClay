using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DictionaryGenerator
{
    public abstract class BaseRootWordNetSource
    {
        protected BaseRootWordNetSource(string localPath, int skipFirstLines)
        {
            mSkipFirstLines = skipFirstLines;

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"RootNounsWordNetSource rootPath = {rootPath}");
#endif

            mPath = Path.Combine(rootPath, localPath);

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"RootNounsWordNetSource mPath = {mPath}");
#endif
        }

        private string mPath;
        private int mSkipFirstLines = 30;

        protected void Read(Action<string> handler)
        {
            using (var fs = File.OpenRead(mPath))
            {
                using (var sr = new StreamReader(fs))
                {
                    var currentLine = string.Empty;

                    var n = 0;

                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        n++;

                        if (n < mSkipFirstLines)
                        {
                            continue;
                        }

#if DEBUG
                        //NLog.LogManager.GetCurrentClassLogger().Info($"RootNounsWordNetSource currentLine = {currentLine}");
#endif

                        handler(currentLine);

#if DEBUG
                        //if (n > 200)
                        //{
                        //    break;
                        //}
#endif
                    }
                }
            }
        }
    }
}

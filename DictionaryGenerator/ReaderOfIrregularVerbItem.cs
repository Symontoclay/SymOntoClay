using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public static class ReaderOfIrregularVerbItem
    {
        public static IrregularVerbItem Read(string source)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"Read source = {source}");
#endif

            var n = 0;

            var result = new IrregularVerbItem();

            var sb = new StringBuilder();
            var strValue = string.Empty;

            var lastCharNum = 0;

            foreach (var ch in source)
            {
                var charNum = (int)ch;

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read ch = {ch} charNum = {charNum}");
#endif

                if (charNum == 32)
                {
                    if (lastCharNum != charNum)
                    {
                        n++;
                    }

                    lastCharNum = charNum;

                    strValue = sb.ToString();

#if DEBUG
                    //NLog.LogManager.GetCurrentClassLogger().Info($"Read strValue = {strValue}");
#endif
                    switch (n)
                    {
                        case 1:
                            result.RootWord = strValue.Trim();
                            break;

                        case 2:
                            result.PastForm = strValue.Trim();
                            break;
                    }

                    sb = new StringBuilder();

                    continue;
                }


                lastCharNum = charNum;
                sb.Append(ch);
            }

            strValue = sb.ToString();

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"Read strValue = {strValue}");
#endif

            result.ParticleForm = strValue.Trim();

            return result;
        }
    }
}

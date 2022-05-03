using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public static class ReaderOfRootAdvSourceWordItem
    {
        public static RootAdvSourceWordItem Read(string source)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"Read source = {source}");
#endif

            var n = 0;

            var result = new RootAdvSourceWordItem();

            var sb = new StringBuilder();

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

                    var strValue = sb.ToString();

                    switch (n)
                    {
                        case 1:
                            {
                                var numOfWord = int.Parse(strValue);
                                result.WordNum = numOfWord;
                            }
                            break;

                        case 5:
                            result.Word = strValue.Replace("(a)", string.Empty).Replace("(p)", string.Empty).Trim();
                            break;
                    }

                    sb = new StringBuilder();

                    continue;
                }

                lastCharNum = charNum;
                sb.Append(ch);
            }

            ReaderOfRootSourceWordItemHelper.FillMetrixOfSourceWordNetItem(result);
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class ReaderOfExceptionCaseItem
    {
        public static List<ExceptionCaseItem> Read(string source)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"Read source = {source}");
#endif

            var n = 0;

            var result = new List<ExceptionCaseItem>();

            var sb = new StringBuilder();
            var strValue = string.Empty;

            var lastCharNum = 0;

            var exceptValue = string.Empty;

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
                            exceptValue = strValue.Trim();
                            break;

                        default:
                            {
                                var item = new ExceptionCaseItem();
                                item.RootWord = strValue.Trim();
                                item.ExceptWord = exceptValue;
                                result.Add(item);

#if DEBUG
                                //NLog.LogManager.GetCurrentClassLogger().Info($"Read item = {item}");
#endif
                            }
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

            {
                var item = new ExceptionCaseItem();
                item.RootWord = strValue.Trim();
                item.ExceptWord = exceptValue;
                result.Add(item);

#if DEBUG
                //NLog.LogManager.GetCurrentClassLogger().Info($"Read item = {item}");
#endif
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class ReaderOfRootVerbSourceWordItem
    {
        public static RootVerbSourceWordItem Read(string source)
        {
#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"Read source = {source}");
#endif

            var n = 0;

            var result = new RootVerbSourceWordItem();

            var sb = new StringBuilder();

            var lastCharNum = 0;
            var hasAtsign = false;

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

                        default:
                            if (n > 5)
                            {
                                if (hasAtsign)
                                {
                                    var numOfWord = int.Parse(strValue);
                                    result.ParentWordNumsList.Add(numOfWord);
                                    hasAtsign = false;
                                }
                                else
                                {
                                    if (strValue == "@")
                                    {
                                        hasAtsign = true;
                                    }
                                }
                            }
#if DEBUG
                            //NLog.LogManager.GetCurrentClassLogger().Info($"Read strValue = '{strValue}' n = {n}");
#endif
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

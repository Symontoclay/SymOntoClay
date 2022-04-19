using NLog;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.NLP
{
    public class TstSimpleWordsDict: IWordsDict
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public IList<WordFrame> GetWordFrames(string word)
        {
#if DEBUG
            _gbcLogger.Info($"word = {word}");
#endif

            throw new NotImplementedException();
        }
    }
}

using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal
{
    public class ATNLexer
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public ATNLexer(string text)
        {
            _lexer = new ATNStringLexer(text);
            InitTransformsDict();
        }

        private ATNStringLexer _lexer;
        private Queue<(string, int, int)> _recoveredSourceItems = new Queue<(string, int, int)> ();
        private Dictionary<string, List<string>> _transformsDict = new Dictionary<string, List<string>>();

        public void GetToken()
        {
#if DEBUG
            (string, int, int) item;
            while ((item = GetSourceItem()).Item1 != null)
            {
                _gbcLogger.Info($"item = {item}");
            }
#endif

            throw new NotImplementedException();
        }

        private (string, int, int) GetSourceItem()
        {
            if(_recoveredSourceItems.Count > 0)
            {
                return _recoveredSourceItems.Dequeue();
            }

            var item = _lexer.GetItem();

            var itemStrVal = item.Item1;

            if (itemStrVal == null)
            {
                return (null, 0, 0);
            }

            if(_transformsDict.ContainsKey(itemStrVal))
            {
                var targetList = _transformsDict[itemStrVal].ToList();

                var firstItem = targetList.First();

                targetList.Remove(firstItem);

                targetList.Reverse();

                foreach (var targetWord in targetList)
                {
                    _recoveredSourceItems.Enqueue((targetWord, item.Item2, item.Item3));
                }

                return (firstItem, item.Item2, item.Item3);
            }

            return item;
        }

        private void InitTransformsDict()
        {
            _transformsDict.Add("can't", new List<string>() { "can", "not" });
            _transformsDict.Add("cannot", new List<string>() { "can", "not" });
            _transformsDict.Add("couldn't", new List<string>() { "could", "not" });
            _transformsDict.Add("mayn't", new List<string>() { "may", "not" });
            _transformsDict.Add("mightn't", new List<string>() { "might", "not" });
            _transformsDict.Add("mustn't", new List<string>() { "must", "not" });
            _transformsDict.Add("haven't", new List<string>() { "have", "not" });
            _transformsDict.Add("don't", new List<string>() { "do", "not" });
            _transformsDict.Add("doesn't", new List<string>() { "does", "not" });
            _transformsDict.Add("didn't", new List<string>() { "did", "not" });
            _transformsDict.Add("isn't", new List<string>() { "is", "not" });
            _transformsDict.Add("shouldn't", new List<string>() { "should", "not" });
            _transformsDict.Add("wouldn't", new List<string>() { "would", "not" });
            _transformsDict.Add("oughtn't", new List<string>() { "ought", "not" });
            _transformsDict.Add("there's", new List<string>() { "there", "is" });
        }
    }
}

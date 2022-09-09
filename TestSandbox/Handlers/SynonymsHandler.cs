using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class SynonymsHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            Case1();

            _logger.Log("End");
        }

        private void Case1()
        {
            AddSynonym("to", "direction");
            AddSynonym("to", "somesynonim1");
            AddSynonym("direction", ":)");

            var synonymsList = GetSynonyms("to");

            _logger.Log($"synonymsList = {synonymsList.WritePODListToString()}");
        }

        private List<string> GetSynonyms(string word)
        {
            _logger.Log($"word = {word}");

            var result = new List<string>();

            var currentProcessedList = new List<string>() { word };

            while(currentProcessedList.Any())
            {
                var futureProcessedList = new List<string>();

                foreach(var processedItem in currentProcessedList)
                {
                    _logger.Log($"processedItem = {processedItem}");

                    if (result.Contains(processedItem))
                    {
                        continue;
                    }                    

                    _logger.Log($"NEXT processedItem = {processedItem}");

                    var synonymsList = GetSynonymsDirectly(processedItem);

                    _logger.Log($"synonymsList = {synonymsList.WritePODListToString()}");

                    if(synonymsList == null)
                    {
                        continue;
                    }

                    throw new NotImplementedException();
                }

                currentProcessedList = futureProcessedList;
            }

            return result;
        }

        private List<string> GetSynonymsDirectly(string word)
        {
            _logger.Log($"word = {word}");

            if(_synonymsDict.ContainsKey(word))
            {
                return _synonymsDict[word];
            }

            return null;
        }

        private readonly Dictionary<string, List<string>> _synonymsDict = new Dictionary<string, List<string>>();

        private void AddSynonym(string name, string obj)
        {
            AddWord(name, obj);
            AddWord(obj, name);
        }

        private void AddWord(string name, string obj)
        {
            if (_synonymsDict.ContainsKey(name))
            {
                var targetList = _synonymsDict[name];

                if (!targetList.Contains(obj))
                {
                    targetList.Add(obj);
                }

                return;
            }

            _synonymsDict[name] = new List<string>() { obj };
        }
    }
}

/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
        private static readonly IEntityLogger _logger = new LoggerNLogImpementation();

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

        private List<string> GetSynonyms(string name)
        {
            _logger.Log($"name = {name}");

            var result = new List<string>();

            var currentProcessedList = new List<string>() { name };

            while(currentProcessedList.Any())
            {
                var futureProcessedList = new List<string>();

                _logger.Log($"currentProcessedList = {currentProcessedList.WritePODListToString()}");

                foreach (var processedItem in currentProcessedList)
                {
                    _logger.Log($"processedItem = {processedItem}");

                    var synonymsList = GetSynonymsDirectly(processedItem);

                    _logger.Log($"synonymsList = {synonymsList.WritePODListToString()}");

                    if(synonymsList == null)
                    {
                        continue;
                    }

                    foreach(var item in synonymsList)
                    {
                        _logger.Log($"item = {item}");

                        if (item == name || result.Contains(item))
                        {
                            continue;
                        }

                        result.Add(item);
                        futureProcessedList.Add(item);
                    }
                }

                _logger.Log($"futureProcessedList = {futureProcessedList.WritePODListToString()}");

                currentProcessedList = futureProcessedList;
            }

            return result;
        }

        private List<string> GetSynonymsDirectly(string name)
        {
            _logger.Log($"name = {name}");

            if(_synonymsDict.ContainsKey(name))
            {
                return _synonymsDict[name];
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

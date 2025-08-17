/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System.Collections.Generic;
using System.Linq;

namespace TestSandbox.Handlers
{
    public class SynonymsHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        public void Run()
        {
            _logger.Info("A49F397C-8760-4F11-A909-2F9EC9F9B922", "Begin");

            Case1();

            _logger.Info("CC99B88C-E5C1-4FE9-95D2-46CBB1008979", "End");
        }

        private void Case1()
        {
            AddSynonym("to", "direction");
            AddSynonym("to", "somesynonim1");
            AddSynonym("direction", ":)");

            var synonymsList = GetSynonyms("to");

            _logger.Info("E97CB54E-95CA-491A-BF86-DB16302AFFD7", $"synonymsList = {synonymsList.WritePODListToString()}");
        }

        private List<string> GetSynonyms(string name)
        {
            _logger.Info("73697413-FFBB-4DBD-BB6D-50EA8C551FF2", $"name = {name}");

            var result = new List<string>();

            var currentProcessedList = new List<string>() { name };

            while(currentProcessedList.Any())
            {
                var futureProcessedList = new List<string>();

                _logger.Info("FFED62D7-BC9B-42E7-A272-17B3F1E17052", $"currentProcessedList = {currentProcessedList.WritePODListToString()}");

                foreach (var processedItem in currentProcessedList)
                {
                    _logger.Info("ACDE8739-08A4-4CA2-A1B1-CBDE5430844B", $"processedItem = {processedItem}");

                    var synonymsList = GetSynonymsDirectly(processedItem);

                    _logger.Info("332EC78A-162A-4B42-A360-D458DD296655", $"synonymsList = {synonymsList.WritePODListToString()}");

                    if(synonymsList == null)
                    {
                        continue;
                    }

                    foreach(var item in synonymsList)
                    {
                        _logger.Info("89CDC0B0-DD40-43E3-A49D-BEB483DA7932", $"item = {item}");

                        if (item == name || result.Contains(item))
                        {
                            continue;
                        }

                        result.Add(item);
                        futureProcessedList.Add(item);
                    }
                }

                _logger.Info("FCD1EB06-A13C-490E-AF68-7ADEE2818F2A", $"futureProcessedList = {futureProcessedList.WritePODListToString()}");

                currentProcessedList = futureProcessedList;
            }

            return result;
        }

        private List<string> GetSynonymsDirectly(string name)
        {
            _logger.Info("41D2EBE7-751A-4F60-B6C4-A653124CC648", $"name = {name}");

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

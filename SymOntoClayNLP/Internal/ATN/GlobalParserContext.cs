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

using SymOntoClay.Common;
using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class GlobalParserContext : IObjectToString
    {
        public GlobalParserContext(IMonitorLogger logger, IWordsDict wordsDict, string text)
            : this(logger, wordsDict, text, false, string.Empty)
        {
        }

        public GlobalParserContext(IMonitorLogger logger, IWordsDict wordsDict, string text, bool dumpToLogDirOnExit, string logDir)
        {
            _logger = logger;

            DumpToLogDirOnExit = dumpToLogDirOnExit;
            LogDir = logDir;

            AddContext(new ParserContext(this, logger, wordsDict, text));
        }

        private readonly IMonitorLogger _logger;
        public readonly bool DumpToLogDirOnExit;
        public readonly string LogDir;
        private readonly List<ParserContext> parserContexts = new List<ParserContext>();
        private readonly List<WholeTextParsingResult> _resultsList = new List<WholeTextParsingResult>();

        private int _contextNum;

        public int GetContextNum()
        {
            _contextNum++;

            return _contextNum;
        }

        public void Run()
        {
            while (true)
            {
                var itemsList = parserContexts.Where(p => p.IsActive).ToList();

                if(!itemsList.Any())
                {
                    break;
                }

                foreach(var item in itemsList)
                {
                    item.Run();
                }
#if DEBUG


#endif
            }
        }

        public void AddContext(ParserContext parserContext)
        {
            if(parserContexts.Contains(parserContext))
            {
                return;
            }

            parserContexts.Add(parserContext);
        }

        public void RemoveContext(ParserContext parserContext)
        {
            if (parserContexts.Contains(parserContext))
            {
                parserContexts.Remove(parserContext);
            }
        }

        public void AddResult(WholeTextParsingResult result)
        {
            _resultsList.Add(result);
        }
        
        public List<BaseSentenceItem> GetPhrases()
        {
            return _resultsList.Where(p => p.IsSuccess).FirstOrDefault().Results.Select(p => p.Phrase).ToList();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }
    }
}

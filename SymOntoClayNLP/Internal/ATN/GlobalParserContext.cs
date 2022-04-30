using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class GlobalParserContext : IObjectToString
    {
        public GlobalParserContext(INLPContext context, IWordsDict wordsDict, string text)
        {
            _logger = context.Logger;

            AddContext(new ParserContext(this, context, wordsDict, text));
        }

        private readonly IEntityLogger _logger;
        private readonly List<ParserContext> parserContexts = new List<ParserContext>();
        private readonly List<WholeTextParsingResult> _resultsList = new List<WholeTextParsingResult>();

        public void Run()
        {
#if DEBUG
            var n = 1;
#endif

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
                n++;

                _logger.Log($"n = {n}");

                if (n > 10)
                {
                    //break;
                }
#endif
            }
        }

        public void AddContext(ParserContext parserContext)
        {
#if DEBUG
            _logger.Log($"parserContext = {parserContext}");
#endif

            if(parserContexts.Contains(parserContext))
            {
                return;
            }

            parserContexts.Add(parserContext);
        }

        public void RemoveContext(ParserContext parserContext)
        {
#if DEBUG
            _logger.Log($"parserContext = {parserContext}");
#endif

            if (parserContexts.Contains(parserContext))
            {
                parserContexts.Remove(parserContext);
            }
        }

        public void AddResult(WholeTextParsingResult result)
        {
#if DEBUG
            _logger.Log($"result = {result}");
#endif

            _resultsList.Add(result);
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

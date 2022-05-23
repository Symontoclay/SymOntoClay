﻿using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.ATN;
using SymOntoClay.NLP.Internal.ConvertingCGToInternal;
using SymOntoClay.NLP.Internal.ConvertingFactToInternalCG;
using SymOntoClay.NLP.Internal.ConvertingInternalCGToFact;
using SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure;
using SymOntoClay.NLP.Internal.ConvertingPhraseStructureToText;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP
{
    public class NLPConverter: INLPConverter
    {
        public NLPConverter(IEntityLogger logger, IWordsDict wordsDict)
        {
            _wordsDict = wordsDict;
            _logger = logger;
        }

        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        /// <inheritdoc/>
        public IList<RuleInstance> Convert(string text)
        {
            var result = new List<RuleInstance>();

            var parser = new ATNParser(_logger, _wordsDict);

            var compactizer = new PhraseCompactizer(_logger, _wordsDict);

            var converterToPlainSentences = new ConverterToPlainSentences(_logger);

            var semanticAnalyzer = new SemanticAnalyzer(_logger, _wordsDict);

            var convertorCGToInternal = new ConvertorCGToInternal(_logger);

            var converterInternalCGToFact = new ConverterInternalCGToFact(_logger);

            var sentenceItemsList = parser.Run(text);

            foreach (var item in sentenceItemsList)
            {
                compactizer.Run(item);

                var plainSentencesList = converterToPlainSentences.Run(item);

                foreach (var plainSentence in plainSentencesList)
                {
                    var conceptualGraph = semanticAnalyzer.Run(plainSentence);

                    var internalCG = convertorCGToInternal.Convert(conceptualGraph);

                    var ruleInstancesList = converterInternalCGToFact.ConvertConceptualGraph(internalCG);

                    result.AddRange(ruleInstancesList);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public string Convert(RuleInstance fact, INLPConverterContext nlpContext)
        {
            var converterFactToCG = new ConverterFactToInternalCG(_logger);

            var internalCG = converterFactToCG.Convert(fact, nlpContext);

            var converterInternalCGToPhraseStructure = new ConverterInternalCGToPhraseStructure(_logger, _wordsDict);

            var sentenceItem = converterInternalCGToPhraseStructure.Convert(internalCG, nlpContext);

            var converterPhraseStructureToText = new ConverterPhraseStructureToText(_logger);

            var text = converterPhraseStructureToText.Convert(sentenceItem);

            return text;
        }
    }
}

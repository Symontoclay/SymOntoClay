using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class ConverterInternalCGToPhraseStructure
    {
        public ConverterInternalCGToPhraseStructure(IEntityLogger logger, IWordsDict wordsDict)
        {
            _wordsDict = wordsDict;
            _logger = logger;
        }

        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        public BaseSentenceItem Convert(InternalConceptualGraph source, INLPConverterContext nlpContext)
        {
            var context = new ContextOfConvertingInternalCGToPhraseStructure();
            context.Logger = _logger;
            context.WordsDict = _wordsDict;
            context.NLPContext = nlpContext;

            context.KindOfQuestion = source.KindOfQuestion;
            context.Tense = source.Tense;
            context.Aspect = source.Aspect;
            context.Voice = source.Voice;
            context.Mood = source.Mood;
            context.AbilityModality = source.AbilityModality;
            context.PermissionModality = source.PermissionModality;
            context.ObligationModality = source.ObligationModality;
            context.ProbabilityModality = source.ProbabilityModality;
            context.ConditionalModality = source.ConditionalModality;

            var sentenceNode = new SentenceNode(source, context);

            var nodeResult = sentenceNode.Run();

#if DEBUG
            _logger.Log($"nodeResult = {nodeResult}");
#endif            

            return nodeResult.SentenceItem;
        }
    }
}

using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ConverterFactToInternalCG
    {
        public ConverterFactToInternalCG(IEntityLogger logger)
        {
            _logger = logger;
        }

        private readonly IEntityLogger _logger;

        public InternalConceptualGraph Convert(RuleInstance fact, INLPConverterContext nlpContext)
        {
            var outerConceptualGraph = new InternalConceptualGraph();
            var context = new ContextOfConverterFactToInternalCG();
            context.ConceptualGraph = outerConceptualGraph;
            context.Logger = _logger;
            context.NLPContext = nlpContext;

            var factNode = new RuleInstanceNode(fact, context);
            var factNodeResult = factNode.Run();

#if DEBUG
            //_logger.Log($"factNodeResult = {factNodeResult}");
#endif

            outerConceptualGraph.Tense = GrammaticalTenses.Present;
            outerConceptualGraph.Aspect = GrammaticalAspect.Simple;
            outerConceptualGraph.Voice = GrammaticalVoice.Active;
            outerConceptualGraph.Mood = GrammaticalMood.Indicative;

#if DEBUG
            //_logger.Log($"outerConceptualGraph = {outerConceptualGraph}");
#endif

            return outerConceptualGraph;
        }
    }
}

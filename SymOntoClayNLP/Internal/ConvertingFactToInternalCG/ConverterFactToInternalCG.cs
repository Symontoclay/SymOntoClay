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

            outerConceptualGraph.KindOfQuestion = KindOfQuestion.None;//tmp
            outerConceptualGraph.Tense = GrammaticalTenses.Present;//tmp
            outerConceptualGraph.Aspect = GrammaticalAspect.Simple;//tmp
            outerConceptualGraph.Voice = GrammaticalVoice.Active;//tmp
            outerConceptualGraph.Mood = GrammaticalMood.Indicative;//tmp

            outerConceptualGraph.AbilityModality = AbilityModality.None;//tmp
            outerConceptualGraph.PermissionModality = PermissionModality.None;//tmp
            outerConceptualGraph.ObligationModality = ObligationModality.None;//tmp
            outerConceptualGraph.ProbabilityModality = ProbabilityModality.None;//tmp
            outerConceptualGraph.ConditionalModality = ConditionalModality.None;//tmp

#if DEBUG
            //_logger.Log($"outerConceptualGraph = {outerConceptualGraph}");
#endif

            return outerConceptualGraph;
        }
    }
}

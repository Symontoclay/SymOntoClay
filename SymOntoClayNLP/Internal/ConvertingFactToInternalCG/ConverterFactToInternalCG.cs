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

using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ConverterFactToInternalCG
    {
        public ConverterFactToInternalCG(IMonitorLogger logger)
        {
            _logger = logger;
        }

        private readonly IMonitorLogger _logger;

        public InternalConceptualGraph Convert(IMonitorLogger logger, RuleInstance fact, INLPConverterContext nlpContext)
        {
            var outerConceptualGraph = new InternalConceptualGraph();
            var context = new ContextOfConverterFactToInternalCG();
            context.ConceptualGraph = outerConceptualGraph;
            context.Logger = _logger;
            context.NLPContext = nlpContext;

            var factNode = new RuleInstanceNode(fact, context);
            var factNodeResult = factNode.Run(logger);

            outerConceptualGraph.KindOfQuestion = KindOfQuestion.None;//tmp
            outerConceptualGraph.Tense = GrammaticalTenses.Present;//tmp
            outerConceptualGraph.Aspect = GrammaticalAspect.Simple;//tmp
            outerConceptualGraph.Voice = GrammaticalVoice.Active;//tmp

            outerConceptualGraph.Mood = GrammaticalMood.Imperative;//tmp

            var logicalValueModalityResolver = nlpContext.LogicalValueModalityResolver;

            if (logicalValueModalityResolver.IsHigh(logger, fact.ObligationModality) || logicalValueModalityResolver.IsHigh(logger, fact.SelfObligationModality))
            {
                outerConceptualGraph.Mood = GrammaticalMood.Imperative;
                outerConceptualGraph.ObligationModality = ObligationModality.Imperative;
            }
            else
            {
                outerConceptualGraph.ObligationModality = ObligationModality.None;
                outerConceptualGraph.Mood = GrammaticalMood.Indicative;
            }

            outerConceptualGraph.AbilityModality = AbilityModality.None;//tmp
            outerConceptualGraph.PermissionModality = PermissionModality.None;//tmp
            outerConceptualGraph.ProbabilityModality = ProbabilityModality.None;//tmp
            outerConceptualGraph.ConditionalModality = ConditionalModality.None;//tmp

            return outerConceptualGraph;
        }

        public bool IsSubjectRelation(string name)
        {
            switch(name)
            {
                case SpecialNamesOfRelations.ExperiencerRelationName:
                case SpecialNamesOfRelations.AgentRelationName:
                    return true;              

                default:
                    return false;
            }
        }
    }
}

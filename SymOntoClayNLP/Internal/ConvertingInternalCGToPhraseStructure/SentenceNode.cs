using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class SentenceNode
    {
        public SentenceNode(InternalConceptualGraph source, ContextOfConvertingInternalCGToPhraseStructure context)
        {
            _context = context;
            _logger = context.Logger;
            _source = source;
        }

        public SentenceNode()
        {
        }

        private readonly ContextOfConvertingInternalCGToPhraseStructure _context;
        private readonly IEntityLogger _logger;
        private readonly InternalConceptualGraph _source;

        public ResultOfNode Run()
        {
#if DEBUG
            var dotStr = DotConverter.ConvertToString(_source);
            _logger.Log($"dotStr = '{dotStr}'");
#endif

            var mood = _context.Mood;

            switch(mood)
            {
                case GrammaticalMood.Indicative:
                    return ProcessIndicativeMood();

                default:
                    throw new ArgumentOutOfRangeException(nameof(mood), mood, null);
            }
        }

        private KeyConceptsOfSentenceNode GetKeyConcepts()
        {
            var statesList = _source.Children.Where(p => p.Kind == KindOfCGNode.Relation && p.Name == SpecialNamesOfRelations.StateRelationName).Select(p => p.AsRelationNode).ToList();

            if (statesList.Any())
            {
                if (statesList.Count > 1)
                {
                    throw new NotImplementedException();
                }

                var stateRelation = statesList.First();

#if DEBUG
                _logger.Log($"stateRelation = {stateRelation}");
#endif

                _context.VisitedRelations.Add(stateRelation);

                if (stateRelation.Inputs.Count != 1)
                {
                    throw new NotImplementedException();
                }

                if (stateRelation.Outputs.Count != 1)
                {
                    throw new NotImplementedException();
                }

                var subjectConcept = stateRelation.Inputs.First();

#if DEBUG
                _logger.Log($"subjectConcept = {subjectConcept}");
#endif

                var experiencerRelation = subjectConcept.Inputs.Where(p => p.Name == SpecialNamesOfRelations.ExperiencerRelationName).Select(p => p.AsRelationNode).Single();

#if DEBUG
                _logger.Log($"experiencerRelation = {experiencerRelation}");
#endif

                _context.VisitedRelations.Add(experiencerRelation);

                var stateConcept = stateRelation.Outputs.First();

#if DEBUG
                _logger.Log($"stateConcept = {stateConcept}");
#endif

                return new KeyConceptsOfSentenceNode()
                { 
                    SubjectConcept = subjectConcept.AsGraphOrConceptNode,
                    VerbConcept = stateConcept.AsConceptNode
                };
            }

            throw new NotImplementedException();
        }

        private ResultOfNode ProcessIndicativeMood()
        {
            var keyConcepts = GetKeyConcepts();

#if DEBUG
            _logger.Log($"keyConcepts = {keyConcepts}");
#endif

            var subjectNode = new NounNode(keyConcepts.SubjectConcept, RoleOfNoun.Subject, _context);
            var subjectResult = subjectNode.Run();

#if DEBUG
            _logger.Log($"subjectResult = {subjectResult}");
#endif

            var verbNode = new VerbNode(keyConcepts.VerbConcept, subjectResult.SentenceItem, _context);

            var verbResult = verbNode.Run();

#if DEBUG
            _logger.Log($"verbResult = {verbResult}");
#endif

#if DEBUG
            _logger.Log($"subjectResult.SentenceItem = {subjectResult.SentenceItem.ToDbgString()}");
            _logger.Log($"verbResult.SentenceItem = {verbResult.SentenceItem.ToDbgString()}");
#endif

            var sentence = new Sentence();
            sentence.Subject = subjectResult.SentenceItem;
            sentence.Predicate = verbResult.SentenceItem;

#if DEBUG
            _logger.Log($"sentence = {sentence}");
            _logger.Log($"sentence = {sentence.ToDbgString()}");
#endif

            return new ResultOfNode()
            {
                SentenceItem = sentence
            };
        }
    }
}

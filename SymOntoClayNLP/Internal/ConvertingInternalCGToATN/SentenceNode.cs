using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToATN
{
    public class SentenceNode
    {
        public SentenceNode(InternalConceptualGraph source, ContextOfConvertingInternalCGToText context)
        {
            _context = context;
            _logger = context.Logger;
            _source = source;
        }

        public SentenceNode()
        {
        }

        private readonly ContextOfConvertingInternalCGToText _context;
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

                var stateConcept = stateRelation.Outputs.First();

#if DEBUG
                _logger.Log($"stateConcept = {stateConcept}");
#endif

                var disabledRelations = new List<string>() { "experiencer", "state" };

                return new KeyConceptsOfSentenceNode()
                { 
                    SubjectConcept = subjectConcept.AsGraphOrConceptNode,
                    DisabledSubjectRelations = disabledRelations,
                    VerbConcept = stateConcept.AsConceptNode,
                    DisabledVerbRelations = disabledRelations
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

            var subjectNode = new NounNode(keyConcepts.SubjectConcept, keyConcepts.DisabledSubjectRelations, _context);
            var subjectResult = subjectNode.Run();

#if DEBUG
            _logger.Log($"subjectResult = {subjectResult}");
#endif

            var verbNode = new VerbNode(keyConcepts.VerbConcept, keyConcepts.DisabledVerbRelations, subjectResult.RootWord, _context);

            var verbResult = verbNode.Run();

#if DEBUG
            _logger.Log($"verbResult = {verbResult}");
#endif

            throw new NotImplementedException();
        }
    }
}

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToText
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

        private ResultOfNode ProcessIndicativeMood()
        {
            var statesList = _source.Children.Where(p => p.Kind == KindOfCGNode.Relation && p.Name == SpecialNamesOfRelations.StateRelationName).Select(p => p.AsRelationNode).ToList();

            if (statesList.Any())
            {
                return ProcessIndicativeState(statesList);
            }

            throw new NotImplementedException();
        }

        private ResultOfNode ProcessIndicativeState(List<InternalRelationCGNode> statesList)
        {
            if(statesList.Count > 1)
            {
                throw new NotImplementedException();
            }

            var stateRelation = statesList.First();

#if DEBUG
            _logger.Log($"stateRelation = {stateRelation}");
#endif

            if(stateRelation.Inputs.Count != 1)
            {
                throw new NotImplementedException();
            }

            if(stateRelation.Outputs.Count != 1)
            {
                throw new NotImplementedException();
            }

            var subjectConcept = stateRelation.Inputs.First();

#if DEBUG
            _logger.Log($"subjectConcept = {subjectConcept}");
#endif

            var subjectNode = new NounNode(subjectConcept.AsGraphOrConceptNode, new List<string>() { "experiencer", "state" }, _context);
            var subjectResult = subjectNode.Run();

#if DEBUG
            _logger.Log($"subjectResult = {subjectResult}");
#endif

            var stateConcept = stateRelation.Outputs.First();

#if DEBUG
            _logger.Log($"stateConcept = {stateConcept}");
#endif

            var verbNode = new VerbNode(stateConcept.AsConceptNode, new List<string>() { "experiencer", "state" }, subjectResult.RootWord, _context);

            var verbResult = verbNode.Run();

#if DEBUG
            _logger.Log($"verbResult = {verbResult}");
#endif

            throw new NotImplementedException();
        }
    }
}

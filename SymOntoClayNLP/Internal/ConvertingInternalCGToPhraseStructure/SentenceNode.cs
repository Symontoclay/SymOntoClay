/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
            //var dotStr = DotConverter.ConvertToString(_source);
            //_logger.Log($"dotStr = '{dotStr}'");
#endif

            var mood = _context.Mood;

#if DEBUG
            //_logger.Log($"mood = {mood}");
#endif

            switch (mood)
            {
                case GrammaticalMood.Indicative:
                    return ProcessIndicativeMood();

                case GrammaticalMood.Imperative:
                    return ProcessImperativeMood();

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
                //_logger.Log($"stateRelation = {stateRelation}");
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
                //_logger.Log($"subjectConcept = {subjectConcept}");
#endif

                var experiencerRelation = subjectConcept.Inputs.Where(p => p.Name == SpecialNamesOfRelations.ExperiencerRelationName).Select(p => p.AsRelationNode).Single();

#if DEBUG
                //_logger.Log($"experiencerRelation = {experiencerRelation}");
#endif

                _context.VisitedRelations.Add(experiencerRelation);

                var stateConcept = stateRelation.Outputs.First();

#if DEBUG
                //_logger.Log($"stateConcept = {stateConcept}");
#endif

                return new KeyConceptsOfSentenceNode()
                { 
                    SubjectConcept = subjectConcept.AsGraphOrConceptNode,
                    VerbConcept = stateConcept.AsConceptNode
                };
            }

            var actionsList = _source.Children.Where(p => p.Kind == KindOfCGNode.Relation && p.Name == SpecialNamesOfRelations.ActionRelationName).Select(p => p.AsRelationNode).ToList();

            if(actionsList.Any())
            {
                if (actionsList.Count > 1)
                {
                    throw new NotImplementedException();
                }

                var actionRelation = actionsList.First();

#if DEBUG
                //_logger.Log($"actionRelation = {actionRelation}");
#endif

                _context.VisitedRelations.Add(actionRelation);

                if (actionRelation.Inputs.Count != 1)
                {
                    throw new NotImplementedException();
                }

                if (actionRelation.Outputs.Count != 1)
                {
                    throw new NotImplementedException();
                }

                var subjectConcept = actionRelation.Inputs.First();

#if DEBUG
                //_logger.Log($"subjectConcept = {subjectConcept}");
#endif

                var agentRelation = subjectConcept.Inputs.Where(p => p.Name == SpecialNamesOfRelations.AgentRelationName).Select(p => p.AsRelationNode).Single();

#if DEBUG
                //_logger.Log($"agentRelation = {agentRelation}");
#endif

                _context.VisitedRelations.Add(agentRelation);

                var actionConcept = actionRelation.Outputs.First();

#if DEBUG
                //_logger.Log($"actionConcept = {actionConcept}");
#endif

                return new KeyConceptsOfSentenceNode()
                {
                    SubjectConcept = subjectConcept.AsGraphOrConceptNode,
                    VerbConcept = actionConcept.AsConceptNode
                };
            }

            throw new NotImplementedException();
        }

        private ResultOfNode ProcessIndicativeMood()
        {
            var keyConcepts = GetKeyConcepts();

#if DEBUG
            //_logger.Log($"keyConcepts = {keyConcepts}");
#endif

            var subjectNode = new NounNode(keyConcepts.SubjectConcept, RoleOfNoun.Subject, _context);
            var subjectResult = subjectNode.Run();

#if DEBUG
            //_logger.Log($"subjectResult = {subjectResult}");
#endif

            var verbNode = new VerbNode(keyConcepts.VerbConcept, subjectResult.SentenceItem, _context);

            var verbResult = verbNode.Run();

#if DEBUG
            //_logger.Log($"verbResult = {verbResult}");
#endif

#if DEBUG
            //_logger.Log($"subjectResult.SentenceItem = {subjectResult.SentenceItem.ToDbgString()}");
            //_logger.Log($"verbResult.SentenceItem = {verbResult.SentenceItem.ToDbgString()}");
#endif

            var sentence = new Sentence();
            sentence.Subject = subjectResult.SentenceItem;
            sentence.Predicate = verbResult.SentenceItem;

#if DEBUG
            //_logger.Log($"sentence = {sentence}");
            //_logger.Log($"sentence = {sentence.ToDbgString()}");
#endif

            return new ResultOfNode()
            {
                SentenceItem = sentence
            };
        }

        private ResultOfNode ProcessImperativeMood()
        {
            var keyConcepts = GetKeyConcepts();

#if DEBUG
            //_logger.Log($"keyConcepts = {keyConcepts}");
#endif

            var verbNode = new VerbNode(keyConcepts.VerbConcept, null, _context);

            var verbResult = verbNode.Run();

#if DEBUG
            //_logger.Log($"verbResult = {verbResult}");
#endif

            var sentence = new Sentence();
            sentence.Predicate = verbResult.SentenceItem;

#if DEBUG
            //_logger.Log($"sentence = {sentence}");
            //_logger.Log($"sentence = {sentence.ToDbgString()}");
#endif

            return new ResultOfNode()
            {
                SentenceItem = sentence
            };
        }
    }
}

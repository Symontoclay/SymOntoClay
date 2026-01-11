/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class ConditionalEntityNode
    {
        public ConditionalEntityNode(InternalConceptualGraph source, RoleOfNoun roleOfNoun, BaseContextOfConvertingInternalCGToPhraseStructure baseContext)
        {
            _baseContext = baseContext;
            _source = source;
            _logger = baseContext.Logger;
            _wordsDict = baseContext.WordsDict;
            _nlpContext = baseContext.NLPContext;
            _visitedRelations = baseContext.VisitedRelations;
            _roleOfNoun = roleOfNoun;
        }

        private readonly BaseContextOfConvertingInternalCGToPhraseStructure _baseContext;
        private readonly InternalConceptualGraph _source;
        private readonly IMonitorLogger _logger;
        private readonly IWordsDict _wordsDict;
        private readonly INLPConverterContext _nlpContext;
        private readonly List<InternalRelationCGNode> _visitedRelations;
        private readonly RoleOfNoun _roleOfNoun;

        public ResultOfNode Run()
        {
            var context = new ContextOfConditionalEntityNode();
            
            FindRootConcept(context);
            ProcessRelationsOfConcept(context.RootConcept, context.NounPhrase, context);
            ProcessAnotherRelations(context);

            return new ResultOfNode()
            {
                SentenceItem = context.NounPhrase
            };
        }

        private void FindRootConcept(ContextOfConditionalEntityNode context)
        {
            var rootConcept = _source.Children.Where(p => p.IsConceptNode).Select(p => p.AsConceptNode).First(p => p.IsRootConceptOfEntitiCondition);

            context.RootConcept = rootConcept;

            var convertingContext = new ContextOfConvertingInternalCGToPhraseStructure()
            {
                Logger = _logger,
                NLPContext = _nlpContext,
                WordsDict = _wordsDict
            };

            var nounWordNode = new NounWordNode(rootConcept.Name, _roleOfNoun, _logger, _wordsDict, GrammaticalMood.Undefined);
            var nounPhrase = nounWordNode.GetNounPhrase();

            context.NounPhrase = nounPhrase;

            context.PhrasesDict[rootConcept] = nounPhrase;
        }

        private void ProcessRelationsOfConcept(InternalConceptCGNode targetConcept, NounPhrase targetNounPhrase, ContextOfConditionalEntityNode context)
        {
            var inputRelationsList = targetConcept.Inputs.Select(p => p.AsRelationNode).ToList();

            foreach(var inputRelation in inputRelationsList)
            {
                if(_visitedRelations.Contains(inputRelation))
                {
                    continue;
                }

                _visitedRelations.Add(inputRelation);

                var relationName = inputRelation.Name;

                switch(relationName)
                {
                    case "possess":
                        {
                            var ownerConcept = inputRelation.Inputs.Select(p => p.AsGraphOrConceptNode).Single();

                            var nounNode = new NounNode(ownerConcept, RoleOfNoun.PossessDeterminer, _baseContext);

                            var nounNodeResult = nounNode.Run();

                            targetNounPhrase.D = nounNodeResult.SentenceItem;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(relationName), relationName, null);
                }
            }

        }

        private void ProcessAnotherRelations(ContextOfConditionalEntityNode context)
        {
            var anotherRelationsList = _source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).Where(p => !_visitedRelations.Contains(p)).ToList();

            if (!anotherRelationsList.Any())
            {
                return;
            }

            foreach(var relation in anotherRelationsList)
            {
                if (_visitedRelations.Contains(relation))
                {
                    continue;
                }

                _visitedRelations.Add(relation);

                var relationName = relation.Name;

                switch (relationName)
                {
                    case "color":
                        {
                            var objConcept = relation.Inputs.First().AsGraphOrConceptNode;

                            var nounPhrase = context.PhrasesDict[objConcept].AsNounPhrase;

                            var colorConcept = relation.Outputs.First().AsGraphOrConceptNode;

                            var adjectiveNode = new AdjectiveNode(colorConcept, _baseContext);

                            var adjectiveResult = adjectiveNode.Run();

                            nounPhrase.AP = adjectiveResult.SentenceItem;

                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(relationName), relationName, null);
                }
            }
        }
    }
}

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

using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
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
        private readonly IEntityLogger _logger;
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

#if DEBUG
            //_logger.Log($"context.NounPhrase = {context.NounPhrase}");
            //_logger.Log($"context.NounPhrase = {context.NounPhrase.ToDbgString()}");
#endif

            return new ResultOfNode()
            {
                SentenceItem = context.NounPhrase
            };
        }

        private void FindRootConcept(ContextOfConditionalEntityNode context)
        {
            var rootConcept = _source.Children.Where(p => p.IsConceptNode).Select(p => p.AsConceptNode).First(p => p.IsRootConceptOfEntitiCondition);

#if DEBUG
            //_logger.Log($"rootConcept = {rootConcept}");
#endif

            context.RootConcept = rootConcept;

            var convertingContext = new ContextOfConvertingInternalCGToPhraseStructure()
            {
                Logger = _logger,
                NLPContext = _nlpContext,
                WordsDict = _wordsDict
            };

            var nounWordNode = new NounWordNode(rootConcept.Name, _roleOfNoun, _logger, _wordsDict, GrammaticalMood.Undefined);
            var nounPhrase = nounWordNode.GetNounPhrase();

#if DEBUG
            //_logger.Log($"nounPhrase = {nounPhrase}");
#endif

            context.NounPhrase = nounPhrase;

            context.PhrasesDict[rootConcept] = nounPhrase;
        }

        private void ProcessRelationsOfConcept(InternalConceptCGNode targetConcept, NounPhrase targetNounPhrase, ContextOfConditionalEntityNode context)
        {
            var inputRelationsList = targetConcept.Inputs.Select(p => p.AsRelationNode).ToList();

#if DEBUG
            //_logger.Log($"inputRelationsList = {inputRelationsList.WriteListToString()}");
#endif

            foreach(var inputRelation in inputRelationsList)
            {
#if DEBUG
                //_logger.Log($"inputRelation = {inputRelation}");
#endif

                if(_visitedRelations.Contains(inputRelation))
                {
                    continue;
                }

                _visitedRelations.Add(inputRelation);

                var relationName = inputRelation.Name;

#if DEBUG
                //_logger.Log($"relationName = '{relationName}'");
#endif

                switch(relationName)
                {
                    case "possess":
                        {
                            var ownerConcept = inputRelation.Inputs.Select(p => p.AsGraphOrConceptNode).Single();

#if DEBUG
                            //_logger.Log($"ownerConcept = {ownerConcept}");
#endif

                            var nounNode = new NounNode(ownerConcept, RoleOfNoun.PossessDeterminer, _baseContext);

                            var nounNodeResult = nounNode.Run();

#if DEBUG
                            //_logger.Log($"nounNodeResult = {nounNodeResult}");
#endif

                            targetNounPhrase.D = nounNodeResult.SentenceItem;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(relationName), relationName, null);
                }
            }

#if DEBUG
            //_logger.Log($"targetNounPhrase = {targetNounPhrase.ToDbgString()}");
#endif
        }

        private void ProcessAnotherRelations(ContextOfConditionalEntityNode context)
        {
            var anotherRelationsList = _source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).Where(p => !_visitedRelations.Contains(p)).ToList();

#if DEBUG
            //_logger.Log($"anotherRelationsList = {anotherRelationsList.WriteListToString()}");
#endif

            if (!anotherRelationsList.Any())
            {
                return;
            }

            foreach(var relation in anotherRelationsList)
            {
#if DEBUG
                //_logger.Log($"relation = {relation}");
#endif

                if (_visitedRelations.Contains(relation))
                {
                    continue;
                }

                _visitedRelations.Add(relation);

                var relationName = relation.Name;

#if DEBUG
                //_logger.Log($"relationName = '{relationName}'");
#endif

                switch (relationName)
                {
                    case "color":
                        {
#if DEBUG
                            //_logger.Log($"context.NounPhrase = {context.NounPhrase}");
                            //_logger.Log($"context.RootConcept = {context.RootConcept}");
#endif

                            var objConcept = relation.Inputs.First().AsGraphOrConceptNode;

#if DEBUG
                            //_logger.Log($"objConcept = {objConcept}");
#endif

                            var nounPhrase = context.PhrasesDict[objConcept].AsNounPhrase;

#if DEBUG
                            //_logger.Log($"nounPhrase = {nounPhrase}");
                            //_logger.Log($"nounPhrase = {nounPhrase.ToDbgString()}");
#endif

                            var colorConcept = relation.Outputs.First().AsGraphOrConceptNode;

#if DEBUG
                            //_logger.Log($"colorConcept = {colorConcept}");
#endif

                            var adjectiveNode = new AdjectiveNode(colorConcept, _baseContext);

                            var adjectiveResult = adjectiveNode.Run();

#if DEBUG
                            //_logger.Log($"adjectiveResult = {adjectiveResult}");
                            //_logger.Log($"adjectiveResult.SentenceItem = {adjectiveResult.SentenceItem.ToDbgString()}");
#endif

                            nounPhrase.AP = adjectiveResult.SentenceItem;

#if DEBUG
                            //_logger.Log($"nounPhrase (1) = {nounPhrase}");
                            //_logger.Log($"nounPhrase (1) = {nounPhrase.ToDbgString()}");
#endif
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(relationName), relationName, null);
                }
            }
        }
    }
}

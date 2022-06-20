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

using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class NounPhraseNodeOfSemanticAnalyzer: BaseNodeOfSemanticAnalyzer
    {
        public NounPhraseNodeOfSemanticAnalyzer(ContextOfSemanticAnalyzer context, NounPhrase nounPhrase)
            : base(context)
        {
            _nounPhrase = nounPhrase;
        }

        private NounPhrase _nounPhrase;
        private ConceptCGNode _concept;

        public ResultOfNodeOfSemanticAnalyzer Run()
        {
#if DEBUG
            //Context.Logger.Log($"_nounPhrase = {_nounPhrase}");
#endif

            var result = new ResultOfNodeOfSemanticAnalyzer();            
            var resultSecondaryRolesDict = result.SecondaryRolesDict;
            var conceptualGraph = Context.ConceptualGraph;

            if (_nounPhrase.N == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                ProcessNAsBaseSentenceItem(_nounPhrase.N, result);
            }

            if (_nounPhrase.D != null)
            {
                ProcessDAsBaseSentenceItem(_nounPhrase.D, result);
            }

            if (_nounPhrase.QP != null)
            {
                throw new NotImplementedException();
            }

            if (_nounPhrase.AP != null)
            {
                ProcessAPAsBaseSentenceItem(_nounPhrase.AP, result);
            }

            if (_nounPhrase.NounAdjunct != null)
            {
                throw new NotImplementedException();
            }

            if (_nounPhrase.Negation != null)
            {
                throw new NotImplementedException();
            }

            if (_nounPhrase.HasPossesiveMark)
            {
                throw new NotImplementedException();
            }

            if (_nounPhrase.PP != null)
            {
                throw new NotImplementedException();
            }

            if (_nounPhrase.AdvP != null)
            {
                throw new NotImplementedException();
            }

            if (_nounPhrase.RelativeClauses != null)
            {
                throw new NotImplementedException();
            }

            if (_nounPhrase.OtherClauses != null)
            {
                throw new NotImplementedException();
            }

            if (_nounPhrase.InfinitivePhrases != null)
            {
                throw new NotImplementedException();
            }

            
            //            var ajectivesList = _nounPhrase.AdjectivePhrasesList;



            //            if (!ajectivesList.IsEmpty())
            //            {
            //#if DEBUG
            //                Context.Logger.Log($"ajectivesList.Count = {ajectivesList.Count}");
            //#endif

            //                foreach (var ajective in ajectivesList)
            //                {
            //#if DEBUG
            //                    Context.Logger.Log($"ajective = {ajective}");
            //#endif

            //                    var ajectiveNode = new AjectivePhraseNodeOfSemanticAnalyzer(Context, _sentence, ajective);
            //                    var ajectiveNodeResult = ajectiveNode.Run();

            //#if DEBUG
            //                    Context.Logger.Log($"ajectiveNodeResult = {ajectiveNodeResult}");
            //#endif
            //                    var role = ajective.Adjective.LogicalMeaning.FirstOrDefault();

            //#if DEBUG
            //                    Context.Logger.Log($"role = {role}");
            //#endif

            //                    if (!string.IsNullOrWhiteSpace(role))
            //                    {
            //                        var ajectiveConcept = ajectiveNodeResult.RootConcept;

            //#if DEBUG
            //                        Context.Logger.Log($"ajectiveConcept = {ajectiveConcept.ToBriefString()}");
            //#endif

            //                        var ajectiveRelation = new RelationCGNode();
            //                        ajectiveRelation.Parent = conceptualGraph;
            //                        ajectiveRelation.Name = role;

            //                        MarkAsEntityCondition(ajectiveRelation);

            //                        ajectiveRelation.AddInputNode(_concept);
            //                        ajectiveRelation.AddOutputNode(ajectiveConcept);
            //                    }
            //                }
            //            }

            //            var additionalInfoList = _nounPhrase.AdditionalInfoList;

            //            if (!additionalInfoList.IsEmpty())
            //            {
            //                throw new NotImplementedException();
            //            }

            //            var possesiveList = _nounPhrase.PossesiveList;

            //            if (!possesiveList.IsEmpty())
            //            {
            //                throw new NotImplementedException();
            //            }

#if DEBUG
            //Context.Logger.Log($"PrimaryRolesDict = {PrimaryRolesDict}");
            //Context.Logger.Log("End");
#endif

            return result;
        }

        private void ProcessNAsBaseSentenceItem(BaseSentenceItem sentenceItem, ResultOfNodeOfSemanticAnalyzer result)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //Context.Logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.Word:
                    ProcessNAsWord(sentenceItem.AsWord, result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessNAsWord(Word word, ResultOfNodeOfSemanticAnalyzer result)
        {
#if DEBUG
            //Context.Logger.Log($"word = {word}");
#endif

            var conceptualGraph = Context.ConceptualGraph;

            var baseGrammaticalWordFrame = word.WordFrame;

            if (baseGrammaticalWordFrame.IsNoun && baseGrammaticalWordFrame.AsNoun.IsName)
            {
                _concept = new ConceptCGNode();
                result.RootConcept = _concept;
                _concept.Parent = conceptualGraph;
                _concept.Name = "entity";

                var nameConcept = new ConceptCGNode();
                nameConcept.Parent = conceptualGraph;
                nameConcept.Name = word.RootWordAsString;

                var nameRelation = new RelationCGNode();
                nameRelation.Parent = conceptualGraph;
                nameRelation.Name = "name";

                _concept.AddOutputNode(nameRelation);
                nameRelation.AddOutputNode(nameConcept);

                Context.RelationStorage.AddRelation(_concept.Name, nameConcept.Name, nameRelation.Name);

                MarkAsEntityCondition(nameRelation);
            }
            else
            {
                _concept = new ConceptCGNode();
                result.RootConcept = _concept;
                _concept.Parent = conceptualGraph;
                _concept.Name = word.RootWordAsString;
            }

            var nounFullLogicalMeaning = baseGrammaticalWordFrame.FullLogicalMeaning;

            if (!nounFullLogicalMeaning.IsNullOrEmpty())
            {
                var resultPrimaryRolesDict = result.PrimaryRolesDict;

                foreach (var logicalMeaning in nounFullLogicalMeaning)
                {
#if DEBUG
                    //Context.Logger.Log($"logicalMeaning = {logicalMeaning}");
#endif

                    PrimaryRolesDict.Add(logicalMeaning, _concept);
                    resultPrimaryRolesDict.Add(logicalMeaning, _concept);
                }
            }
        }

        private void ProcessDAsBaseSentenceItem(BaseSentenceItem sentenceItem, ResultOfNodeOfSemanticAnalyzer result)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //Context.Logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.Word:
                    ProcessDAsWord(sentenceItem.AsWord, result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessDAsWord(Word word, ResultOfNodeOfSemanticAnalyzer result)
        {
#if DEBUG
            //Context.Logger.Log($"word = {word}");
#endif

            CreateDeterminerMark(_concept, word);
        }

        private void CreateDeterminerMark(ConceptCGNode concept, Word determiner)
        {
            var relationName = CGGramamaticalNamesOfRelations.DeterminerName;

            if (IsPossesDeterminer(determiner))
            {
                CreatePossessMark(concept, determiner);
                return;
            }

#if DEBUG
            //Context.Logger.Log($"relationName = {relationName}");
#endif

            var determinerConceptName = determiner.RootWordAsString;

#if DEBUG
            //Context.Logger.Log($"determinerConceptName = {determinerConceptName}");
#endif

            if (Context.RelationStorage.ContainsRelation(concept.Name, determinerConceptName, relationName))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var determinerConcept = new ConceptCGNode();
            determinerConcept.Parent = conceptualGraph;
            determinerConcept.Name = determinerConceptName;

            var determinerRelation = new RelationCGNode();
            determinerRelation.Parent = conceptualGraph;
            determinerRelation.Name = relationName;

            concept.AddOutputNode(determinerRelation);
            determinerRelation.AddOutputNode(determinerConcept);

            Context.RelationStorage.AddRelation(concept.Name, determinerConceptName, relationName);

            MarkAsEntityCondition(determinerRelation);
        }

        private void CreatePossessMark(ConceptCGNode concept, Word determiner)
        {
            var relationName = SpecialNamesOfRelations.PossessName;

#if DEBUG
            //Context.Logger.Log($"relationName = {relationName}");
#endif

            var determinerConceptName = determiner.RootWordAsString;

#if DEBUG
            //Context.Logger.Log($"determinerConceptName = {determinerConceptName}");
#endif

            if (Context.RelationStorage.ContainsRelation(determinerConceptName, concept.Name, relationName))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var determinerConcept = new ConceptCGNode();
            determinerConcept.Parent = conceptualGraph;
            determinerConcept.Name = determinerConceptName;

            var determinerRelation = new RelationCGNode();
            determinerRelation.Parent = conceptualGraph;
            determinerRelation.Name = relationName;

            determinerConcept.AddOutputNode(determinerRelation);
            determinerRelation.AddOutputNode(concept);

            Context.RelationStorage.AddRelation(determinerConceptName, concept.Name, relationName);

            MarkAsEntityCondition(determinerRelation);

            //throw new NotImplementedException();
        }

        public bool IsPossesDeterminer(Word determiner)
        {
            switch(determiner.Content)
            {
                case "my":
                case "our":
                case "your":
                case "his":
                case "her":
                case "its":
                case "their":
                    return true;

                default:
                    return false;
            }
        }

        private void ProcessAPAsBaseSentenceItem(BaseSentenceItem sentenceItem, ResultOfNodeOfSemanticAnalyzer result)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //Context.Logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.Word:
                    ProcessAPAsWord(sentenceItem.AsWord, result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessAPAsWord(Word word, ResultOfNodeOfSemanticAnalyzer result)
        {
#if DEBUG
            //Context.Logger.Log($"word = {word}");
#endif

            var role = word.WordFrame.LogicalMeaning.FirstOrDefault();

#if DEBUG
            //Context.Logger.Log($"role = {role}");
#endif

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new NotImplementedException();
            }

            var ajectiveConceptName = word.RootWordAsString;

#if DEBUG
            //Context.Logger.Log($"ajectiveConceptName = {ajectiveConceptName}");
#endif

            if (Context.RelationStorage.ContainsRelation(ajectiveConceptName, _concept.Name, role))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var ajectiveConcept = new ConceptCGNode();
            ajectiveConcept.Parent = conceptualGraph;
            ajectiveConcept.Name = ajectiveConceptName;

            var ajectiveRelation = new RelationCGNode();
            ajectiveRelation.Parent = conceptualGraph;
            ajectiveRelation.Name = role;

            MarkAsEntityCondition(ajectiveRelation);

            ajectiveRelation.AddInputNode(_concept);
            ajectiveRelation.AddOutputNode(ajectiveConcept);

            Context.RelationStorage.AddRelation(ajectiveConceptName, _concept.Name, role);
        }
    }
}

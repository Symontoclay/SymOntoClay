using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
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
                throw new NotImplementedException();
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
                nameConcept.Name = GetName(word);

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
                _concept.Name = GetName(word);
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

            var determinerConceptName = GetName(determiner);

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

            var determinerConceptName = GetName(determiner);

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
    }
}

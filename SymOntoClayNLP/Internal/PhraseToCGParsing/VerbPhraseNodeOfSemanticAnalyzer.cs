﻿using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class VerbPhraseNodeOfSemanticAnalyzer : BaseNodeOfSemanticAnalyzer
    {
        public VerbPhraseNodeOfSemanticAnalyzer(ContextOfSemanticAnalyzer context, VerbPhrase verbPhrase)
            : base(context)
        {
            _verbPhrase = verbPhrase;
            _verbsRolesStorage = new RolesStorageOfSemanticAnalyzer();
            _objectsRolesStorage = new RolesStorageOfSemanticAnalyzer();
        }

        private VerbPhrase _verbPhrase;
        private ConceptCGNode _concept;
        private RolesStorageOfSemanticAnalyzer _verbsRolesStorage;
        private RolesStorageOfSemanticAnalyzer _objectsRolesStorage;
        private IList<string> _verbFullLogicalMeaning;

        public ResultOfNodeOfSemanticAnalyzer Run()
        {
#if DEBUG
            //Context.Logger.Log($"_verbPhrase = {_verbPhrase}");
#endif

            var result = new ResultOfNodeOfSemanticAnalyzer();
            var resultPrimaryRolesDict = result.PrimaryRolesDict;
            var resultSecondaryRolesDict = result.SecondaryRolesDict;

            if (_verbPhrase.Intrusion != null)
            {
                throw new NotImplementedException();
            }

            if (_verbPhrase.V != null)
            {
                ProcessVAsBaseSentenceItem(_verbPhrase.V, result);
            }

            if (_verbPhrase.VP != null)
            {
                throw new NotImplementedException();
            }

            if (_verbPhrase.Negation != null)
            {
                throw new NotImplementedException();
            }

            if (_verbPhrase.Object != null)
            {
                ProcessObjectAsBaseSentenceItem(_verbPhrase.Object, result);
            }

            if (_verbPhrase.PP != null)
            {
                ProcessPPAsBaseSentenceItem(_verbPhrase.PP, result);
            }

            if (_verbPhrase.CP != null)
            {
                throw new NotImplementedException();
            }

            var verbAndObjectsMixRolesStorage = new RolesStorageOfSemanticAnalyzer();
            verbAndObjectsMixRolesStorage.Assing(_objectsRolesStorage);
            verbAndObjectsMixRolesStorage.Assing(_verbsRolesStorage);

#if DEBUG
            //Context.Logger.Log($"_objectsRolesStorage = {_objectsRolesStorage}");
            //Context.Logger.Log($"verbAndObjectsMix = {verbAndObjectsMixRolesStorage}");
            //Context.Logger.Log($"_verbFullLogicalMeaning = {_verbFullLogicalMeaning.WritePODListToString()}");
#endif

            if (_verbFullLogicalMeaning.Contains("event") || _verbFullLogicalMeaning.Contains("state"))
            {
                var entitiesList = verbAndObjectsMixRolesStorage.GetByRole("entity");

                if (!entitiesList.IsNullOrEmpty())
                {
                    foreach (var entityConcept in entitiesList)
                    {
#if DEBUG
                        //Context.Logger.Log($"entityConcept = {entityConcept}");
#endif

                        CreateObjectRelation(_concept, entityConcept);
                    }
                }
            }

            return result;
        }

        private void CreateObjectRelation(ConceptCGNode verbConcept, ConceptCGNode objectConcept)
        {
            var relationName = SpecialNamesOfRelations.ObjectRelationName;

            if (Context.RelationStorage.ContainsRelation(verbConcept.Name, objectConcept.Name, relationName))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var relation = new RelationCGNode();
            relation.Parent = conceptualGraph;
            relation.Name = relationName;

            verbConcept.AddOutputNode(relation);
            relation.AddOutputNode(objectConcept);

            Context.RelationStorage.AddRelation(verbConcept.Name, objectConcept.Name, relationName);
        }

        private void ProcessVAsBaseSentenceItem(BaseSentenceItem sentenceItem, ResultOfNodeOfSemanticAnalyzer result)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //Context.Logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.Word:
                    ProcessVAsWord(sentenceItem.AsWord, result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessVAsWord(Word word, ResultOfNodeOfSemanticAnalyzer result)
        {
#if DEBUG
            //Context.Logger.Log($"word = {word}");
#endif

            var conceptualGraph = Context.ConceptualGraph;

            _concept = new ConceptCGNode();
            result.RootConcept = _concept;
            _concept.Parent = conceptualGraph;

            _concept.Name = word.RootWordAsString;

            var baseGrammaticalWordFrame = word.WordFrame;

#if DEBUG
            //Context.Logger.Log($"baseGrammaticalWordFrame = {baseGrammaticalWordFrame}");
#endif

            _verbFullLogicalMeaning = baseGrammaticalWordFrame.FullLogicalMeaning;

            var resultPrimaryRolesDict = result.PrimaryRolesDict;

            foreach (var logicalMeaning in _verbFullLogicalMeaning)
            {
#if DEBUG
                //Context.Logger.Log($"logicalMeaning = {logicalMeaning}");
#endif

                PrimaryRolesDict.Add(logicalMeaning, _concept);
                resultPrimaryRolesDict.Add(logicalMeaning, _concept);
                _verbsRolesStorage.Add(logicalMeaning, _concept);
            }

            if(_verbPhrase.Object == null)
            {
                var isAct = baseGrammaticalWordFrame.FullLogicalMeaning.Contains("act");
                var isState = baseGrammaticalWordFrame.FullLogicalMeaning.Contains("state");

#if DEBUG
                //Context.Logger.Log($"isAct = {isAct}");
                //Context.Logger.Log($"isState = {isState}");
#endif

                if (isAct)
                {
                    var relationName = SpecialNamesOfRelations.ActionRelationName;
                    var relation = new RelationCGNode();
                    relation.Parent = conceptualGraph;
                    relation.Name = relationName;

                    relation.AddOutputNode(_concept);
                }
                else
                {
                    if (isState)
                    {
                        var relationName = SpecialNamesOfRelations.StateRelationName;
                        var relation = new RelationCGNode();
                        relation.Parent = conceptualGraph;
                        relation.Name = relationName;

                        relation.AddOutputNode(_concept);
                    }
                }
            }
        }

        private void ProcessObjectAsBaseSentenceItem(BaseSentenceItem sentenceItem, ResultOfNodeOfSemanticAnalyzer result)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //Context.Logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.NounPhrase:
                    ProcessObjectAsNounPhrase(sentenceItem.AsNounPhrase, result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessObjectAsNounPhrase(NounPhrase nounPhrase, ResultOfNodeOfSemanticAnalyzer result)
        {
#if DEBUG
            //Context.Logger.Log($"nounPhrase = {nounPhrase}");
#endif

            var nounPhraseNode = new NounPhraseNodeOfSemanticAnalyzer(Context, nounPhrase);
            var nounResult = nounPhraseNode.Run();

#if DEBUG
            //Context.Logger.Log($"nounResult = {nounResult}");
#endif

            PrimaryRolesDict.Assing(nounResult.PrimaryRolesDict);
            _objectsRolesStorage.Assing(nounResult.PrimaryRolesDict);
        }

        private void ProcessPPAsBaseSentenceItem(BaseSentenceItem sentenceItem, ResultOfNodeOfSemanticAnalyzer result)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //Context.Logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.PreOrPostpositionalPhrase:
                    ProcessPPAsPreOrPostpositionalPhrase(sentenceItem.AsPreOrPostpositionalPhrase, result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessPPAsPreOrPostpositionalPhrase(PreOrPostpositionalPhrase pp, ResultOfNodeOfSemanticAnalyzer result)
        {
#if DEBUG
            //Context.Logger.Log($"pp = {pp}");
            //Context.Logger.Log($"pp.RootWordAsString = {pp.RootWordAsString}");
            //Context.Logger.Log($"_verbPhrase.RootWordAsString = {_verbPhrase.RootWordAsString}");
            //Context.Logger.Log($"pp.GetConditionalLogicalMeaning(_verbPhrase.RootWordAsString) = {pp.GetConditionalLogicalMeaning(_verbPhrase.RootWordAsString).WritePODListToString()}");
#endif

            var conditionalLogicalMeaning = pp.GetConditionalLogicalMeaningAsSingleString(_verbPhrase.RootWordAsString);

#if DEBUG
            //Context.Logger.Log($"conditionalLogicalMeaning = {conditionalLogicalMeaning}");
#endif

            if (string.IsNullOrWhiteSpace(conditionalLogicalMeaning))
            {
                throw new NotImplementedException();
            }

            if(pp.NP == null)
            {
                throw new NotImplementedException();
            }

            var nounOfPrepositionalResult = ProcessNPOfPPAsBaseSentenceItem(pp.NP);

#if DEBUG
            //Context.Logger.Log($"nounOfPrepositionalResult = {nounOfPrepositionalResult}");
#endif

            var nounConcept = nounOfPrepositionalResult.RootConcept;

#if DEBUG
            //Context.Logger.Log($"nounConcept = {nounConcept}");
#endif

            var ppRelation = new RelationCGNode();
            ppRelation.Parent = Context.ConceptualGraph;
            ppRelation.Name = conditionalLogicalMeaning;

            ppRelation.AddInputNode(_concept);
            ppRelation.AddOutputNode(nounConcept);
        }

        private ResultOfNodeOfSemanticAnalyzer ProcessNPOfPPAsBaseSentenceItem(BaseSentenceItem sentenceItem)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

#if DEBUG
            //Context.Logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
#endif

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.NounPhrase:
                    return ProcessNPOfPPAsNounPhrase(sentenceItem.AsNounPhrase);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private ResultOfNodeOfSemanticAnalyzer ProcessNPOfPPAsNounPhrase(NounPhrase nounPhrase)
        {
#if DEBUG
            //Context.Logger.Log($"nounPhrase = {nounPhrase}");
#endif

            var nounPhraseNode = new NounPhraseNodeOfSemanticAnalyzer(Context, nounPhrase);
            return nounPhraseNode.Run();
        }
    }
}

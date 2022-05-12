using SymOntoClay.CoreHelper.CollectionsHelpers;
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
        public VerbPhraseNodeOfSemanticAnalyzer(ContextOfSemanticAnalyzer context, Sentence sentence, VerbPhrase verbPhrase)
            : base(context)
        {
            _sentence = sentence;
            _verbPhrase = verbPhrase;
            _verbsRolesStorage = new RolesStorageOfSemanticAnalyzer();
            _objectsRolesStorage = new RolesStorageOfSemanticAnalyzer();
        }

        private Sentence _sentence;
        private VerbPhrase _verbPhrase;
        private ConceptCGNode _concept;
        private RolesStorageOfSemanticAnalyzer _verbsRolesStorage;
        private RolesStorageOfSemanticAnalyzer _objectsRolesStorage;
        private IList<string> _verbFullLogicalMeaning;

        public ResultOfNodeOfSemanticAnalyzer Run()
        {
#if DEBUG
            Context.Logger.Log($"_verbPhrase = {_verbPhrase}");
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
                throw new NotImplementedException();
            }

            if (_verbPhrase.CP != null)
            {
                throw new NotImplementedException();
            }

            var verbAndObjectsMixRolesStorage = new RolesStorageOfSemanticAnalyzer();
            verbAndObjectsMixRolesStorage.Assing(_objectsRolesStorage);
            verbAndObjectsMixRolesStorage.Assing(_verbsRolesStorage);

#if DEBUG
            Context.Logger.Log($"_objectsRolesStorage = {_objectsRolesStorage}");
            Context.Logger.Log($"verbAndObjectsMix = {verbAndObjectsMixRolesStorage}");
            Context.Logger.Log($"_verbFullLogicalMeaning = {_verbFullLogicalMeaning.WritePODListToString()}");
#endif

            if (_verbFullLogicalMeaning.Contains("event") || _verbFullLogicalMeaning.Contains("state"))
            {
                var entitiesList = verbAndObjectsMixRolesStorage.GetByRole("entity");

                if (!entitiesList.IsNullOrEmpty())
                {
                    foreach (var entityConcept in entitiesList)
                    {
#if DEBUG
                        Context.Logger.Log($"entityConcept = {entityConcept}");
#endif

                        CreateObjectRelation(_concept, entityConcept);
                    }
                }
            }

            //            var prepositionalList = _verbPhrase.PrepositionalList;

            //            if (!prepositionalList.IsEmpty())
            //            {
            //#if DEBUG
            //                Context.Logger.Log($"prepositionalList.Count = {prepositionalList.Count}");
            //#endif

            //                foreach (var prepositional in prepositionalList)
            //                {
            //#if DEBUG
            //                    Context.Logger.Log($"prepositional = {prepositional}");
            //#endif

            //                    var conditionalLogicalMeaning = GetConditionalLogicalMeaning(prepositional.Preposition.RootWord, verb.RootWord);

            //#if DEBUG
            //                    Context.Logger.Log($"conditionalLogicalMeaning = {conditionalLogicalMeaning}");
            //#endif
            //                    if (!string.IsNullOrWhiteSpace(conditionalLogicalMeaning))
            //                    {
            //                        var nounOfPrepositionalList = prepositional.ChildrenNodesList.Select(p => p.AsNounPhrase).Where(p => p != null).ToList();

            //#if DEBUG
            //                        Context.Logger.Log($"nounOfPrepositionalList.Count = {nounOfPrepositionalList.Count}");
            //#endif

            //                        foreach (var nounOfPrepositional in nounOfPrepositionalList)
            //                        {
            //#if DEBUG
            //                            Context.Logger.Log($"nounOfPrepositional = {nounOfPrepositional}");
            //#endif

            //                            var nodeOfNounOfPrepositional = new NounPhraseNodeOfSemanticAnalyzer(Context, _sentence, nounOfPrepositional);
            //                            var nounOfPrepositionalResult = nodeOfNounOfPrepositional.Run();

            //#if DEBUG
            //                            Context.Logger.Log($"nounOfPrepositionalResult = {nounOfPrepositionalResult}");
            //#endif

            //                            var phisobjList = nounOfPrepositionalResult.PrimaryRolesDict.GetByRole("entity");

            //#if DEBUG
            //                            Context.Logger.Log($"phisobjList.Count = {phisobjList.Count}");
            //#endif

            //                            foreach (var phisobj in phisobjList)
            //                            {
            //#if DEBUG
            //                                //LogInstance.Log($"phisobj = {phisobj}");
            //#endif

            //                                var directionRelation = new RelationCGNode();
            //                                directionRelation.Parent = conceptualGraph;
            //                                directionRelation.Name = conditionalLogicalMeaning;

            //                                directionRelation.AddInputNode(_concept);
            //                                directionRelation.AddOutputNode(phisobj);
            //                            }
            //                        }
            //                    }
            //                }
            //            }

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
            Context.Logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
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
            Context.Logger.Log($"word = {word}");
#endif

            var conceptualGraph = Context.ConceptualGraph;

            _concept = new ConceptCGNode();
            result.RootConcept = _concept;
            _concept.Parent = conceptualGraph;

            _concept.Name = GetName(word);

            var baseGrammaticalWordFrame = word.WordFrame;

#if DEBUG
            Context.Logger.Log($"baseGrammaticalWordFrame = {baseGrammaticalWordFrame}");
#endif

            _verbFullLogicalMeaning = baseGrammaticalWordFrame.FullLogicalMeaning;

            var resultPrimaryRolesDict = result.PrimaryRolesDict;

            foreach (var logicalMeaning in _verbFullLogicalMeaning)
            {
#if DEBUG
                Context.Logger.Log($"logicalMeaning = {logicalMeaning}");
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
                Context.Logger.Log($"isAct = {isAct}");
                Context.Logger.Log($"isState = {isState}");
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
            Context.Logger.Log($"kindOfSentenceItem = {kindOfSentenceItem}");
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
            Context.Logger.Log($"nounPhrase = {nounPhrase}");
#endif

            var nounPhraseNode = new NounPhraseNodeOfSemanticAnalyzer(Context, _sentence, nounPhrase);
            var nounResult = nounPhraseNode.Run();

#if DEBUG
            Context.Logger.Log($"nounResult = {nounResult}");
#endif

            PrimaryRolesDict.Assing(nounResult.PrimaryRolesDict);
            _objectsRolesStorage.Assing(nounResult.PrimaryRolesDict);
        }
    }
}

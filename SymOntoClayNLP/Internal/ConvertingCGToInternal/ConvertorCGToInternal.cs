/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.Helpers;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.NLP.Internal.ConvertingCGToInternal
{
    public class ConvertorCGToInternal
    {
        public ConvertorCGToInternal(IMonitorLogger logger)
        {
            _logger = logger;
        }

        private readonly IMonitorLogger _logger;

        public InternalConceptualGraph Convert(ConceptualGraph source)
        {
            var context = new ContextOfConvertingCGToInternal();

            while (IsWrapperGraph(source))
            {
                context.WrappersList.Add(source);
                source = (ConceptualGraph)source.Children.SingleOrDefault(p => p.Kind == KindOfCGNode.Graph);
            }

            return ConvertConceptualGraph(source, null, context);
        }

        private bool IsWrapperGraph(ConceptualGraph source)
        {
            var countOfConceptualGraphs = source.Children.Count(p => p.Kind == KindOfCGNode.Graph);

            var kindOfRelationsList = source.Children.Where(p => p.Kind == KindOfCGNode.Relation).Select(p => GrammaticalElementsHelper.GetKindOfGrammaticalRelationFromName(p.Name));

            var isGrammaticalRelationsOnly = !kindOfRelationsList.Any(p => p == KindOfGrammaticalRelation.Undefined);

            if (countOfConceptualGraphs == 1 && isGrammaticalRelationsOnly)
            {
                return true;
            }

            return false;
        }

        private InternalConceptualGraph ConvertConceptualGraph(ConceptualGraph source, InternalConceptualGraph targetParent, ContextOfConvertingCGToInternal context)
        {
            if (context.WrappersList.Contains(source))
            {
                return null;
            }

            if (context.ConceptualGraphsDict.ContainsKey(source))
            {
                return context.ConceptualGraphsDict[source];
            }

            var result = new InternalConceptualGraph();

            context.ConceptualGraphsDict[source] = result;

            if (targetParent == null)
            {
                if (source.Parent != null)
                {
                    var parentForResult = ConvertConceptualGraph(source.Parent, null, context);
                    result.Parent = parentForResult;
                }
            }
            else
            {
                result.Parent = targetParent;
            }

            result.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.Graph;

            FillName(source, result, context);

            SetGrammaticalOptions(source, result);

            CreateChildren(source, result, context);

            TransformResultToCanonicalView(result, context);

            return result;
        }

        private void TransformResultToCanonicalView(InternalConceptualGraph dest, ContextOfConvertingCGToInternal context)
        {
            TransformResultToCanonicalViewForActionConcepts(dest, context);
            TransformResultToCanonicalViewForStateConcepts(dest, context);
        }

        private void TransformResultToCanonicalViewForActionConcepts(InternalConceptualGraph dest, ContextOfConvertingCGToInternal context)
        {
            var actionsConceptsList = dest.Children.Where(p => p.IsConceptNode && !p.Inputs.IsNullOrEmpty() && p.Inputs.Any(x => x.Name == SpecialNamesOfRelations.ActionRelationName)).Select(p => p.AsConceptNode).ToList();

            foreach (var actionConcept in actionsConceptsList)
            {
                var actionRelationsList = actionConcept.Inputs.Where(p => p.Name == SpecialNamesOfRelations.ActionRelationName).Select(p => p.AsRelationNode).ToList();

                foreach (var actionRelation in actionRelationsList)
                {
                    if (!actionRelation.Inputs.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var stubOfOfSubject = new InternalConceptCGNode();
                    stubOfOfSubject.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.Concept;
                    stubOfOfSubject.Parent = dest;
                    stubOfOfSubject.AddOutputNode(actionRelation);
                    stubOfOfSubject.Name = SpecialNamesOfConcepts.SomeOne;

                    var agentRelation = new InternalRelationCGNode();
                    agentRelation.Parent = dest;
                    agentRelation.Name = SpecialNamesOfRelations.AgentRelationName;
                    agentRelation.AddInputNode(actionConcept);
                    agentRelation.AddOutputNode(stubOfOfSubject);
                }

                TransformResultToCanonicalViewTryFillObjectNode(actionConcept, context);
            }
        }

        private void TransformResultToCanonicalViewForStateConcepts(InternalConceptualGraph dest, ContextOfConvertingCGToInternal context)
        {
            var statesConceptsList = dest.Children.Where(p => p.IsConceptNode && !p.Inputs.IsNullOrEmpty() && p.Inputs.Any(x => x.Name == SpecialNamesOfRelations.StateRelationName)).Select(p => p.AsConceptNode).ToList();

            foreach (var stateConcept in statesConceptsList)
            {
                var stateRelationsList = stateConcept.Inputs.Where(p => p.Name == SpecialNamesOfRelations.StateRelationName).Select(p => p.AsRelationNode).ToList();

                foreach (var stateRelation in stateRelationsList)
                {
                    if (!stateRelation.Inputs.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var stubOfOfSubject = new InternalConceptCGNode();
                    stubOfOfSubject.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.Concept;
                    stubOfOfSubject.Parent = dest;
                    stubOfOfSubject.AddOutputNode(stateRelation);
                    stubOfOfSubject.Name = SpecialNamesOfConcepts.SomeOne;
                }

                TransformResultToCanonicalViewTryFillObjectNode(stateConcept, context);
            }
        }

        private void TransformResultToCanonicalViewTryFillObjectNode(InternalConceptCGNode concept, ContextOfConvertingCGToInternal context)
        {
            var objectRelationsList = concept.Outputs.Where(p => p.Name == SpecialNamesOfRelations.ObjectRelationName).Select(p => p.AsRelationNode).ToList();
            var parent = concept.Parent;

            if (objectRelationsList.Count == 0)
            {
                var objectRelation = new InternalRelationCGNode();
                objectRelation.Parent = parent;
                objectRelation.Name = SpecialNamesOfRelations.ObjectRelationName;
                concept.AddOutputNode(objectRelation);

                var stubOfObject = new InternalConceptCGNode();
                stubOfObject.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.Concept;
                stubOfObject.Parent = parent;
                stubOfObject.Name = SpecialNamesOfConcepts.Self;
                objectRelation.AddOutputNode(stubOfObject);
            }
        }

        private void FillName(BaseCGNode source, BaseInternalCGNode result, ContextOfConvertingCGToInternal context)
        {
#if DEBUG
            //_logger.Info("5169FD08-30A2-4877-BDCB-7AAA0797EB5C", $"source.Name = '{source.Name}'");
#endif

            result.Name = NlpStringHelper.PrepareString(source.Name);
        }

        private void CreateChildren(ConceptualGraph source, InternalConceptualGraph result, ContextOfConvertingCGToInternal context)
        {
            var childrenList = source.Children;

            var entitiesConditionsMarksRelationsList = childrenList.Where(p => GrammaticalElementsHelper.IsEntityCondition(p.Name)).ToList();


            var notDirectlyClonedNodesList = GetNotDirectlyClonedNodesList(entitiesConditionsMarksRelationsList);

            var clustersOfLinkedNodesDict = GetClustersOfLinkedNodes(notDirectlyClonedNodesList);

            foreach (var clustersOfLinkedNodesKVPItem in clustersOfLinkedNodesDict)
            {
                CreateEntityCondition(result, clustersOfLinkedNodesKVPItem.Value, context);
            }

            var nodesForDirectlyCloningList = childrenList.Where(p => !notDirectlyClonedNodesList.Contains(p)).ToList();

            CreateChildrenByAllNodes(nodesForDirectlyCloningList, null, context);

            var conceptsSourceItemsList = nodesForDirectlyCloningList.Where(p => p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph).Select(p => (BaseConceptCGNode)p).ToList();

            var relationStorage = new RelationStorageOfSemanticAnalyzer();

            foreach (var sourceItem in conceptsSourceItemsList)
            {
                BaseInternalConceptCGNode resultItem = null;

                var kind = sourceItem.Kind;

                switch (kind)
                {
                    case KindOfCGNode.Graph:
                        resultItem = context.ConceptualGraphsDict[(ConceptualGraph)sourceItem];
                        break;

                    case KindOfCGNode.Concept:
                        resultItem = context.ConceptsDict[(ConceptCGNode)sourceItem];
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }

                var inputsNodesList = sourceItem.Inputs.Select(p => (RelationCGNode)p).ToList();

                foreach (var inputNode in inputsNodesList)
                {
                    var resultRelationItem = context.RelationsDict[inputNode];

                    var relationsInputsNodesList = inputNode.Inputs.Where(p => p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph).Select(p => (BaseConceptCGNode)p).ToList();

                    if (relationsInputsNodesList.Count == 0)
                    {
                        if (!relationStorage.ContainsRelation(resultRelationItem.Name, resultItem.Name))
                        {
                            resultItem.AddInputNode(resultRelationItem);
                            relationStorage.AddRelation(resultRelationItem.Name, resultItem.Name);
                        }
                    }
                    else
                    {
                        foreach (var relationInputNode in relationsInputsNodesList)
                        {
                            var resultRelationInputNode = GetBaseConceptCGNodeForMakingCommonRelation(relationInputNode, context);

                            if (!relationStorage.ContainsRelation(resultRelationInputNode.Name, resultRelationItem.Name, resultItem.Name))
                            {
                                resultItem.AddInputNode(resultRelationItem);
                                resultRelationItem.AddInputNode(resultRelationInputNode);

                                relationStorage.AddRelation(resultRelationInputNode.Name, resultRelationItem.Name, resultItem.Name);
                            }
                        }
                    }

                }


                var outputsNodesList = sourceItem.Outputs.Select(p => (RelationCGNode)p).ToList();

                foreach (var outputNode in outputsNodesList)
                {
                    var resultRelationItem = context.RelationsDict[outputNode];

                    var relationsOutputsNodesList = outputNode.Outputs.Where(p => p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph).Select(p => (BaseConceptCGNode)p).ToList();

                    foreach (var relationOutputNode in relationsOutputsNodesList)
                    {
                        var resultRelationOutputNode = GetBaseConceptCGNodeForMakingCommonRelation(relationOutputNode, context);

                        if (!relationStorage.ContainsRelation(resultItem.Name, resultRelationItem.Name, resultRelationOutputNode.Name))
                        {
                            resultItem.AddOutputNode(resultRelationItem);
                            resultRelationItem.AddOutputNode(resultRelationOutputNode);

                            relationStorage.AddRelation(resultItem.Name, resultRelationItem.Name, resultRelationOutputNode.Name);
                        }
                    }
                }
            }

        }

        public BaseInternalConceptCGNode GetBaseConceptCGNodeForMakingCommonRelation(BaseConceptCGNode sourceNode, ContextOfConvertingCGToInternal context)
        {
            if (context.EntityConditionsDict.ContainsKey(sourceNode))
            {
                return context.EntityConditionsDict[sourceNode];
            }

            var kind = sourceNode.Kind;

            switch (kind)
            {
                case KindOfCGNode.Graph:
                    return context.ConceptualGraphsDict[(ConceptualGraph)sourceNode];

                case KindOfCGNode.Concept:
                    return context.ConceptsDict[(ConceptCGNode)sourceNode];

                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private void CreateEntityCondition(InternalConceptualGraph parent, List<BaseCGNode> sourceItems, ContextOfConvertingCGToInternal context)
        {
            var entityCondition = new InternalConceptualGraph();
            entityCondition.Parent = parent;
            entityCondition.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.EntityCondition;
            var entityConditionName = NlpStringHelper.PrepareString(NameHelper.CreateRuleOrFactName());

#if DEBUG
            //_logger.Info("3FE5B3EF-2155-4FAE-B838-C749E6466C7C", $"entityConditionName = {entityConditionName}");
#endif

            entityCondition.Name = entityConditionName;

            var entityConditionsDict = context.EntityConditionsDict;

            sourceItems = sourceItems.Where(p => !GrammaticalElementsHelper.IsEntityCondition(p.Name)).ToList();

            foreach (var sourceItem in sourceItems)
            {
                entityConditionsDict[sourceItem] = entityCondition;
            }

            CreateChildrenByAllNodes(sourceItems, entityCondition, context);

            var conceptsSourceItemsList = sourceItems.Where(p => p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph).Select(p => (BaseConceptCGNode)p).ToList();

            var relationStorage = new RelationStorageOfSemanticAnalyzer();

            foreach (var sourceItem in conceptsSourceItemsList)
            {
                BaseInternalConceptCGNode resultItem = null;

                var kind = sourceItem.Kind;

                switch (kind)
                {
                    case KindOfCGNode.Graph:
                        resultItem = context.ConceptualGraphsDict[(ConceptualGraph)sourceItem];
                        break;

                    case KindOfCGNode.Concept:
                        resultItem = context.ConceptsDict[(ConceptCGNode)sourceItem];
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }

                var inputsNodesList = sourceItem.Inputs.Where(p => sourceItems.Contains(p)).Select(p => (RelationCGNode)p).ToList();

                foreach (var inputNode in inputsNodesList)
                {
                    var resultRelationItem = context.RelationsDict[inputNode];

                    var relationsInputsNodesList = inputNode.Inputs.Where(p => (p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph) && sourceItems.Contains(p)).Select(p => (BaseConceptCGNode)p).ToList();

                    foreach (var relationInputNode in relationsInputsNodesList)
                    {
                        BaseInternalConceptCGNode resultRelationInputNode = null;

                        var relationInputNodeKind = relationInputNode.Kind;

                        switch (relationInputNodeKind)
                        {
                            case KindOfCGNode.Graph:
                                resultRelationInputNode = context.ConceptualGraphsDict[(ConceptualGraph)relationInputNode];
                                break;

                            case KindOfCGNode.Concept:
                                resultRelationInputNode = context.ConceptsDict[(ConceptCGNode)relationInputNode];
                                break;

                            default: throw new ArgumentOutOfRangeException(nameof(relationInputNodeKind), relationInputNodeKind, null);
                        }

                        if (relationStorage.ContainsRelation(resultRelationInputNode.Name, resultRelationItem.Name, resultItem.Name))
                        {
                            continue;
                        }

                        resultItem.AddInputNode(resultRelationItem);
                        resultRelationItem.AddInputNode(resultRelationInputNode);

                        relationStorage.AddRelation(resultRelationInputNode.Name, resultRelationItem.Name, resultItem.Name);
                    }
                }

                var outputsNodesList = sourceItem.Outputs.Where(p => sourceItems.Contains(p)).Select(p => (RelationCGNode)p).ToList();

                foreach (var outputNode in outputsNodesList)
                {
                    var resultRelationItem = context.RelationsDict[outputNode];

                    var relationsOutputsNodesList = outputNode.Outputs.Where(p => (p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph) && sourceItems.Contains(p)).Select(p => (BaseConceptCGNode)p).ToList();

                    foreach (var relationOutputNode in relationsOutputsNodesList)
                    {
                        BaseInternalConceptCGNode resultRelationOutputNode = null;

                        var relationOutputNodeKind = relationOutputNode.Kind;

                        switch (relationOutputNodeKind)
                        {
                            case KindOfCGNode.Graph:
                                resultRelationOutputNode = context.ConceptualGraphsDict[(ConceptualGraph)relationOutputNode];
                                break;

                            case KindOfCGNode.Concept:
                                resultRelationOutputNode = context.ConceptsDict[(ConceptCGNode)relationOutputNode];
                                break;


                            default: throw new ArgumentOutOfRangeException(nameof(relationOutputNodeKind), relationOutputNodeKind, null);
                        }

                        if (relationStorage.ContainsRelation(resultItem.Name, resultRelationItem.Name, resultRelationOutputNode.Name))
                        {
                            continue;
                        }

                        resultItem.AddOutputNode(resultRelationItem);
                        resultRelationItem.AddOutputNode(resultRelationOutputNode);

                        relationStorage.AddRelation(resultItem.Name, resultRelationItem.Name, resultRelationOutputNode.Name);
                    }
                }
            }
        }

        private List<BaseCGNode> GetNotDirectlyClonedNodesList(List<BaseCGNode> entitiesConditionsMarksRelationsList)
        {
            var notDirectlyClonedNodesList = new List<BaseCGNode>();

            foreach (var entityConditionMarkRelation in entitiesConditionsMarksRelationsList)
            {
                notDirectlyClonedNodesList.Add(entityConditionMarkRelation);

                var firstOrdersRelationsList = entityConditionMarkRelation.Outputs.Where(p => p.Kind == KindOfCGNode.Relation).Select(p => (RelationCGNode)p).ToList();

                foreach (var firstOrderRelation in firstOrdersRelationsList)
                {
                    if (!notDirectlyClonedNodesList.Contains(firstOrderRelation))
                    {
                        notDirectlyClonedNodesList.Add(firstOrderRelation);
                    }

                    var inputConceptsList = firstOrderRelation.Inputs.Where(p => p.Kind == KindOfCGNode.Concept).Select(p => (ConceptCGNode)p).ToList();

                    foreach (var inputConcept in inputConceptsList)
                    {
                        if (!notDirectlyClonedNodesList.Contains(inputConcept))
                        {
                            notDirectlyClonedNodesList.Add(inputConcept);
                        }
                    }

                    var outputConceptsList = firstOrderRelation.Outputs.Where(p => p.Kind == KindOfCGNode.Concept).Select(p => (ConceptCGNode)p).ToList();

                    foreach (var outputConcept in outputConceptsList)
                    {
                        if (!notDirectlyClonedNodesList.Contains(outputConcept))
                        {
                            notDirectlyClonedNodesList.Add(outputConcept);
                        }
                    }
                }
            }

            return notDirectlyClonedNodesList;
        }

        private Dictionary<int, List<BaseCGNode>> GetClustersOfLinkedNodes(List<BaseCGNode> source)
        {
            var result = new Dictionary<int, List<BaseCGNode>>();

            var currentTargetNodesList = source.ToList();
            var n = 0;
            while (currentTargetNodesList.Count > 0)
            {
                n++;
                var nodesForThisN = GetLinkedNodes(currentTargetNodesList);

                if (nodesForThisN.Count == 0)
                {
                    break;
                }

                result[n] = nodesForThisN;

                currentTargetNodesList = currentTargetNodesList.Where(p => !nodesForThisN.Contains(p)).ToList();
            }

            return result;
        }

        private List<BaseCGNode> GetLinkedNodes(List<BaseCGNode> source)
        {
            var result = new List<BaseCGNode>();

            foreach (var sourceItem in source)
            {
                NGetLinkedNodes(source, sourceItem, ref result);
            }

            return result;
        }

        private void NGetLinkedNodes(List<BaseCGNode> source, BaseCGNode targetNode, ref List<BaseCGNode> result)
        {
            if (result.Contains(targetNode))
            {
                return;
            }

            result.Add(targetNode);

            var inputNodesList = targetNode.Inputs;

            if (inputNodesList.Count > 0)
            {
                var tmpNodesList = inputNodesList.Where(p => source.Contains(p)).ToList();

                foreach (var tmpNode in tmpNodesList)
                {
                    NGetLinkedNodes(source, tmpNode, ref result);
                }
            }

            var outputNodesList = targetNode.Outputs;

            if (outputNodesList.Count > 0)
            {
                var tmpNodesList = outputNodesList.Where(p => source.Contains(p)).ToList();

                foreach (var tmpNode in tmpNodesList)
                {
                    NGetLinkedNodes(source, tmpNode, ref result);
                }
            }
        }

        private void CreateChildrenByAllNodes(IList<BaseCGNode> childrenList, InternalConceptualGraph targetParent, ContextOfConvertingCGToInternal context)
        {
            foreach (var child in childrenList)
            {
                var kind = child.Kind;

                switch (kind)
                {
                    case KindOfCGNode.Graph:
                        ConvertConceptualGraph((ConceptualGraph)child, targetParent, context);
                        break;

                    case KindOfCGNode.Concept:
                        ConvertConcept((ConceptCGNode)child, targetParent, context);
                        break;

                    case KindOfCGNode.Relation:
                        ConvertRelation((RelationCGNode)child, targetParent, context);
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                }
            }
        }

        private void SetGrammaticalOptions(ConceptualGraph source, InternalConceptualGraph result)
        {
            var outputNodesGroupedDict = source.Outputs.GroupBy(p => GrammaticalElementsHelper.GetKindOfGrammaticalRelationFromName(p.Name)).ToDictionary(p => p.Key, p => p.ToList());

            foreach (var outputNodesGroupedKVPItem in outputNodesGroupedDict)
            {
                var kindOfGrammaticalRelation = outputNodesGroupedKVPItem.Key;
                var relationsList = outputNodesGroupedKVPItem.Value;

                switch (kindOfGrammaticalRelation)
                {
                    case KindOfGrammaticalRelation.Undefined:
                        continue;

                    case KindOfGrammaticalRelation.Aspect:
                        {
                            if (relationsList.Count > 1)
                            {
                                throw new NotSupportedException();
                            }

                            var relation = relationsList.Single();

                            var outputNodesOfTheRelation = relation.Outputs;

                            if (outputNodesOfTheRelation.Count != 1)
                            {
                                throw new NotSupportedException();
                            }

                            var outputNodeOfTheRelation = outputNodesOfTheRelation.Single();

                            var aspect = GrammaticalElementsHelper.GetAspectFromName(outputNodeOfTheRelation.Name);

                            if (aspect == GrammaticalAspect.Undefined)
                            {
                                throw new NotSupportedException();
                            }

                            result.Aspect = aspect;
                        }
                        break;

                    case KindOfGrammaticalRelation.Tense:
                        {
                            if (relationsList.Count > 1)
                            {
                                throw new NotSupportedException();
                            }

                            var relation = relationsList.Single();

                            var outputNodesOfTheRelation = relation.Outputs;

                            if (outputNodesOfTheRelation.Count != 1)
                            {
                                throw new NotSupportedException();
                            }

                            var outputNodeOfTheRelation = outputNodesOfTheRelation.Single();

                            var tense = GrammaticalElementsHelper.GetTenseFromName(outputNodeOfTheRelation.Name);

                            if (tense == GrammaticalTenses.Undefined)
                            {
                                throw new NotSupportedException();
                            }

                            result.Tense = tense;
                        }
                        break;

                    case KindOfGrammaticalRelation.Voice:
                        {
                            if (relationsList.Count > 1)
                            {
                                throw new NotSupportedException();
                            }

                            var relation = relationsList.Single();

                            var outputNodesOfTheRelation = relation.Outputs;

                            if (outputNodesOfTheRelation.Count != 1)
                            {
                                throw new NotSupportedException();
                            }

                            var outputNodeOfTheRelation = outputNodesOfTheRelation.Single();

                            var voice = GrammaticalElementsHelper.GetVoiceFromName(outputNodeOfTheRelation.Name);

                            if (voice == GrammaticalVoice.Undefined)
                            {
                                throw new NotSupportedException();
                            }

                            result.Voice = voice;
                        }
                        break;

                    case KindOfGrammaticalRelation.Mood:
                        {
                            if (relationsList.Count > 1)
                            {
                                throw new NotSupportedException();
                            }

                            var relation = relationsList.Single();

                            var outputNodesOfTheRelation = relation.Outputs;

                            if (outputNodesOfTheRelation.Count != 1)
                            {
                                throw new NotSupportedException();
                            }

                            var outputNodeOfTheRelation = outputNodesOfTheRelation.Single();

                            var mood = GrammaticalElementsHelper.GetMoodFromName(outputNodeOfTheRelation.Name);

                            if (mood == GrammaticalMood.Undefined)
                            {
                                throw new NotSupportedException();
                            }

                            result.Mood = mood;
                        }
                        break;

                    case KindOfGrammaticalRelation.AbilityModality:
                        {
                            if (relationsList.Count > 1)
                            {
                                throw new NotSupportedException();
                            }

                            var relation = relationsList.Single();

                            var outputNodesOfTheRelation = relation.Outputs;

                            if (outputNodesOfTheRelation.Count != 1)
                            {
                                throw new NotSupportedException();
                            }

                            var outputNodeOfTheRelation = outputNodesOfTheRelation.Single();

                            var modal = GrammaticalElementsHelper.GetAbilityModalityFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            //_logger.Info("782BDC0B-F8EC-4A84-B00E-6CD8C20D3F4E", $"modal = {modal}");
#endif

                            if (modal == AbilityModality.Undefined)
                            {
                                throw new NotSupportedException();
                            }

                            result.AbilityModality = modal;
                        }
                        break;

                    case KindOfGrammaticalRelation.PermissionModality:
                        {
                            if (relationsList.Count > 1)
                            {
                                throw new NotSupportedException();
                            }

                            var relation = relationsList.Single();

                            var outputNodesOfTheRelation = relation.Outputs;

                            if (outputNodesOfTheRelation.Count != 1)
                            {
                                throw new NotSupportedException();
                            }

                            var outputNodeOfTheRelation = outputNodesOfTheRelation.Single();

                            var modal = GrammaticalElementsHelper.GetPermissionModalityFromName(outputNodeOfTheRelation.Name);

                            if (modal == PermissionModality.Undefined)
                            {
                                throw new NotSupportedException();
                            }

                            result.PermissionModality = modal;
                        }
                        break;

                    case KindOfGrammaticalRelation.ObligationModality:
                        {
                            if (relationsList.Count > 1)
                            {
                                throw new NotSupportedException();
                            }

                            var relation = relationsList.Single();

                            var outputNodesOfTheRelation = relation.Outputs;

                            if (outputNodesOfTheRelation.Count != 1)
                            {
                                throw new NotSupportedException();
                            }

                            var outputNodeOfTheRelation = outputNodesOfTheRelation.Single();

                            var modal = GrammaticalElementsHelper.GetObligationModalityFromName(outputNodeOfTheRelation.Name);

                            if (modal == ObligationModality.Undefined)
                            {
                                throw new NotSupportedException();
                            }

                            result.ObligationModality = modal;
                        }
                        break;

                    case KindOfGrammaticalRelation.ProbabilityModality:
                        {
                            if (relationsList.Count > 1)
                            {
                                throw new NotSupportedException();
                            }

                            var relation = relationsList.Single();

                            var outputNodesOfTheRelation = relation.Outputs;

                            if (outputNodesOfTheRelation.Count != 1)
                            {
                                throw new NotSupportedException();
                            }

                            var outputNodeOfTheRelation = outputNodesOfTheRelation.Single();

                            var modal = GrammaticalElementsHelper.GetProbabilityModalityFromName(outputNodeOfTheRelation.Name);

                            if (modal == ProbabilityModality.Undefined)
                            {
                                throw new NotSupportedException();
                            }

                            result.ProbabilityModality = modal;
                        }
                        break;

                    case KindOfGrammaticalRelation.ConditionalModality:
                        {
                            if (relationsList.Count > 1)
                            {
                                throw new NotSupportedException();
                            }

                            var relation = relationsList.Single();

                            var outputNodesOfTheRelation = relation.Outputs;

                            if (outputNodesOfTheRelation.Count != 1)
                            {
                                throw new NotSupportedException();
                            }

                            var outputNodeOfTheRelation = outputNodesOfTheRelation.Single();

                            var modal = GrammaticalElementsHelper.GetConditionalModalityFromName(outputNodeOfTheRelation.Name);

                            if (modal == ConditionalModality.Undefined)
                            {
                                throw new NotSupportedException();
                            }

                            result.ConditionalModality = modal;
                        }
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(kindOfGrammaticalRelation), kindOfGrammaticalRelation, null);
                }
            }

            if(result.Tense == GrammaticalTenses.Undefined)
            {
                result.Tense = GrammaticalTenses.Present;
            }

            if (result.Aspect == GrammaticalAspect.Undefined)
            {
                result.Aspect = GrammaticalAspect.Simple;
            }

            if (result.Voice == GrammaticalVoice.Undefined)
            {
                result.Voice = GrammaticalVoice.Active;
            }

            if (result.Mood == GrammaticalMood.Undefined)
            {
                result.Mood = GrammaticalMood.Indicative;
            }

            if (result.KindOfQuestion == KindOfQuestion.Undefined)
            {
                result.KindOfQuestion = KindOfQuestion.None;
            }

            if(result.AbilityModality == AbilityModality.Undefined)
            {
                result.AbilityModality = AbilityModality.None;
            }

            if (result.PermissionModality == PermissionModality.Undefined)
            {
                result.PermissionModality = PermissionModality.None;
            }

            if (result.ObligationModality == ObligationModality.Undefined)
            {
                result.ObligationModality = ObligationModality.None;
            }

            if (result.ProbabilityModality == ProbabilityModality.Undefined)
            {
                result.ProbabilityModality = ProbabilityModality.None;
            }

            if (result.ConditionalModality == ConditionalModality.Undefined)
            {
                result.ConditionalModality = ConditionalModality.None;
            }
        }

        private InternalConceptCGNode ConvertConcept(ConceptCGNode source, InternalConceptualGraph targetParent, ContextOfConvertingCGToInternal context)
        {
            if (context.ConceptsDict.ContainsKey(source))
            {
                return context.ConceptsDict[source];
            }

            var result = new InternalConceptCGNode();

            context.ConceptsDict[source] = result;

            if (targetParent == null)
            {
                var parentForResult = ConvertConceptualGraph(source.Parent, null, context);
                result.Parent = parentForResult;
            }
            else
            {
                result.Parent = targetParent;
            }

            result.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.Concept;

            FillName(source, result, context);

            if(!source.InputNodes.IsNullOrEmpty() && source.InputNodes.Any(p => p.Name == SpecialNamesOfRelations.ObjectRelationName || p.Name == SpecialNamesOfRelations.DirectionRelationName))
            {
                result.IsRootConceptOfEntitiCondition = true;
            }

            return result;
        }

        private InternalRelationCGNode ConvertRelation(RelationCGNode source, InternalConceptualGraph targetParent, ContextOfConvertingCGToInternal context)
        {
            if (context.RelationsDict.ContainsKey(source))
            {
                return context.RelationsDict[source];
            }

            var result = new InternalRelationCGNode();

            context.RelationsDict[source] = result;

            if (targetParent == null)
            {
                var parentForResult = ConvertConceptualGraph(source.Parent, null, context);
                result.Parent = parentForResult;
            }
            else
            {
                result.Parent = targetParent;
            }

            FillName(source, result, context);

            return result;
        }
    }
}

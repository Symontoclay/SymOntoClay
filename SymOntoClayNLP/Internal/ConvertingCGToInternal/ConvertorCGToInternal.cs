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

using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingCGToInternal
{
    public class ConvertorCGToInternal
    {
        public ConvertorCGToInternal(IEntityLogger logger)
        {
            _logger = logger;
        }

        private readonly IEntityLogger _logger;

        public InternalConceptualGraph Convert(ConceptualGraph source)
        {
#if DEBUG
            //LogInstance.Log($"source = {source}");
#endif

            var context = new ContextOfConvertingCGToInternal();

            while (IsWrapperGraph(source))
            {
                context.WrappersList.Add(source);
                source = (ConceptualGraph)source.Children.SingleOrDefault(p => p.Kind == KindOfCGNode.Graph);
            }

#if DEBUG
            //LogInstance.Log($"source = {source}");
#endif

            return ConvertConceptualGraph(source, null, context);
        }

        private bool IsWrapperGraph(ConceptualGraph source)
        {
#if DEBUG
            //LogInstance.Log($"source = {source}");
#endif

            var countOfConceptualGraphs = source.Children.Count(p => p.Kind == KindOfCGNode.Graph);

#if DEBUG
            //LogInstance.Log($"countOfConceptualGraphs = {countOfConceptualGraphs}");
#endif

            var kindOfRelationsList = source.Children.Where(p => p.Kind == KindOfCGNode.Relation).Select(p => GrammaticalElementsHelper.GetKindOfGrammaticalRelationFromName(p.Name));

            var isGrammaticalRelationsOnly = !kindOfRelationsList.Any(p => p == KindOfGrammaticalRelation.Undefined);

#if DEBUG
            //LogInstance.Log($"isGrammaticalRelationsOnly = {isGrammaticalRelationsOnly}");
#endif

            if (countOfConceptualGraphs == 1 && isGrammaticalRelationsOnly)
            {
                return true;
            }

            return false;
        }

        private InternalConceptualGraph ConvertConceptualGraph(ConceptualGraph source, InternalConceptualGraph targetParent, ContextOfConvertingCGToInternal context)
        {
#if DEBUG
            //LogInstance.Log($"source = {source}");
#endif

            if (context.WrappersList.Contains(source))
            {
                return null;
            }

            if (context.ConceptualGraphsDict.ContainsKey(source))
            {
                return context.ConceptualGraphsDict[source];
            }

#if DEBUG
            //LogInstance.Log($"NEXT source = {source}");
            //var dotStr = DotConverter.ConvertToString(source);
            //LogInstance.Log($"dotStr = {dotStr}");
#endif

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

#if DEBUG
            //LogInstance.Log($"before result = {result}");
#endif

            CreateChildren(source, result, context);

#if DEBUG
            //LogInstance.Log($"after result = {result}");
            //dotStr = DotConverter.ConvertToString(result);
            //LogInstance.Log($"dotStr = {dotStr}");
#endif

            TransformResultToCanonicalView(result, context);

#if DEBUG
            //LogInstance.Log($"NEXT after result = {result}");
            //dotStr = DotConverter.ConvertToString(result);
            //LogInstance.Log($"dotStr = {dotStr}");
#endif

            return result;
        }

        private void TransformResultToCanonicalView(InternalConceptualGraph dest, ContextOfConvertingCGToInternal context)
        {
#if DEBUG
            //LogInstance.Log($"dest = {dest}");
            //var dotStr = DotConverter.ConvertToString(dest);
            //LogInstance.Log($"dotStr = {dotStr}");
#endif

            TransformResultToCanonicalViewForActionConcepts(dest, context);
            TransformResultToCanonicalViewForStateConcepts(dest, context);
        }

        private void TransformResultToCanonicalViewForActionConcepts(InternalConceptualGraph dest, ContextOfConvertingCGToInternal context)
        {
            var actionsConceptsList = dest.Children.Where(p => p.IsConceptNode && !p.Inputs.IsNullOrEmpty() && p.Inputs.Any(x => x.Name == SpecialNamesOfRelations.ActionRelationName)).Select(p => p.AsConceptNode).ToList();

#if DEBUG
            //LogInstance.Log($"actionsConceptsList.Count = {actionsConceptsList.Count}");
            //foreach(var tmpChild in dest.Children)
            //{
            //    LogInstance.Log($"tmpChild = {tmpChild}");
            //}
#endif
            foreach (var actionConcept in actionsConceptsList)
            {
#if DEBUG
                //LogInstance.Log($"actionConcept = {actionConcept}");
#endif

                var actionRelationsList = actionConcept.Inputs.Where(p => p.Name == SpecialNamesOfRelations.ActionRelationName).Select(p => p.AsRelationNode).ToList();

#if DEBUG
                //LogInstance.Log($"actionRelationsList.Count = {actionRelationsList.Count}");
#endif

                foreach (var actionRelation in actionRelationsList)
                {
#if DEBUG
                    //LogInstance.Log($"actionRelation = {actionRelation}");
#endif

                    if (!actionRelation.Inputs.IsNullOrEmpty())
                    {
                        continue;
                    }

#if DEBUG
                    //LogInstance.Log("Add stub of subject !!!!");
#endif

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

#if DEBUG
            //LogInstance.Log($"statesConceptsList.Count = {statesConceptsList.Count}");
#endif

            foreach (var stateConcept in statesConceptsList)
            {
#if DEBUG
                //LogInstance.Log($"stateConcept = {stateConcept}");
#endif

                var stateRelationsList = stateConcept.Inputs.Where(p => p.Name == SpecialNamesOfRelations.StateRelationName).Select(p => p.AsRelationNode).ToList();

#if DEBUG
                //LogInstance.Log($"stateRelationsList.Count = {stateRelationsList.Count}");
#endif

                foreach (var stateRelation in stateRelationsList)
                {
                    if (!stateRelation.Inputs.IsNullOrEmpty())
                    {
                        continue;
                    }

#if DEBUG
                    //LogInstance.Log("Add stub of subject !!!!");
#endif

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

#if DEBUG
            //_logger.Log($"objectRelationsList.Count = {objectRelationsList.Count}");
#endif

            if (objectRelationsList.Count == 0)
            {
#if DEBUG
                //LogInstance.Log("Add stub of object !!!!");
#endif

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
            result.Name = source.Name;
        }

        private void CreateChildren(ConceptualGraph source, InternalConceptualGraph result, ContextOfConvertingCGToInternal context)
        {
            var childrenList = source.Children;

#if DEBUG
            //LogInstance.Log($"childrenList.Count = {childrenList.Count}");
#endif

            var entitiesConditionsMarksRelationsList = childrenList.Where(p => GrammaticalElementsHelper.IsEntityCondition(p.Name)).ToList();

#if DEBUG
            //LogInstance.Log($"entitiesConditionsMarksRelationsList.Count = {entitiesConditionsMarksRelationsList.Count}");
#endif

            //if(entitiesConditionsMarksRelationsList.Count == 0)
            //{
            //CreateChildrenByAllNodes(childrenList, null, context);
            //return;
            //}

            var notDirectlyClonedNodesList = GetNotDirectlyClonedNodesList(entitiesConditionsMarksRelationsList);

#if DEBUG
            //LogInstance.Log($"notDirectlyClonedNodesList.Count = {notDirectlyClonedNodesList.Count}");
            //foreach (var notDirectlyClonedNode in notDirectlyClonedNodesList)
            //{
            //    LogInstance.Log($"notDirectlyClonedNode = {notDirectlyClonedNode}");
            //}
#endif

            var clustersOfLinkedNodesDict = GetClastersOfLinkedNodes(notDirectlyClonedNodesList);

#if DEBUG
            //LogInstance.Log($"clustersOfLinkedNodesDict.Count = {clustersOfLinkedNodesDict.Count}");
#endif
            foreach (var clustersOfLinkedNodesKVPItem in clustersOfLinkedNodesDict)
            {
#if DEBUG
                //LogInstance.Log($"clustersOfLinkedNodesKVPItem.Key = {clustersOfLinkedNodesKVPItem.Key}");
#endif
                CreateEntityCondition(result, clustersOfLinkedNodesKVPItem.Value, context);
            }

            var nodesForDirectlyClonningList = childrenList.Where(p => !notDirectlyClonedNodesList.Contains(p)).ToList();

            CreateChildrenByAllNodes(nodesForDirectlyClonningList, null, context);

            var conceptsSourceItemsList = nodesForDirectlyClonningList.Where(p => p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph).Select(p => (BaseConceptCGNode)p).ToList();

            var relationStorage = new RelationStorageOfSemanticAnalyzer();

            foreach (var sourceItem in conceptsSourceItemsList)
            {
#if DEBUG
                //LogInstance.Log($"sourceItem = {sourceItem}");
#endif

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

#if DEBUG
                //LogInstance.Log($"resultItem = {resultItem}");
#endif

                var inputsNodesList = sourceItem.Inputs.Select(p => (RelationCGNode)p).ToList();

#if DEBUG
                //LogInstance.Log($"inputsNodesList.Count = {inputsNodesList.Count}");
#endif

                foreach (var inputNode in inputsNodesList)
                {
#if DEBUG
                    //LogInstance.Log($"Begin inputNode (Relation) = {inputNode}");
#endif

                    var resultRelationItem = context.RelationsDict[inputNode];

#if DEBUG
                    //LogInstance.Log($"resultRelationItem = {resultRelationItem}");
#endif

                    var relationsInputsNodesList = inputNode.Inputs.Where(p => p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph).Select(p => (BaseConceptCGNode)p).ToList();

#if DEBUG
                    //LogInstance.Log($"relationsInputsNodesList.Count = {relationsInputsNodesList.Count}");
#endif

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
#if DEBUG
                            //LogInstance.Log($"relationInputNode = {relationInputNode}");
#endif

                            var resultRelationInputNode = GetBaseConceptCGNodeForMakingCommonRelation(relationInputNode, context);

#if DEBUG
                            //LogInstance.Log($"resultRelationInputNode = {resultRelationInputNode}");
#endif
                            if (!relationStorage.ContainsRelation(resultRelationInputNode.Name, resultRelationItem.Name, resultItem.Name))
                            {
                                resultItem.AddInputNode(resultRelationItem);
                                resultRelationItem.AddInputNode(resultRelationInputNode);

                                relationStorage.AddRelation(resultRelationInputNode.Name, resultRelationItem.Name, resultItem.Name);
                            }
                        }
                    }

#if DEBUG
                    //LogInstance.Log($"End inputNode (Relation) = {inputNode}");
#endif
                }

                //throw new NotImplementedException();

                var outputsNodesList = sourceItem.Outputs.Select(p => (RelationCGNode)p).ToList();

#if DEBUG
                //LogInstance.Log($"outputsNodesList.Count = {outputsNodesList.Count}");
#endif

                foreach (var outputNode in outputsNodesList)
                {
#if DEBUG
                    //LogInstance.Log($"Begin outputNode (Relation) = {outputNode}");
#endif

                    var resultRelationItem = context.RelationsDict[outputNode];

#if DEBUG
                    //LogInstance.Log($"resultRelationItem = {resultRelationItem}");
#endif

                    var relationsOutputsNodesList = outputNode.Outputs.Where(p => p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph).Select(p => (BaseConceptCGNode)p).ToList();

#if DEBUG
                    //LogInstance.Log($"relationsOutputsNodesList.Count = {relationsOutputsNodesList.Count}");
#endif

                    foreach (var relationOutputNode in relationsOutputsNodesList)
                    {
#if DEBUG
                        //LogInstance.Log($"relationOutputNode = {relationOutputNode}");
#endif
                        var resultRelationOutputNode = GetBaseConceptCGNodeForMakingCommonRelation(relationOutputNode, context);

#if DEBUG
                        //LogInstance.Log($"resultRelationOutputNode = {resultRelationOutputNode}");
#endif

                        if (!relationStorage.ContainsRelation(resultItem.Name, resultRelationItem.Name, resultRelationOutputNode.Name))
                        {
                            resultItem.AddOutputNode(resultRelationItem);
                            resultRelationItem.AddOutputNode(resultRelationOutputNode);

                            relationStorage.AddRelation(resultItem.Name, resultRelationItem.Name, resultRelationOutputNode.Name);
                        }
                    }
#if DEBUG
                    //LogInstance.Log($"End outputNode (Relation) = {outputNode}");
#endif
                }
            }

            //throw new NotImplementedException();
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
#if DEBUG
            //_logger.Log($"sourceItems.Count = {sourceItems.Count}");
#endif

            var entityCondition = new InternalConceptualGraph();
            entityCondition.Parent = parent;
            entityCondition.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.EntityCondition;
            var entityConditionName = NameHelper.CreateRuleOrFactName().NameValue;
            entityCondition.Name = entityConditionName;

            var entityConditionsDict = context.EntityConditionsDict;

            sourceItems = sourceItems.Where(p => !GrammaticalElementsHelper.IsEntityCondition(p.Name)).ToList();

            foreach (var sourceItem in sourceItems)
            {
#if DEBUG
                //LogInstance.Log($"sourceItem = {sourceItem}");
#endif

                entityConditionsDict[sourceItem] = entityCondition;
            }

            CreateChildrenByAllNodes(sourceItems, entityCondition, context);

            var conceptsSourceItemsList = sourceItems.Where(p => p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph).Select(p => (BaseConceptCGNode)p).ToList();

            var relationStorage = new RelationStorageOfSemanticAnalyzer();

            foreach (var sourceItem in conceptsSourceItemsList)
            {
#if DEBUG
                //LogInstance.Log($"sourceItem (2) = {sourceItem}");
#endif

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

#if DEBUG
                //LogInstance.Log($"resultItem = {resultItem}");
#endif

                var inputsNodesList = sourceItem.Inputs.Where(p => sourceItems.Contains(p)).Select(p => (RelationCGNode)p).ToList();

#if DEBUG
                //LogInstance.Log($"inputsNodesList.Count = {inputsNodesList.Count}");
#endif

                foreach (var inputNode in inputsNodesList)
                {
#if DEBUG
                    //LogInstance.Log($"inputNode = {inputNode}");
#endif

                    var resultRelationItem = context.RelationsDict[inputNode];

#if DEBUG
                    //LogInstance.Log($"resultRelationItem = {resultRelationItem}");
#endif

                    var relationsInputsNodesList = inputNode.Inputs.Where(p => (p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph) && sourceItems.Contains(p)).Select(p => (BaseConceptCGNode)p).ToList();

#if DEBUG
                    //LogInstance.Log($"relationsInputsNodesList.Count = {relationsInputsNodesList.Count}");
#endif

                    foreach (var relationInputNode in relationsInputsNodesList)
                    {
#if DEBUG
                        //LogInstance.Log($"relationInputNode = {relationInputNode}");
#endif

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

#if DEBUG
                        //LogInstance.Log($"resultRelationInputNode = {resultRelationInputNode}");
#endif

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

#if DEBUG
                //LogInstance.Log($"outputsNodesList.Count = {outputsNodesList.Count}");
#endif

                foreach (var outputNode in outputsNodesList)
                {
#if DEBUG
                    //LogInstance.Log($"outputNode = {outputNode}");
#endif

                    var resultRelationItem = context.RelationsDict[outputNode];

#if DEBUG
                    //LogInstance.Log($"resultRelationItem = {resultRelationItem}");
#endif

                    var relationsOutputsNodesList = outputNode.Outputs.Where(p => (p.Kind == KindOfCGNode.Concept || p.Kind == KindOfCGNode.Graph) && sourceItems.Contains(p)).Select(p => (BaseConceptCGNode)p).ToList();

#if DEBUG
                    //LogInstance.Log($"relationsOutputsNodesList.Count = {relationsOutputsNodesList.Count}");
#endif

                    foreach (var relationOutputNode in relationsOutputsNodesList)
                    {
#if DEBUG
                        //LogInstance.Log($"relationOutputNode = {relationOutputNode}");
#endif
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

#if DEBUG
                        //LogInstance.Log($"resultRelationOutputNode = {resultRelationOutputNode}");
#endif

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
#if DEBUG
                //LogInstance.Log($"entityConditionMarkRelation = {entityConditionMarkRelation}");
#endif

                notDirectlyClonedNodesList.Add(entityConditionMarkRelation);

                var firstOrdersRelationsList = entityConditionMarkRelation.Outputs.Where(p => p.Kind == KindOfCGNode.Relation).Select(p => (RelationCGNode)p).ToList();

#if DEBUG
                //LogInstance.Log($"firstOrdersRelationsList.Count = {firstOrdersRelationsList.Count}");
#endif

                foreach (var firstOrderRelation in firstOrdersRelationsList)
                {
#if DEBUG
                    //LogInstance.Log($"firstOrderRelation = {firstOrderRelation}");
#endif

                    if (!notDirectlyClonedNodesList.Contains(firstOrderRelation))
                    {
                        notDirectlyClonedNodesList.Add(firstOrderRelation);
                    }

                    var inputConceptsList = firstOrderRelation.Inputs.Where(p => p.Kind == KindOfCGNode.Concept).Select(p => (ConceptCGNode)p).ToList();

#if DEBUG
                    //LogInstance.Log($"inputConceptsList.Count = {inputConceptsList.Count}");
#endif

                    foreach (var inputConcept in inputConceptsList)
                    {
#if DEBUG
                        //LogInstance.Log($"inputConcept = {inputConcept}");
#endif

                        if (!notDirectlyClonedNodesList.Contains(inputConcept))
                        {
                            notDirectlyClonedNodesList.Add(inputConcept);
                        }
                    }

                    var outputConceptsList = firstOrderRelation.Outputs.Where(p => p.Kind == KindOfCGNode.Concept).Select(p => (ConceptCGNode)p).ToList();

#if DEBUG
                    //LogInstance.Log($"outputConceptsList.Count = {outputConceptsList.Count}");
#endif

                    foreach (var outputConcept in outputConceptsList)
                    {
#if DEBUG
                        //LogInstance.Log($"outputConcept = {outputConcept}");
#endif

                        if (!notDirectlyClonedNodesList.Contains(outputConcept))
                        {
                            notDirectlyClonedNodesList.Add(outputConcept);
                        }
                    }
                }
            }

            return notDirectlyClonedNodesList;
        }

        private Dictionary<int, List<BaseCGNode>> GetClastersOfLinkedNodes(List<BaseCGNode> source)
        {
            var result = new Dictionary<int, List<BaseCGNode>>();

            var currentTargetNodesList = source.ToList();
            var n = 0;
            while (currentTargetNodesList.Count > 0)
            {
                n++;
#if DEBUG
                //LogInstance.Log($"currentTargetNodesList.Count = {currentTargetNodesList.Count} n = {n}");
#endif

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
#if DEBUG
            //LogInstance.Log($"targetNode = {targetNode}");
#endif

            if (result.Contains(targetNode))
            {
                return;
            }

            result.Add(targetNode);

            var inputNodesList = targetNode.Inputs;

#if DEBUG
            //LogInstance.Log($"inputNodesList.Count = {inputNodesList.Count}");
#endif

            if (inputNodesList.Count > 0)
            {
                var tmpNodesList = inputNodesList.Where(p => source.Contains(p)).ToList();

#if DEBUG
                //LogInstance.Log($"tmpNodesList.Count = {tmpNodesList.Count}");
#endif

                foreach (var tmpNode in tmpNodesList)
                {
                    NGetLinkedNodes(source, tmpNode, ref result);
                }
            }

            var outputNodesList = targetNode.Outputs;

#if DEBUG
            //LogInstance.Log($"outputNodesList.Count = {outputNodesList.Count}");
#endif

            if (outputNodesList.Count > 0)
            {
                var tmpNodesList = outputNodesList.Where(p => source.Contains(p)).ToList();

#if DEBUG
                //LogInstance.Log($"tmpNodesList.Count = {tmpNodesList.Count}");
#endif

                foreach (var tmpNode in tmpNodesList)
                {
                    NGetLinkedNodes(source, tmpNode, ref result);
                }
            }
        }

        private void CreateChildrenByAllNodes(IList<BaseCGNode> childrenList, InternalConceptualGraph targetParent, ContextOfConvertingCGToInternal context)
        {
#if DEBUG
            //LogInstance.Log($"childrenList.Count = {childrenList.Count}");
#endif

            foreach (var child in childrenList)
            {
#if DEBUG
                //LogInstance.Log($"child = {child}");
#endif

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

#if DEBUG
            //LogInstance.Log($"outputNodesGroupedDict.Count = {outputNodesGroupedDict.Count}");
#endif

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

#if DEBUG
                            //LogInstance.Log($"relation = {relation}");
                            //LogInstance.Log($"outputNodeOfTheRelation = {outputNodeOfTheRelation}");
#endif

                            var aspect = GrammaticalElementsHelper.GetAspectFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            //LogInstance.Log($"aspect = {aspect}");
#endif

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

#if DEBUG
                            //LogInstance.Log($"relation = {relation}");
                            //LogInstance.Log($"outputNodeOfTheRelation = {outputNodeOfTheRelation}");
#endif

                            var tense = GrammaticalElementsHelper.GetTenseFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            //LogInstance.Log($"tense = {tense}");
#endif

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

#if DEBUG
                            //LogInstance.Log($"relation = {relation}");
                            //LogInstance.Log($"outputNodeOfTheRelation = {outputNodeOfTheRelation}");
#endif

                            var voice = GrammaticalElementsHelper.GetVoiceFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            //LogInstance.Log($"voice = {voice}");
#endif

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

#if DEBUG
                            //LogInstance.Log($"relation = {relation}");
                            //LogInstance.Log($"outputNodeOfTheRelation = {outputNodeOfTheRelation}");
#endif

                            var mood = GrammaticalElementsHelper.GetMoodFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            //LogInstance.Log($"mood = {mood}");
#endif

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

#if DEBUG
                            //LogInstance.Log($"relation = {relation}");
                            //LogInstance.Log($"outputNodeOfTheRelation = {outputNodeOfTheRelation}");
#endif

                            var modal = GrammaticalElementsHelper.GetAbilityModalityFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            _logger.Log($"modal = {modal}");
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

#if DEBUG
                            //LogInstance.Log($"relation = {relation}");
                            //LogInstance.Log($"outputNodeOfTheRelation = {outputNodeOfTheRelation}");
#endif

                            var modal = GrammaticalElementsHelper.GetPermissionModalityFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            //LogInstance.Log($"modal = {modal}");
#endif

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

#if DEBUG
                            //LogInstance.Log($"relation = {relation}");
                            //LogInstance.Log($"outputNodeOfTheRelation = {outputNodeOfTheRelation}");
#endif

                            var modal = GrammaticalElementsHelper.GetObligationModalityFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            //LogInstance.Log($"modal = {modal}");
#endif

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

#if DEBUG
                            //LogInstance.Log($"relation = {relation}");
                            //LogInstance.Log($"outputNodeOfTheRelation = {outputNodeOfTheRelation}");
#endif

                            var modal = GrammaticalElementsHelper.GetProbabilityModalityFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            //LogInstance.Log($"modal = {modal}");
#endif

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

#if DEBUG
                            //LogInstance.Log($"relation = {relation}");
                            //LogInstance.Log($"outputNodeOfTheRelation = {outputNodeOfTheRelation}");
#endif

                            var modal = GrammaticalElementsHelper.GetConditionalModalityFromName(outputNodeOfTheRelation.Name);

#if DEBUG
                            //LogInstance.Log($"modal = {modal}");
#endif

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

#if DEBUG
            //_logger.Log($"result = {result}");
#endif

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
#if DEBUG
            //_logger.Log($"source = {source}");
            //_logger.Log($"targetParent == null = {targetParent == null}");
#endif

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

#if DEBUG
            //if(source.Name == "cat")
            //{
            //    _logger.Log($"result = {result}");
            //}
#endif

            return result;
        }

        private InternalRelationCGNode ConvertRelation(RelationCGNode source, InternalConceptualGraph targetParent, ContextOfConvertingCGToInternal context)
        {
#if DEBUG
            //_logger.Log($"source = {source}");
            //_logger.Log($"targetParent == null = {targetParent == null}");
#endif

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

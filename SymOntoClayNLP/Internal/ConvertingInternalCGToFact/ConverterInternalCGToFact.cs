using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToFact
{
    public class ConverterInternalCGToFact
    {
        public ConverterInternalCGToFact(IEntityLogger logger)
        {
            _logger = logger;
        }

        private readonly IEntityLogger _logger;

        public IList<RuleInstance> ConvertConceptualGraph(InternalConceptualGraph source)
        {
#if DEBUG
            _logger.Log($"source = {source}");
#endif

            var context = new ContextOfConvertingInternalCGToFact();

            return ConvertConceptualGraph(source, context);
        }

        private IList<RuleInstance> ConvertConceptualGraph(InternalConceptualGraph source, ContextOfConvertingInternalCGToFact context)
        {
            PreliminaryCreation(source, context);

#if DEBUG
            _logger.Log($"context.RuleInstancesDict.Count = {context.RuleInstancesDict.Count}");
#endif

            foreach (var ruleInstancesDictKVPItem in context.RuleInstancesDict)
            {
                FillRuleInstances(ruleInstancesDictKVPItem.Key, ruleInstancesDictKVPItem.Value, context);
            }

            var resultsList = context.RuleInstancesDict.Values.Where(p => p.KindOfRuleInstance == KindOfRuleInstance.Fact && ((p.PrimaryPart != null && p.PrimaryPart.Expression != null) || (p.SecondaryParts.IsNullOrEmpty()))).ToList();

            resultsList.AddRange(context.AnnotationsList);

            return resultsList;
        }

        private void PreliminaryCreation(InternalConceptualGraph source, ContextOfConvertingInternalCGToFact context)
        {
#if DEBUG
            _logger.Log($"source = {source}");
#endif

            if (context.RuleInstancesDict.ContainsKey(source))
            {
                return;
            }

            var ruleInstance = new RuleInstance();
            ruleInstance.TypeOfAccess = TypeOfAccess.Public;
            context.RuleInstancesDict[source] = ruleInstance;

            if (string.IsNullOrWhiteSpace(source.Name))
            {
                ruleInstance.Name = NameHelper.CreateRuleOrFactName();
            }
            else
            {
                ruleInstance.Name = NameHelper.CreateName(source.Name);

                if (ruleInstance.Name.KindOfName != KindOfName.RuleOrFact)
                {
                    throw new Exception($"Invalid name format `{ruleInstance.Name}`!");
                }
            }

            if (source.KindOfGraphOrConcept == KindOfInternalGraphOrConceptNode.EntityCondition)
            {
                ruleInstance.KindOfRuleInstance = KindOfRuleInstance.EntityCondition;
            }
            else
            {
                ruleInstance.KindOfRuleInstance = KindOfRuleInstance.Fact;
            }

            var graphsChildrenList = source.Children.Where(p => p.IsConceptualGraph).Select(p => p.AsConceptualGraph).ToList();

#if DEBUG
            _logger.Log($"graphsChildrenList.Count = {graphsChildrenList.Count}");
#endif

#if DEBUG
            _logger.Log($"context.RuleInstancesDict.Count (1) = {context.RuleInstancesDict.Count}");
#endif

            foreach (var graphsChild in graphsChildrenList)
            {
                PreliminaryCreation(graphsChild, context);
            }

#if DEBUG
            _logger.Log($"context.RuleInstancesDict.Count (2) = {context.RuleInstancesDict.Count}");
            _logger.Log($"context.RuleInstancesDict.Keys.Select(p => p.Name) = {context.RuleInstancesDict.Keys.Select(p => p.Name).ToList().WritePODListToString()}");
#endif
        }

        private void FillRuleInstances(InternalConceptualGraph source, RuleInstance dest, ContextOfConvertingInternalCGToFact context)
        {
#if DEBUG
            _logger.Log($"source = {source}");
            _logger.Log($"dest = {dest}");
            var dotStr = DotConverter.ConvertToString(source);
            _logger.Log($"dotStr (5) = {dotStr}");
#endif

            var contextForSingleRuleInstance = new ContextForSingleRuleInstanceOfConvertingInternalCGToFact();
            contextForSingleRuleInstance.CurrentRuleInstance = dest;

            //var variablesQuantification = new VariablesQuantificationPart();
            //dest.VariablesQuantification = variablesQuantification;
            //variablesQuantification.Items = new List<VarExpressionNode>();

            var part = new PrimaryRulePart();
            dest.PrimaryPart = part;
            part.Parent = dest;
            part.IsActive = true;

            var kindOfGraphOrConcept = source.KindOfGraphOrConcept;

#if DEBUG
            _logger.Log($"kindOfGraphOrConcept = {kindOfGraphOrConcept}");
#endif

            switch (kindOfGraphOrConcept)
            {
                case KindOfInternalGraphOrConceptNode.Graph:
                    PrepareForGraphConditionExpression(source, dest, context);
                    break;

                case KindOfInternalGraphOrConceptNode.EntityCondition:
                    PrepareForEntityConditionExpression(source, dest, context);
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(kindOfGraphOrConcept), kindOfGraphOrConcept, null);
            }

#if DEBUG
            dotStr = DotConverter.ConvertToString(source);
            _logger.Log($"dotStr (6) = {dotStr}");
#endif

            var expression = CreateExpressionByWholeGraph(source, context, dest, contextForSingleRuleInstance);

#if DEBUG
            _logger.Log($"expression = {expression}");
            var debugStr = DebugHelperForRuleInstance.ToString(expression);
            _logger.Log($"debugStr = {debugStr}");
#endif

            //throw new NotImplementedException();

            part.Expression = expression;

            //throw new NotImplementedException();

            //            var etityConditionsDict = contextForSingleRuleInstance.EntityConditionsDict;

            //#if DEBUG
            //            //LogInstance.Log($"etityConditionsDict.Count = {etityConditionsDict.Count}");
            //#endif

            //            if (etityConditionsDict.Count > 0)
            //            {
            //                var entitiesConditions = new EntitiesConditions();
            //                dest.EntitiesConditions = entitiesConditions;
            //                entitiesConditions.Items = new List<EntityConditionItem>();

            //                foreach (var etityConditionsKVPItem in etityConditionsDict)
            //                {
            //#if DEBUG
            //                    //LogInstance.Log($"etityConditionsKVPItem.Key = {etityConditionsKVPItem.Key}");
            //                    //LogInstance.Log($"etityConditionsKVPItem.Value = {etityConditionsKVPItem.Value}");
            //#endif

            //                    var entityConditionItem = new EntityConditionItem();
            //                    entitiesConditions.Items.Add(entityConditionItem);
            //                    entityConditionItem.Name = etityConditionsKVPItem.Key;
            //                    entityConditionItem.VariableName = etityConditionsKVPItem.Value;
            //                }

            //                //throw new NotImplementedException();
            //            }

#if DEBUG
            _logger.Log($"End source = {source}");
            dotStr = DotConverter.ConvertToString(source);
            _logger.Log($"dotStr (~~) = {dotStr}");
            _logger.Log($"End dest = {dest}");
#endif
        }

        private void PrepareForGraphConditionExpression(InternalConceptualGraph source, RuleInstance dest, ContextOfConvertingInternalCGToFact context)
        {
            var processedSpecialConceptsList = new List<BaseInternalConceptCGNode>();
            var relationsForRemoving = new List<InternalRelationCGNode>();
            //var processedRelationsList = new List<InternalRelationCGNode>();

            var initRelationsList = source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).ToList();

#if DEBUG
            //LogInstance.Log($"initRelationsList.Count = {initRelationsList.Count}");
#endif

            foreach (var initRelation in initRelationsList)
            {
#if DEBUG
                //LogInstance.Log($"initRelation = {initRelation}");
#endif

                var kindOfSpecialRelation = SpecialElementsHelper.GetKindOfSpecialRelation(initRelation.Name);
                initRelation.KindOfSpecialRelation = kindOfSpecialRelation;
#if DEBUG
                //LogInstance.Log($"kindOfSpecialRelation = {kindOfSpecialRelation}");
#endif

                switch (kindOfSpecialRelation)
                {
                    case KindOfSpecialRelation.Undefinded:
                        break;

                    case KindOfSpecialRelation.Object:
#if DEBUG
                        //LogInstance.Log($"initRelation = {initRelation}");
                        //LogInstance.Log($"kindOfSpecialRelation = {kindOfSpecialRelation}");
#endif
                        if (!relationsForRemoving.Contains(initRelation))
                        {
                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.Experiencer:
#if DEBUG
                        //LogInstance.Log($"initRelation = {initRelation}");
                        //LogInstance.Log($"kindOfSpecialRelation = {kindOfSpecialRelation}");
#endif
                        if (!relationsForRemoving.Contains(initRelation))
                        {
                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.State:
#if DEBUG
                        //LogInstance.Log($"initRelation = {initRelation}");
                        //LogInstance.Log($"kindOfSpecialRelation = {kindOfSpecialRelation}");
#endif
                        if (!relationsForRemoving.Contains(initRelation))
                        {
#if DEBUG
                            //LogInstance.Log("NEXT");
#endif
                            ModifyClusterAroundSpecialRelation(source, initRelation, kindOfSpecialRelation, context);

                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.Agent:
#if DEBUG
                        //LogInstance.Log($"initRelation = {initRelation}");
                        //LogInstance.Log($"kindOfSpecialRelation = {kindOfSpecialRelation}");
#endif
                        if (!relationsForRemoving.Contains(initRelation))
                        {
                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.Action:
#if DEBUG
                        //LogInstance.Log($"initRelation = {initRelation}");
                        //LogInstance.Log($"kindOfSpecialRelation = {kindOfSpecialRelation}");
#endif

                        if (!relationsForRemoving.Contains(initRelation))
                        {
#if DEBUG
                            //LogInstance.Log("NEXT");
#endif
                            ModifyClusterAroundSpecialRelation(source, initRelation, kindOfSpecialRelation, context);

                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.Command:
#if DEBUG
                        //LogInstance.Log($"initRelation = {initRelation}");
                        //LogInstance.Log($"kindOfSpecialRelation = {kindOfSpecialRelation}");
#endif
                        //Please create yet another list for annotations for command

                        if (!relationsForRemoving.Contains(initRelation))
                        {
#if DEBUG
                            //LogInstance.Log("NEXT");
#endif
                            ModifyClusterAroundSpecialRelation(source, initRelation, kindOfSpecialRelation, context);

                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(kindOfSpecialRelation), kindOfSpecialRelation, null);
                }
            }

#if DEBUG
            //LogInstance.Log($"relationsForRemoving.Count = {relationsForRemoving.Count}");
#endif
            foreach (var relationForRemoving in relationsForRemoving)
            {
#if DEBUG
                //LogInstance.Log($"relationForRemoving = {relationForRemoving}");
#endif
                relationForRemoving.Destroy();
            }

#if DEBUG
            //var dotStr = DotConverter.ConvertToString(source);
            //LogInstance.Log($"dotStr (3) = {dotStr}");
            //throw new NotImplementedException();
#endif
        }

        private void ModifyClusterAroundSpecialRelation(InternalConceptualGraph source, InternalRelationCGNode relation, KindOfSpecialRelation kindOfSpecialRelation, ContextOfConvertingInternalCGToFact context)
        {
            var firstConceptsList = relation.Inputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).ToList();

#if DEBUG
            _logger.Log($"kindOfSpecialRelation = {kindOfSpecialRelation}");
            _logger.Log($"firstConceptsList.Count = {firstConceptsList.Count}");
#endif

            var outputNode = relation.Outputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

#if DEBUG
            _logger.Log($"outputNode = {outputNode}");
#endif

            context.AddRelationAsAnnotation(outputNode, relation);

            if (kindOfSpecialRelation == KindOfSpecialRelation.Command)
            {
                return;
            }

            var objectRelationsList = outputNode.Outputs.Where(p => p.IsRelationNode && p.Name == SpecialNamesOfRelations.ObjectRelationName).Select(p => p.AsRelationNode).ToList();

#if DEBUG
            _logger.Log($"objectRelationsList.Count = {objectRelationsList.Count}");
#endif

            var hasAnotherRelations = outputNode.Outputs.Any(p => p.IsRelationNode && p.Name != SpecialNamesOfRelations.ObjectRelationName && p.Name != SpecialNamesOfRelations.ExperiencerRelationName);

#if DEBUG
            _logger.Log($"hasAnotherRelations = {hasAnotherRelations}");
#endif

            var linkedVarName = string.Empty;
            var relationName = outputNode.Name;
           
            if (hasAnotherRelations)
            {
                source.MaxVarCount++;
                var n = source.MaxVarCount;
#if DEBUG
                _logger.Log($"n = {n}");
#endif

                linkedVarName = $"$X{n}";

                source.AddLinkRelationToVarName(outputNode.Name, linkedVarName);
            }

            foreach (var firstConcept in firstConceptsList)
            {
#if DEBUG
                _logger.Log($"firstConcept = {firstConcept}");
#endif

                foreach (var objectRelation in objectRelationsList)
                {
#if DEBUG
                    _logger.Log($"objectRelation = {objectRelation}");
#endif

                    var objectsConceptsList = objectRelation.Outputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).ToList();

#if DEBUG
                    _logger.Log($"objectsConceptsList.Count = {objectsConceptsList.Count}");
#endif

                    foreach (var objectConcept in objectsConceptsList)
                    {
#if DEBUG
                        _logger.Log($"objectConcept = {objectConcept}");
#endif

                        var resultRelation = new InternalRelationCGNode();
                        resultRelation.Parent = source;
                        resultRelation.Name = relationName;
                        resultRelation.KindOfSpecialRelation = kindOfSpecialRelation;
                        resultRelation.LinkedVarName = linkedVarName;

                        resultRelation.AddInputNode(firstConcept);
                        resultRelation.AddOutputNode(objectConcept);

                        context.ModifiedRelationsDict[resultRelation] = outputNode;
                    }
                }
            }

#if DEBUG
            var dotStr = DotConverter.ConvertToString(source);
            _logger.Log($"dotStr (4) = {dotStr}");
            //throw new NotImplementedException();
#endif
        }

        private void PrepareForEntityConditionExpression(InternalConceptualGraph source, RuleInstance dest, ContextOfConvertingInternalCGToFact context)
        {
#if DEBUG
            var dotStr = DotConverter.ConvertToString(source);
            _logger.Log($"dotStr = {dotStr}");
            //throw new NotImplementedException();
#endif

            var conceptsNamesDict = new Dictionary<string, string>();
            var processedInRelationsConceptsList = new List<string>();

            //var variablesQuantification = dest.VariablesQuantification;

            var initRelationsList = source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).ToList();

#if DEBUG
            _logger.Log($"initRelationsList.Count = {initRelationsList.Count}");
#endif

            var n = 0;

            foreach (var initRelation in initRelationsList)
            {
#if DEBUG
                _logger.Log($"initRelation = {initRelation}");
#endif

                var inputNodesList = initRelation.Inputs.Where(p => p.IsConceptNode).Select(p => p.AsConceptNode).ToList();

                var outputNodesList = initRelation.Outputs.Where(p => p.IsConceptNode).Select(p => p.AsConceptNode).ToList();

                var allNodesList = inputNodesList.Concat(outputNodesList);

                foreach(var inputNode in allNodesList)
                {
#if DEBUG
                    _logger.Log($"inputNode = {inputNode}");
#endif

                    var conceptName = inputNode.Name;

#if DEBUG
                    _logger.Log($"conceptName = {conceptName}");
#endif

                    //if(conceptName == "_someone")
                    //{
                    //    break;
                    //}

                    if (conceptName != "i" && !conceptsNamesDict.ContainsKey(conceptName))
                    {
                        var varName = string.Empty;

                        if (inputNode.IsRootConceptOfEntitiCondition)
                        {
                            varName = "$_";
                        }
                        else
                        {
                            n++;

                            varName = $"$X{n}";
                        }

                        conceptsNamesDict[conceptName] = varName;

#if DEBUG
                        _logger.Log($"varName = {varName}");
#endif

                        //var varQuant_1 = new VarExpressionNode();
                        //varQuant_1.Quantifier = KindOfQuantifier.Existential;
                        //varQuant_1.Name = varName;
                        //variablesQuantification.Items.Add(varQuant_1);
                    }
                }

                //                var inputNode = initRelation.Inputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

                //#if DEBUG
                //                _logger.Log($"inputNode = {inputNode}");
                //#endif

                //                var kindOfInputNode = inputNode.KindOfGraphOrConcept;

                //#if DEBUG
                //                _logger.Log($"kindOfInputNode = {kindOfInputNode}");
                //#endif

                //                switch (kindOfInputNode)
                //                {
                //                    case KindOfInternalGraphOrConceptNode.Concept:
                //                        {
                //                            var conceptName = inputNode.Name;

                //#if DEBUG
                //                            _logger.Log($"conceptName = {conceptName}");
                //#endif

                //                            //if(conceptName == "_someone")
                //                            //{
                //                            //    break;
                //                            //}

                //                            if (/*conceptName != "i" ||*/ !conceptsNamesDict.ContainsKey(conceptName))
                //                            {
                //                                n++;

                //                                var varName = $"$X{n}";
                //                                conceptsNamesDict[conceptName] = varName;

                //                                //var varQuant_1 = new VarExpressionNode();
                //                                //varQuant_1.Quantifier = KindOfQuantifier.Existential;
                //                                //varQuant_1.Name = varName;
                //                                //variablesQuantification.Items.Add(varQuant_1);
                //                            }
                //                        }
                //                        break;

                //                    default: throw new ArgumentOutOfRangeException(nameof(kindOfInputNode), kindOfInputNode, null);
                //                }
            }

            foreach (var initRelation in initRelationsList)
            {
#if DEBUG
                _logger.Log($"initRelation = {initRelation}");
#endif

                var inputNode = initRelation.Inputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

#if DEBUG
                _logger.Log($"inputNode = {inputNode}");
#endif

                var outputNode = initRelation.Outputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

#if DEBUG
                _logger.Log($"outputNode = {outputNode}");
#endif

                PrepareRealtionForEntityConditionExpression(inputNode, initRelation, true, conceptsNamesDict, ref processedInRelationsConceptsList, context);
                PrepareRealtionForEntityConditionExpression(outputNode, initRelation, false, conceptsNamesDict, ref processedInRelationsConceptsList, context);

                //throw new NotImplementedException();
            }

#if DEBUG
            _logger.Log($"processedInRelationsConceptsList.Count = {processedInRelationsConceptsList.Count}");
#endif

            foreach (var processedInRelationsConcept in processedInRelationsConceptsList)
            {
#if DEBUG
                _logger.Log($"processedInRelationsConcept = {processedInRelationsConcept}");
#endif

                //if(processedInRelationsConcept == "i")
                //{
                //    continue;
                //}

                var varName = conceptsNamesDict[processedInRelationsConcept];

#if DEBUG
                _logger.Log($"varName = {varName}");
#endif

                var realtionForClass = new InternalRelationCGNode();
                realtionForClass.Parent = source;
                realtionForClass.Name = processedInRelationsConcept;

                var varNode = new InternalConceptCGNode();
                varNode.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.Variable;
                varNode.Name = varName;

                realtionForClass.AddInputNode(varNode);

#if DEBUG
                _logger.Log($"realtionForClass = {realtionForClass}");
#endif
            }

            //throw new NotImplementedException();
        }

        private void PrepareRealtionForEntityConditionExpression(BaseInternalConceptCGNode conceptNode, InternalRelationCGNode relation, bool isInputNode, Dictionary<string, string> conceptsNamesDict, ref List<string> processedInRelationsConceptsList, ContextOfConvertingInternalCGToFact context)
        {
#if DEBUG
            _logger.Log($"conceptNode = {conceptNode}");
            _logger.Log($"relation = {relation}");
            _logger.Log($"isInputNode = {isInputNode}");
#endif

            var kindOfInputNode = conceptNode.KindOfGraphOrConcept;

            switch (kindOfInputNode)
            {
                case KindOfInternalGraphOrConceptNode.Concept:
                    {
                        var conceptName = conceptNode.Name;

#if DEBUG
                        _logger.Log($"conceptName = {conceptName}");
                        _logger.Log($"conceptsNamesDict.Count = {conceptsNamesDict.Count}");
                        _logger.Log($"conceptsNamesDict.Keys = {conceptsNamesDict.Keys.ToList().WritePODListToString()}");
#endif

                        if (conceptsNamesDict.ContainsKey(conceptName))
                        {
                            var varName = conceptsNamesDict[conceptName];

#if DEBUG
                            _logger.Log($"varName = {varName}");
#endif

                            var varNode = new InternalConceptCGNode();
                            varNode.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.Variable;
                            varNode.Name = varName;

                            processedInRelationsConceptsList.Add(conceptName);

                            if (isInputNode)
                            {
                                relation.RemoveInputNode(conceptNode);
                                relation.AddInputNode(varNode);
                            }
                            else
                            {
                                relation.RemoveOutputNode(conceptNode);
                                relation.AddOutputNode(varNode);
                            }
                        }
                    }
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(kindOfInputNode), kindOfInputNode, null);
            }
        }

        private LogicalQueryNode CreateExpressionByWholeGraph(InternalConceptualGraph source, ContextOfConvertingInternalCGToFact context, RuleInstance ruleInstance, ContextForSingleRuleInstanceOfConvertingInternalCGToFact contextForSingleRuleInstance)
        {
            var relationsList = source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).ToList();

#if DEBUG
            if(ruleInstance.KindOfRuleInstance == KindOfRuleInstance.EntityCondition)
            {
                _logger.Log($"relationsList.Count = {relationsList.Count}");
            }            
#endif

            if (relationsList.Count == 0)
            {
                return null;
            }

            if (relationsList.Count == 1)
            {
                return CreateExpressionByRelation(relationsList.Single(), source, ruleInstance, context, contextForSingleRuleInstance);
            }

            var result = new LogicalQueryNode {Kind = KindOfLogicalQueryNode.BinaryOperator, KindOfOperator = KindOfOperatorOfLogicalQueryNode.And };

            var relationsListEnumerator = relationsList.GetEnumerator();

            LogicalQueryNode currentAndNode = result;
            LogicalQueryNode prevRelation = null;

            var n = 0;

            while (relationsListEnumerator.MoveNext())
            {
                n++;
                var relation = relationsListEnumerator.Current;

                var relationExpr = CreateExpressionByRelation(relation, source, ruleInstance, context, contextForSingleRuleInstance);

#if DEBUG
                //LogInstance.Log($"n = {n} relationExpr = {relationExpr}");
#endif

                if (prevRelation == null)
                {
                    currentAndNode.Left = relationExpr;
                }
                else
                {
                    if (n == relationsList.Count)
                    {
                        currentAndNode.Right = relationExpr;
                    }
                    else
                    {
                        var tmpAndNode = new LogicalQueryNode { Kind = KindOfLogicalQueryNode.BinaryOperator, KindOfOperator = KindOfOperatorOfLogicalQueryNode.And };
                        currentAndNode.Right = tmpAndNode;
                        currentAndNode = tmpAndNode;
                        currentAndNode.Left = relationExpr;
                    }
                }

                prevRelation = relationExpr;
            }

            return result;
        }

        private LogicalQueryNode CreateExpressionByRelation(InternalRelationCGNode relation, InternalConceptualGraph internalConceptualGraph, RuleInstance ruleInstance, ContextOfConvertingInternalCGToFact context, ContextForSingleRuleInstanceOfConvertingInternalCGToFact contextForSingleRuleInstance)
        {
#if DEBUG
            if (ruleInstance.KindOfRuleInstance == KindOfRuleInstance.EntityCondition)
            {
                _logger.Log($"relation (!) = {relation}");
            }            
#endif

            var linkedVarName = internalConceptualGraph.GetVarNameForRelation(relation.Name);

#if DEBUG
            if (ruleInstance.KindOfRuleInstance == KindOfRuleInstance.EntityCondition)
            {
                _logger.Log($"linkedVarName = {linkedVarName}");
            }            
#endif

            var relationExpr = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.Relation };
            relationExpr.Name = NameHelper.CreateName(relation.Name);
            relationExpr.ParamsList = new List<LogicalQueryNode>();

            var inputNode = relation.Inputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

#if DEBUG
            if (ruleInstance.KindOfRuleInstance == KindOfRuleInstance.EntityCondition)
            {
                _logger.Log($"inputNode = {inputNode}");
            }            
#endif
            if (inputNode != null)
            {
                var inputNodeExpr = CreateExpressionByGraphOrConceptNode(inputNode, internalConceptualGraph, ruleInstance, context, contextForSingleRuleInstance);

                relationExpr.ParamsList.Add(inputNodeExpr);
            }

            var outputNode = relation.Outputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

#if DEBUG
            if (ruleInstance.KindOfRuleInstance == KindOfRuleInstance.EntityCondition)
            {
                _logger.Log($"outputNode = {outputNode}");
            }            
#endif

            if (outputNode != null)
            {
                var outputNodeExpr = CreateExpressionByGraphOrConceptNode(outputNode, internalConceptualGraph, ruleInstance, context, contextForSingleRuleInstance);

                relationExpr.ParamsList.Add(outputNodeExpr);
            }

            if (!string.IsNullOrWhiteSpace(linkedVarName))
            {
                throw new NotImplementedException();

                //var varNodeForRelation = new VarExpressionNode();
                //varNodeForRelation.Quantifier = KindOfQuantifier.Existential;
                //varNodeForRelation.Name = linkedVarName;

                //relationExpr.LinkedVars = new List<VarExpressionNode>();
                //relationExpr.LinkedVars.Add(varNodeForRelation);

                //var varNodeForQuantification = new VarExpressionNode();
                //varNodeForQuantification.Quantifier = KindOfQuantifier.Existential;
                //varNodeForQuantification.Name = linkedVarName;

                //ruleInstance.VariablesQuantification.Items.Add(varNodeForQuantification);
            }

            //var annotionRelationsList = context.GetAnnotationRelations(relation);

            //#if DEBUG
            //            _logger.Log($"relation (!!) = {relation}");
            //            _logger.Log($"annotadedRelationsList.Count = {annotionRelationsList.Count}");
            //#endif

            //            if (annotionRelationsList.Count > 0)
            //            {
            //                foreach (var annotionRelation in annotionRelationsList)
            //                {
            //                    _logger.Log($"annotionRelation = {annotionRelation}");

            //                    AddAnnotationForRelation(annotionRelation.Name, relationExpr, context, contextForSingleRuleInstance);
            //                }

            //                //throw new NotImplementedException();
            //            }



#if DEBUG
            if (ruleInstance.KindOfRuleInstance == KindOfRuleInstance.EntityCondition)
            {
                _logger.Log($"relationExpr = {relationExpr}");
            }            
#endif

            //throw new NotImplementedException();
            return relationExpr;
        }

        private void AddAnnotationForRelation(string annotationName, LogicalQueryNode relation, ContextOfConvertingInternalCGToFact context, ContextForSingleRuleInstanceOfConvertingInternalCGToFact contextForSingleRuleInstance)
        {
#if DEBUG
            //LogInstance.Log($"annotationName = {annotationName}");
            //LogInstance.Log($"relation = {relation}");
#endif

            return;

            //var annotationInstance = new RuleInstance();
            //annotationInstance.KindOfRuleInstance = KindOfRuleInstance.Annotation;
            //var name = NameHelper.CreateRuleOrFactName();
            //annotationInstance.Name = name;

            //var partOfAnnotation = new PrimaryRulePart();
            //partOfAnnotation.IsActive = true;
            //partOfAnnotation.Parent = annotationInstance;
            //annotationInstance.PrimaryPart = partOfAnnotation;

            //var relationOfAnnotation = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.Relation };
            //partOfAnnotation.Expression = relationOfAnnotation;
            //name = NameHelper.CreateName(annotationName);
            //relationOfAnnotation.Name = name;
            //relationOfAnnotation.ParamsList = new List<LogicalQueryNode>();

            //var param = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.Var };
            //relationOfAnnotation.ParamsList.Add(param);
            //var varName = "$X";
            //param.Name = NameHelper.CreateName(varName);
            ////param.Quantifier = KindOfQuantifier.Existential;

            ////var variablesQuantification = new VariablesQuantificationPart();
            ////annotationInstance.VariablesQuantification = variablesQuantification;
            ////variablesQuantification.Items = new List<VarExpressionNode>();

            ////var varQuant_1 = new VarExpressionNode();
            ////varQuant_1.Quantifier = KindOfQuantifier.Existential;
            ////varQuant_1.Name = varName;
            ////variablesQuantification.Items.Add(varQuant_1);

            //context.AnnotationsList.Add(annotationInstance);

            ////var annotation = new LogicalAnnotation();
            //if (relation.Annotations.IsNullOrEmpty())
            //{
            //    relation.Annotations = new List<RuleInstance>();
            //}

            //relation.Annotations.Add(annotationInstance);
            ////annotation.RuleInstanceKey = annotationInstance.Key;
        }

        private LogicalQueryNode CreateExpressionByGraphOrConceptNode(BaseInternalConceptCGNode graphOrConcept, InternalConceptualGraph internalConceptualGraph, RuleInstance ruleInstance, ContextOfConvertingInternalCGToFact context, ContextForSingleRuleInstanceOfConvertingInternalCGToFact contextForSingleRuleInstance)
        {
#if DEBUG
            if (ruleInstance.KindOfRuleInstance == KindOfRuleInstance.EntityCondition)
            {
                _logger.Log($"graphOrConcept = {graphOrConcept}");
                _logger.Log($"graphOrConcept.GetType().FullName = {graphOrConcept.GetType().FullName}");
            }
#endif

            var kindOfGraphOrConcept = graphOrConcept.KindOfGraphOrConcept;

            switch (kindOfGraphOrConcept)
            {
                case KindOfInternalGraphOrConceptNode.Concept:
                    {
                        var linkedVarName = internalConceptualGraph.GetVarNameForRelation(graphOrConcept.Name);

#if DEBUG
                        if (ruleInstance.KindOfRuleInstance == KindOfRuleInstance.EntityCondition)
                        {
                            _logger.Log($"linkedVarName = {linkedVarName}");
                        }                       
#endif

                        var conceptExpression = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.Concept };

                        if (string.IsNullOrWhiteSpace(linkedVarName))
                        {
                            conceptExpression.Name = NameHelper.CreateName(graphOrConcept.Name);
                        }
                        else
                        {
                            conceptExpression.Name = NameHelper.CreateName(linkedVarName);
                        }

                        return conceptExpression;
                    }

                case KindOfInternalGraphOrConceptNode.EntityCondition:
                    {
                        var futureEntityName = graphOrConcept.Name;

#if DEBUG
                        _logger.Log($"futureEntityName = {futureEntityName}");
                        var dotStr = DotConverter.ConvertToString(internalConceptualGraph);
                        _logger.Log($"dotStr = '{dotStr}'");
                        dotStr = DotConverter.ConvertToString(graphOrConcept);
                        _logger.Log($"dotStr (~1) = '{dotStr}'");
                        _logger.Log($"graphOrConcept.IsConceptualGraph = {graphOrConcept.IsConceptualGraph}");
                        _logger.Log($"graphOrConcept.AsConceptualGraph = {graphOrConcept.AsConceptualGraph}");
#endif

                        if(!graphOrConcept.IsConceptualGraph)
                        {
                            throw new NotImplementedException();
                        }

                        var targetRuleInstance = context.RuleInstancesDict[graphOrConcept.AsConceptualGraph];

#if DEBUG
                        _logger.Log($"context.RuleInstancesDict.Count = {context.RuleInstancesDict.Count}");
                        _logger.Log($"context.RuleInstancesDict.Keys = {context.RuleInstancesDict.Keys.Select(p => p.Name).WritePODListToString()}");

                        var debugStr = DebugHelperForRuleInstance.ToString(targetRuleInstance);
                        _logger.Log($"debugStr = {debugStr}");
#endif

                        var entityCondition = new ConditionalEntitySourceValue(targetRuleInstance, NameHelper.CreateName("#@"));

                        var value = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.Value };
                        value.Value = entityCondition;

                        return value;
                    }

                case KindOfInternalGraphOrConceptNode.Variable:
                    {
                        var varExpr = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.Var };
                        varExpr.Name = NameHelper.CreateName(graphOrConcept.Name);
                        return varExpr;
                    }

                default: throw new ArgumentOutOfRangeException(nameof(kindOfGraphOrConcept), kindOfGraphOrConcept, null);
            }
        }
    }
}

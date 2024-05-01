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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToFact
{
    public class ConverterInternalCGToFact
    {
        public ConverterInternalCGToFact(IMonitorLogger logger)
        {
            _logger = logger;
        }

        private readonly IMonitorLogger _logger;

        public IList<RuleInstance> ConvertConceptualGraph(InternalConceptualGraph source)
        {
            var context = new ContextOfConvertingInternalCGToFact();

            return ConvertConceptualGraph(source, context);
        }

        private IList<RuleInstance> ConvertConceptualGraph(InternalConceptualGraph source, ContextOfConvertingInternalCGToFact context)
        {
            PreliminaryCreation(source, context);

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

            foreach (var graphsChild in graphsChildrenList)
            {
                PreliminaryCreation(graphsChild, context);
            }

        }

        private void FillRuleInstances(InternalConceptualGraph source, RuleInstance dest, ContextOfConvertingInternalCGToFact context)
        {
            if(source.Mood == GrammaticalMood.Imperative)
            {
                dest.ObligationModality = LogicalValue.TrueValue;
            }
            else
            {
                if(source.ObligationModality != ObligationModality.Undefined && source.ObligationModality != ObligationModality.None)
                {
                    throw new NotImplementedException();
                }
            }

            var contextForSingleRuleInstance = new ContextForSingleRuleInstanceOfConvertingInternalCGToFact();
            contextForSingleRuleInstance.CurrentRuleInstance = dest;


            var part = new PrimaryRulePart();
            dest.PrimaryPart = part;
            part.Parent = dest;
            part.IsActive = true;

            var kindOfGraphOrConcept = source.KindOfGraphOrConcept;

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

            var expression = CreateExpressionByWholeGraph(source, context, dest, part, contextForSingleRuleInstance);


            part.Expression = expression;








        }

        private void PrepareForGraphConditionExpression(InternalConceptualGraph source, RuleInstance dest, ContextOfConvertingInternalCGToFact context)
        {
            var processedSpecialConceptsList = new List<BaseInternalConceptCGNode>();
            var relationsForRemoving = new List<InternalRelationCGNode>();

            var initRelationsList = source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).ToList();

            foreach (var initRelation in initRelationsList)
            {
                var kindOfSpecialRelation = SpecialElementsHelper.GetKindOfSpecialRelation(initRelation.Name);
                initRelation.KindOfSpecialRelation = kindOfSpecialRelation;
                switch (kindOfSpecialRelation)
                {
                    case KindOfSpecialRelation.Undefinded:
                        break;

                    case KindOfSpecialRelation.Object:
                        if (!relationsForRemoving.Contains(initRelation))
                        {
                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.Experiencer:
                        if (!relationsForRemoving.Contains(initRelation))
                        {
                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.State:
                        if (!relationsForRemoving.Contains(initRelation))
                        {
                            ModifyClusterAroundSpecialRelation(source, initRelation, kindOfSpecialRelation, context);

                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.Agent:
                        if (!relationsForRemoving.Contains(initRelation))
                        {
                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.Action:
                        if (!relationsForRemoving.Contains(initRelation))
                        {
                            ModifyClusterAroundSpecialRelation(source, initRelation, kindOfSpecialRelation, context);

                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    case KindOfSpecialRelation.Command:
                        if (!relationsForRemoving.Contains(initRelation))
                        {
                            ModifyClusterAroundSpecialRelation(source, initRelation, kindOfSpecialRelation, context);

                            relationsForRemoving.Add(initRelation);
                        }
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(kindOfSpecialRelation), kindOfSpecialRelation, null);
                }
            }

            foreach (var relationForRemoving in relationsForRemoving)
            {
                relationForRemoving.Destroy();
            }

        }

        private void ModifyClusterAroundSpecialRelation(InternalConceptualGraph source, InternalRelationCGNode relation, KindOfSpecialRelation kindOfSpecialRelation, ContextOfConvertingInternalCGToFact context)
        {
            var firstConceptsList = relation.Inputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).ToList();

            var outputNode = relation.Outputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

            context.AddRelationAsAnnotation(outputNode, relation);

            if (kindOfSpecialRelation == KindOfSpecialRelation.Command)
            {
                return;
            }

            var objectRelationsList = outputNode.Outputs.Where(p => p.IsRelationNode && p.Name == SpecialNamesOfRelations.ObjectRelationName).Select(p => p.AsRelationNode).ToList();

            var hasAnotherRelations = outputNode.Outputs.Any(p => p.IsRelationNode && p.Name != SpecialNamesOfRelations.ObjectRelationName && p.Name != SpecialNamesOfRelations.ExperiencerRelationName);

            var linkedVarName = string.Empty;
            var relationName = outputNode.Name;
           
            if (hasAnotherRelations)
            {
                source.MaxVarCount++;
                var n = source.MaxVarCount;
                linkedVarName = $"$X{n}";

                source.AddLinkRelationToVarName(outputNode.Name, linkedVarName);
            }

            foreach (var firstConcept in firstConceptsList)
            {
                foreach (var objectRelation in objectRelationsList)
                {
                    var objectsConceptsList = objectRelation.Outputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).ToList();

                    foreach (var objectConcept in objectsConceptsList)
                    {
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

        }

        private void PrepareForEntityConditionExpression(InternalConceptualGraph source, RuleInstance dest, ContextOfConvertingInternalCGToFact context)
        {
            var conceptsNamesDict = new Dictionary<string, string>();
            var processedInRelationsConceptsList = new List<string>();


            var initRelationsList = source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).ToList();

            var n = 0;

            foreach (var initRelation in initRelationsList)
            {
                var inputNodesList = initRelation.Inputs.Where(p => p.IsConceptNode).Select(p => p.AsConceptNode).ToList();

                var outputNodesList = initRelation.Outputs.Where(p => p.IsConceptNode).Select(p => p.AsConceptNode).ToList();

                var allNodesList = inputNodesList.Concat(outputNodesList);

                foreach(var inputNode in allNodesList)
                {
                    var conceptName = inputNode.Name;


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

                    }
                }











            }

            foreach (var initRelation in initRelationsList)
            {
                var inputNode = initRelation.Inputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

                var outputNode = initRelation.Outputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

                PrepareRealtionForEntityConditionExpression(inputNode, initRelation, true, conceptsNamesDict, ref processedInRelationsConceptsList, context);
                PrepareRealtionForEntityConditionExpression(outputNode, initRelation, false, conceptsNamesDict, ref processedInRelationsConceptsList, context);

            }

            foreach (var processedInRelationsConcept in processedInRelationsConceptsList)
            {

                var varName = conceptsNamesDict[processedInRelationsConcept];

                var realtionForClass = new InternalRelationCGNode();
                realtionForClass.Parent = source;
                realtionForClass.Name = processedInRelationsConcept;

                var varNode = new InternalConceptCGNode();
                varNode.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.Variable;
                varNode.Name = varName;

                realtionForClass.AddInputNode(varNode);

            }

        }

        private void PrepareRealtionForEntityConditionExpression(BaseInternalConceptCGNode conceptNode, InternalRelationCGNode relation, bool isInputNode, Dictionary<string, string> conceptsNamesDict, ref List<string> processedInRelationsConceptsList, ContextOfConvertingInternalCGToFact context)
        {
            var kindOfInputNode = conceptNode.KindOfGraphOrConcept;

            switch (kindOfInputNode)
            {
                case KindOfInternalGraphOrConceptNode.Concept:
                    {
                        var conceptName = conceptNode.Name;

                        if (conceptsNamesDict.ContainsKey(conceptName))
                        {
                            var varName = conceptsNamesDict[conceptName];

                            var varNode = new InternalConceptCGNode();
                            varNode.KindOfGraphOrConcept = KindOfInternalGraphOrConceptNode.Variable;
                            varNode.Parent = conceptNode.Parent;
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

        private LogicalQueryNode CreateExpressionByWholeGraph(InternalConceptualGraph source, ContextOfConvertingInternalCGToFact context, RuleInstance ruleInstance, BaseRulePart part, ContextForSingleRuleInstanceOfConvertingInternalCGToFact contextForSingleRuleInstance)
        {
            var relationsList = source.Children.Where(p => p.IsRelationNode).Select(p => p.AsRelationNode).ToList();

            if (relationsList.Count == 0)
            {
                return null;
            }

            if (relationsList.Count == 1)
            {
                return CreateExpressionByRelation(relationsList.Single(), source, ruleInstance, part, context, contextForSingleRuleInstance);
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

                var relationExpr = CreateExpressionByRelation(relation, source, ruleInstance, part, context, contextForSingleRuleInstance);

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

        private LogicalQueryNode CreateExpressionByRelation(InternalRelationCGNode relation, InternalConceptualGraph internalConceptualGraph, RuleInstance ruleInstance, BaseRulePart part, ContextOfConvertingInternalCGToFact context, ContextForSingleRuleInstanceOfConvertingInternalCGToFact contextForSingleRuleInstance)
        {
            var linkedVarName = internalConceptualGraph.GetVarNameForRelation(relation.Name);

            var relationExpr = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.Relation };
            relationExpr.Name = NameHelper.CreateName(relation.Name);
            relationExpr.ParamsList = new List<LogicalQueryNode>();

            var inputNode = relation.Inputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

            if (inputNode != null)
            {
                var inputNodeExpr = CreateExpressionByGraphOrConceptNode(inputNode, internalConceptualGraph, ruleInstance, context, contextForSingleRuleInstance);

                relationExpr.ParamsList.Add(inputNodeExpr);
            }

            var outputNode = relation.Outputs.Where(p => p.IsGraphOrConceptNode).Select(p => p.AsGraphOrConceptNode).FirstOrDefault();

            if (outputNode != null)
            {
                var outputNodeExpr = CreateExpressionByGraphOrConceptNode(outputNode, internalConceptualGraph, ruleInstance, context, contextForSingleRuleInstance);

                relationExpr.ParamsList.Add(outputNodeExpr);
            }

            if (!string.IsNullOrWhiteSpace(linkedVarName))
            {


                var linkedVarNameValue = NameHelper.CreateName(linkedVarName);

                if (part.AliasesDict == null)
                {
                    part.AliasesDict = new Dictionary<StrongIdentifierValue, LogicalQueryNode>();
                }

                part.AliasesDict[NameHelper.CreateName(linkedVarName)] = relationExpr;

                var varNodeForRelation = new LogicalQueryNode() { Kind = KindOfLogicalQueryNode.LogicalVar };
                varNodeForRelation.Name = linkedVarNameValue;

                relationExpr.LinkedVars = new List<LogicalQueryNode>();
                relationExpr.LinkedVars.Add(varNodeForRelation);
            }









            return relationExpr;
        }

        private void AddAnnotationForRelation(string annotationName, LogicalQueryNode relation, ContextOfConvertingInternalCGToFact context, ContextForSingleRuleInstanceOfConvertingInternalCGToFact contextForSingleRuleInstance)
        {
            return;




            ////param.Quantifier = KindOfQuantifier.Existential;

            ////var variablesQuantification = new VariablesQuantificationPart();
            ////annotationInstance.VariablesQuantification = variablesQuantification;
            ////variablesQuantification.Items = new List<VarExpressionNode>();

            ////var varQuant_1 = new VarExpressionNode();
            ////varQuant_1.Quantifier = KindOfQuantifier.Existential;
            ////varQuant_1.Name = varName;
            ////variablesQuantification.Items.Add(varQuant_1);


            ////var annotation = new LogicalAnnotation();

            ////annotation.RuleInstanceKey = annotationInstance.Key;
        }

        private LogicalQueryNode CreateExpressionByGraphOrConceptNode(BaseInternalConceptCGNode graphOrConcept, InternalConceptualGraph internalConceptualGraph, RuleInstance ruleInstance, ContextOfConvertingInternalCGToFact context, ContextForSingleRuleInstanceOfConvertingInternalCGToFact contextForSingleRuleInstance)
        {
            var kindOfGraphOrConcept = graphOrConcept.KindOfGraphOrConcept;

            switch (kindOfGraphOrConcept)
            {
                case KindOfInternalGraphOrConceptNode.Concept:
                    {
                        var linkedVarName = internalConceptualGraph.GetVarNameForRelation(graphOrConcept.Name);

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

                        if(!graphOrConcept.IsConceptualGraph)
                        {
                            throw new NotImplementedException();
                        }

                        var targetRuleInstance = context.RuleInstancesDict[graphOrConcept.AsConceptualGraph];

#if DEBUG

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

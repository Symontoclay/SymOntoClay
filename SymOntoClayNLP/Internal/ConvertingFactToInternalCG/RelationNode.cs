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

using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class RelationNode: BaseNode
    {
        public RelationNode(LogicalQueryNode relation, ContextOfConverterFactToInternalCG context)
            : base(context)
        {
            _relation = relation;

            var nlpContext = context.NLPContext;

            _relationsResolver = nlpContext.RelationsResolver;
            _inheritanceResolver = nlpContext.InheritanceResolver;
        }

        private readonly LogicalQueryNode _relation;
        
        private readonly IPackedRelationsResolver _relationsResolver;
        private readonly IPackedInheritanceResolver _inheritanceResolver;
        
        public ResultOfNode Run(IMonitorLogger logger)
        {
            if(_context.VisitedRelations.ContainsKey(_relation))
            {
                return _context.VisitedRelations[_relation];
            }

            if(_relation.ParamsList.Count == 1)
            {
                return ProcessInheritanceRelation();
            }

            var relationName = _relation.Name.NormalizedNameValue;

            switch (relationName)
            {
                case SpecialNamesOfRelations.PossessName:
                    return ProcessPossessRelation(logger);

                default:
                    return ProcessRelation(logger);
            }
        }

        private ResultOfNode ProcessPossessRelation(IMonitorLogger logger)
        {
            var relationName = _relation.Name;

            var relationDescription = _relationsResolver.GetRelation(logger, relationName, _relation.ParamsList.Count);

            if (relationDescription == null)
            {
                throw new Exception($"Relation `{_relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}` is not described!");
            }

            var result = new ResultOfNode();
            _context.VisitedRelations[_relation] = result;
            result.LogicalQueryNode = _relation;
            result.KindOfResult = KindOfResultOfNode.ProcessRelation;

            var relationRelation = new InternalRelationCGNode() { Name = relationName.NameValue.Replace("`", string.Empty), Parent = _context.ConceptualGraph };

            result.CGNode = relationRelation;

            var n = 0;

            foreach (var param in _relation.ParamsList)
            {
                var paramResult = LogicalQueryNodeProcessorFactory.Run(logger, param, _context);

                var paramDescription = relationDescription.Arguments[n];
                n++;

                var meaningRolesList = paramDescription.MeaningRolesList.Select(p => ValueToNormalizedNameValue(p));

                if (meaningRolesList.Contains("owner"))
                {
                    var ownerConcept = paramResult.CGNode.AsGraphOrConceptNode;

                    relationRelation.AddInputNode(ownerConcept);
                }
                else
                {
                    if (meaningRolesList.Contains("possessions"))
                    {
                        var possessionsConcept = paramResult.CGNode.AsGraphOrConceptNode;

                        possessionsConcept.AddInputNode(relationRelation);
                    }
                    else
                    {
                        throw new NotImplementedException("BFB8FC11-691C-4168-8478-D6867DBAFEA3");
                    }
                }
            }

            return result;
        }

        private ResultOfNode ProcessRelation(IMonitorLogger logger)
        {
            var relationName = _relation.Name;

            var relationDescription = _relationsResolver.GetRelation(logger, relationName, _relation.ParamsList.Count);

            if (relationDescription == null)
            {
                throw new Exception($"Relation `{_relation.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}` is not described!");
            }
            
            var superClassesList = _inheritanceResolver.GetSuperClassesKeysList(logger, relationName);

            var isState = superClassesList.Any(p => p.NormalizedNameValue == "state");
            var isAct = superClassesList.Any(p => p.NormalizedNameValue == "act");
            var isEvent = superClassesList.Any(p => p.NormalizedNameValue == "event");

            if(isState || isAct || isEvent)
            {
                return ProcessStateActOrEventRelation(logger, relationDescription, superClassesList);
            }

            var result = new ResultOfNode();
            _context.VisitedRelations[_relation] = result;
            result.LogicalQueryNode = _relation;
            result.KindOfResult = KindOfResultOfNode.ProcessRelation;

            var relationRelation = new InternalRelationCGNode { Name = relationName.NameValue, Parent = _context.ConceptualGraph };

            result.CGNode = relationRelation;

            var n = 0;

            foreach (var param in _relation.ParamsList)
            {
                var paramResult = LogicalQueryNodeProcessorFactory.Run(logger, param, _context);

                var paramDescription = relationDescription.Arguments[n];
                n++;

                var meaningRolesList = paramDescription.MeaningRolesList.Select(p => ValueToNormalizedNameValue(p));

                if(meaningRolesList.Contains("owner"))
                {
                    var targetConcept = paramResult.CGNode.AsGraphOrConceptNode;

                    relationRelation.AddInputNode(targetConcept);
                }
                else
                {
                    var targetConcept = paramResult.CGNode.AsGraphOrConceptNode;
                    targetConcept.AddInputNode(relationRelation);
                }
            }

            return result;
        }

        private ResultOfNode ProcessStateActOrEventRelation(IMonitorLogger logger, RelationDescription relationDescription, IList<StrongIdentifierValue> superClassesList)
        {
            var relationName = _relation.Name;

            var isState = superClassesList.Any(p => p.NormalizedNameValue == "state");
            var isAct = superClassesList.Any(p => p.NormalizedNameValue == "act");
            var isEvent = superClassesList.Any(p => p.NormalizedNameValue == "event");

            var result = new ResultOfNode();
            _context.VisitedRelations[_relation] = result;
            result.LogicalQueryNode = _relation;
            result.KindOfResult = KindOfResultOfNode.ProcessRelation;

            var relationConcept = CreateOrGetExistingInternalConceptCGNode(PrepareName(relationName.NameValue));

            result.CGNode = relationConcept;

            var n = 0;

            foreach (var param in _relation.ParamsList)
            {
                var paramResult = LogicalQueryNodeProcessorFactory.Run(logger, param, _context);

                var paramDescription = relationDescription.Arguments[n];
                n++;

                var meaningRolesList = paramDescription.MeaningRolesList.Select(p => ValueToNormalizedNameValue(p));

                if (isState && meaningRolesList.Contains("experiencer"))
                {
                    var targetConcept = paramResult.CGNode.AsGraphOrConceptNode;

                    var experiencerRelation = new InternalRelationCGNode() { Name = SpecialNamesOfRelations.ExperiencerRelationName, Parent = _context.ConceptualGraph };
                    experiencerRelation.AddInputNode(relationConcept);
                    targetConcept.AddInputNode(experiencerRelation);

                    var stateRelation = new InternalRelationCGNode() { Name = SpecialNamesOfRelations.StateRelationName, Parent = _context.ConceptualGraph };
                    stateRelation.AddInputNode(targetConcept);
                    relationConcept.AddInputNode(stateRelation);
                }
                else
                {
                    if (meaningRolesList.Contains("object"))
                    {
                        var targetConcept = paramResult.CGNode.AsGraphOrConceptNode;

                        var objectRelation = new InternalRelationCGNode() { Name = SpecialNamesOfRelations.ObjectRelationName, Parent = _context.ConceptualGraph };
                        objectRelation.AddInputNode(relationConcept);
                        targetConcept.AddInputNode(objectRelation);
                    }
                    else
                    {
                        if (isAct && meaningRolesList.Contains("agent"))
                        {
                            var targetConcept = paramResult.CGNode.AsGraphOrConceptNode;

                            var agentRelation = new InternalRelationCGNode() { Name = SpecialNamesOfRelations.AgentRelationName, Parent = _context.ConceptualGraph };
                            agentRelation.AddInputNode(relationConcept);
                            targetConcept.AddInputNode(agentRelation);

                            var actRelation = new InternalRelationCGNode() { Name = SpecialNamesOfRelations.ActionRelationName, Parent = _context.ConceptualGraph };
                            relationConcept.AddInputNode(actRelation);
                            actRelation.AddInputNode(targetConcept);

                        }
                        else
                        {
                            throw new NotImplementedException("A7B6F1E1-566D-4E3E-87BE-1A98B37541C7");
                        }
                    }
                }
            }

            return result;
        }

        private ResultOfNode ProcessInheritanceRelation()
        {
            throw new NotImplementedException("CAFDDF98-C58C-45B8-B241-6FCBD1C809A9");
        }

        private string ValueToNormalizedNameValue(Value value)
        {
            var kindOfValue = value.KindOfValue;

            switch(kindOfValue)
            {
                case KindOfValue.StrongIdentifierValue:
                    return value.AsStrongIdentifierValue.NormalizedNameValue;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }
        }
    }
}

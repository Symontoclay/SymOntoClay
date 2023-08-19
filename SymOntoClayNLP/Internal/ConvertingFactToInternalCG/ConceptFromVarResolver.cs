/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ConceptFromVarResolver
    {
        public ConceptFromVarResolver(StrongIdentifierValue name, ContextOfConverterFactToInternalCG context)
        {
            _name = name;
            _context = context;
            _logger = context.Logger;
            _visitedRelations = context.VisitedRelations;
        }

        private readonly StrongIdentifierValue _name;
        private readonly ContextOfConverterFactToInternalCG _context;
        private readonly Dictionary<LogicalQueryNode, ResultOfNode> _visitedRelations;
        private readonly IMonitorLogger _logger;

        public string Resolve()
        {
            var resolvingContext = new ContextOfConceptFromVarResolver();

            ProcessExpression(_context.CurrentRulePart.Expression, resolvingContext);

            if(resolvingContext.FoundItems.Count == 1)
            {
                return ProcessTargetResultItem(resolvingContext.FoundItems[0]);
            }

            throw new NotImplementedException();
        }

        private string ProcessTargetResultItem(ResultItemOfConceptFromVarResolver resultItem)
        {
            var relation = resultItem.Relation;

            var result = new ResultOfNode();
            result.KindOfResult = KindOfResultOfNode.ResolveVar;
            result.LogicalQueryNode = relation;

            if(relation.ParamsList.Count == 1)
            {
                _context.VisitedRelations[relation] = result;
            }            

            return resultItem.Name.NameValue;
        }

        private void ProcessExpression(LogicalQueryNode expr, ContextOfConceptFromVarResolver resolvingContext)
        {
            var kind = expr.Kind;

            switch (kind)
            {
                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.Value:
                case KindOfLogicalQueryNode.LogicalVar:
                    break;

                case KindOfLogicalQueryNode.Relation:
                    ProcessRelation(expr, resolvingContext);
                    break;

                case KindOfLogicalQueryNode.BinaryOperator:
                    ProcessExpression(expr.Left, resolvingContext);
                    ProcessExpression(expr.Right, resolvingContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private void ProcessRelation(LogicalQueryNode relation, ContextOfConceptFromVarResolver resolvingContext)
        {
            if (_visitedRelations.ContainsKey(relation))
            {
                return;
            }

            if(!relation.LinkedVars.IsNullOrEmpty())
            {
                foreach(var linkedVar in relation.LinkedVars)
                {
                    if(linkedVar.Name == _name)
                    {
                        resolvingContext.FoundItems.Add(new ResultItemOfConceptFromVarResolver()
                        {
                            Name = relation.Name,
                            Relation = relation
                        });
                    }
                }
            }

            if (relation.ParamsList.Count == 1)
            {
                ProcessInheritanceRelation(relation, resolvingContext);
                return;
            }

            foreach(var param in relation.ParamsList)
            {
                ProcessExpression(param, resolvingContext);
            }
        }

        private void ProcessInheritanceRelation(LogicalQueryNode relation, ContextOfConceptFromVarResolver resolvingContext)
        {
            var param = relation.ParamsList[0];

            if (param.Kind != KindOfLogicalQueryNode.LogicalVar)
            {
                return;
            }

            if(param.Name != _name)
            {
                return;
            }

            resolvingContext.FoundItems.Add(new ResultItemOfConceptFromVarResolver()
            {
                Name = relation.Name,
                Relation = relation
            });
        }
    }
}

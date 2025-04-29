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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.StandardFacts
{
    public class StandardFactsBuilder : IStandardFactsBuilder
    {
        private StrongIdentifierValue _targetLogicalVarName = NameHelper.CreateName("$_");
        private NumberValue _defaultInheritanceRank = new NumberValue(1);

        /// <inheritdoc/>
        public virtual LogicalQueryNode BuildPropertyVirtualRelationInstance(StrongIdentifierValue propertyName, StrongIdentifierValue propertyInstanceName, Value propertyValue)
        {
            var fact = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            fact.PrimaryPart = primaryPart;

            var relation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = propertyName
            };

            primaryPart.Expression = relation;

            relation.ParamsList = new List<LogicalQueryNode>()
                {
                    StrongIdentifierNameToLogicalQueryNode(propertyInstanceName),
                    ValueToLogicalQueryNode(propertyValue)
                };

            fact.CheckDirty();

            return relation;
        }

        public virtual RuleInstance BuildImplicitPropertyQueryInstance(StrongIdentifierValue propertyName, StrongIdentifierValue propertyInstanceName)
        {
            var fact = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            fact.PrimaryPart = primaryPart;

            var relation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = propertyName
            };

            primaryPart.Expression = relation;

            relation.ParamsList = new List<LogicalQueryNode>()
                {
                    StrongIdentifierNameToLogicalQueryNode(propertyInstanceName),
                    StrongIdentifierNameToLogicalQueryNode(_targetLogicalVarName)
                };

            fact.CheckDirty();

            return fact;
        }

        /// <inheritdoc/>
        public virtual string BuildDefaultInheritanceFactString(string obj, string superObj)
        {
            var sb = new StringBuilder();
            sb.Append("{: is (");
            sb.Append(obj);
            sb.Append(",");
            sb.Append(superObj);
            sb.Append(", 1) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildDefaultInheritanceFactInstance(string obj, string superObj)
        {
            return BuildDefaultInheritanceFactInstance(NameHelper.CreateName(obj), NameHelper.CreateName(superObj));
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildDefaultInheritanceFactInstance(StrongIdentifierValue obj, StrongIdentifierValue superObj)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("is")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StrongIdentifierNameToLogicalQueryNode(obj),
                StrongIdentifierNameToLogicalQueryNode(superObj),
                ValueToLogicalQueryNode(_defaultInheritanceRank)
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildSayFactString(string selfId, string factStr)
        {
            var sb = new StringBuilder();

            sb.Append("{: say(");
            sb.Append(selfId);
            sb.Append(", ");
            sb.Append(factStr);
            sb.Append(") :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildSayFactInstance(string selfId, RuleInstance fact)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("say")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode(selfId),
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Fact,
                    Fact = fact
                }
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildSoundFactString(double distance, float directionToPosition, string factStr)
        {
            var varName = GetTargetVarName(factStr);

            var distanceStr = distance.ToString(CultureInfo.InvariantCulture);
            var directionStr = directionToPosition.ToString(CultureInfo.InvariantCulture);

            var sb = new StringBuilder();

            sb.Append("{: ");
            sb.Append(varName);
            sb.Append(" = ");
            sb.Append(factStr);
            sb.Append($" & hear(I, {varName})");
            sb.Append($" & distance(I, {varName}, {distanceStr})");
            sb.Append($" & direction({varName}, {directionStr})");
            sb.Append($" & point({varName}, #@[{distanceStr}, {directionStr}])");
            sb.Append(" :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildSoundFactInstance(double distance, float directionToPosition, RuleInstance fact)
        {
            var varName = NameHelper.CreateName(GetTargetVarName(fact));

            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var andOp1 = new LogicalQueryNode() 
            { 
                Kind = KindOfLogicalQueryNode.BinaryOperator, 
                KindOfOperator = KindOfOperatorOfLogicalQueryNode.And 
            };

            primaryPart.Expression = andOp1;

            var andOp2 = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.BinaryOperator,
                KindOfOperator = KindOfOperatorOfLogicalQueryNode.And
            };

            andOp1.Left = andOp2;

            var andOp3 = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.BinaryOperator,
                KindOfOperator = KindOfOperatorOfLogicalQueryNode.And
            };

            andOp2.Left = andOp3;

            var andOp4 = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.BinaryOperator,
                KindOfOperator = KindOfOperatorOfLogicalQueryNode.And
            };

            andOp3.Left = andOp4;

            var factNode = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Fact
            };

            factNode.Fact = fact;

            factNode.LinkedVars = new List<LogicalQueryNode>()
            {
                StrongIdentifierNameToLogicalQueryNode(varName)
            };

            andOp4.Left = factNode;

            var hearRelation = new LogicalQueryNode() 
            { 
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("hear")
            };

            andOp4.Right = hearRelation;

            hearRelation.ParamsList = new List<LogicalQueryNode>() {
                StringNameToLogicalQueryNode("i"),
                StrongIdentifierNameToLogicalQueryNode(varName)
            };

            var distanceRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("distance")
            };

            andOp3.Right = distanceRelation;

            distanceRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode("i"),
                StrongIdentifierNameToLogicalQueryNode(varName),
                ValueToLogicalQueryNode(new NumberValue(distance))
            };

            var directionRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("direction")
            };

            andOp2.Right = directionRelation;

            directionRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StrongIdentifierNameToLogicalQueryNode(varName),
                ValueToLogicalQueryNode(new NumberValue(directionToPosition))
            };

            var pointRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("point")
            };

            andOp1.Right = pointRelation;

            pointRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StrongIdentifierNameToLogicalQueryNode(varName),
                ValueToLogicalQueryNode(new WaypointSourceValue(new NumberValue(distance), new NumberValue(directionToPosition), NameHelper.CreateName("#@")))
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildAliveFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: state(");
            sb.Append(selfId);
            sb.Append(", alive) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildAliveFactInstance(string selfId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("state")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode(selfId),
                StringNameToLogicalQueryNode("alive")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildDeadFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: state(");
            sb.Append(selfId);
            sb.Append(", dead) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildDeadFactInstance(string selfId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("state")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode(selfId),
                StringNameToLogicalQueryNode("dead")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildStopFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: act(");
            sb.Append(selfId);
            sb.Append(", stop) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildStopFactInstance(string selfId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("act")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode(selfId),
                StringNameToLogicalQueryNode("stop")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildWalkFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: act(");
            sb.Append(selfId);
            sb.Append(", walk) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildWalkFactInstance(string selfId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("act")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode(selfId),
                StringNameToLogicalQueryNode("walk")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildWalkSoundFactString()
        {
            return "{: act(someone, walk) :}";
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildWalkSoundFactInstance()
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("act")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode("someone"),
                StringNameToLogicalQueryNode("walk")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildRunFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: act(");
            sb.Append(selfId);
            sb.Append(", run) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildRunFactInstance(string selfId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("act")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode(selfId),
                StringNameToLogicalQueryNode("run")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildRunSoundFactString()
        {
            return "{: act(someone, run) :}";
        }
        
        /// <inheritdoc/>
        public virtual RuleInstance BuildRunSoundFactInstance()
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("act")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode("someone"),
                StringNameToLogicalQueryNode("run")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildHoldFactString(string selfId, string heldThingId)
        {
            var sb = new StringBuilder();

            sb.Append("{: hold(");
            sb.Append(selfId);
            sb.Append(", ");
            sb.Append(heldThingId);
            sb.Append(") :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildHoldFactInstance(string selfId, string heldThingId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("hold")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode(selfId),
                StringNameToLogicalQueryNode(heldThingId)
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildShootFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: act(");
            sb.Append(selfId);
            sb.Append(", shoot) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildShootFactInstance(string selfId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("act")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode(selfId),
                StringNameToLogicalQueryNode("shoot")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildShootSoundFactString()
        {
            return "{: act(someone, shoot) :}";
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildShootSoundFactInstance()
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("act")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode("someone"),
                StringNameToLogicalQueryNode("shoot")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildReadyForShootFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: ready(");
            sb.Append(selfId);
            sb.Append(", shoot) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildReadyForShootFactInstance(string selfId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("ready")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode(selfId),
                StringNameToLogicalQueryNode("shoot")
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildSeeFactString(string seenObjId)
        {
            var sb = new StringBuilder();
            sb.Append("{: see(I, ");
            sb.Append(seenObjId);
            sb.Append(") :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildSeeFactInstance(string seenObjId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("see")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode("I"),
                StringNameToLogicalQueryNode(seenObjId)
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildFocusFactString(string seenObjId)
        {
            var sb = new StringBuilder();
            sb.Append("{: focus(I, ");
            sb.Append(seenObjId);
            sb.Append(") :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildFocusFactInstance(string seenObjId)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("focus")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode("I"),
                StringNameToLogicalQueryNode(seenObjId)
            };

            result.CheckDirty();

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildDistanceFactString(string objId, float distance)
        {
            return NBuildDistanceFactString(objId, distance.ToString(CultureInfo.InvariantCulture));
        }

        /// <inheritdoc/>
        public virtual string BuildDistanceFactString(string objId, double distance)
        {
            return NBuildDistanceFactString(objId, distance.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual string NBuildDistanceFactString(string objId, string distanceStrValue)
        {
            return $"distance(I, {objId}, {distanceStrValue})";
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildDistanceFactInstance(string objId, float distance)
        {
            return BuildDistanceFactInstance(objId, new NumberValue(distance));
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildDistanceFactInstance(string objId, double distance)
        {
            return BuildDistanceFactInstance(objId, new NumberValue(distance));
        }

        /// <inheritdoc/>
        public virtual RuleInstance BuildDistanceFactInstance(string objId, Value distance)
        {
            var result = new RuleInstance()
            {
                Name = NameHelper.CreateRuleOrFactName()
            };
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("distance")
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                StringNameToLogicalQueryNode("I"),
                StringNameToLogicalQueryNode(objId),
                ValueToLogicalQueryNode(distance)
            };

            result.CheckDirty();

            return result;
        }

        protected virtual string GetTargetVarName(string factStr)
        {
            return RuleInstanceHelper.GetNewUniqueVarNameWithPrefix("$x", factStr);
        }

        protected virtual string GetTargetVarName(RuleInstance fact)
        {
            return RuleInstanceHelper.GetNewUniqueVarNameWithPrefix("$x", fact);
        }

        protected virtual LogicalQueryNode StringNameToLogicalQueryNode(string name)
        {
            return StrongIdentifierNameToLogicalQueryNode(NameHelper.CreateName(name));
        }

        protected virtual LogicalQueryNode StrongIdentifierNameToLogicalQueryNode(StrongIdentifierValue value)
        {
            var node = new LogicalQueryNode()
            {
                Name = value
            };

            var kindOfName = value.KindOfName;

            switch(kindOfName)
            {
                case KindOfName.Entity:
                    node.Kind = KindOfLogicalQueryNode.Entity;
                    break;

                case KindOfName.CommonConcept:
                    node.Kind = KindOfLogicalQueryNode.Concept;
                    break;

                case KindOfName.LogicalVar:
                    node.Kind = KindOfLogicalQueryNode.LogicalVar;
                    break;
                     
                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }

            return node;
        }

        protected virtual LogicalQueryNode ValueToLogicalQueryNode(Value value)
        {
            return new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Value,
                Value = value
            };
        }
    }
}

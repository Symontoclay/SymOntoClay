using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.StandardFacts
{
    public class StandardFactsBuilder : IStandardFactsBuilder
    {
        //private static readonly IEntityLogger _logger = new LoggerImpementation();

        /// <inheritdoc/>
        public virtual string BuildSayFactString(string selfId, string factStr)
        {
            var sb = new StringBuilder();

            var varName = GetTargetVarName(factStr);

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
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(selfId)
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Fact,
                    Fact = fact
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public virtual string BuildSoundFactString(double power, double distance, float directionToPosition, string factStr)
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

        public virtual RuleInstance BuildSoundFactInstance(double power, double distance, float directionToPosition, RuleInstance fact)
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.LogicalVar,
                    Name = NameHelper.CreateName("$x")
                }
            };

            andOp4.Left = factNode;

            var hearRelation = new LogicalQueryNode() 
            { 
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("hear")
            };

            andOp4.Right = hearRelation;

            hearRelation.ParamsList = new List<LogicalQueryNode>() { 
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("i")
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.LogicalVar,
                    Name = NameHelper.CreateName("$x")
                }
            };

            var distanceRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("distance")
            };

            andOp3.Right = distanceRelation;

            distanceRelation.ParamsList = new List<LogicalQueryNode>()
            {
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("i")
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.LogicalVar,
                    Name = NameHelper.CreateName("$x")
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Value,
                    Value = new NumberValue(distance)
                }
            };

            var directionRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("direction")
            };

            andOp2.Right = directionRelation;

            directionRelation.ParamsList = new List<LogicalQueryNode>()
            {
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.LogicalVar,
                    Name = NameHelper.CreateName("$x")
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Value,
                    Value = new NumberValue(directionToPosition)
                }
            };

            var pointRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = NameHelper.CreateName("point")
            };

            andOp1.Right = pointRelation;

            pointRelation.ParamsList = new List<LogicalQueryNode>()
            {
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.LogicalVar,
                    Name = NameHelper.CreateName("$x")
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Value,
                    Value = new WaypointSourceValue(new NumberValue(distance), new NumberValue(directionToPosition), NameHelper.CreateName("#@"))
                }
            };

            return result;
        }

        protected virtual string GetTargetVarName(string factStr)
        {
            return "$x";
        }
    }
}

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
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
                    Name = varName
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
                    Name = varName
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
                    Name = varName
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
                    Name = varName
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
                    Name = varName
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Value,
                    Value = new WaypointSourceValue(new NumberValue(distance), new NumberValue(directionToPosition), NameHelper.CreateName("#@"))
                }
            };

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
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(selfId)
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("alive")
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildDeadFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: state(");
            sb.Append(selfId);
            sb.Append(", dead) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public RuleInstance BuildDeadFactInstance(string selfId)
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(selfId)
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("dead")
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildStopFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: act(");
            sb.Append(selfId);
            sb.Append(", stop) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public RuleInstance BuildStopFactInstance(string selfId)
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(selfId)
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("stop")
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildWalkFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: act(");
            sb.Append(selfId);
            sb.Append(", walk) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public RuleInstance BuildWalkFactInstance(string selfId)
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(selfId)
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("walk")
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildWalkSoundFactString()
        {
            return "{: act(someone, walk) :}";
        }

        /// <inheritdoc/>
        public RuleInstance BuildWalkSoundFactInstance()
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("someone")
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("walk")
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildRunFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: act(");
            sb.Append(selfId);
            sb.Append(", run) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public RuleInstance BuildRunFactInstance(string selfId)
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(selfId)
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("run")
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildRunSoundFactString()
        {
            return "{: act(someone, run) :}";
        }
        
        /// <inheritdoc/>
        public RuleInstance BuildRunSoundFactInstance()
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("someone")
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("run")
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildHoldFactString(string selfId, string heldThingId)
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
        public RuleInstance BuildHoldFactInstance(string selfId, string heldThingId)
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(selfId)
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(heldThingId)
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildShootFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: act(");
            sb.Append(selfId);
            sb.Append(", shoot) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public RuleInstance BuildShootFactInstance(string selfId)
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(selfId)
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("shoot")
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildShootSoundFactString()
        {
            return "{: act(someone, shoot) :}";
        }

        /// <inheritdoc/>
        public RuleInstance BuildShootSoundFactInstance()
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("someone")
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("shoot")
                }
            };

            return result;
        }

        /// <inheritdoc/>
        public string BuildReadyForShootFactString(string selfId)
        {
            var sb = new StringBuilder();

            sb.Append("{: ready(");
            sb.Append(selfId);
            sb.Append(", shoot) :}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public RuleInstance BuildReadyForShootFactInstance(string selfId)
        {
            var result = new RuleInstance();
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
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName(selfId)
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Concept,
                    Name = NameHelper.CreateName("shoot")
                }
            };

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
    }
}

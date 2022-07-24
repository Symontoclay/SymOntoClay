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
            return $"say({selfId}, {factStr})";
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

        protected virtual string GetTargetVarName(string factStr)
        {
            return "$x";
        }
    }
}

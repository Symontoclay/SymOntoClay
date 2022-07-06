using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.SoundPerception
{
    public abstract class BaseSoundReceiverComponent : BaseComponent, ISoundReceiver
    {
        protected BaseSoundReceiverComponent(IEntityLogger logger, int instanceId)
            : base(logger)
        {
            _instanceId = instanceId;
        }

        private readonly int _instanceId;

        /// <inheritdoc/>
        public int InstanceId => _instanceId;

        /// <inheritdoc/>
        public abstract Vector3 Position { get; }

        /// <inheritdoc/>
        public abstract double Threshold { get; }

        /// <inheritdoc/>
        public abstract void CallBack(double power, double distance, Vector3 position, string query);

        protected virtual string ConvertQuery(double power, double distance, Vector3 position, string query)
        {
#if DEBUG
            //Log($"power = {power}");
            //Log($"distance = {distance}");
            //Log($"position = {position}");
            //Log($"query = {query}");
#endif
            var varName = GetTargetVarName(query);

#if DEBUG
            //Log($"varName = {varName}");
#endif

            var directionToPosition = GetDirectionToPosition(position);

#if DEBUG
            //Log($"directionToPosition = {directionToPosition}");
#endif

            var distanceStr = distance.ToString(CultureInfo.InvariantCulture);
            var directionStr = directionToPosition.ToString(CultureInfo.InvariantCulture);

            var sb = new StringBuilder();

            sb.Append("{: ");
            sb.Append(varName);
            sb.Append(" = ");
            sb.Append(query);
            sb.Append($" & hear(I, {varName})");
            sb.Append($" & distance(I, {varName}, {distanceStr})");
            sb.Append($" & direction({varName}, {directionStr})");
            sb.Append($" & point({varName}, #@[{distanceStr}, {directionStr}])");
            sb.Append(" :}");

            return sb.ToString();
        }

        protected abstract string GetTargetVarName(string query);
        protected abstract float GetDirectionToPosition(Vector3 position);
    }
}

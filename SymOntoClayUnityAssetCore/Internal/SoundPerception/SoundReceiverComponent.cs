using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.SoundPerception
{
    public class SoundReceiverComponent : BaseComponent, ISoundReceiver
    {
        public SoundReceiverComponent(IEntityLogger logger, int instanceId, HumanoidNPCGameComponentContext internalContext, IWorldCoreGameComponentContext worldContext)
            : base(logger)
        {
            _internalContext = internalContext;
            _instanceId = instanceId;
            _soundBus = worldContext.SoundBus;            

            _soundBus.AddReceiver(this);
        }

        private readonly HumanoidNPCGameComponentContext _internalContext;
        private readonly ISoundBus _soundBus;
        private IHostSupport _hostSupport;
        private Engine _coreEngine;
        private readonly int _instanceId;

        /// <inheritdoc/>
        int ISoundReceiver.InstanceId => _instanceId;

        /// <inheritdoc/>
        Vector3 ISoundReceiver.Position => _hostSupport.GetCurrentAbsolutePosition();

        /// <inheritdoc/>
        IEntityLogger ISoundReceiver.Logger => Logger;

        /// <inheritdoc/>
        double ISoundReceiver.Threshold => 0;

        public void LoadFromSourceCode()
        {
            _hostSupport = _internalContext.HostSupportComponent;
            _coreEngine = _internalContext.CoreEngine;
        }

        /// <inheritdoc/>
        void ISoundReceiver.CallBack(double power, double distance, Vector3 position, string query)
        {
#if DEBUG
            Log($"power = {power}");
            Log($"distance = {distance}");
            Log($"position = {position}");
            Log($"query = {query}");
#endif
            var convertedQuery = ConvertQuery(power, distance, position, query);

#if DEBUG
            Log($"convertedQuery = {convertedQuery}");
#endif

            _coreEngine

            throw new NotImplementedException();
        }

        private string ConvertQuery(double power, double distance, Vector3 position, string query)
        {
#if DEBUG
            Log($"power = {power}");
            Log($"distance = {distance}");
            Log($"position = {position}");
            Log($"query = {query}");
#endif
            var varName = GetTargetVarName(query);

#if DEBUG
            Log($"varName = {varName}");
#endif

            var directionToPosition = _hostSupport.GetDirectionToPosition(position);

#if DEBUG
            Log($"directionToPosition = {directionToPosition}");
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

        private string GetTargetVarName(string query)
        {
            return "$x";
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _soundBus.RemoveReceiver(this);

            base.OnDisposed();
        }
    }
}

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.SoundPerception
{
    public class SoundReceiverComponent : BaseComponent, ISoundReceiver
    {
        public SoundReceiverComponent(IEntityLogger logger, int instanceId, IHostSupport hostSupport, IWorldCoreGameComponentContext worldContext)
            : base(logger)
        {
            _instanceId = instanceId;
            _soundBus = worldContext.SoundBus;
            _hostSupport = hostSupport;

            _soundBus.AddReceiver(this);
        }

        private readonly ISoundBus _soundBus;
        private readonly IHostSupport _hostSupport;
        private readonly int _instanceId;

        /// <inheritdoc/>
        int ISoundReceiver.InstanceId => _instanceId;

        /// <inheritdoc/>
        Vector3 ISoundReceiver.Position => _hostSupport.GetCurrentAbsolutePosition();

        /// <inheritdoc/>
        IEntityLogger ISoundReceiver.Logger => Logger;

        /// <inheritdoc/>
        double ISoundReceiver.Threshold => 0;

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

            throw new NotImplementedException();
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

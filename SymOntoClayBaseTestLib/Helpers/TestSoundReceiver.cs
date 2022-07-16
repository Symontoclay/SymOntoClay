using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Tests.Helpers
{
    public class TestSoundReceiver : BaseSoundReceiverComponent
    {
        public TestSoundReceiver(int instanceId, Vector3 position, Action<double, double, Vector3, string, string> callBack)
            : base(new EmptyLogger(), instanceId)
        {
            _position = position;
            _callBack = callBack;
        }

        private Vector3 _position;

        /// <inheritdoc/>
        public override Vector3 Position => _position;

        /// <inheritdoc/>
        public override double Threshold => 0;

        private readonly Action<double, double, Vector3, string, string> _callBack;

        /// <inheritdoc/>
        public override void CallBack(double power, double distance, Vector3 position, string query)
        {
            var convertedQuery = ConvertQuery(power, distance, position, query);

            _callBack(power, distance, position, query, convertedQuery);
        }

        /// <inheritdoc/>
        protected override string GetTargetVarName(string query)
        {
            return "$x";
        }

        /// <inheritdoc/>
        protected override float GetDirectionToPosition(Vector3 position)
        {
            return 12;
        }
    }
}

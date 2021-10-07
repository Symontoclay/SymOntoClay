using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.SoundBusHandler
{
    public class TstSoundReceiver: ISoundReceiver
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TstSoundReceiver(Vector3 position)
        {
            _position = position;
        }

        private Vector3 _position;

        public Vector3 Position => _position;

        public double Threshold { get; set; }

        public void CallBack(double power, double distance, Vector3 position, string query)
        {
            _logger.Info($"power = {power}");
            _logger.Info($"distance = {distance}");
            _logger.Info($"position = {position}");
            _logger.Info($"query = {query}");

            var convertedQuery = ConvertQuery(power, distance, position, query);

            _logger.Info($"convertedQuery = {convertedQuery}");
        }

        private string ConvertQuery(double power, double distance, Vector3 position, string query)
        {
            _logger.Info($"power = {power}");
            _logger.Info($"distance = {distance}");
            _logger.Info($"position = {position}");
            _logger.Info($"query = {query}");

            var varName = GetTargetVarName(query);

            var horizontalAngle = GetHorizontalAngle(position);

            var distanceStr = distance.ToString(CultureInfo.InvariantCulture);
            var directionStr = horizontalAngle.ToString(CultureInfo.InvariantCulture);

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

        private float GetHorizontalAngle(Vector3 position)
        {
            return 12;
        }
    }
}

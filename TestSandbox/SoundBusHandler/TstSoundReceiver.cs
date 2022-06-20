/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.SoundBusHandler
{
    public class TstSoundReceiver: ISoundReceiver
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public TstSoundReceiver(int instanceId, Vector3 position)
        {
            _instanceId = instanceId;
            _position = position;
        }

        private int _instanceId;

        public int InstanceId => _instanceId;

        private Vector3 _position;

        public Vector3 Position => _position;

        IEntityLogger ISoundReceiver.Logger => _logger;

        public double Threshold { get; set; }

        public void CallBack(double power, double distance, Vector3 position, string query)
        {
            _logger.Log($"power = {power}");
            _logger.Log($"distance = {distance}");
            _logger.Log($"position = {position}");
            _logger.Log($"query = {query}");

            var convertedQuery = ConvertQuery(power, distance, position, query);

            _logger.Log($"convertedQuery = {convertedQuery}");
        }

        private string ConvertQuery(double power, double distance, Vector3 position, string query)
        {
            _logger.Log($"power = {power}");
            _logger.Log($"distance = {distance}");
            _logger.Log($"position = {position}");
            _logger.Log($"query = {query}");

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

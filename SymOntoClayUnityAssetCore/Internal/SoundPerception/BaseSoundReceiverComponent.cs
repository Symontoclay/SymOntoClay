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

using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
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
        protected BaseSoundReceiverComponent(IEntityLogger logger, int instanceId, IStandardFactsBuilder standardFactsBuilder)
            : base(logger)
        {
            _instanceId = instanceId;
            _standardFactsBuilder = standardFactsBuilder;
        }

        private readonly int _instanceId;
        private readonly IStandardFactsBuilder _standardFactsBuilder;

        /// <inheritdoc/>
        public int InstanceId => _instanceId;

        /// <inheritdoc/>
        public abstract Vector3 Position { get; }

        /// <inheritdoc/>
        public abstract double Threshold { get; }

        /// <inheritdoc/>
        public abstract void CallBack(double power, double distance, Vector3 position, string query);
        public abstract void CallBack(double power, double distance, Vector3 position, RuleInstance fact);

        protected virtual string ConvertQuery(double power, double distance, Vector3 position, string query)
        {
#if DEBUG
            //Log($"power = {power}");
            //Log($"distance = {distance}");
            //Log($"position = {position}");
            //Log($"query = {query}");
#endif

            var directionToPosition = GetDirectionToPosition(position);

#if DEBUG
            //Log($"directionToPosition = {directionToPosition}");
#endif

            return _standardFactsBuilder.BuildSoundFactString(power, distance, directionToPosition, query);
        }

        protected virtual RuleInstance ConvertQuery(double power, double distance, Vector3 position, RuleInstance query)
        {
#if DEBUG
            Log($"power = {power}");
            Log($"distance = {distance}");
            Log($"position = {position}");
            Log($"query = {query.ToHumanizedString()}");
#endif

            var directionToPosition = GetDirectionToPosition(position);

#if DEBUG
            Log($"directionToPosition = {directionToPosition}");
#endif

            return _standardFactsBuilder.BuildSoundFactInstance(power, distance, directionToPosition, query);
        }

        protected abstract float GetDirectionToPosition(Vector3 position);
    }
}

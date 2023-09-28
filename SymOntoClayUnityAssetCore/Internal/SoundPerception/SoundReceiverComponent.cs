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

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.SoundPerception
{
    public class SoundReceiverComponent : BaseSoundReceiverComponent
    {
        public SoundReceiverComponent(IMonitorLogger logger, int instanceId, HumanoidNPCGameComponentContext internalContext, IWorldCoreGameComponentContext worldContext)
            : base(logger, instanceId, worldContext.StandardFactsBuilder)
        {
            _internalContext = internalContext;
            _standardFactsBuilder = worldContext.StandardFactsBuilder;

            _soundBus = worldContext.SoundBus;

            _soundBus?.AddReceiver(this);
        }

        private readonly HumanoidNPCGameComponentContext _internalContext;
        private readonly ISoundBus _soundBus;
        private IHostSupport _hostSupport;
        private Engine _coreEngine;
        private readonly IStandardFactsBuilder _standardFactsBuilder;

        /// <inheritdoc/>
        public override Vector3 Position => _hostSupport.GetCurrentAbsolutePosition(Logger);

        /// <inheritdoc/>
        public override double Threshold => 0;

        public void LoadFromSourceCode()
        {
            _hostSupport = _internalContext.HostSupportComponent;
            _coreEngine = _internalContext.CoreEngine;
        }

        /// <inheritdoc/>
        public override void CallBack(double power, double distance, Vector3 position, string query)
        {
            var convertedQuery = ConvertQuery(power, distance, position, query);

            _coreEngine.InsertListenedFact(Logger, convertedQuery);
        }

        /// <inheritdoc/>
        public override void CallBack(double power, double distance, Vector3 position, RuleInstance fact)
        {
            var convertedQuery = ConvertQuery(power, distance, position, fact);

            _coreEngine.InsertListenedFact(Logger, convertedQuery);
        }

        /// <inheritdoc/>
        protected override float GetDirectionToPosition(Vector3 position)
        {
            return _hostSupport.GetDirectionToPosition(Logger, position);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _soundBus?.RemoveReceiver(this);

            base.OnDisposed();
        }
    }
}

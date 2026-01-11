/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.SoundPerception
{
    public class SoundPublisherComponent : BaseComponent, ISoundPublisherProvider
    {
        public SoundPublisherComponent(IMonitorLogger logger, int instanceId, string idForFacts, IHostSupport hostSupport, IWorldCoreGameComponentContext worldContext)
            : base(logger)
        {
            _instanceId = instanceId;
            _idForFacts = idForFacts;
            _standardFactsBuilder = worldContext.StandardFactsBuilder;
            _soundBus = worldContext.SoundBus;
            _hostSupport = hostSupport;
        }

        private readonly ISoundBus _soundBus;
        private readonly IHostSupport _hostSupport;
        private readonly int _instanceId;
        private readonly string _idForFacts;
        private readonly IStandardFactsBuilder _standardFactsBuilder;

        /// <inheritdoc/>
        public void PushSoundFact(float power, string factStr)
        {
            if(_soundBus == null)
            {
                return;
            }

            _soundBus.PushSound(_instanceId, power, _hostSupport.GetCurrentAbsolutePosition(Logger), factStr);
        }

        /// <inheritdoc/>
        public void PushSoundFact(float power, RuleInstance fact)
        {
            var factStr = fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent);

            PushSoundFact(power, factStr);
        }

        /// <inheritdoc/>
        public void PushSpeechFact(float power, string factStr)
        {
            factStr = _standardFactsBuilder.BuildSayFactString(_idForFacts, factStr);

            PushSoundFact(power, factStr);
        }

        /// <inheritdoc/>
        public void PushSpeechFact(float power, RuleInstance fact)
        {
            var factStr = fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent);

            PushSpeechFact(power, factStr);
        }
    }
}

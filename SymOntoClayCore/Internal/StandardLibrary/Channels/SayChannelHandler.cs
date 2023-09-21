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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Channels
{
    public class SayChannelHandler : BaseLoggedComponent, IChannelHandler
    {
        public SayChannelHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;
            _soundPublisherProvider = engineContext.SoundPublisherProvider;
            _id = engineContext.Id;
        }

        private readonly IEngineContext _engineContext;
        private readonly ISoundPublisherProvider _soundPublisherProvider;
        private readonly string _id;

        /// <inheritdoc/>
        public ulong GetLongHashCode()
        {
            return (ulong)Math.Abs(GetHashCode());
        }

        /// <inheritdoc/>
        public Value Read(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = new NullValue();

            return result;
        }

        /// <inheritdoc/>
        public Value Write(IMonitorLogger logger, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var kindOfValue = value.KindOfValue;

            switch(kindOfValue)
            {
                case KindOfValue.RuleInstance:
                    ProcessRuleInstanceValue(value.AsRuleInstance);
                    break;

                case KindOfValue.StringValue:
                    ProcessStringValue(value.AsStringValue);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }

            return value;
        }

        private float DefaultSoundPower = 50;

        private void ProcessRuleInstanceValue(RuleInstance value)
        {
            _soundPublisherProvider?.PushSpeechFact(DefaultSoundPower, value);
        }

        private void ProcessStringValue(StringValue value)
        {
            var factValue = value.ToRuleInstance(_engineContext);

            ProcessRuleInstanceValue(factValue);
        }
    }
}

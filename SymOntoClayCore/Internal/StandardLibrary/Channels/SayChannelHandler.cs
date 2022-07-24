using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
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
        public Value Read(LocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = new NullValue();

            return result;
        }

        /// <inheritdoc/>
        public Value Write(Value value, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            var kindOfValue = value.KindOfValue;

            switch(kindOfValue)
            {
                case KindOfValue.RuleInstanceValue:
                    ProcessRuleInstanceValue(value.AsRuleInstanceValue);
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

        private void ProcessRuleInstanceValue(RuleInstanceValue value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            _soundPublisherProvider?.PushSpeechFact(DefaultSoundPower, value.RuleInstance);
        }

        private void ProcessStringValue(StringValue value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            var factValue = value.ToRuleInstanceValue(_engineContext);

#if DEBUG
            //Log($"factValue = {factValue}");
#endif

            ProcessRuleInstanceValue(factValue);
        }
    }
}

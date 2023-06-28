using Newtonsoft.Json.Linq;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class DateTimeResolver : BaseResolver
    {
        public DateTimeResolver(IMainStorageContext context)
            : base(context)
        {
            _dateTimeProvider = context.DateTimeProvider;
        }

        private readonly IDateTimeProvider _dateTimeProvider;

        public long ConvertTimeValueToTicks(Value value, KindOfDefaultTimeValue kindOfDefaultTimeValue, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"value = {value}");
            Log($"kindOfDefaultTimeValue = {kindOfDefaultTimeValue}");
#endif

            var kindOfValue = value.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.NumberValue:
                    return ConvertNumberValueToTicks(value.AsNumberValue, kindOfDefaultTimeValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }
        }

        private long ConvertNumberValueToTicks(NumberValue value, KindOfDefaultTimeValue kindOfDefaultTimeValue)
        {
#if DEBUG
            Log($"value = {value}");
            Log($"kindOfDefaultTimeValue = {kindOfDefaultTimeValue}");
#endif

            var sysValue = Convert.ToInt64(value.SystemValue);

#if DEBUG
            Log($"sysValue = {sysValue}");
#endif

            switch(kindOfDefaultTimeValue)
            {
                case KindOfDefaultTimeValue.Ticks:
                    return sysValue;

                case KindOfDefaultTimeValue.Seconds:
                    return Convert.ToInt32(sysValue * _dateTimeProvider.SecondsToTicksMultiplicator);

                case KindOfDefaultTimeValue.Milliseconds:
                    return Convert.ToInt32(sysValue * _dateTimeProvider.MillisecondsToTicksMultiplicator);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfDefaultTimeValue), kindOfDefaultTimeValue, null);
            }
        }

        public float GetCurrentSeconds()
        {
            return _dateTimeProvider.CurrentTiks * _dateTimeProvider.TicksToSecondsMultiplicator;
        }
    }
}

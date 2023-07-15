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
            var kindOfValue = value.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.NumberValue:
                    return ConvertNumberValueToTicks(value.AsNumberValue, kindOfDefaultTimeValue);

                case KindOfValue.StrongIdentifierValue:
                    {
                        ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving = null;

                        return ConvertNumberValueToTicks(_context.DataResolversFactory.GetFuzzyLogicResolver().Resolve(value.AsStrongIdentifierValue, reasonOfFuzzyLogicResolving, localCodeExecutionContext), kindOfDefaultTimeValue);
                    }

                case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                    {
                        ReasonOfFuzzyLogicResolving reasonOfFuzzyLogicResolving = null;

                        return ConvertNumberValueToTicks(_context.DataResolversFactory.GetFuzzyLogicResolver().Resolve(value.AsFuzzyLogicNonNumericSequenceValue, reasonOfFuzzyLogicResolving, localCodeExecutionContext), kindOfDefaultTimeValue);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }
        }

        private long ConvertNumberValueToTicks(NumberValue value, KindOfDefaultTimeValue kindOfDefaultTimeValue)
        {
            var sysValue = Convert.ToInt64(value.SystemValue);

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

    }
}

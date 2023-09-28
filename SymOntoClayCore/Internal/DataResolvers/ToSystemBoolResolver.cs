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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ToSystemBoolResolver : BaseLoggedComponent
    {
        public ToSystemBoolResolver(IMonitorLogger logger)
            : base(logger)
        {
        }

        public readonly float TruthThreshold = 0.75F;
        public readonly bool NullValueEquvivalent = false;

        public bool Resolve(IMonitorLogger logger, Value value)
        {
            var kindOfValue = value.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.LogicalValue:
                    return Resolve(logger, value.AsLogicalValue);

                case KindOfValue.NumberValue:
                    return Resolve(logger, value.AsNumberValue);

                case KindOfValue.NullValue:
                    return NullValueEquvivalent;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }

            throw new NotImplementedException();
        }

        public bool Resolve(IMonitorLogger logger, LogicalValue value)
        {
            var systemValue = value.SystemValue;

            if(systemValue.HasValue)
            {
                return Resolve(logger, systemValue.Value);
            }

            return NullValueEquvivalent;
        }

        public bool Resolve(IMonitorLogger logger, NumberValue value)
        {
            var systemValue = value.SystemValue;

            if (systemValue.HasValue)
            {
                return Resolve(logger, systemValue.Value);
            }

            return NullValueEquvivalent;
        }

        public bool Resolve(IMonitorLogger logger, double value)
        {
            return value >= TruthThreshold;
        }

        public bool Resolve(IMonitorLogger logger, float value)
        {
            return value >= TruthThreshold;
        }
    }
}

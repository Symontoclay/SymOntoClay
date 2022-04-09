using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ToSystemBoolResolver : BaseLoggedComponent
    {
        public ToSystemBoolResolver(IEntityLogger logger)
            : base(logger)
        {
        }

        public readonly float TruthThreshold = 0.75F;
        public readonly bool NullValueEquvivalent = false;

        public bool Resolve(Value value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            var kindOfValue = value.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.LogicalValue:
                    return Resolve(value.AsLogicalValue);

                case KindOfValue.NumberValue:
                    return Resolve(value.AsNumberValue);

                case KindOfValue.NullValue:
                    return NullValueEquvivalent;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }

            throw new NotImplementedException();
        }

        public bool Resolve(LogicalValue value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            var systemValue = value.SystemValue;

            if(systemValue.HasValue)
            {
                return Resolve(systemValue.Value);
            }

            return NullValueEquvivalent;
        }

        public bool Resolve(NumberValue value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            var systemValue = value.SystemValue;

            if (systemValue.HasValue)
            {
                return Resolve(systemValue.Value);
            }

            return NullValueEquvivalent;
        }

        public bool Resolve(double value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            return value >= TruthThreshold;
        }

        public bool Resolve(float value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            return value >= TruthThreshold;
        }
    }
}

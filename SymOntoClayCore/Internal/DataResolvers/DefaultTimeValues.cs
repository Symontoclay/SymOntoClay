using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public static class DefaultTimeValues
    {
        public const KindOfDefaultTimeValue EachTimerDefaultTimeValue = KindOfDefaultTimeValue.Seconds;
        public const KindOfDefaultTimeValue OnceTimerDefaultTimeValue = KindOfDefaultTimeValue.Seconds;
        public const KindOfDefaultTimeValue DurationDefaultTimeValue = KindOfDefaultTimeValue.Seconds;
        public const KindOfDefaultTimeValue TimeoutDefaultTimeValue = KindOfDefaultTimeValue.Seconds;
    }
}

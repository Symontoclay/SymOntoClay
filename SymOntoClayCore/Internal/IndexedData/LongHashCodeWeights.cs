using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public static class LongHashCodeWeights
    {
        public const ulong StubWeight = 18446744073709551615;
        public const ulong NullWeight = 18446744073709551614;
        public const ulong HostWeight = 18446744073709551613;

        public const ulong BaseOperatorWeight = 184467440737090;

        public const ulong BaseFunctionWeight = 18446744070;
        public const ulong BaseParamWeight = 1844670;

        public const ulong BaseModalityWeight = 20000;
        public const ulong BaseCommandWeight = 100000;
    }
}

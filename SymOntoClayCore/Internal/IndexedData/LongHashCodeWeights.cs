/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

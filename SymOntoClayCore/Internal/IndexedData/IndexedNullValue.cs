/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedNullValue : IndexedValue
    {
        public NullValue OriginalNullValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalNullValue;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.NullValue;

        /// <inheritdoc/>
        public override bool IsNullValue => true;

        /// <inheritdoc/>
        public override IndexedNullValue AsNullValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return null;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return LongHashCodeWeights.NullWeight ^ base.CalculateLongHashCode();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}NULL";
        }
    }
}

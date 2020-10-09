/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class NumberValueLinearResolver : BaseResolver
    {
        public NumberValueLinearResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedNumberValue Resolve(IndexedValue source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch (sourceKind)
            {
                case KindOfValue.NullValue:
                    return IndexedValueConvertor.ConvertNullValueToNumberValue(source.AsNullValue, _context);

                case KindOfValue.LogicalValue:
                    return IndexedValueConvertor.ConvertLogicalValueToNumberValue(source.AsLogicalValue, _context);

                case KindOfValue.NumberValue:
                    return source.AsNumberValue;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}

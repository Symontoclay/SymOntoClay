/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalValueLinearResolver: BaseResolver
    {
        public LogicalValueLinearResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public LogicalValue Resolve(Value source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch (sourceKind)
            {
                case KindOfValue.NullValue:
                    return new LogicalValue(null);

                case KindOfValue.LogicalValue:
                    return source.AsLogicalValue;

                case KindOfValue.NumberValue:
                    return new LogicalValue((float?)source.AsNumberValue.SystemValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }

        public IndexedLogicalValue Resolve(IndexedValue source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch(sourceKind)
            {
                case KindOfValue.NullValue:
                    return IndexedValueConvertor.ConvertNullValueToLogicalValue(source.AsNullValue, _context);

                case KindOfValue.LogicalValue:
                    return source.AsLogicalValue;

                case KindOfValue.NumberValue:
                    return IndexedValueConvertor.ConvertNumberValueToLogicalValue(source.AsNumberValue, _context);

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}

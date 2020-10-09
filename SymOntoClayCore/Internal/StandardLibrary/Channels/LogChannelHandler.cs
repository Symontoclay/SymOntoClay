/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using NLog.Fluent;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Channels
{
    public class LogChannelHandler : BaseLoggedComponent, IChannelHandler
    {
        public LogChannelHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;
        }

        private readonly IEngineContext _engineContext;

        /// <inheritdoc/>
        public IndexedValue Read()
        {
            var result = new NullValue();
            
            return result.GetIndexed(_engineContext);
        }

        /// <inheritdoc/>
        public IndexedValue Write(IndexedValue value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            switch(value.KindOfValue)
            {
                case KindOfValue.LogicalSearchResultValue:
                    Log(DebugHelperForLogicalSearchResult.ToString(value.AsLogicalSearchResultValue.LogicalSearchResult, _engineContext.Dictionary));
                    break;

                case KindOfValue.NullValue:
                    Log("NULL");
                    break;

                default:
                    Log(value.GetSystemValue()?.ToString());
                    break;
            }

            return value;
        }
    }
}

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

            if(value.IsLogicalSearchResultValue)
            {
                Log(DebugHelperForLogicalSearchResult.ToString(value.AsLogicalSearchResultValue.LogicalSearchResult, _engineContext.Dictionary));
            }
            else
            {
                Log(value.GetSystemValue()?.ToString());
            }            

            return value;
        }
    }
}

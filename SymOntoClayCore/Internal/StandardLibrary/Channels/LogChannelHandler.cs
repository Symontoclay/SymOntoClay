using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
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
        public Value Read()
        {
            return new NullValue();
        }

        /// <inheritdoc/>
        public Value Write(Value value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            Log(value.GetSystemValue()?.ToString());

            return value;
        }
    }
}

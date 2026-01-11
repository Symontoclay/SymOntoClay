/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using NLog.Fluent;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Channels
{
    public class LogChannelHandler : BaseLoggedComponent, IChannelHandler
    {
        public LogChannelHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;
            _debugHelperOptions = DebugHelperOptions.FromHumanizedOptions();
            _debugHelperOptions.ShowPrefixesForConceptLikeIdentifier = false;
        }

        private readonly IEngineContext _engineContext;
        private readonly DebugHelperOptions _debugHelperOptions;

        /// <inheritdoc/>
        public ulong GetLongHashCode()
        {
            return (ulong)Math.Abs(GetHashCode());
        }

        /// <inheritdoc/>
        public Value Read(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var result = new NullValue();
            
            return result;
        }

        /// <inheritdoc/>
        public Value Write(IMonitorLogger logger, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            logger.Output("90A48AF8-3AF3-4D2B-B3FD-EB80ED2E25E0", value.ToHumanizedString(_debugHelperOptions));

            return value;
        }
    }
}

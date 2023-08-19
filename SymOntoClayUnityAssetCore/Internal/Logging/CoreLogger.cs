/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Logging
{
    [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
    public class CoreLogger: IWorldCoreComponent
    {
        private readonly object _lockObj = new object();
        //private readonly IWorldCoreContext _coreContext;
        //private readonly CoreLoggerSettings _settings;
        private readonly LoggerContext _loggerContext = new LoggerContext();
        //private readonly IEntityLogger _coreLogger;
        //private readonly string _todaysDir;

        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public CoreLogger(LoggingSettings settings, IWorldCoreContext coreContext)
        {
        }

        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public string LogicalSearchExplainDumpDir { get; private set; }

        /// <summary>
        /// Gets or sets value of enable logging.
        /// It alows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public bool EnableLogging
        {
            get
            {
                lock(_lockObj)
                {
                    return _loggerContext.Enable;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    _loggerContext.Enable = value;
                }
            }
        }

        /// <inheritdoc/>
        [Obsolete("It should be replaced to Monitor or MonitorNode", true)]
        public bool IsDisposed { get => false; }
    }
}
